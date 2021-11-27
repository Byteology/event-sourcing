namespace Byteology.EventSourcing.Storage;

public record EventRecord(IEvent Event, IEventMetadata Metadata);
