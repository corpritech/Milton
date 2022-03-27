using Milton.Abstractions;

namespace Milton.Extensions.Microsoft.DependencyInjection;

public class StateContainerProxy<T> : IStateContainer<T> where T : class
{
    public event Action<IStateContainer<T>>? OnChange;
    public T State => _stateContainer.State;
    
    private readonly IStateContainer<T> _stateContainer;

    public StateContainerProxy(IStateContainerFactory stateContainerFactory)
    {
        _stateContainer = stateContainerFactory.CreateStateContainer<T>();
        _stateContainer.OnChange += NotifyStateChanged;
    }

    internal void NotifyStateChanged(IStateContainer<T> container) => OnChange?.Invoke(container);
}