namespace Byteology.EventSourcing;

public record PersistedEventRecord(IEvent Event, IPersistedEventMetadata Metadata);
