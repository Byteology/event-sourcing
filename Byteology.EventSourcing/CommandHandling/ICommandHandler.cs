namespace Byteology.EventSourcing.CommandHandling;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    void HandleCommand(TCommand command, CommandMetadata metadata);
}
