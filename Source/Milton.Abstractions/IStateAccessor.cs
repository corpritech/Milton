namespace CorpriTech.Milton;

/// <summary>
/// Allows accessing a state with a scoped lifetime from a singleton service.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
public interface IStateAccessor<TState> where TState : class
{
    /// <summary>
    /// The state.
    /// </summary>
    IState<TState>? State { get; }
}