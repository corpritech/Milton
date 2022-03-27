﻿using Milton.Abstractions;
using Milton.Meta;

namespace Milton;

public class StateContainerFactory : IStateContainerFactory
{
    private readonly IStateContainerRegistry _stateContainerRegistry;

    public StateContainerFactory(IStateContainerRegistry stateContainerRegistry)
    {
        _stateContainerRegistry = stateContainerRegistry;
    }

    public IStateContainer<T> CreateStateContainer<T>() where T : class
    {
        var stateContainerMeta = StateContainerMeta.Build<IStateContainer<T>, T>();
        var stateContainer = CreateStateContainer(stateContainerMeta);
        var typedStateContainer = stateContainer as IStateContainer<T>;

        ThrowIfTypeDefault(typeof(IStateContainer<T>), typedStateContainer);

        return typedStateContainer!;
    }

    private object CreateStateContainer(StateContainerMeta meta)
    {
        var state = Activator.CreateInstance(meta.State.AssignedType);

        ThrowIfTypeDefault(meta.State.AssignedType, state);

        var stateContainer = _stateContainerRegistry.GetOrAdd(meta.AssignedType, t =>
        {
            AssignStateProperties(state!, meta.State);
            
            var container = Activator.CreateInstance(t, state, meta)!;

            ThrowIfTypeDefault(t, container);

            return container;
        });

        return stateContainer;
    }

    private void AssignStateProperties(object state, StateMeta meta)
    {
        foreach (var valueMeta in meta.Values)
        {
            if (valueMeta.InitialValue == default)
            {
                throw new InvalidOperationException($"Cannot assign property with missing {nameof(InitialValueAttribute)} attribute.");
            }

            var stateValue = Activator.CreateInstance(valueMeta.AssignedType, valueMeta.InitialValue);
            
            ThrowIfTypeDefault(valueMeta.AssignedType, stateValue);
            
            valueMeta.DeclaringProperty.SetValue(state, stateValue);
        }

        foreach (var containerMeta in meta.Containers)
        {
            if (containerMeta.DeclaringProperty == null)
            {
                throw new InvalidOperationException("Nested state containers must have an assigned property.");
            }

            containerMeta.DeclaringProperty.SetValue(state, CreateStateContainer(containerMeta));
        }
    }

    private static void ThrowIfTypeDefault(Type type, object? o)
    {
        if (o == default)
        {
            throw new InvalidCastException($"Failed to create instance of type \"{type}\".");
        }
    }
}