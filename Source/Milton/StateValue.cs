using Milton.Abstractions;

namespace Milton;

public record StateValue<TValue>(TValue _value) : IStateValue<TValue>
{
    public event Action<IStateValue<TValue>>? OnChange;
    public TValue Value
    {
        set
        {
            _value = value;
            NotifyStateChanged();
        }
        get => _value;
    }
    private TValue _value = _value;

    private void NotifyStateChanged()
    {
        OnChange?.Invoke(this);
    }
}