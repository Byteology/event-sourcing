namespace Byteology.EventSourcing.EventStorage;

public record PersistedEventMetadata(
    Guid EventStreamId,
    Type AggregateRootType,
    ulong EventStreamPosition,
    DateTimeOffset Timestamp,
    string? Issuer,
    Guid TransactionId,
    ulong GlobalEventStreamPosition) 
        : EventMetadata(EventStreamId, AggregateRootType, EventStreamPosition, Timestamp, Issuer, TransactionId);
