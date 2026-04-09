using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BuildingBlocks.Persistence.Hooks
{
    public interface IDbHookHandler<TContext, TId> where TContext : DbContext
    {
        bool HasImportantSaveHooks();

        IEnumerable<IDbSaveHook<TContext, TId>> TriggerPreSaveHooks(
            IEnumerable<IHookedEntity<TContext, TId>> entries,
            bool importantHooksOnly,
            out bool anyStateChanged);

        IEnumerable<IDbSaveHook<TContext, TId>> TriggerPostSaveHooks(
            IEnumerable<IHookedEntity<TContext, TId>> entries,
            bool importantHooksOnly);
    }
}