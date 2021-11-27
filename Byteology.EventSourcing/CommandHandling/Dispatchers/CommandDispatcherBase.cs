namespace Byteology.EventSourcing.CommandHandling.Dispatchers;

using Byteology.EventSourcing.CommandHandling;

public abstract class CommandDispatcherBase : ICommandDispatcher
{
    public void Dispatch<TCommand>(TCommand command) 
        where TCommand : ICommand
    {
        CommandMetadata metadata = CreateCommandMetadata();
        ICommandHandler<TCommand> handler = GetCommandHandler(command);
        handler.HandleCommand(command, metadata);
    }

    protected abstract CommandMetadata CreateCommandMetadata();
    protected abstract ICommandHandler<TCommand> GetCommandHandler<TCommand>(TCommand command)
        where TCommand : ICommand;
}
