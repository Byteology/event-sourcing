namespace Byteology.EventSourcing.EventStorage;

public record EventMetadata(
    Guid EventStreamId,
    Type AggregateRootType,
    ulong EventStreamPosition,
    DateTimeOffset Timestamp,
    string? Issuer,
    Guid TransactionId);
