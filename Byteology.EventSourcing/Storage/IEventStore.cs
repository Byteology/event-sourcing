using System;
using System.Collections.Generic;

namespace Byteology.EventSourcing.Storage
{
    public interface IEventStore : IDataStore
    {
        void AddEvent(IEvent @event, long aggregateVersion);

        (IAggregateRoot? aggregate, long version)? GetLatestSnapshot(Guid aggregateId);
        (IEnumerable<IEvent> events, long lastVersion)? GetOrderedEvents(Guid aggregateId, long? minVersion);
    }
}
