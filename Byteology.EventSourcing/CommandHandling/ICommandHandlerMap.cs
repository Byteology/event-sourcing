namespace Byteology.EventSourcing.CommandHandling;

public interface ICommandHandlerMap
{
    ICommandHandler<TCommand> GetCommandHandler<TCommand>(TCommand command)
        where TCommand : ICommand;
}
