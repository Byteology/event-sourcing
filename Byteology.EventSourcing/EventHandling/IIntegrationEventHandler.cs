using System.Threading.Tasks;

namespace Byteology.EventSourcing.EventHandling
{
    public interface IIntegrationEventHandler
    {
        Task HandleAsync(IEventContext context);
    }

    public interface IIntegrationEventHandler<TEvent, TAggregate> : IIntegrationEventHandler
        where TEvent : class, IEvent<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
        Task HandleAsync(EventContext<TEvent, TAggregate> context);

        async Task IIntegrationEventHandler.HandleAsync(IEventContext context)
        {
            if (context is EventContext<TEvent, TAggregate> castedContext)
                await HandleAsync(castedContext).ConfigureAwait(false);
        }
    }
}
