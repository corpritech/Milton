using Microsoft.Extensions.DependencyInjection;
using Milton.Abstractions;

namespace Milton.Extensions.Microsoft.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddState(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IStateContainerRegistry, StateContainerRegistry>();
        serviceCollection.AddSingleton<IStateContainerFactory, StateContainerFactory>();
        serviceCollection.AddSingleton(typeof(IStateContainer<>), typeof(StateContainerProxy<>));

        return serviceCollection;
    }
}