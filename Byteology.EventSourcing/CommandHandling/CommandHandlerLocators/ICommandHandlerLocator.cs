namespace Byteology.EventSourcing.CommandHandling.CommandHandlerLocators;

public interface ICommandHandlerLocator
{
    ICommandHandler<TCommand, TAggregateRoot> Locate<TCommand, TAggregateRoot>()
        where TCommand : ICommand<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot, new();
}
