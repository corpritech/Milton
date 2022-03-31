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

        var valueProperties = GetInnerStateValueProperties(innerStateType);
        var stateProperties = GetStateProperties(innerStateType);

        AssignInnerStateValueProperties(innerState, valueProperties);
        AssignStateProperties(innerState, stateProperties);
        
        return innerState;
    }

    public IInnerStateValue<TValue> CreateInnerStateValue<TValue>(TValue? initialValue)
    {
        var innerStateValueType = typeof(InnerStateValue<TValue>);
        var innerStateValue = Activator.CreateInstance(innerStateValueType, new object?[] {initialValue}) as IInnerStateValue<TValue>;
        
        ThrowIfTypeDefault(typeof(TValue), innerStateValue);

        return innerStateValue;
    }

    protected IEnumerable<PropertyInfo> GetInnerStateValueProperties(Type type)
        => type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(x => x.IsInnerStateValue());
    
    protected IEnumerable<PropertyInfo> GetStateProperties(Type type)
        => type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(x => x.IsState());

    protected void AssignInnerStateValueProperties(object innerState, IEnumerable<PropertyInfo> valueProperties)
    {
        foreach (var property in valueProperties)
        {
            if (!CanAssignInnerStateValueProperty(innerState, property))
            {
                continue;
            }
            
            var initialValue = property.GetCustomAttribute<InitialValueAttribute>()?.Value ?? default;
            
            property.SetValue(innerState, GetType()
                .GetMethod(nameof(CreateInnerStateValue))!
                .MakeGenericMethod(property.PropertyType.GetGenericArguments()[0])
                .Invoke(this, new object?[] {initialValue}));
        }
    }

    protected bool CanAssignInnerStateValueProperty(object innerState, PropertyInfo valueProperty)
        => valueProperty.GetValue(innerState) == default;

    protected void AssignStateProperties(object innerState, IEnumerable<PropertyInfo> stateProperties)
    {
        foreach (var property in stateProperties)
        {
            if (!CanAssignStateProperty(innerState, property))
            {
                continue;
            }
            
            property.SetValue(innerState, GetType()
                .GetMethod(nameof(CreateState))!
                .MakeGenericMethod(property.PropertyType.GetGenericArguments()[0])
                .Invoke(this, Array.Empty<object>()));
        }
    }
    
    protected bool CanAssignStateProperty(object innerState, PropertyInfo stateProperty)
        => stateProperty.GetValue(innerState) == default;
    
    protected static void ThrowIfTypeDefault(Type type, object? o)
    {
        if (o == default)
        {
            throw new InvalidCastException($"Failed to create instance of type \"{type}\".");
        }
    }
}