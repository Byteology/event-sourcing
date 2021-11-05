using Byteology.EventSourcing.EventHandling;
using System;

namespace Byteology.EventSourcing
{
    public interface IEvent 
    { 
        public Guid AggregateId { get; }
        public Type AggregateType { get; }

        public IEventContext CreateContext(IAggregateRoot? aggregate)
        {
            Type contextType = typeof(EventContext<,>).MakeGenericType(this.GetType(), AggregateType);

            IEventContext? result = Activator.CreateInstance(contextType, this, aggregate) as IEventContext;
            return result ?? throw new InvalidOperationException("Unable to create event context.");
        }
    }

    public interface IEvent<TAggregate> : IEvent
        where TAggregate : class, IAggregateRoot
    {
        Type IEvent.AggregateType => typeof(TAggregate);
    }
}
