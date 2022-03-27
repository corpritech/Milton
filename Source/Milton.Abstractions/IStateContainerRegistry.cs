namespace Milton.Abstractions;

public interface IStateContainerRegistry : IDisposable
{
    public void Add<TContainer, TState>(TContainer stateContainer) where TContainer : IStateContainer<TState> where TState : class;
    public void Add(Type type, object stateContainer);
    public TContainer? Get<TContainer, TState>() where TContainer : IStateContainer<TState> where TState : class;
    public object? Get(Type type);
    public TContainer GetOrAdd<TContainer, TState>(Func<TContainer> newValueFactory) where TContainer : IStateContainer<TState> where TState : class;
    public object GetOrAdd(Type type, Func<Type, object> newValueFactory);
    public void Clear();
}