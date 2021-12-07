namespace Byteology.EventSourcing.CommandHandling;

public interface ICommandHandler<TCommand, in TAggregateRoot>
    where TCommand : ICommand<TAggregateRoot>
    where TAggregateRoot : IAggregateRoot, new()
{
    void HandleCommand(TCommand command, CommandMetadata metadata, TAggregateRoot aggregateRoot);
}
