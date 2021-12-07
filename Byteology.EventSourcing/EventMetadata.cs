namespace Byteology.EventSourcing;

public record EventMetadata(
    Guid EventStreamId,
    Type AggregateRootType,
    ulong EventStreamPosition,
    DateTimeOffset Timestamp,
    string? Issuer,
    Guid TransactionId);
