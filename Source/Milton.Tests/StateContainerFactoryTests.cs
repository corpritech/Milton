using Milton.Abstractions;
using Xunit;

namespace Milton.Tests;

public class StateContainerFactoryTests
{
    [Fact]
    public void StateContainerFactoryCanBeInstantiated()
    {
        var stateContainerFactory = new StateContainerFactory();
        
        Assert.NotNull(stateContainerFactory);
    }

    [Fact]
    public void StateContainerFactoryGeneratesAccurateStateContainer()
    {
        var stateContainerFactory = new StateContainerFactory();
        var stateContainer = stateContainerFactory.CreateStateContainer<MockState>();
        
        Assert.NotNull(stateContainer.CurrentState);
        Assert.Null(stateContainer.PreviousState);
        Assert.NotNull(stateContainer.CurrentState.TestProperty1);
        Assert.NotNull(stateContainer.CurrentState.TestProperty1.Value);
        Assert.Equal("", stateContainer.CurrentState.TestProperty1.Value);
        Assert.NotNull(stateContainer.CurrentState.TestProperty2);
        Assert.NotNull(stateContainer.CurrentState.TestProperty2.Value);
        Assert.Equal("milton", stateContainer.CurrentState.TestProperty2.Value);
        Assert.NotNull(stateContainer.CurrentState.InnerState1);
        Assert.NotNull(stateContainer.CurrentState.InnerState1.CurrentState);
        Assert.Null(stateContainer.CurrentState.InnerState1.PreviousState);
        Assert.Null(stateContainer.CurrentState.InnerState1.CurrentState.TestProperty);
        Assert.NotNull(stateContainer.CurrentState.InnerState2);
        Assert.Equal(0, stateContainer.CurrentState.InnerState2.CurrentState.TestProperty.Value);
    }
    
    private class MockState
    {
        public IStateValue<string> TestProperty1 { get; set; } = new StateValue<string>("");
        [InitialValue("milton")]
        public IStateValue<string> TestProperty2 { get; set; } = null!;
        public IStateContainer<InnerMockState> InnerState1 { get; set; } = new StateContainer<InnerMockState>(new InnerMockState());
        public IStateContainer<InnerMockState> InnerState2 { get; set; } = null!;
    }

    private class InnerMockState
    {
        [InitialValue(0)] 
        public IStateValue<int> TestProperty { get; set; } = null!;
    }
}