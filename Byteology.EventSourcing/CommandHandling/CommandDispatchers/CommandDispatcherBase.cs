namespace Byteology.EventSourcing.CommandHandling.CommandDispatchers;

using Byteology.EventSourcing.CommandHandling.CommandHandlerLocators;

public abstract class CommandDispatcherBase : ICommandDispatcher
{
    private readonly ICommandHandlerLocator _commandHandlerLocator;

    protected CommandDispatcherBase(ICommandHandlerLocator commandHandlerLocator)
    {
        _commandHandlerLocator = commandHandlerLocator;
    }

    public void Dispatch<TAggregateRoot>(ICommand<TAggregateRoot> command) 
        where TAggregateRoot : IAggregateRoot, new()
    {
        Type commandType = command.GetType();
        Type aggregateRootType = typeof(TAggregateRoot);

        object handler = getEventHandler(commandType, aggregateRootType);

        execute((dynamic)handler, (dynamic)command);
    }

    protected abstract string? GetIssuer();

    protected virtual DateTimeOffset GetTimestamp() => DateTimeOffset.Now;

    private object getEventHandler(Type commandType, Type aggregateRootType)
    {
        object handler = typeof(ICommandHandlerLocator)
            .GetMethod(nameof(ICommandHandlerLocator.Locate))!
            .MakeGenericMethod(commandType, aggregateRootType)!
            .Invoke(_commandHandlerLocator, null)!;

        return handler;
    }

    private void execute<TCommand, TAggregateRoot>(ICommandHandler<TCommand, TAggregateRoot> handler, TCommand command)
        where TCommand : ICommand<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot, new()
    {
        CommandMetadata metadata = new(GetIssuer(), GetTimestamp());
        handler.HandleCommand(command, metadata);
    }
}
