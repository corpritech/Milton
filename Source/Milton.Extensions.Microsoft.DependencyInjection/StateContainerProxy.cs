using Milton.Abstractions;

namespace Milton.Extensions.Microsoft.DependencyInjection;

public class StateContainerProxy<T> : IStateContainer<T> where T : class
{
    public T CurrentState => _stateContainer.CurrentState;
    public T? PreviousState => _stateContainer.PreviousState;

    private readonly IStateContainer<T> _stateContainer;

    public StateContainerProxy(IStateContainerFactory stateContainerFactory)
    {
        _stateContainer = stateContainerFactory.CreateStateContainer<T>();
    }

    void IStateContainer<T>.OnChange(Action<IStateContainer<T>> handler)
        => _stateContainer.OnChange(handler);
}