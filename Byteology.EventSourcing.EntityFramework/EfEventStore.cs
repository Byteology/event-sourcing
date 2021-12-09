namespace Byteology.EventSourcing.EntityFramework;

using Byteology.EventSourcing.Configuration;
using Byteology.EventSourcing.EventStorage;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class EfEventStore : DbContext, IEventStore
{
    private readonly TypeRegistry<IEvent> _eventTypesRegistry;
    private readonly JsonSerializerOptions _serializerOptions;

    public EfEventStore(
        DbContextOptions<EfEventStore> dbOptions,
        TypeRegistry<IEvent> eventTypesRegistry,
        JsonSerializerOptions? serializerOptions = null) : base(dbOptions)
    {
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
                .HasIndex(nameof(Event.StreamId));

            builder
                .HasIndex(nameof(Event.StreamId), nameof(Event.StreamPosition))
                .IsUnique();

            builder
                .Property(nameof(Event.Type))
                .IsRequired();

            builder
                .Property(nameof(Event.Payload))
                .IsRequired();
        });
    }

    public IEnumerable<EventRecord> AddEvents(IEnumerable<EventRecord> events)
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

        return events;
    }

    public IEnumerable<EventRecord> GetEventStream(Guid id)
    {
        // The events are immutable so we are avoiding the overhead of setting up the change tracker.
        IQueryable<Event> entitites = Set<Event>()
            .Where(e => e.StreamId == id)
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
            StreamId = record.Metadata.EventStreamId,
            Issuer = record.Metadata.Issuer,
            Payload = JsonSerializer.Serialize(record.Event, _serializerOptions),
            StreamPosition = record.Metadata.EventStreamPosition,
            Timestamp = record.Metadata.Timestamp,
            TransactionId = record.Metadata.TransactionId,
            Type = _eventTypesRegistry.GetTypeName(record.Event.GetType())
        };
    }

    private EventRecord convertEntityToRecord(Event entity)
    {
        Type eventType = _eventTypesRegistry.GetTypeByName(entity.Type);
        IEvent @event = (JsonSerializer.Deserialize(entity.Payload, eventType) as IEvent)!;

        EventMetadata metadata = new(entity.StreamId, entity.StreamPosition, 
            entity.Timestamp, entity.Issuer, entity.TransactionId);

        return new EventRecord(@event, metadata);
    }
}
