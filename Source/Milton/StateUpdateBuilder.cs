using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using CorpriTech.Milton.Abstractions;

namespace CorpriTech.Milton;

public class StateUpdateBuilder<TInnerState> : IStateUpdateBuilder<TInnerState> where TInnerState : class
{
    internal ConcurrentDictionary<PropertyInfo, object?> ChangedProperties { get; } = new();

    public IStateUpdateBuilder<TInnerState> Update<TProperty>(Expression<Func<TInnerState, TProperty>> propertyExpression, TProperty value)
    {
        var member = (MemberExpression) propertyExpression.Body;
        var property = (PropertyInfo) member.Member;

        ChangedProperties.AddOrUpdate(property, _ => value, (_, _) => value);

        return this;
    }
}