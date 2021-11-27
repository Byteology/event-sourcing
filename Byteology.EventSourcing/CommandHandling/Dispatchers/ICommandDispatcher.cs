namespace Byteology.EventSourcing.CommandHandling.Dispatchers;

using Byteology.EventSourcing.CommandHandling;

public interface ICommandDispatcher
{
    void Dispatch<TCommand>(TCommand command)
        where TCommand : ICommand;
}
