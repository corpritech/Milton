namespace Milton.Abstractions;

public interface IStateContainer<out T> where T : class
{
    public T CurrentState { get; }
    public T? PreviousState { get; }
    public void OnChange(Action<IStateContainer<T>> handler);
}