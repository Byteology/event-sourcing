namespace Byteology.EventSourcing.EventHandling;

public interface IEventContext
{
    Guid AggregateId { get; }
    Type AggregateType { get; }
    DateTimeOffset Timestamp { get; }
    ulong EventSequence { get; }
    IEvent Event { get; }
}
