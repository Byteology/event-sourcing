namespace Byteology.EventSourcing.EntityFramework.EventStorage;

public interface IEventEntity
{
    Guid Id { get; set; }
    Guid AggregateRootId { get; set; }
    ulong Sequence { get; set; }
    DateTimeOffset Timestamp { get; set; }
    public string Type { get; set; }
    public string Payload { get; set; }
}
