namespace Byteology.EventSourcing.EventHandling
{
    public interface IDomainEventHandler
    {
        void Handle(IEventContext context);
    }

    public interface IDomainEventHandler<TEvent, TAggregate> : IDomainEventHandler
        where TEvent : class, IEvent<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
        void Handle(EventContext<TEvent, TAggregate> context);

        void IDomainEventHandler.Handle(IEventContext context)
        {
            if (context is EventContext<TEvent, TAggregate> castedContext)
                Handle(castedContext);
        }
    }
}
