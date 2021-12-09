namespace Byteology.EventSourcing.EntityFramework;

using Byteology.EventSourcing.EventStorage;
using Microsoft.EntityFrameworkCore;

public class EfEventStore : DbContext, IEventStore
{
    private readonly TypeRegistry<IEvent> _eventTypesRegistry;
    private readonly ISerializer<IEvent> _eventSerializer;

    public EfEventStore(
        DbContextOptions<EfEventStore> dbOptions,
        TypeRegistry<IEvent> eventTypesRegistry,
        ISerializer<IEvent> eventSerializer) : base(dbOptions)
    {
        _eventTypesRegistry = eventTypesRegistry;
        _eventSerializer = eventSerializer;
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
        string typeName = _eventTypesRegistry.GetTypeName(record.Event.GetType());
        string serializedEvent = _eventSerializer.Serialize(record.Event);

        return new Event()
        {
            StreamId = record.Metadata.EventStreamId,
            Issuer = record.Metadata.Issuer,
            StreamPosition = record.Metadata.EventStreamPosition,
            Timestamp = record.Metadata.Timestamp,
            TransactionId = record.Metadata.TransactionId,
            Type = typeName,
            Payload = serializedEvent
        };
    }

    private EventRecord convertEntityToRecord(Event entity)
    {
        Type type = _eventTypesRegistry.GetTypeByName(entity.Type);
        IEvent @event = _eventSerializer.Deserialize(type, entity.Payload);

        EventMetadata metadata = new(entity.StreamId, entity.StreamPosition, 
            entity.Timestamp, entity.Issuer, entity.TransactionId);

        return new EventRecord(@event, metadata);
    }
}
