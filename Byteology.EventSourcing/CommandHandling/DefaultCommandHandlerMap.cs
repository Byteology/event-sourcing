namespace Byteology.EventSourcing.CommandHandling;

using Byteology.EventSourcing.Storage;

public class DefaultCommandHandlerMap : ICommandHandlerMap
{
    private readonly IEventStore _eventStore;

    public DefaultCommandHandlerMap(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public virtual ICommandHandler<TCommand> GetCommandHandler<TCommand>(TCommand command)
        where TCommand : ICommand =>
            new DefaultCommandHandler<TCommand>(_eventStore);
}
