using CorpriTech.Milton.Abstractions;
using CorpriTech.Milton.Extensions.Microsoft.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable CS1591
public static class ServiceCollectionExtensions
#pragma warning restore CS1591
{
    /// <summary>
    /// Adds Milton to the service collection, allowing instances of <see cref="IState{TState}"/> and <see cref="IStateAccessor{TState}"/> to be injected.
    /// </summary>
    /// <remarks>
    /// <see cref="IState{TState}"/> instances are singleton instances.
    /// </remarks>
    /// <param name="serviceCollection">The service collection Milton will be added to.</param>
    /// <param name="stateLifetime">The lifetime for injected state containers.</param>
    /// <returns>The original <see cref="IServiceCollection"/> instance so that additional calls may be chained.</returns>
    public static IServiceCollection AddMilton(this IServiceCollection serviceCollection, ServiceLifetime stateLifetime = ServiceLifetime.Scoped)
    {
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IState<>), typeof(InjectedState<>), stateLifetime));
        serviceCollection.TryAddSingleton(typeof(IStateAccessor<>), typeof(InjectedStateAccessor<>));

        return serviceCollection;
    }
}