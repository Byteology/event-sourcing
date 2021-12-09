namespace Byteology.EventSourcing;

public record EventMetadata(
    Guid EventStreamId,
    ulong EventStreamPosition,
    DateTimeOffset Timestamp,
    string? Issuer,
    Guid TransactionId);
