using System.Linq.Expressions;

namespace CorpriTech.Milton.Abstractions;

/// <summary>
/// A state wrapper that wraps an object of a given type and provides methods for observing and updating the state.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
public interface IState<TState> where TState : class
{
    /// <summary>
    /// The current state.
    /// </summary>
    public TState CurrentState { get; }
    /// <summary>
    /// Updates the specified property, setting the property value to the value provided.
    /// </summary>
    /// <remarks>
    /// Updating takes place immediately.
    ///
    /// Any observers of the updated property will be invoked, afterwards a final invocation of any global state observers will occur.
    /// </remarks>
    /// <param name="propertyExpression">The expression identifying which property to update.</param>
    /// <param name="value">The value to assign to the specified property.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>A task representing the update operation.</returns>
    public Task UpdateAsync<TProperty>(Expression<Func<TState, TProperty>> propertyExpression, TProperty value);
    /// <summary>
    /// Updates the properties specified within the provided action using a <see cref="IStateUpdateBuilder{TInnerState}"/>.
    /// </summary>
    /// <remarks>
    /// Updating takes place after the provided action returns.
    ///
    /// Each property update will trigger observers of the updated property to be invoked as the update is processed.
    ///
    /// Once all updates are processed, a final invocation of any global state observers will occur.
    /// </remarks>
    /// <param name="updateAction">The action which will be provided a <see cref="IStateUpdateBuilder{TInnerState}"/>.</param>
    /// <returns>A task representing the update operation.</returns>
    public Task UpdateAsync(Action<IStateUpdateBuilder<TState>> updateAction);
    /// <summary>
    /// Adds a global observer to this state.
    /// </summary>
    /// <remarks>
    /// The global observer action will be invoked in the event that any properties are updated within the state.
    /// </remarks>
    /// <param name="onChangeAction">The action to invoke in the event of a state change.</param>
    public void OnChange(Action<IState<TState>> onChangeAction);
    /// <summary>
    /// Adds a property observer to this state.
    /// </summary>
    /// <remarks>
    /// The property observer action will be invoked in the event that the property specified is updated within the state.
    /// </remarks>
    /// <param name="propertyExpression">The expression identifying which property to observe.</param>
    /// <param name="onChangeAction">The action to invoke in the event that the property value is updated.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public void OnChange<TProperty>(Expression<Func<TState, TProperty>> propertyExpression, Action<TProperty> onChangeAction);
    /// <summary>
    /// Adds a property observer to this state.
    /// </summary>
    /// <remarks>
    /// The property observer action will be invoked in the event that the property specified is updated within the state.
    /// </remarks>
    /// <param name="propertyExpression">The expression identifying which property to observe.</param>
    /// <param name="onChangeAction">The action to invoke in the event that the property value is updated.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public void OnChange<TProperty>(Expression<Func<TState, TProperty>> propertyExpression, Action<TProperty, TProperty> onChangeAction);
}