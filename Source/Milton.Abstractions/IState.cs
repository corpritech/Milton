namespace Milton.Abstractions;

public interface IState<out TInnerState> where TInnerState : class
{
    public TInnerState Properties { get; }
    public void OnChange(Action<IState<TInnerState>> handler);
}