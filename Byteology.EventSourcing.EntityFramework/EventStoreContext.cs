namespace Byteology.EventSourcing.EntityFramework;

using Byteology.EventSourcing.EventHandling;
using Byteology.EventSourcing.Storage;
using Microsoft.EntityFrameworkCore;

public abstract class EventStoreContext<TEventContext> : DbContext, IEventStoreContext
    where TEventContext : class, IEventContext
{
    protected EventStoreContext(DbContextOptions options) : base(options) { }

    protected abstract TEventContext CreateNewRecord(IEventContext eventContext);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TEventContext>();
    }

    IEnumerable<IEventContext> IEventStoreContext.GetEventStream(Guid aggregateRootId)
        => Set<TEventContext>().Where(e => e.AggregateId == aggregateRootId).ToList();

    void IEventStoreContext.AddEvents(IEnumerable<IEventContext> eventStream)
    {
        Set<TEventContext>().AddRange(eventStream.Select(e => CreateNewRecord(e)));
        SaveChanges();
    }
}
