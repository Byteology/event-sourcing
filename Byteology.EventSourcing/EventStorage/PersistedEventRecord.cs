namespace Byteology.EventSourcing.EventStorage;

public record PersistedEventRecord(IEvent Event, PersistedEventMetadata Metadata);
