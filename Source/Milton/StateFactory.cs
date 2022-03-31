using System.Reflection;
using Milton.Abstractions;
using Milton.Extensions;

namespace Milton;

public class StateFactory : IStateFactory
{
    public IState<TInnerState> CreateState<TInnerState>() where TInnerState : class
    {
        var stateType = typeof(State<TInnerState>);
        var innerState = CreateInnerState<TInnerState>();
        var state = Activator.CreateInstance(stateType, new object?[] {innerState}) as IState<TInnerState>;

        ThrowIfTypeDefault(stateType, state);

        return state!;
    }

    public TInnerState CreateInnerState<TInnerState>() where TInnerState : class
    {
        var innerStateType = typeof(TInnerState);
        
        if (innerStateType.GetConstructor(Type.EmptyTypes) == null)
        {
            throw new InvalidOperationException("Inner state class must have a public parameterless constructor.");
        }
        
        var innerState = Activator.CreateInstance<TInnerState>();

        ThrowIfTypeDefault(innerStateType, innerState);

        var valueProperties = GetStateProperties(innerStateType);
        var stateProperties = GetNestedStateProperties(innerStateType);

        AssignStateProperties(innerState, valueProperties);
        AssignNestedStates(innerState, stateProperties);
        
        return innerState;
    }

    public IStateProperty<TValue> CreateStateProperty<TValue>(TValue? initialValue)
    {
        var innerStateValueType = typeof(StateProperty<TValue>);
        var stateProperty = Activator.CreateInstance(innerStateValueType, new object?[] {initialValue}) as IStateProperty<TValue>;
        
        ThrowIfTypeDefault(typeof(TValue), stateProperty);

        return stateProperty!;
    }

    protected IEnumerable<PropertyInfo> GetStateProperties(Type type)
        => type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(x => x.IsStateProperty());
    
    protected IEnumerable<PropertyInfo> GetNestedStateProperties(Type type)
        => type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(x => x.IsState());

    protected void AssignStateProperties(object innerState, IEnumerable<PropertyInfo> valueProperties)
    {
        foreach (var property in valueProperties)
        {
            if (!CanAssignStateProperty(innerState, property))
            {
                continue;
            }
            
            var initialValue = property.GetCustomAttribute<InitialValueAttribute>()?.Value ?? default;
            
            property.SetValue(innerState, GetType()
                .GetMethod(nameof(CreateStateProperty))!
                .MakeGenericMethod(property.PropertyType.GetGenericArguments()[0])
                .Invoke(this, new object?[] {initialValue}));
        }
    }

    protected bool CanAssignStateProperty(object innerState, PropertyInfo valueProperty)
        => valueProperty.GetValue(innerState) == default;

    protected void AssignNestedStates(object innerState, IEnumerable<PropertyInfo> stateProperties)
    {
        foreach (var property in stateProperties)
        {
            if (!CanAssignNestedState(innerState, property))
            {
                continue;
            }
            
            property.SetValue(innerState, GetType()
                .GetMethod(nameof(CreateState))!
                .MakeGenericMethod(property.PropertyType.GetGenericArguments()[0])
                .Invoke(this, Array.Empty<object>()));
        }
    }
    
    protected bool CanAssignNestedState(object innerState, PropertyInfo stateProperty)
        => stateProperty.GetValue(innerState) == default;
    
    protected static void ThrowIfTypeDefault(Type type, object? o)
    {
        if (o == default)
        {
            throw new InvalidCastException($"Failed to create instance of type \"{type}\".");
        }
    }
}