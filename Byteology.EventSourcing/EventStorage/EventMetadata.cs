namespace Byteology.EventSourcing.EventStorage;

public record EventMetadata(
    Guid AggregateRootId,
    Type AggregateRootType,
    ulong EventStreamPosition,
    DateTimeOffset Timestamp,
    string? Issuer,
    Guid TransactionId);
