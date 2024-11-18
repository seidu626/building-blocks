
using Mediator;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.SeedWork;

public interface IDomainEventDispatcher
{
    ValueTask Dispatch(IEnumerable<INotification> domainEvents, CancellationToken cancellationToken);
}