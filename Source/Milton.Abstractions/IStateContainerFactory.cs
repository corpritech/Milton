namespace Milton.Abstractions;

public interface IStateContainerFactory
{
    public IStateContainer<TState> CreateStateContainer<TState>() where TState : class;
}