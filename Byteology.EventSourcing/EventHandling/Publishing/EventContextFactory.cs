namespace Byteology.EventSourcing.EventHandling.Publishing;

using Byteology.EventSourcing.EventHandling.Storage;

internal static class EventContextFactory
{
    public static IEventContext<IEvent> Create(
        IEventStreamRecord eventRecord, 
        Guid aggregateRootId,
        Type aggregateRootType)
    {
        Type type = typeof(EventContext<>);
        Type eventType = eventRecord.Event.GetType();
        type = type.MakeGenericType(eventType);

        object instance = Activator.CreateInstance(type, new object?[] {
            aggregateRootId,
            aggregateRootType,
            eventRecord.EventTimestamp,
            eventRecord.EventSequence,
            eventRecord.Event
        })!;

        return (instance as IEventContext<IEvent>)!;
    }

    private sealed class EventContext<TEvent> : IEventContext<TEvent>
        where TEvent : IEvent
    {
        public Guid AggregateRootId { get; }
        public Type AggregateRootType { get; }
        public DateTimeOffset EventTimestamp { get; }
        public ulong EventSequence { get; }
        public TEvent Event { get; }

        #pragma warning disable S1144 // Unused private types or members should be removed
        // This is called by reflection from the factory method
            public EventContext(
                Guid aggregateRootId,
                Type aggregateRootType,
                DateTimeOffset eventTimestamp,
                ulong eventSequence,
                TEvent @event)
            {
                AggregateRootId = aggregateRootId;
                AggregateRootType = aggregateRootType;
                EventTimestamp = eventTimestamp;
                EventSequence = eventSequence;
                Event = @event;
            }
        #pragma warning restore S1144 // Unused private types or members should be removed
    }
}
