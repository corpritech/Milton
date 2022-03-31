using Milton.Abstractions;

namespace Milton;

public record InnerStateValue<TValue> : IInnerStateValue<TValue>
{
    public TValue Value { get; }
    
    private bool _changed;
    
    private readonly StateValueEventWrapper _events;
    private readonly object _setValueLock = new();

    public InnerStateValue(TValue value)
    {
        Value = value;
        _events = new StateValueEventWrapper();
    }
    
    private InnerStateValue(StateValueEventWrapper events, TValue value)
    {
        Value = value;
        _events = events;
    }

    public IInnerStateValue<TValue> SetValue(TValue value)
    {
        lock (_setValueLock)
        {
            if (_changed)
            {
                throw new InvalidOperationException("State value has already changed.");
            }
            _changed = true;
        }
        
        IInnerStateValue<TValue> newInnerStateValue;

        if (value is ICloneable cloneableValue)
        {
            newInnerStateValue = new InnerStateValue<TValue>(_events, (TValue) cloneableValue.Clone());
        }
        else
        {
            newInnerStateValue = new InnerStateValue<TValue>(_events, value);
        }
        
        NotifyStateChanged(newInnerStateValue);
        
        return newInnerStateValue;
    }

    public Task<IInnerStateValue<TValue>> SetValueAsync(TValue value)
    {
        var newStateValue = SetValue(value);
        return Task.FromResult(newStateValue);
    }

    public void OnChange(Action<IInnerStateValue<TValue>, IInnerStateValue<TValue>> handler)
    {
        _events.OnChange += handler;
    }

    public virtual bool Equals(InnerStateValue<TValue>? other)
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

    private void NotifyStateChanged(IInnerStateValue<TValue> newInnerStateValue)
    {
        _events.InvokeOnChange(newInnerStateValue, this);
    }
    
    private class StateValueEventWrapper
    {
        public event Action<IInnerStateValue<TValue>, IInnerStateValue<TValue>>? OnChange;

        internal void InvokeOnChange(IInnerStateValue<TValue> newInnerState, IInnerStateValue<TValue> oldInnerState)
            => OnChange?.Invoke(newInnerState, oldInnerState);
    }
}