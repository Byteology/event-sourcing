namespace Byteology.EventSourcing;

public interface IPersistedEventMetadata : IEventMetadata
{
    ulong GlobalEventStreamPosition { get; }
}
