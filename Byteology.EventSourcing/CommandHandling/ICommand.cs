namespace Byteology.EventSourcing.CommandHandling;

public interface ICommand<TAggregateRoot>
    where TAggregateRoot : IAggregateRoot, new()
{
    Guid AggregateRootId { get; }
}
