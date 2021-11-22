namespace Byteology.EventSourcing.EntityFramework;

using Byteology.EventSourcing.EventHandling;
using Byteology.EventSourcing.EventHandling.Storage;
using Microsoft.EntityFrameworkCore;

public class DefaultEventStore : IEventStore
{
    private readonly DbContextOptions _dbOptions;
    private readonly SerializationRegistry<IEvent> _eventSerializationRegistry;

    public DefaultEventStore(
        DbContextOptions dbOptions,
        SerializationRegistry<IEvent> eventSerializationRegistry)
    {
        _dbOptions = dbOptions;
        _eventSerializationRegistry = eventSerializationRegistry;
    }

    public IEventStoreContext CreateContext()
        => new EventStoreContext<Event>(_dbOptions, _eventSerializationRegistry);
}
