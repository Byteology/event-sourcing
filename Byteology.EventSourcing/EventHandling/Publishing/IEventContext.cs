namespace Byteology.EventSourcing.EventHandling.Publishing;

public interface IEventContext<out TEvent>
    where TEvent : IEvent
{
    Guid AggregateRootId { get; }
    Type AggregateRootType { get; }
    DateTimeOffset EventTimestamp { get; }
    ulong EventSequence { get; }
    TEvent Event { get; }
}
