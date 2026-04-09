using BuildingBlocks.Exceptions;
using BuildingBlocks.Extensions;
using BuildingBlocks.SeedWork;
using BuildingBlocks.Types.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Persistence.Hooks
{
    public class DefaultDbHookHandler<TContext, TId> : IDbHookHandler<TContext, TId> where TContext : DbContext
    {
        private readonly IEnumerable<Lazy<IDbSaveHook<TContext, TId>, HookMetadata>> _saveHooks;
        private readonly Multimap<RequestHookKey, IDbSaveHook<TContext, TId>> _hooksRequestCache = new Multimap<RequestHookKey, IDbSaveHook<TContext, TId>>();
        private readonly HashSet<HookedEntityKey> _hookedEntities = new HashSet<HookedEntityKey>();
        private static HashSet<Type> _importantSaveHookTypes;
        private static readonly object _lock = new object();
        private static readonly HashSet<HookKey> _voidHooks = new HashSet<HookKey>();

        public DefaultDbHookHandler(IEnumerable<Lazy<IDbSaveHook<TContext, TId>, HookMetadata>> hooks)
        {
            _saveHooks = hooks;
        }

        public ILogger Logger { get; set; } = NullLogger.Instance;

        public bool HasImportantSaveHooks()
        {
            if (_importantSaveHookTypes == null)
            {
                lock (_lock)
                {
                    if (_importantSaveHookTypes == null)
                    {
                        _importantSaveHookTypes = new HashSet<Type>();
                        _importantSaveHookTypes.AddRange(_saveHooks
                            .Where(x => x.Metadata.Important)
                            .Select(x => x.Metadata.ImplType));
                    }
                }
            }
            return _importantSaveHookTypes.Any();
        }

        public IEnumerable<IDbSaveHook<TContext, TId>> TriggerPreSaveHooks(
            IEnumerable<IHookedEntity<TContext, TId>> entries,
            bool importantHooksOnly,
            out bool anyStateChanged)
        {
            Guard.AgainstNull(entries, nameof(entries));
            anyStateChanged = false;
            var dbSaveHooks = new HashSet<IDbSaveHook<TContext, TId>>();

            if (!entries.Any() || !_saveHooks.Any() || (importantHooksOnly && !HasImportantSaveHooks()))
                return dbSaveHooks;

            foreach (var entry in entries)
            {
                if (HandledAlready(entry, HookStage.PreSave)) continue;

                foreach (var hook in GetSaveHookInstancesFor(entry, HookStage.PreSave, importantHooksOnly))
                {
                    try
                    {
                        hook.OnBeforeSave(entry);
                        dbSaveHooks.Add(hook);
                    }
                    catch (Exception ex) when (ex is NotImplementedException || ex is NotSupportedException)
                    {
                        RegisterVoidHook(hook, entry, HookStage.PreSave);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "PreSaveHook exception ({0})", hook.GetType().FullName);
                    }

                    if (entry.HasStateChanged)
                    {
                        entry.InitialState = entry.State;
                        anyStateChanged = true;
                    }
                }
            }

            dbSaveHooks.ForEach(x => x.OnBeforeSaveCompleted());
            return dbSaveHooks;
        }

        public IEnumerable<IDbSaveHook<TContext, TId>> TriggerPostSaveHooks(
            IEnumerable<IHookedEntity<TContext, TId>> entries,
            bool importantHooksOnly)
        {
            Guard.AgainstNull(entries, nameof(entries));
            var dbSaveHooks = new HashSet<IDbSaveHook<TContext, TId>>();

            if (!entries.Any() || !_saveHooks.Any() || (importantHooksOnly && !HasImportantSaveHooks()))
                return dbSaveHooks;

            foreach (var entry in entries)
            {
                if (HandledAlready(entry, HookStage.PostSave)) continue;

                foreach (var hook in GetSaveHookInstancesFor(entry, HookStage.PostSave, importantHooksOnly))
                {
                    try
                    {
                        hook.OnAfterSave(entry);
                        dbSaveHooks.Add(hook);
                    }
                    catch (Exception ex) when (ex is NotImplementedException || ex is NotSupportedException)
                    {
                        RegisterVoidHook(hook, entry, HookStage.PostSave);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "PostSaveHook exception ({0})", hook.GetType().FullName);
                    }
                }
            }

            dbSaveHooks.ForEach(x => x.OnAfterSaveCompleted());
            return dbSaveHooks;
        }

        private IEnumerable<IDbSaveHook<TContext, TId>> GetSaveHookInstancesFor(
            IHookedEntity<TContext, TId> entry,
            HookStage stage,
            bool importantOnly)
        {
            var key = new RequestHookKey(entry, stage, importantOnly);
            if (_hooksRequestCache.ContainsKey(key))
                return _hooksRequestCache[key];

            var hooks = _saveHooks
                .Where(x => !importantOnly || x.Metadata.Important)
                .Where(x => !IsVoidHook(x.Value, entry, stage))
                .Select(x => x.Value)
                .ToList();

            _hooksRequestCache.AddRange(key, hooks);
            return hooks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HandledAlready(IHookedEntity<TContext, TId> entry, HookStage stage)
        {
            var key = new HookedEntityKey(entry, stage, entry.Entity?.Id?.GetHashCode() ?? 0);
            if (_hookedEntities.Contains(key))
                return true;

            _hookedEntities.Add(key);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterVoidHook(
            IDbSaveHook<TContext, TId> hook,
            IHookedEntity<TContext, TId> entry,
            HookStage stage)
        {
            var key = new HookKey(hook.GetType(), entry, stage);
            _voidHooks.Add(key);
        }

        private bool IsVoidHook(IDbSaveHook<TContext, TId> hook, IHookedEntity<TContext, TId> entry, HookStage stage)
        {
            var key = new HookKey(hook.GetType(), entry, stage);
            return _voidHooks.Contains(key);
        }

        private enum HookStage
        {
            PreSave,
            PostSave,
        }

        private class HookedEntityKey : 
            Tuple<Type, Type, int, EntityState, HookStage>
        {
            public HookedEntityKey(
                IHookedEntity<TContext, TId> entry,
                HookStage stage,
                int entityId)
                : base(entry.ContextType, entry.EntityType, entityId, entry.InitialState, stage)
            {
            }
        }

        private class RequestHookKey : 
            Tuple<Type, Type, EntityState, HookStage, bool>
        {
            public RequestHookKey(
                IHookedEntity<TContext, TId> entry,
                HookStage stage,
                bool importantOnly)
                : base(entry.ContextType, entry.EntityType, entry.InitialState, stage, importantOnly)
            {
            }
        }

        private class HookKey : 
            Tuple<Type, Type, EntityState, HookStage>
        {
            public HookKey(
                Type hookType,
                IHookedEntity<TContext, TId> entry,
                HookStage stage)
                : base(hookType, entry.EntityType, entry.InitialState, stage)
            {
            }
        }
    }
}
