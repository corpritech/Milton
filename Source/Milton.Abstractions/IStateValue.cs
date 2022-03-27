namespace Milton.Abstractions;

public interface IStateValue<T>
{
    public event Action<IStateValue<T>> OnChange;
    public T Value { get; set; }
}