namespace Byteology.EventSourcing;

public record EventRecord(IEvent Event, IEventMetadata Metadata);
