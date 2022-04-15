using CorpriTech.Milton.Abstractions;

namespace CorpriTech.Milton;

public class StateAccessor<TState> : IStateAccessor<TState> where TState : class
{
    public IState<TState> State => LocalState.Value;

    protected readonly AsyncLocal<IState<TState>> LocalState = new();
}