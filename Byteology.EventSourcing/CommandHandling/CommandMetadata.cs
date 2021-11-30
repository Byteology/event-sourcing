namespace Byteology.EventSourcing.CommandHandling;

public record CommandMetadata(string? Issuer, DateTimeOffset Timestamp);
