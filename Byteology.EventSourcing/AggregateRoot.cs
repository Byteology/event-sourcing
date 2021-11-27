namespace Byteology.EventSourcing;

using Byteology.EventSourcing.CommandHandling;
using Byteology.EventSourcing.Storage;

public abstract class AggregateRoot : IAggregateRoot
{
    private readonly List<IEvent> _newEvents = new();

    public Guid Id { get; private set; }
    public ulong Version { get; private set; }

    protected abstract void ExecuteCommand(ICommand command, CommandMetadata metadata);

    protected abstract void HandleEvent(IEvent @event);

    protected void ApplyNewEvent(IEvent @event)
    {
        HandleEvent(@event);
        Version++;
        _newEvents.Add(@event);
    }

    #region IAggregateRoot Support

    Guid IAggregateRoot.Id { get => Id; set => Id = value; }
    ulong IAggregateRoot.Version => Version;

    IEnumerable<IEvent> IAggregateRoot.GetUncommitedEvents() => _newEvents;

    void IAggregateRoot.MarkAllEventsAsCommited() => _newEvents.Clear();

    void IAggregateRoot.ExecuteCommand(ICommand command, CommandMetadata metadata) 
        => ExecuteCommand(command, metadata);

    void IAggregateRoot.ReplayEvent(PersistedEventRecord record)
    {
        if (record.Metadata.AggregateRootType != this.GetType() || record.Metadata.AggregateRootId != Id)
            throw new ArgumentException("The specified event is meant for another aggregate root.");

        if (record.Metadata.EventStreamPosition <= Version)
            throw new ArgumentException("The specified event is in an already processed position in the event stream.");

        HandleEvent(record.Event);
        Version = record.Metadata.EventStreamPosition;
    }

    #endregion
}


