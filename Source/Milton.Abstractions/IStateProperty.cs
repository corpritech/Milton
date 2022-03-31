namespace Milton.Abstractions;

public interface IStateProperty<TValue>
{
    public TValue Value { get; }
    public IStateProperty<TValue> SetValue(TValue value);
    public Task<IStateProperty<TValue>> SetValueAsync(TValue value);
    public void OnChange(Action<IStateProperty<TValue>, IStateProperty<TValue>> handler);
}