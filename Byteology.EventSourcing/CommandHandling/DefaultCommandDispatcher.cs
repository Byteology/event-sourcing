namespace Byteology.EventSourcing.CommandHandling;

using Byteology.EventSourcing.EventHandling;

public class DefaultCommandDispatcher : ICommandDispatcher
{
    private readonly ICommandHandlerMap _commandHandlerMap;
    private readonly Func<DateTimeOffset> _dateTimeProvider;
    private readonly IEnumerable<IEventListener> _eventListeners;

    public DefaultCommandDispatcher(
        ICommandHandlerMap commandHandlerMap,
        Func<DateTimeOffset> dateTimeProvider,
        IEnumerable<IEventListener> eventListeners)
    {
        _commandHandlerMap = commandHandlerMap;
        _dateTimeProvider = dateTimeProvider;
        _eventListeners = eventListeners;
    }

    public virtual void Dispatch<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        ICommandHandler<TCommand> handler = _commandHandlerMap.GetCommandHandler(command);

        IEnumerable<IEventContext> eventStream = handler.HandleCommand(command, _dateTimeProvider());

        foreach (IEventContext eventContext in eventStream)
            foreach (IEventListener eventListener in _eventListeners)
                try
                {
                    _ = eventListener.OnEventAsync(eventContext);
                }
                catch { /* We want to notify all listeners regardless of one failing. */ }
    }
}
