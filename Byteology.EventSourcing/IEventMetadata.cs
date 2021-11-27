namespace Byteology.EventSourcing;

public interface IEventMetadata
{
    Guid AggregateRootId { get; }
    Type AggregateRootType { get; }
    ulong EventStreamPosition { get; }
    DateTimeOffset Timestamp { get; }
    string? Issuer { get; }
    Guid TransactionId { get; }
}
