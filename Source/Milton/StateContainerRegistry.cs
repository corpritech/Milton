using System.Collections.Concurrent;
using Milton.Abstractions;

namespace Milton;

public class StateContainerRegistry : IStateContainerRegistry
{
    private readonly ConcurrentDictionary<Type, object> _stateContainers = new();

    public void Add<TContainer, TState>(TContainer stateContainer) where TContainer : IStateContainer<TState> where TState : class
        => Add(stateContainer.GetType(), stateContainer);

    public void Add(Type type, object stateContainer)
    {
        if (!_stateContainers.TryAdd(type, stateContainer))
        {
            throw new InvalidOperationException("A container of the same type is already registered.");
        }
    }

    public TContainer? Get<TContainer, TState>() where TContainer : IStateContainer<TState> where TState : class
    {
        var stateContainer = Get(typeof(TContainer));

        if (stateContainer == null)
        {
            return default;
        }

        return (TContainer) stateContainer;
    }

    public object? Get(Type type)
        => _stateContainers.GetValueOrDefault(type);

    public TContainer GetOrAdd<TContainer, TState>(Func<TContainer> newValueFactory) where TContainer : IStateContainer<TState> where TState : class
        => (TContainer) GetOrAdd(typeof(TContainer), type => newValueFactory());

    public object GetOrAdd(Type type, Func<Type, object> newValueFactory)
        => _stateContainers.GetOrAdd(type, newValueFactory);

    public void Clear()
        => _stateContainers.Clear();

    public void Dispose()
        => Clear();
}