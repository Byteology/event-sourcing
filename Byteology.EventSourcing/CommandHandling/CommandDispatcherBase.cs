namespace Byteology.EventSourcing.CommandHandling;

public abstract class CommandDispatcherBase : ICommandDispatcher
{
    private readonly Func<DateTimeOffset> _dateTimeProvider;

    protected CommandDispatcherBase(Func<DateTimeOffset> dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public void Dispatch<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        ICommandHandler<TCommand> handler = GetCommandHandler(command);
        CommandContext<TCommand> context = new(command, _dateTimeProvider());
        handler.HandleCommand(context);
    }

    protected abstract ICommandHandler<TCommand> GetCommandHandler<TCommand>(TCommand command)
        where TCommand : ICommand;
}
