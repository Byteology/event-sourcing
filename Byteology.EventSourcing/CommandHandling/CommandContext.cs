namespace Byteology.EventSourcing.CommandHandling;

public sealed class CommandContext<TCommand>
    where TCommand : ICommand
{
    public TCommand Command { get; }
    public DateTimeOffset CommandTimestamp { get; }

    public CommandContext(TCommand command, DateTimeOffset commandTimestamp)
    {
        Command = command;
        CommandTimestamp = commandTimestamp;
    }
}
