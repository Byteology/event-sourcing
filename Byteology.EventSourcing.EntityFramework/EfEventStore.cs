namespace Byteology.EventSourcing.EntityFramework;

using Byteology.EventSourcing.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class EfEventStore : DbContext, IEventStore
{
    private readonly TypeRegistry<IAggregateRoot> _aggregateRootTypesRegistry;
    private readonly TypeRegistry<IEvent> _eventTypesRegistry;
    private readonly JsonSerializerOptions _serializerOptions;

    public EfEventStore(
        DbContextOptions dbOptions,
        TypeRegistry<IAggregateRoot> aggregateRootTypesRegistry,
        TypeRegistry<IEvent> eventTypesRegistry,
        JsonSerializerOptions? serializerOptions = null) : base(dbOptions)
    {
        _aggregateRootTypesRegistry = aggregateRootTypesRegistry;
        _eventTypesRegistry = eventTypesRegistry;
        _serializerOptions = serializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Event>(builder =>
        {
            builder
                .HasKey(nameof(Event.Id));

            builder
                .HasIndex(nameof(Event.AggregateRootId));

            builder
                .HasIndex(nameof(Event.AggregateRootId), nameof(Event.StreamPosition))
                .IsUnique();

            builder
                .Property(nameof(Event.AggregateRootType))
                .IsRequired();

            builder
                .Property(nameof(Event.Type))
                .IsRequired();

            builder
                .Property(nameof(Event.Payload))
                .IsRequired();
        });
    }

    public void AddEvents(IEnumerable<EventRecord> events)
    {
        try
        {
            Set<Event>().AddRange(events.Select(e => createNewEntity(e)));
            SaveChanges();
        }
        finally
        {
            ChangeTracker.Clear();
        }
    }

    public IEnumerable<PersistedEventRecord> GetEventStream(Guid aggregateRootId)
    {
        // The events are immutable so we are avoiding the overhead of setting up the change tracker.
        IQueryable<Event> entitites = Set<Event>()
            .Where(e => e.AggregateRootId == aggregateRootId)
            .OrderBy(e => e.StreamPosition)
            .AsNoTracking();

        // By doing this we are streaming the events instead of loading them all into the memory.
        foreach (Event entity in entitites)
            yield return convertEntityToRecord(entity);
    }

    private Event createNewEntity(EventRecord record)
    {
        return new Event()
        {
            AggregateRootId = record.Metadata.AggregateRootId,
            AggregateRootType = _aggregateRootTypesRegistry.GetTypeName(record.Metadata.AggregateRootType),
            Issuer = record.Metadata.Issuer,
            Payload = JsonSerializer.Serialize(record.Event, _serializerOptions),
            StreamPosition = record.Metadata.EventStreamPosition,
            Timestamp = record.Metadata.Timestamp,
            TransactionId = record.Metadata.TransactionId,
            Type = _eventTypesRegistry.GetTypeName(record.Event.GetType())
        };
    }

    private PersistedEventRecord convertEntityToRecord(Event entity)
    {
        Type eventType = _eventTypesRegistry.GetTypeByName(entity.Type);
        IEvent @event = (JsonSerializer.Deserialize(entity.Payload, eventType) as IEvent)!;

        Metadata metadata = new(entity.Id, entity.AggregateRootId, _aggregateRootTypesRegistry.GetTypeByName(entity.AggregateRootType), entity.StreamPosition, entity.Timestamp, entity.Issuer, entity.TransactionId);

        return new PersistedEventRecord(@event, metadata);
    }

    private sealed record Metadata(ulong GlobalEventStreamPosition, Guid AggregateRootId, Type AggregateRootType, ulong EventStreamPosition, DateTimeOffset Timestamp, string ? Issuer, Guid TransactionId) : IPersistedEventMetadata;
}
