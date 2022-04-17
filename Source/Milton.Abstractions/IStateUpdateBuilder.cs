using System.Linq.Expressions;

namespace CorpriTech.Milton;

/// <summary>
/// A builder for state updates, allowing multiple properties to be specified in a single update.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
public interface IStateUpdateBuilder<TState> where TState : class
{
    /// <summary>
    /// Adds a property update to the builder. When updating commences, the specified property will be updated and it's value will be set to the value provided.
    /// </summary>
    /// <param name="propertyExpression">The expression identifying which property to update.</param>
    /// <param name="value">The value to assign to the specified property.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>The original <see cref="IStateUpdateBuilder{TInnerState}"/> instance so that additional calls may be chained.</returns>
    public IStateUpdateBuilder<TState> Update<TProperty>(Expression<Func<TState, TProperty>> propertyExpression, TProperty value);
}