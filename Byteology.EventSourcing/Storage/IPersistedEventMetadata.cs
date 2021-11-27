namespace Byteology.EventSourcing.Storage;

public interface IPersistedEventMetadata : IEventMetadata
{
    ulong GlobalEventStreamPosition { get; }
}
