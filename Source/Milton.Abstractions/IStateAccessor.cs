namespace CorpriTech.Milton.Abstractions;

public interface IStateAccessor<TState> where TState : class
{
    IState<TState> State { get; }
}