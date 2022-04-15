using CorpriTech.Milton.Abstractions;

namespace CorpriTech.Milton.Extensions.Microsoft.DependencyInjection;

/// <inheritdoc cref="State{TState}"/>
public class InjectedState<TState> : State<TState> where TState : class
{
    /// <inheritdoc cref="State{TState}()"/>
    public InjectedState(IStateAccessor<IState<TState>> stateAccessor)
    {
        if (stateAccessor is InjectedStateAccessor<TState> injectedStateAccessor)
        {
            injectedStateAccessor.SetState(this);
        }
    }
}