using CorpriTech.Milton.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CorpriTech.Milton.Extensions.Microsoft.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddState(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(typeof(IState<>), typeof(State<>));

        return serviceCollection;
    }
}