namespace Byteology.EventSourcing.EntityFramework.EventStorage;

using Byteology.EventSourcing.EventHandling;
using Microsoft.EntityFrameworkCore;

public sealed class DefaultEventStore : EventStoreBase<Event>
{
    public DefaultEventStore(
        DbContextOptions<DefaultEventStore> dbOptions, 
        SerializationRegistry<IEvent> eventSerializationRegistry) 
            : base(dbOptions, eventSerializationRegistry)
    {
    }
}
