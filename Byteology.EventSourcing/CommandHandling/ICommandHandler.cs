namespace Byteology.EventSourcing.CommandHandling;

using Byteology.EventSourcing.EventHandling;

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    IEnumerable<IEventContext> HandleCommand(TCommand command, DateTimeOffset timestamp);
}