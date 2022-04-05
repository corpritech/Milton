using Milton.Abstractions;

namespace Milton;

public record StateProperty<TValue> : IStateProperty<TValue>
{
    public TValue Value
    {
        get => _value;
        set => SetValue(value);
    }
    public bool IsLatestRevision { get; private set; } = true;

    private readonly TValue _value;
    private readonly StatePropertyEvents _events;
    private readonly object _setValueLock = new();

    public StateProperty(TValue value)
    {
        _value = value;
        _events = new StatePropertyEvents();
    }
    
    private StateProperty(StatePropertyEvents events, TValue value)
    {
        _value = value;
        _events = events;
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
    
    private IStateProperty<TValue> SetValue(TValue value)
    {
        lock (_setValueLock)
        {
            if (!IsLatestRevision)
            {
                throw new InvalidOperationException("Cannot set value on past revision.");
            }
            IsLatestRevision = false;
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


    private void NotifyStateChanged(IStateProperty<TValue> newStateProperty)
    {
        _events.InvokeOnChange(newStateProperty, this);
    }
    
    private class StatePropertyEvents
    {
        public event Action<IStateProperty<TValue>, IStateProperty<TValue>>? OnChange;

        internal void InvokeOnChange(IStateProperty<TValue> newState, IStateProperty<TValue> oldState)
            => OnChange?.Invoke(newState, oldState);
    }
}