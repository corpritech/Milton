using Microsoft.Extensions.DependencyInjection;
using Milton.Abstractions;

namespace Milton.Extensions.Microsoft.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddState(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IStateFactory, StateFactory>();
        serviceCollection.AddSingleton(typeof(IState<>), typeof(StateProxy<>));

        return serviceCollection;
    }
}