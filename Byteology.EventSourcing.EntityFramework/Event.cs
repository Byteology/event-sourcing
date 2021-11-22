namespace Byteology.EventSourcing.EntityFramework;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// Entity Framework is oblivious of NRT

public class Event : IEventEntity
{
    public Guid Id { get; set; }
    public Guid AggregateRootId { get; set; }
    public ulong Sequence { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Type { get; set; }
    public string Payload { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

