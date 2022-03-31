namespace Milton.Abstractions;

public interface IInnerStateValue<TValue>
{
    public TValue Value { get; }
    public IInnerStateValue<TValue> SetValue(TValue value);
    public Task<IInnerStateValue<TValue>> SetValueAsync(TValue value);
    public void OnChange(Action<IInnerStateValue<TValue>, IInnerStateValue<TValue>> handler);
}