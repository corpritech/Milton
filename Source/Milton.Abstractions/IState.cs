namespace Milton.Abstractions;

public interface IState<out TInnerState> where TInnerState : class
{
    public TInnerState CurrentState { get; }
    public TInnerState? PreviousState { get; }
    public void OnChange(Action<IState<TInnerState>> handler);
}