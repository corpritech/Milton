﻿using System.Linq.Expressions;
using CorpriTech.Milton.Abstractions;

namespace Microsoft.AspNetCore.Components.Extensions;

public static class StateExtensions
{
    /// <summary>
    /// Adds a property observer to this state.
    /// </summary>
    /// <remarks>
    /// The property observer event callback will be invoked in the event that the property specified is updated within the state.
    /// </remarks>
    /// <param name="state">The state an observer will be added to.</param>
    /// <param name="propertyExpression">The expression identifying which property to observe.</param>
    /// <param name="eventCallback">The <see cref="EventCallback{TValue}"/> to invoke in the event that the property value is updated.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public static void OnChange<TState, TProperty>(this IState<TState> state, Expression<Func<TState, TProperty>> propertyExpression,
        EventCallback<PropertyUpdateEvent<TProperty>> eventCallback) where TState : class
    {
        state.OnChange(propertyExpression, (newValue, oldValue) => eventCallback.InvokeAsync(new PropertyUpdateEvent<TProperty>(newValue, oldValue)));
    }
}