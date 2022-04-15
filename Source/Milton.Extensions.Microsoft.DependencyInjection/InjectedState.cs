using CorpriTech.Milton.Abstractions;

namespace CorpriTech.Milton.Extensions.Microsoft.DependencyInjection;

public class InjectedState<TState> : State<TState> where TState : class
{
    public InjectedState(IStateAccessor<IState<TState>> stateAccessor)
    {
        if (stateAccessor is InjectedStateAccessor<TState> injectedStateAccessor)
        {
            injectedStateAccessor.SetState(this);
        }
    }
}