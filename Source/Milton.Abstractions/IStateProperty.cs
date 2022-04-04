namespace Milton.Abstractions;

public interface IStateProperty<TValue>
{
    public TValue Value { get; set; }
    public bool  HasChanged { get; }
    public Task<IStateProperty<TValue>> SetValueAsync(TValue value);
    public void OnChange(Action<IStateProperty<TValue>, IStateProperty<TValue>> handler);
}