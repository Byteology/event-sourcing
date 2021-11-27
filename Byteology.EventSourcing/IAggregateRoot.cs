namespace Byteology.EventSourcing;

using Byteology.EventSourcing.CommandHandling;
using Byteology.EventSourcing.Storage;

public interface IAggregateRoot
{
    Guid Id { get; set; }
    ulong Version { get; }

    IEnumerable<IEvent> GetUncommitedEvents();
    void MarkAllEventsAsCommited();

    void ExecuteCommand(ICommand command, CommandMetadata metadata);
    void ReplayEvent(PersistedEventRecord record);
}
