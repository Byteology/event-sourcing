namespace Byteology.EventSourcing.EntityFramework;

using Byteology.EventSourcing.EventHandling;
using Byteology.EventSourcing.EventHandling.Storage;
using Microsoft.EntityFrameworkCore;

public class EventStoreContext<TEventEntity> : DbContext, IEventStoreContext
    where TEventEntity : class, IEventEntity, new()
{
    private readonly SerializationRegistry<IEvent> _eventSerializationRegistry;

    public EventStoreContext(
        DbContextOptions dbOptions,
        SerializationRegistry<IEvent> eventSerializationRegistry) : base(dbOptions) 
    {
        _eventSerializationRegistry = eventSerializationRegistry;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TEventEntity>()
            .HasKey(nameof(IEventEntity.Id));

        modelBuilder.Entity<TEventEntity>()
            .HasIndex(nameof(IEventEntity.AggregateRootId), nameof(IEventEntity.Sequence))
            .IsUnique();
    }

    protected virtual TEventEntity CreateNewEntity(IEventStreamRecord eventContext)
    {
        TEventEntity entity = new ();
        entity.AggregateRootId = eventContext.AggregateRootId;
        entity.Sequence = eventContext.EventSequence;
        entity.Timestamp = eventContext.EventTimestamp;
        entity.Payload = _eventSerializationRegistry.Serialize(eventContext.Event);

        return entity;
    }

    private IEventStreamRecord convertEntityToRecord(TEventEntity entity)
    {
        IEvent @event = _eventSerializationRegistry.Deserialize(entity.Type, entity.Payload);
        EventStreamRecord record = new(entity.AggregateRootId, @event, entity.Timestamp, entity.Sequence);
        return record;
    }

    IEnumerable<IEventStreamRecord> IEventStoreContext.GetEventStream(Guid aggregateRootId)
    {
        return Set<TEventEntity>()
            .Where(e => e.AggregateRootId == aggregateRootId)
            .AsEnumerable()
            .Select(e => convertEntityToRecord(e))
            .ToList();
    }

    void IEventStoreContext.AddEvents(IEnumerable<IEventStreamRecord> eventStream)
    {
        Set<TEventEntity>().AddRange(eventStream.Select(e => CreateNewEntity(e)));
        SaveChanges();
    }
}