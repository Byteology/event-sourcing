using Byteology.EventSourcing.EventHandling;
using Byteology.EventSourcing.Storage;
using Byteology.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Byteology.EventSourcing.EventProcessing
{
    public class LiveEventProcessor : IEventProcessor
    {
        private readonly IStoreFactory _storeFactory;
        private readonly IEnumerable<IDomainEventHandler>? _domainEventHandlers;
        private readonly IEnumerable<IIntegrationEventHandler>? _integrationEventHandlers;
        private readonly IEnumerable<IProjectionEventHandler>? _projectionEventHandlers;

        public LiveEventProcessor(
            IStoreFactory storeFactory,
            IEnumerable<IDomainEventHandler>? domainEventHandlers,
            IEnumerable<IIntegrationEventHandler>? integrationEventHandlers,
            IEnumerable<IProjectionEventHandler>? projectionEventHandlers)
        {
            Guard.Argument(storeFactory, nameof(storeFactory))
                .NotNull();

            Guard.Argument(domainEventHandlers, nameof(domainEventHandlers))
                .AllElements(e => e.NotNull());

            Guard.Argument(integrationEventHandlers, nameof(integrationEventHandlers))
                .AllElements(e => e.NotNull());

            Guard.Argument(projectionEventHandlers, nameof(projectionEventHandlers))
                .AllElements(e => e.NotNull());

            _storeFactory = storeFactory;
            _domainEventHandlers = domainEventHandlers;
            _integrationEventHandlers = integrationEventHandlers;
            _projectionEventHandlers = projectionEventHandlers;
        }

        public void Process(IEnumerable<IEvent> events) => Process(events, out Task _);

        public void Process(IEnumerable<IEvent> events, out Task integrationEventHandlersTask)
        {
            List<IEvent> eventsList = events?.Where(e => e != null)?.ToList() ?? new();

            Dictionary<Guid, IAggregateRoot?> cachedAggregates = new();
            Dictionary<Guid, long> lastAggregateVersions = new();

            (IEventStore eventStore, IProjectionStore? projectionStore) = _storeFactory.CreateStores();

            try
            {
                Guard.Argument(eventStore, nameof(eventStore)).NotNull();

                for (int i = 0; i < eventsList.Count; i++)
                {
                    IEvent @event = eventsList[i];

                    buildAndCacheAggregateIfNeeded(@event.AggregateId);
                    applyDomainHandlers(@event, i);
                    addEventToStore(@event);
                }

                commitEventStoreIfAsynchronous();

                if (projectionStore != null)
                {
                    foreach (IEvent @event in eventsList)
                        applyProjectionHandlers(@event);

                    projectionStore.Commit();
                }
            }
            finally
            {
                disposeStores();
            }

            integrationEventHandlersTask = applyIntegrationHandlersTask();

            return;

            void buildAndCacheAggregateIfNeeded(Guid aggregateId)
            {
                bool isCached = cachedAggregates.ContainsKey(aggregateId);
                if (!isCached)
                {
                    (IAggregateRoot? aggregate, long version)? snapshot = eventStore.GetLatestSnapshot(aggregateId);

                    IAggregateRoot? aggregate = snapshot?.aggregate;
                    long aggregateVersion = snapshot?.version ?? 0;

                    (IEnumerable<IEvent> eventsToApply, long aggregateVersion)? eventsQueryResult =
                            eventStore.GetOrderedEvents(aggregateId, snapshot?.version + 1);

                    if (eventsQueryResult.HasValue)
                    {
                        foreach (IEvent @event in eventsQueryResult.Value.eventsToApply)
                            if (_domainEventHandlers != null)
                                foreach (IDomainEventHandler handler in _domainEventHandlers)
                                {
                                    IEventContext context = @event.CreateContext(aggregate);
                                    handler.Handle(context);
                                    aggregate = context.Aggregate;
                                }

                        aggregateVersion = eventsQueryResult.Value.aggregateVersion;
                    }

                    cachedAggregates.Add(aggregateId, aggregate);
                    lastAggregateVersions.Add(aggregateId, aggregateVersion);
                }
            }

            void applyDomainHandlers(IEvent @event, int eventIndex)
            {
                IEventContext context = @event.CreateContext(cachedAggregates[@event.AggregateId]);

                if (_domainEventHandlers != null)
                    foreach (IDomainEventHandler handler in _domainEventHandlers)
                        handler.Handle(context);

                cachedAggregates[@event.AggregateId] = context.Aggregate;
                eventsList.InsertRange(eventIndex + 1, context.EventsToRaise);
            }

            void addEventToStore(IEvent @event)
            {
                eventStore.AddEvent(@event, ++lastAggregateVersions[@event.AggregateId]);
            }

            void commitEventStoreIfAsynchronous()
            {
                if (!eventStore.Equals(projectionStore))
                    eventStore.Commit();
            }

            void applyProjectionHandlers(IEvent @event)
            {
                if (_projectionEventHandlers != null && _projectionEventHandlers.Any())
                    foreach (IProjectionEventHandler handler in _projectionEventHandlers)
                        handler.Handle(@event, projectionStore);
            }

            async Task applyIntegrationHandlersTask()
            {
                List<Task> tasks = new();

                if (_integrationEventHandlers != null && _integrationEventHandlers.Any())
                    foreach (IEvent @event in eventsList)
                        foreach (IIntegrationEventHandler handler in _integrationEventHandlers)
                            tasks.Add(applyIntegrationHandlerAsync(@event, handler));

                await Task.WhenAll(tasks);

                IEnumerable<Exception> exceptions = tasks.Where(t => t.Exception != null).Select(t => t.Exception!);

                if (exceptions.Any())
                    throw new AggregateException("Processing of some of the integration event handlers has failed.", exceptions);
            }

            async Task applyIntegrationHandlerAsync(IEvent @event, IIntegrationEventHandler handler)
            {
                IEventContext context = @event.CreateContext(cachedAggregates[@event.AggregateId]);
                await handler.HandleAsync(context).ConfigureAwait(false);

                IEnumerable<IEvent> eventsToRaise = context.EventsToRaise.Where(e => e != null);
                if (eventsToRaise.Any())
                {
                    Process(eventsToRaise, out Task integrationEventHandlersTask);
                    await integrationEventHandlersTask.ConfigureAwait(false);
                }
            }

            void disposeStores()
            {
                if (eventStore != null && !eventStore.Equals(projectionStore))
                    eventStore.Dispose();

                projectionStore?.Dispose();
            }
        }
    }
}
