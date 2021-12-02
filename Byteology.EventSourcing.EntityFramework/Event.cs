namespace Byteology.EventSourcing.EntityFramework;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// Entity Framework is oblivious of NRT

internal class Event
{
    public ulong GlobalStreamPosition { get; set; }
    public Guid StreamId { get; set; }
    public string AggregateRootType { get; set; }
    public ulong StreamPosition { get; set; }

    public string Type { get; set; }
    public string Payload { get; set; }

    public Guid TransactionId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string? Issuer { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
