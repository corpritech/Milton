namespace Milton.Abstractions;

public interface IStateContainerFactory
{
    public IStateContainer<T> CreateStateContainer<T>() where T : class;
}