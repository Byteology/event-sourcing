namespace Byteology.EventSourcing.CommandHandling;

public interface ICommand
{
    Guid AggregateRootId { get; }
    Type AggregateType { get; }
}

public interface ICommand<TAggregateRoot> : ICommand
    where TAggregateRoot : IAggregateRoot, new()
{
    Type ICommand.AggregateType => typeof(TAggregateRoot);
}
