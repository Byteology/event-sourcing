namespace Byteology.EventSourcing;

using Byteology.EventSourcing.EventStorage;

public abstract class AggregateRoot : IAggregateRoot
{
    private readonly List<IEvent> _newEvents = new();

    public Guid Id { get; private set; }
    public ulong Version { get; private set; }

    public void ApplyNewEvent(IEvent @event)
    {
        HandleEvent(@event);
        Version++;
        _newEvents.Add(@event);
    }

    protected abstract void HandleEvent(IEvent @event);

    Guid IAggregateRoot.Id { get => Id; set => Id = value; }

    IEnumerable<IEvent> IAggregateRoot.GetUncommitedEvents() => _newEvents;

    void IAggregateRoot.MarkAllEventsAsCommited() => _newEvents.Clear();

    void IAggregateRoot.ReplayEvent(PersistedEventRecord record)
    {
        if (record.Metadata.AggregateRootType != this.GetType() || record.Metadata.AggregateRootId != Id)
            throw new ArgumentException("The specified event is meant for another aggregate root.");

        if (record.Metadata.EventStreamPosition <= Version)
            throw new ArgumentException("The specified event is in an already processed position in the event stream.");

        HandleEvent(record.Event);
        Version = record.Metadata.EventStreamPosition;
    }
}


