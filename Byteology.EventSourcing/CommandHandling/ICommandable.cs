namespace Byteology.EventSourcing.CommandHandling;

public interface ICommandable<in TCommand>
    where TCommand : ICommand
{
    void Execute(TCommand command, DateTimeOffset timestamp);
}
