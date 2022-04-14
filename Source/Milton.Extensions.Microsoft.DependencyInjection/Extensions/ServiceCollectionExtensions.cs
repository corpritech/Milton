using CorpriTech.Milton;
using CorpriTech.Milton.Abstractions;

namespace Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable CS1591
public static class ServiceCollectionExtensions
#pragma warning restore CS1591
{
    /// <summary>
    /// Adds Milton to the service collection, allowing instances of <see cref="IState{TState}"/> to be injected.
    /// </summary>
    /// <remarks>
    /// <see cref="IState{TState}"/> instances are singleton instances.
    /// </remarks>
    /// <param name="serviceCollection">The service collection Milton will be added to.</param>
    /// <returns>The original <see cref="IServiceCollection"/> instance so that additional calls may be chained.</returns>
    public static IServiceCollection AddMilton(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(typeof(IState<>), typeof(State<>));

        return serviceCollection;
    }
}