using Byteology.EventSourcing.Storage;

namespace Byteology.EventSourcing.EventHandling
{
    public interface IProjectionEventHandler
    {
        void Handle(IEvent @event, IProjectionStore projectionStore);
    }
}
