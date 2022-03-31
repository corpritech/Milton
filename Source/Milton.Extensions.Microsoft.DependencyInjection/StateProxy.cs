using Milton.Abstractions;

namespace Milton.Extensions.Microsoft.DependencyInjection;

public class StateProxy<TInnerState> : IState<TInnerState> where TInnerState : class
{
    public TInnerState CurrentState => _state.CurrentState;
    public TInnerState? PreviousState => _state.PreviousState;

    private readonly IState<TInnerState> _state;

    public StateProxy(IStateFactory stateFactory)
    {
        _state = stateFactory.CreateState<TInnerState>();
    }

    void IState<TInnerState>.OnChange(Action<IState<TInnerState>> handler)
        => _state.OnChange(handler);
}