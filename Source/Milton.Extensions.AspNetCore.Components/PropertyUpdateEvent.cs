using CorpriTech.Milton.Abstractions;

namespace Microsoft.AspNetCore.Components;

/// <summary>
/// Represents an update to a <see cref="IState{TState}"/> property.
/// </summary>
/// <typeparam name="TProperty">The property type.</typeparam>
public class PropertyUpdateEvent<TProperty>
{
    /// <summary>
    /// The new value.
    /// </summary>
    public TProperty NewValue { get; }

    /// <summary>
    /// The old value.
    /// </summary>
    public TProperty OldValue { get; }

    /// <summary>
    /// Constructs a new <see cref="PropertyUpdateEvent{TProperty}"/> using the values provided.
    /// </summary>
    /// <param name="newValue">The new value.</param>
    /// <param name="oldValue">The old value.</param>
    public PropertyUpdateEvent(TProperty newValue, TProperty oldValue)
    {
        NewValue = newValue;
        OldValue = oldValue;
    }
}