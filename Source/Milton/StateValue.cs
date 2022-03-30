using Milton.Abstractions;

namespace Milton;

public record StateValue<TValue> : IStateValue<TValue>
{
    public TValue Value { get; }
    
    private StateValueEventWrapper _events;

    public StateValue(TValue value)
    {
        Value = value;
        _events = new StateValueEventWrapper();
    }
    
    private StateValue(StateValueEventWrapper events, TValue value)
    {
        Value = value;
        _events = events;
    }

    public IStateValue<TValue> SetValue(TValue value)
    {
        IStateValue<TValue> newStateValue;
        
        if (value is ICloneable cloneableValue)
        {
            newStateValue = new StateValue<TValue>(_events, (TValue) cloneableValue.Clone());
        }
        else
        {
            newStateValue = new StateValue<TValue>(_events, value);
        }
        
        NotifyStateChanged(newStateValue);
        
        return newStateValue;
    }

    public Task<IStateValue<TValue>> SetValueAsync(TValue value)
    {
        var newStateValue = SetValue(value);
        return Task.FromResult(newStateValue);
    }

    public void OnChange(Action<IStateValue<TValue>, IStateValue<TValue>> handler)
    {
        _events.OnChange += handler;
    }

    public virtual bool Equals(StateValue<TValue>? other)
    {
        if (other == null)
        {
            return false;
        }
        
        if (Value == null && other.Value == null)
        {
            return true;
        }

        if (Value != null && other.Value != null)
        {
            return Value.Equals(other.Value);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? typeof(TValue).GetHashCode();
    }

    private void NotifyStateChanged(IStateValue<TValue> newStateValue)
    {
        _events.InvokeOnChange(newStateValue, this);
    }
    
    private class StateValueEventWrapper
    {
        public event Action<IStateValue<TValue>, IStateValue<TValue>>? OnChange;

        internal void InvokeOnChange(IStateValue<TValue> newState, IStateValue<TValue> oldState)
            => OnChange?.Invoke(newState, oldState);
    }
}