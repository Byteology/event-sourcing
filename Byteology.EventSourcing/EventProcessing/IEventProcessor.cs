using System.Collections.Generic;
using System.Threading.Tasks;

namespace Byteology.EventSourcing.EventProcessing
{
    public interface IEventProcessor
    {
        void Process(IEnumerable<IEvent> events);
        void Process(IEnumerable<IEvent> events, out Task integrationEventHandlersTask);
    }
}
