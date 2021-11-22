namespace Byteology.EventSourcing.EventHandling.Storage;

using Byteology.EventSourcing.EventHandling;

public interface IEventStreamRecord
{
    IEvent Event { get; }
    DateTimeOffset EventTimestamp { get; }
    ulong EventSequence { get; }
}
