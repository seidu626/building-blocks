using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BuildingBlocks.Persistence.Hooks
{
    public interface IDbHookHandler<TContext> where TContext : DbContext
    {
        bool HasImportantSaveHooks();

        IEnumerable<IDbSaveHook<TContext>> TriggerPreSaveHooks(
            IEnumerable<IHookedEntity<TContext>> entries,
            bool importantHooksOnly,
            out bool anyStateChanged);

        IEnumerable<IDbSaveHook<TContext>> TriggerPostSaveHooks(
            IEnumerable<IHookedEntity<TContext>> entries,
            bool importantHooksOnly);
    }
}