namespace Byteology.EventSourcing.EventStorage;

public record EventRecord(IEvent Event, EventMetadata Metadata);
