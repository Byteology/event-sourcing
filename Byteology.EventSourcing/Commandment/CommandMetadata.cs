namespace Byteology.EventSourcing.Commandment;

public record CommandMetadata(string? Issuer, DateTimeOffset Timestamp);
