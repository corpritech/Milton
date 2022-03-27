using System.Reflection;
using Milton.Abstractions;
using Milton.Meta;

namespace Milton;

public class StateContainer<TState> : IStateContainer<TState> where TState : class
{
    public event Action<IStateContainer<TState>>? OnChange;
    public TState State { get; }
    private readonly StateContainerMeta _meta;

    public StateContainer(TState state, StateContainerMeta meta)
    {
        State = state;
        _meta = meta;
        SubscribeToStateValues();
        SubscribeToStateContainers();
    }

    private void SubscribeToStateValues()
    {
        foreach (var valueMeta in _meta.State.Values)
        {
            var onChangeEvent = valueMeta.AssignedType.GetEvent(nameof(StateValue<object>.OnChange));
            var onChangeEventHandler = GetType().GetMethod(nameof(HandleStateValueChanged), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(valueMeta.DeclaringPropertyType, valueMeta.ValueType);
            var onChangeEventDelegate = Delegate.CreateDelegate(onChangeEvent!.EventHandlerType!, this, onChangeEventHandler);
            onChangeEvent.AddEventHandler(valueMeta.DeclaringProperty.GetValue(State), onChangeEventDelegate);
        }
    }
    
    private void SubscribeToStateContainers()
    {
        foreach (var containerMeta in _meta.State.Containers)
        {
            var onChangeEvent = containerMeta.AssignedType.GetEvent(nameof(StateContainer<object>.OnChange));
            var onChangeEventHandler = GetType().GetMethod(nameof(HandleStateContainerChanged), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(containerMeta.DeclaringPropertyType, containerMeta.State.AssignedType);
            var onChangeEventDelegate = Delegate.CreateDelegate(onChangeEvent!.EventHandlerType!, this, onChangeEventHandler);
            onChangeEvent.AddEventHandler(containerMeta.DeclaringProperty!.GetValue(State), onChangeEventDelegate);
        }
    }

    private void HandleStateValueChanged<TStateValue, TValue>(TStateValue value) where TStateValue : IStateValue<TValue>
        => NotifyStateChanged();

    private void HandleStateContainerChanged<TStateContainer, TState1>(TStateContainer value) where TStateContainer : IStateContainer<TState1> where TState1 : class
        => NotifyStateChanged();

    private void NotifyStateChanged() => OnChange?.Invoke(this);
}