namespace Byteology.EventSourcing;

using Byteology.EventSourcing.Storage;

public class AggregateRootFactory
{
    private readonly IEventStore _eventStore;

    public AggregateRootFactory(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public IAggregateRoot Build(Guid id, Type type)
    {
        IAggregateRoot? root = null;
        try
        {
            root = Activator.CreateInstance(type) as IAggregateRoot;
        }
        catch { /* Handled bellow */ }

        if (root == null)
            throw new ArgumentException($"The type of the aggregate root should implement '{typeof(IAggregateRoot)}' and should have a public parameterless constructor.");

        root.Id = id;

        IEnumerable<PersistedEventRecord> eventStream = _eventStore.GetEventStream(id);
        foreach (PersistedEventRecord record in eventStream)
            root.ReplayEvent(record);

        return root;
    }

    public TAggregateRoot Build<TAggregateRoot>(Guid id)
        where TAggregateRoot : IAggregateRoot, new()
    {
        return (TAggregateRoot)Build(id, typeof(TAggregateRoot));
    }

}
