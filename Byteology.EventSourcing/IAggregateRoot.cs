namespace Byteology.EventSourcing;

using Byteology.EventSourcing.CommandHandling;
using Byteology.EventSourcing.EventHandling;

public interface IAggregateRoot
{
    Guid Id { get; set; }

    void ExecuteCommand<TCommand>(TCommand command, DateTimeOffset timestamp)
        where TCommand : ICommand;

    void ReplayEvent(IEventContext eventContext);

    IEnumerable<IEventContext> GetUncommitedEvents();
    void MarkAllEventsAsCommited();
}
