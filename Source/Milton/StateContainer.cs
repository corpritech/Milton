using System.Reflection;
using Milton.Abstractions;
using Milton.Meta;

namespace Milton;

public class StateContainer<TState> : IStateContainer<TState> where TState : class
{
    public TState CurrentState { get; private set; }
    public TState? PreviousState { get; private set; }
    
    private event Action<IStateContainer<TState>>? _onChange; 
    private readonly StateMeta _stateMeta;
    private readonly object _stateUpdateLock = new();

    public StateContainer(TState state)
    {
        CurrentState = state;
        _stateMeta = StateMeta.Build<TState>();
        
        SubscribeToStateValues();
        SubscribeToStateContainers();
    }
    
    public void OnChange(Action<IStateContainer<TState>> handler)
        => _onChange += handler;

    private void SubscribeToStateValues()
    {
        foreach (var valueMeta in _stateMeta.Values)
        {
            var onChangeEventMethod = valueMeta.AssignedType.GetMethod(nameof(IStateValue<object>.OnChange));
            var onChangeEventHandler = GetType().GetMethod(nameof(HandleStateValueChanged), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(valueMeta.DeclaringPropertyType, valueMeta.ValueType);
            var onChangeEventDelegate = Delegate.CreateDelegate(onChangeEventMethod!.GetParameters()[0].ParameterType, this, onChangeEventHandler);
            onChangeEventMethod.Invoke(valueMeta.DeclaringProperty.GetValue(CurrentState), new object?[] { onChangeEventDelegate });
        }
    }

    private void SubscribeToStateContainers()
    {
        foreach (var containerMeta in _stateMeta.Containers)
        {
            var onChangeEvent = containerMeta.AssignedType.GetEvent(nameof(StateContainer<object>.OnChange));
            var onChangeEventHandler = GetType().GetMethod(nameof(HandleStateContainerChanged), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(containerMeta.DeclaringPropertyType, containerMeta.State.AssignedType);
            var onChangeEventDelegate = Delegate.CreateDelegate(onChangeEvent!.EventHandlerType!, this, onChangeEventHandler);
            onChangeEvent.AddEventHandler(containerMeta.DeclaringProperty!.GetValue(CurrentState), onChangeEventDelegate);
        }
    }

    private void HandleStateValueChanged<TStateValue, TValue>(TStateValue newValue, TStateValue oldValue) where TStateValue : IStateValue<TValue>
    {
        lock (_stateUpdateLock)
        {
           var property = _stateMeta.AssignedType.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(x => ReferenceEquals(x.GetValue(CurrentState), oldValue));

           if (property == null)
           {
               // Somehow the state was updated prior to this call. Possible race condition?
               throw new InvalidOperationException("Failed to update state. The property could not be found.");
           }

           var newState = CopyState();
           
           property.SetValue(newState, newValue);

           PreviousState = CurrentState;
           CurrentState = newState;
        }
        
        NotifyStateChanged();
    }

    private void HandleStateContainerChanged<TStateContainer, TState1>(TStateContainer _) where TStateContainer : IStateContainer<TState1> where TState1 : class
    {
        NotifyStateChanged();
    }

    private TState CopyState()
    {
        var properties = typeof(TState).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var newState = Activator.CreateInstance<TState>();
        
        foreach (var property in properties)
        {
            property.SetValue(newState, property.GetValue(CurrentState));
        }

        return newState;
    }

    private void NotifyStateChanged() => _onChange?.Invoke(this);
}