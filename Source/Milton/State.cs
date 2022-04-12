using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using CorpriTech.Milton.Abstractions;
using CorpriTech.Milton.Extensions;

namespace CorpriTech.Milton;

/// <inheritdoc cref="IState{TState}"/>
public class State<TState> : IState<TState> where TState : class
{
    public TState CurrentState { get; protected set; }
    
    protected readonly ReadOnlyDictionary<int, object> StateProperties;

    private event Action<IState<TState>>? _onChange;

    public State()
    {
        if (typeof(TState).GetConstructor(Type.EmptyTypes) == null)
        {
            throw new ArgumentException("State must have a public parameterless constructor.", nameof(TState));
        }
        
        CurrentState = Activator.CreateInstance<TState>();
        
        StateProperties = BuildStatePropertyDictionary();
    }

    public State(TState initialState)
    {
        if (typeof(TState).GetConstructor(Type.EmptyTypes) == null && initialState is not ICloneable)
        {
            throw new ArgumentException($"State must have a public parameterless constructor or implement {nameof(ICloneable)}.", nameof(initialState));
        }
        
        CurrentState = initialState;
        
        StateProperties = BuildStatePropertyDictionary();
    }

    public Task UpdateAsync<TProperty>(Expression<Func<TState, TProperty>> propertyExpression, TProperty value)
        => UpdateAsync(builder => builder.Update(propertyExpression, value));

    public Task UpdateAsync(Action<IStateUpdateBuilder<TState>> updateAction)
    {
        var builder = new StateUpdateBuilder<TState>();

        updateAction.Invoke(builder);

        var nextState = CloneState();

        foreach (var (propertyInfo, value) in builder.ChangedProperties)
        {
            GetType().GetMethod(nameof(UpdateProperty), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(propertyInfo.PropertyType).Invoke(this, new object?[] {propertyInfo, nextState, value});
        }

        CurrentState = nextState;
        
        NotifyStateChanged();
        
        return Task.CompletedTask;
    }
    
    public void OnChange(Action<IState<TState>> onChangeAction)
        => _onChange += onChangeAction;

    public void OnChange<TProperty>(Expression<Func<TState, TProperty>> propertyExpression, Action<TProperty> onChangeAction)
        => OnChange(propertyExpression, (newValue, _) => onChangeAction(newValue));
    
    public void OnChange<TProperty>(Expression<Func<TState, TProperty>> propertyExpression, Action<TProperty, TProperty> onChangeAction)
    {
        var member = (MemberExpression) propertyExpression.Body;
        var propertyInfo = (PropertyInfo) member.Member;
        var property = StateProperties[propertyInfo.MetadataToken] as StateProperty<TProperty>;
        
        property!.OnChange += onChangeAction;
    }
    
    protected ReadOnlyDictionary<int, object> BuildStatePropertyDictionary()
    {
        var dictionary = new Dictionary<int, object>();
        
        foreach (var property in GetStateProperties())
        {
            var statePropertyType = typeof(State<>.StateProperty<>).MakeGenericType(typeof(TState), property.PropertyType);
            var stateProperty = Activator.CreateInstance(statePropertyType, new object?[]{property});
            dictionary.Add(property.MetadataToken, stateProperty ?? throw new InvalidCastException($"Failed to construct {nameof(StateProperty<object>)}."));
        }

        return new ReadOnlyDictionary<int, object>(dictionary);
    }
    
    protected void UpdateProperty<TProperty>(PropertyInfo propertyInfo, TState state, TProperty value)
    {
        if (!propertyInfo.IsStateProperty())
        {
            throw new ArgumentException("Property specified is not a valid state property.", nameof(propertyInfo));
        }
        
        (StateProperties[propertyInfo.MetadataToken] as StateProperty<TProperty>)!.UpdateValue(state, value);
    }

    protected TState CloneState()
    {
        TState nextState;

        if (CurrentState is ICloneable cloneableState)
        {
            nextState = cloneableState.Clone() as TState ?? throw new InvalidCastException($"State clone function did not return a state matching the type of {nameof(TState)}.");
        }
        else
        {
            nextState = Activator.CreateInstance<TState>();
            
            foreach (var property in  GetStateProperties())
            {
                property.SetValue(nextState, property.GetValue(CurrentState));
            }
        }
        
        return nextState;
    }

    protected IEnumerable<PropertyInfo> GetStateProperties()
        => typeof(TState).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.IsStateProperty());

    protected void NotifyStateChanged() => _onChange?.Invoke(this);

    protected class StateProperty<TProperty>
    {
        public event Action<TProperty, TProperty>? OnChange;
        
        private readonly PropertyInfo _propertyInfo;

        public StateProperty(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public void UpdateValue(TState state, TProperty newValue)
        {
            var oldValue = (TProperty) _propertyInfo.GetValue(state)!;
            
            _propertyInfo.SetValue(state, newValue);
            
            NotifyStateChanged(oldValue, newValue);
        }

        private void NotifyStateChanged(TProperty newValue, TProperty oldValue)
        {
            OnChange?.Invoke(newValue, oldValue);
        }
    }
}