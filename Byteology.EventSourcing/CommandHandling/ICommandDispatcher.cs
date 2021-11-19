namespace Byteology.EventSourcing.CommandHandling;

public interface ICommandDispatcher
{
    void Dispatch<TCommand>(TCommand command)
        where TCommand : ICommand;
}
