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

        var valueProperties = innerStateType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(x => x.IsInnerStateValue());
        
        var stateProperties = innerStateType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(x => x.IsState());

        foreach (var property in valueProperties)
        {
            if (property.GetValue(innerState) != default)
            {
                continue;
            }
            
            var initialValue = property.GetCustomAttribute<InitialValueAttribute>()?.Value ?? default;
            
            property.SetValue(innerState, GetType()
                .GetMethod(nameof(CreateInnerStateValue))!
                .MakeGenericMethod(property.PropertyType.GetGenericArguments()[0])
                .Invoke(this, new object?[] {initialValue}));
        }

        foreach (var property in stateProperties)
        {
            if (property.GetValue(innerState) != default)
            {
                continue;
            }
            
            property.SetValue(innerState, GetType()
                .GetMethod(nameof(CreateState))!
                .MakeGenericMethod(property.PropertyType.GetGenericArguments()[0])
                .Invoke(this, Array.Empty<object>()));
        }

        return innerState;
    }

    public IInnerStateValue<TValue> CreateInnerStateValue<TValue>(TValue? initialValue)
    {
        var innerStateValueType = typeof(InnerStateValue<TValue>);
        var innerStateValue = Activator.CreateInstance(innerStateValueType, new object?[] {initialValue}) as IInnerStateValue<TValue>;
        
        ThrowIfTypeDefault(typeof(TValue), innerStateValue);

        return innerStateValue;
    }

    private static void ThrowIfTypeDefault(Type type, object? o)
    {
        if (o == default)
        {
            throw new InvalidCastException($"Failed to create instance of type \"{type}\".");
        }
    }
}