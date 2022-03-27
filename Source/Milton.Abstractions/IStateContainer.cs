namespace Milton.Abstractions;

public interface IStateContainer<out T> where T : class
{
    public event Action<IStateContainer<T>> OnChange;
    public T State { get; }
}