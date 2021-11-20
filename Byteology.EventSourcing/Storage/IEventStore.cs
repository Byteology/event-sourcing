namespace Byteology.EventSourcing.Storage;

public interface IEventStore
{
    IEventStoreContext CreateContext();
}
