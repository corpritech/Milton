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
    public PropertyInfo? DeclaringProperty { get; }

    public StateValueMeta(Type valueType, Type assignedType, object? initialValue, PropertyInfo? declaringProperty)
    {
        DeclaringPropertyType = declaringProperty.PropertyType;
        ValueType = valueType;
        AssignedType = assignedType;
        InitialValue = initialValue;
        DeclaringProperty = declaringProperty;
    }
    
    public static StateValueMeta Build<TStateValue, TValue>(PropertyInfo? declaringProperty) where TStateValue : IStateValue<TValue>
    {
        var type = typeof(TStateValue);
        

            var valueType = typeof(TStateValue).GetGenericArguments()[0];
            var assignedType = typeof(StateValue<>).MakeGenericType(valueType);
            var initialValue = declaringProperty == null ? default : declaringProperty.GetCustomAttributes<InitialValueAttribute>().FirstOrDefault()?.Value ?? null;

            return new StateValueMeta(valueType, assignedType, initialValue, declaringProperty);
    }
}