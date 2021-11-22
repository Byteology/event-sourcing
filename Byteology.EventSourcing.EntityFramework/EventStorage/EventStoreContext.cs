﻿namespace Byteology.EventSourcing.EntityFramework.EventStorage;

using Byteology.EventSourcing.EventHandling;
using Byteology.EventSourcing.EventHandling.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

        EntityTypeBuilder<TEventEntity> entityTypeBuilder = modelBuilder.Entity<TEventEntity>();

        entityTypeBuilder
            .HasKey(nameof(IEventEntity.Id));

        entityTypeBuilder
            .HasIndex(nameof(IEventEntity.AggregateRootId));

        entityTypeBuilder
            .HasIndex(nameof(IEventEntity.AggregateRootId), nameof(IEventEntity.Sequence))
            .IsUnique();

        entityTypeBuilder
            .Property(nameof(IEventEntity.Type))
            .IsRequired();

        entityTypeBuilder
            .Property(nameof(IEventEntity.Payload))
            .IsRequired();
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
        // The events are immutable so we are avoiding the overhead of setting up the change tracker.
        IQueryable<TEventEntity> entitites = Set<TEventEntity>()
            .Where(e => e.AggregateRootId == aggregateRootId)
            .AsNoTracking();

        // By doing this we are streaming the events instead of loading them all into the memory.
        foreach (TEventEntity entity in entitites)
            yield return convertEntityToRecord(entity);
    }

    void IEventStoreContext.AddEvents(IEnumerable<IEventStreamRecord> eventStream)
    {
        Set<TEventEntity>().AddRange(eventStream.Select(e => CreateNewEntity(e)));
        SaveChanges();
    }
}