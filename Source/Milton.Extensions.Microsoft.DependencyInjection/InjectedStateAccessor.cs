using CorpriTech.Milton.Abstractions;

namespace CorpriTech.Milton.Extensions.Microsoft.DependencyInjection;

/// <inheritdoc cref="IStateAccessor{TState}"/>
public class InjectedStateAccessor<TState> : StateAccessor<TState> where TState : class
{
    internal void SetState(IState<TState> state)
    {
        LocalState.Value = state;
    }
}