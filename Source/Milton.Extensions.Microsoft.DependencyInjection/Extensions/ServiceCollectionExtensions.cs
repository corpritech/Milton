using Microsoft.Extensions.DependencyInjection;
using Milton.Abstractions;

namespace Milton.Extensions.Microsoft.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddState(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(typeof(IState<>), typeof(State<>));

        return serviceCollection;
    }
}