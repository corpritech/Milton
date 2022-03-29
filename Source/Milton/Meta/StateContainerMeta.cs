using System.Collections.Concurrent;
using System.Reflection;
using Milton.Abstractions;

namespace Milton.Meta;

internal class StateContainerMeta
{
    public Type DeclaringPropertyType { get; }
    public Type AssignedType { get; }
    public PropertyInfo? DeclaringProperty { get; }
    public StateMeta State { get; }

    private static readonly ConcurrentDictionary<Type, StateContainerMeta> KnownStateContainerMeta = new();

    private StateContainerMeta(Type declaringPropertyType, Type assignedType, StateMeta state, PropertyInfo? declaringProperty = null)
    {
        DeclaringPropertyType = declaringPropertyType;
        AssignedType = assignedType;
        DeclaringProperty = declaringProperty;
        State = state;
    }

    public static StateContainerMeta Build<TContainer, TState>(PropertyInfo? assignedProperty = null) where TContainer : IStateContainer<TState> where TState : class
    {
        var type = typeof(TContainer);

        return KnownStateContainerMeta.GetOrAdd(type, t =>
        {
            var assignedType = typeof(StateContainer<>).MakeGenericType(type.GetGenericArguments()[0]);
            var state = (StateMeta) typeof(StateMeta).GetMethod(nameof(StateMeta.Build))!.MakeGenericMethod(t.GetGenericArguments()[0]).Invoke(null, Array.Empty<object>())!;

            return new StateContainerMeta(t, assignedType, state, assignedProperty);
        });
    }
}