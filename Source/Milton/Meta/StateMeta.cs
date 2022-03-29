using System.Collections.Concurrent;
using Milton.Extensions;

namespace Milton.Meta;

internal class StateMeta
{
    public Type AssignedType { get; }
    public IEnumerable<StateValueMeta> Values { get; }
    public IEnumerable<StateContainerMeta> Containers { get; }

    private static readonly ConcurrentDictionary<Type, StateMeta> KnownStateMeta = new();

    private StateMeta(Type assignedType, IEnumerable<StateValueMeta> values, IEnumerable<StateContainerMeta> containers)
    {
        AssignedType = assignedType;
        Values = values;
        Containers = containers;
    }

    public static StateMeta Build<TState>() where TState : class
    {
        var type = typeof(TState);

        return KnownStateMeta.GetOrAdd(type, t =>
        {
            var values = t.GetProperties().Where(property => property.IsStateValue()).Select(property =>
                (StateValueMeta) typeof(StateValueMeta).GetMethod(nameof(StateValueMeta.Build))!
                    .MakeGenericMethod(property.PropertyType, property.PropertyType.GetGenericArguments()[0])
                    .Invoke(null, new object?[] {property})!);
            
            var containers = t.GetProperties().Where(property => property.IsStateContainer()).Select(property =>
                (StateContainerMeta) typeof(StateContainerMeta).GetMethod(nameof(StateContainerMeta.Build))!
                    .MakeGenericMethod(property.PropertyType, property.PropertyType.GetGenericArguments()[0])
                    .Invoke(null, new object?[] {property})!);

            return new StateMeta(t, values, containers);
        });
    }
}