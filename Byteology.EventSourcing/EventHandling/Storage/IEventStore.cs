namespace Byteology.EventSourcing.EventHandling.Storage;

public interface IEventStore
{
    IEventStoreContext CreateContext();
}
