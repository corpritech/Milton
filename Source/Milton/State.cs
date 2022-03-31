using System.Reflection;
using Milton.Abstractions;
using Milton.Extensions;

namespace Milton;

public class State<TInnerState> : IState<TInnerState> where TInnerState : class
{
    public TInnerState CurrentState { get; private set; }
    public TInnerState? PreviousState { get; private set; }

    private event Action<IState<TInnerState>>? _onChange; 
    
    private readonly object _stateUpdateLock = new();

    public State(TInnerState state)
    {
        CurrentState = state;
        
        SubscribeToStateValues();
        SubscribeToStates();
    }
    
    public void OnChange(Action<IState<TInnerState>> handler)
        => _onChange += handler;

    private void SubscribeToStateValues()
    {
        var innerStateType = typeof(TInnerState);
        var properties = innerStateType.GetProperties().Where(x => x.IsStateProperty());
        
        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(CurrentState);
            
            if (propertyValue == default)
            {
                continue;
            }

            var onChangeEventMethod = propertyValue.GetType().GetMethod(nameof(IStateProperty<object>.OnChange));
            var onChangeEventHandler = GetType()
                .GetMethod(nameof(HandleInnerStateValueChanged), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(property.PropertyType, property.PropertyType.GetGenericArguments()[0]);
            var onChangeEventDelegate = Delegate.CreateDelegate(onChangeEventMethod!.GetParameters()[0].ParameterType, this, onChangeEventHandler);
            
            onChangeEventMethod.Invoke(propertyValue, new object?[] { onChangeEventDelegate });
        }
    }

    private void SubscribeToStates()
    {
        var innerStateType = typeof(TInnerState);
        var properties = innerStateType.GetProperties().Where(x => x.IsState());

        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(CurrentState);
            
            if (propertyValue == default)
            {
                continue;
            }
            
            var onChangeEventMethod = propertyValue.GetType().GetMethod(nameof(IState<object>.OnChange));
            var onChangeEventHandler = GetType()
                .GetMethod(nameof(HandleStateContainerChanged), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(property.PropertyType, property.PropertyType.GetGenericArguments()[0]);
            var onChangeEventDelegate = Delegate.CreateDelegate(onChangeEventMethod!.GetParameters()[0].ParameterType, this, onChangeEventHandler);
            
            onChangeEventMethod.Invoke(propertyValue, new object?[] {onChangeEventDelegate});
        }
    }

    private void HandleInnerStateValueChanged<TStateValue, TValue>(TStateValue newValue, TStateValue oldValue) where TStateValue : IStateProperty<TValue>
    {
        lock (_stateUpdateLock)
        {
            var property = typeof(TInnerState)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(x => 
                {
                    if (ReferenceEquals(x.GetValue(CurrentState), oldValue))
                    {
                        return true;
                    }

                    if (ReferenceEquals(x.GetValue(PreviousState), oldValue))
                    {
                        throw new InvalidOperationException("Previous state cannot be changed.");
                    }

                    return false;
                });

           if (property == null)
           {
               // Somehow the state was updated prior to this call. Possible race condition?
               throw new InvalidOperationException("Failed to update state. The property could not be found.");
           }

           var newState = CloneState();
           
           property.SetValue(newState, newValue);

           PreviousState = CurrentState;
           CurrentState = newState;
        }
        
        NotifyStateChanged();
    }

    private void HandleStateContainerChanged<TStateContainer, TState1>(TStateContainer _) where TStateContainer : IState<TState1> where TState1 : class
    {
        NotifyStateChanged();
    }

    private TInnerState CloneState()
    {
        var properties = typeof(TInnerState).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var newState = Activator.CreateInstance<TInnerState>();
        
        foreach (var property in properties)
        {
            property.SetValue(newState, property.GetValue(CurrentState));
        }

        return newState;
    }

    private void NotifyStateChanged() => _onChange?.Invoke(this);
}