using CorpriTech.Milton.Abstractions;

namespace CorpriTech.Milton;

/// <inheritdoc cref="IStateAccessor{TState}"/>
public class StateAccessor<TState> : IStateAccessor<TState> where TState : class
{
    /// <inheritdoc cref="IStateAccessor{TState}.State"/>
    public IState<TState>? State => LocalState.Value;

    /// <summary>
    /// The <see cref="AsyncLocal{T}"/> containing the state instance for the current asynchronous control flow.
    /// </summary>
    protected readonly AsyncLocal<IState<TState>> LocalState = new();
}