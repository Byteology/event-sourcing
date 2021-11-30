namespace Byteology.EventSourcing.CommandHandling.CommandDispatchers;

public interface ICommandDispatcher
{
    void Dispatch<TAggregateRoot>(ICommand<TAggregateRoot> command)
        where TAggregateRoot : IAggregateRoot, new();
}
