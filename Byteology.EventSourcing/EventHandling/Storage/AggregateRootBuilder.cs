namespace Byteology.EventSourcing.EventHandling.Storage;

public class AggregateRootBuilder
{
    private readonly IEventStoreContext _eventStoreContext;

    public AggregateRootBuilder(IEventStoreContext eventStoreContext)
    {
        _eventStoreContext = eventStoreContext;
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
            throw new ArgumentException($"The type of the aggregate root should implement '{typeof(IAggregateRoot).Name}' and should have a public parameterless constructor.");

        root.Id = id;

        IEnumerable<IEventStreamRecord> eventStream = _eventStoreContext.GetEventStream(id);
        foreach (IEventStreamRecord record in eventStream)
            root.ReplayEvent(record);

        return root;
    }

    public TAggregateRoot Build<TAggregateRoot>(Guid id)
        where TAggregateRoot : IAggregateRoot, new()
    {
        return (TAggregateRoot)Build(id, typeof(TAggregateRoot));
    }

}
