using System.Collections.Concurrent;
using System.Reflection;
using Milton.Extensions;

namespace Milton.Meta;

internal class StateMeta
{
    public Type AssignedType { get; }
    public IEnumerable<StateValueMeta> Values { get; }
    public IEnumerable<StateContainerMeta> Containers { get; }

    private StateMeta(Type assignedType, IEnumerable<StateValueMeta> values, IEnumerable<StateContainerMeta> containers)
    {
        AssignedType = assignedType;
        Values = values;
        Containers = containers;
    }

    public static StateMeta Build<TState>() where TState : class
    {
        var type = typeof(TState);
        
            var values = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.IsStateValue()).Select(property =>
                (StateValueMeta) typeof(StateValueMeta).GetMethod(nameof(StateValueMeta.Build))!
                    .MakeGenericMethod(property.PropertyType, property.PropertyType.GetGenericArguments()[0])
                    .Invoke(null, new object?[] {property})!);
            
            var containers = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.IsStateContainer()).Select(property =>
                (StateContainerMeta) typeof(StateContainerMeta).GetMethod(nameof(StateContainerMeta.Build))!
                    .MakeGenericMethod(property.PropertyType, property.PropertyType.GetGenericArguments()[0])
                    .Invoke(null, new object?[] {property})!);

            return new StateMeta(type, values, containers);
    }
}