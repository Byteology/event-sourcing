namespace Byteology.EventSourcing.EventStorage;

public record PersistedEventMetadata(
    ulong GlobalEventStreamPosition,
    Guid AggregateRootId,
    Type AggregateRootType,
    ulong EventStreamPosition,
    DateTimeOffset Timestamp,
    string? Issuer,
    Guid TransactionId) 
        : EventMetadata(AggregateRootId, AggregateRootType, EventStreamPosition, Timestamp, Issuer, TransactionId);
