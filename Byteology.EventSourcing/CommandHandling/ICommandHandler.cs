namespace Byteology.EventSourcing.CommandHandling;

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    void HandleCommand(CommandContext<TCommand> context);
}