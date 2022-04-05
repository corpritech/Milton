using System.Reflection;
using Milton.Abstractions;
using Milton.Extensions;

namespace Milton;

public class State<TInnerState> : IState<TInnerState> where TInnerState : class
{
    public TInnerState Properties { get; private set; }

    private event Action<IState<TInnerState>>? _onChange; 
    
    private readonly object _stateUpdateLock = new();

    public State(TInnerState state)
    {
        Properties = state;
        
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
            var propertyValue = property.GetValue(Properties);
            
            if (propertyValue == default)
            {
                continue;
            }

            var onChangeEventMethod = propertyValue.GetType().GetMethod(nameof(IStateProperty<object>.OnChange));
            var onChangeEventHandler = GetType()
                .GetMethod(nameof(HandleInnerStatePropertyChanged), BindingFlags.NonPublic | BindingFlags.Instance)!
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
            var propertyValue = property.GetValue(Properties);
            
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

    private void HandleInnerStatePropertyChanged<TStateValue, TValue>(TStateValue newValue, TStateValue oldValue) where TStateValue : IStateProperty<TValue>
    {
        lock (_stateUpdateLock)
        {
            var property = typeof(TInnerState)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(x => ReferenceEquals(x.GetValue(Properties), oldValue));

           if (property == null)
           {
               // Somehow the state was updated prior to this call. Possible race condition?
               throw new InvalidOperationException("Failed to update state. The property could not be found.");
           }

           var newInnerState = CloneState();
           
           property.SetValue(newInnerState, newValue);

           Properties = newInnerState;
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
            property.SetValue(newState, property.GetValue(Properties));
        }

        return newState;
    }

    private void NotifyStateChanged() => _onChange?.Invoke(this);
}