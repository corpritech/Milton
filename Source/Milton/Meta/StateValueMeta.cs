using System.Collections.Concurrent;
using System.Reflection;
using Milton.Abstractions;

namespace Milton.Meta;

internal class StateValueMeta
{
    public Type DeclaringPropertyType { get; }
    public Type ValueType { get; }
    public Type AssignedType { get; }
    public object? InitialValue { get; }
    public PropertyInfo DeclaringProperty { get; }

    private static readonly ConcurrentDictionary<PropertyInfo, StateValueMeta> KnownStateValueMeta = new();

    public StateValueMeta(Type valueType, Type assignedType, object? initialValue, PropertyInfo declaringProperty)
    {
        DeclaringPropertyType = declaringProperty.PropertyType;
        ValueType = valueType;
        AssignedType = assignedType;
        InitialValue = initialValue;
        DeclaringProperty = declaringProperty;
    }
    
    public static StateValueMeta Build<TStateValue, TValue>(PropertyInfo declaringProperty) where TStateValue : IStateValue<TValue>
    {
        return KnownStateValueMeta.GetOrAdd(declaringProperty, p =>
        {
            var valueType = typeof(TStateValue).GetGenericArguments()[0];
            var assignedType = typeof(StateValue<>).MakeGenericType(valueType);
            var initialValue = declaringProperty.GetCustomAttributes<InitialValueAttribute>().FirstOrDefault()?.Value ?? null;

            return new StateValueMeta(valueType, assignedType, initialValue, declaringProperty);
        });
    }
}