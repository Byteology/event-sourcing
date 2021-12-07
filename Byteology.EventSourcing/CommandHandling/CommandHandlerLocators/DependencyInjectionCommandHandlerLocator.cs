namespace Byteology.EventSourcing.CommandHandling.CommandHandlerLocators;

using Microsoft.Extensions.DependencyInjection;

public class DependencyInjectionCommandhandlerLocator : ICommandHandlerLocator
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyInjectionCommandhandlerLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICommandHandler<TCommand, TAggregateRoot> Locate<TCommand, TAggregateRoot>()
        where TCommand : ICommand<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot, new() =>
            _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TAggregateRoot>>();
}
