namespace Byteology.EventSourcing.CommandHandling;

public interface ICommandDispatcher
{
    void Dispatch<TAggregateRoot>(ICommand<TAggregateRoot> command)
        where TAggregateRoot : IAggregateRoot, new();
}
