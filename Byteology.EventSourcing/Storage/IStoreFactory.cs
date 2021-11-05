namespace Byteology.EventSourcing.Storage
{
    public interface IStoreFactory
    {
        (IEventStore eventStore, IProjectionStore? projectionStore) CreateStores();
    }
}
