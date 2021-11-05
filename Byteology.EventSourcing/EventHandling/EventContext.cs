using Byteology.GuardClauses;
using System;
using System.Collections.Generic;

namespace Byteology.EventSourcing.EventHandling
{
    public interface IEventContext
    {
        IAggregateRoot? Aggregate { get; }
        IList<IEvent> EventsToRaise { get; }
    }

    public sealed class EventContext<TEvent, TAggregate> : IEventContext
        where TEvent : class, IEvent
        where TAggregate : class, IAggregateRoot
    {
        public TEvent Event { get; }
        public TAggregate? Aggregate { get; set; }
        public IList<IEvent> EventsToRaise { get; } = new List<IEvent>();

        public EventContext(TEvent @event, TAggregate? aggregate)
        {
            Guard.Argument(@event, nameof(@event)).NotNull();

            Event = @event;
            Aggregate = aggregate;
        }

        IAggregateRoot? IEventContext.Aggregate => Aggregate;
    }
}
