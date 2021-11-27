namespace Byteology.EventSourcing.Storage;

public record PersistedEventRecord(IEvent Event, IPersistedEventMetadata Metadata);
