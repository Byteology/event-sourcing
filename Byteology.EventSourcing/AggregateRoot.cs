namespace Byteology.EventSourcing;

using Byteology.EventSourcing.EventStorage;

public abstract class AggregateRoot : IAggregateRoot
{
    private readonly List<IEvent> _newEvents = new();

    public Guid EventStreamId { get; private set; }
    public ulong EventStreamPosition { get; private set; }

    public void ApplyNewEvent(IEvent @event)
    {
        HandleEvent(@event);
        EventStreamPosition++;
        _newEvents.Add(@event);
    }

    protected abstract void HandleEvent(IEvent @event);

    Guid IAggregateRoot.EventStreamId { get => EventStreamId; set => EventStreamId = value; }

    IEnumerable<IEvent> IAggregateRoot.GetUncommitedEvents() => _newEvents;

    void IAggregateRoot.MarkAllEventsAsCommited() => _newEvents.Clear();

    void IAggregateRoot.ReplayEvent(PersistedEventRecord record)
    {
        if (record.Metadata.AggregateRootType != this.GetType() || record.Metadata.EventStreamId != EventStreamId)
            throw new ArgumentException("The specified event is meant for another aggregate root.");

        if (record.Metadata.EventStreamPosition <= EventStreamPosition)
            throw new ArgumentException("The specified event is in an already processed position in the event stream.");

        HandleEvent(record.Event);
        EventStreamPosition = record.Metadata.EventStreamPosition;
    }
}


