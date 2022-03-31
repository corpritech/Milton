using Milton.Abstractions;

namespace Milton;

public record StateProperty<TValue> : IStateProperty<TValue>
{
    public TValue Value { get; }
    
    private bool _changed;
    
    private readonly StateValueEventWrapper _events;
    private readonly object _setValueLock = new();

    public StateProperty(TValue value)
    {
        Value = value;
        _events = new StateValueEventWrapper();
    }
    
    private StateProperty(StateValueEventWrapper events, TValue value)
    {
        Value = value;
        _events = events;
    }

    public IStateProperty<TValue> SetValue(TValue value)
    {
        lock (_setValueLock)
        {
            if (_changed)
            {
                throw new InvalidOperationException("State value has already changed.");
            }
            _changed = true;
        }
        
        IStateProperty<TValue> newStateProperty;

        if (value is ICloneable cloneableValue)
        {
            newStateProperty = new StateProperty<TValue>(_events, (TValue) cloneableValue.Clone());
        }
        else
        {
            newStateProperty = new StateProperty<TValue>(_events, value);
        }
        
        NotifyStateChanged(newStateProperty);
        
        return newStateProperty;
    }

    public Task<IStateProperty<TValue>> SetValueAsync(TValue value)
    {
        var newStateValue = SetValue(value);
        return Task.FromResult(newStateValue);
    }

    public void OnChange(Action<IStateProperty<TValue>, IStateProperty<TValue>> handler)
    {
        _events.OnChange += handler;
    }

    public virtual bool Equals(StateProperty<TValue>? other)
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

    private void NotifyStateChanged(IStateProperty<TValue> newStateProperty)
    {
        _events.InvokeOnChange(newStateProperty, this);
    }
    
    private class StateValueEventWrapper
    {
        public event Action<IStateProperty<TValue>, IStateProperty<TValue>>? OnChange;

        internal void InvokeOnChange(IStateProperty<TValue> newState, IStateProperty<TValue> oldState)
            => OnChange?.Invoke(newState, oldState);
    }
}