namespace Byteology.EventSourcing.CommandHandling;

public interface ICommand
{
    Guid AggregateRootId { get; }
    Type AggregateRootType { get; }
}

public interface ICommand<TAggregateRoot> : ICommand
    where TAggregateRoot : IAggregateRoot, new()
{
    Type ICommand.AggregateRootType => typeof(TAggregateRoot);
}
