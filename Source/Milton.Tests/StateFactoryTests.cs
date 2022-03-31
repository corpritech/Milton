using Milton.Abstractions;
using Xunit;

namespace Milton.Tests;

public class StateFactoryTests
{
    [Fact]
    public void StateFactoryCanBeInstantiated()
    {
        var stateContainerFactory = new StateFactory();
        
        Assert.NotNull(stateContainerFactory);
    }

    [Fact]
    public void StateFactoryGeneratesAccurateStateContainer()
    {
        var stateContainerFactory = new StateFactory();
        var stateContainer = stateContainerFactory.CreateState<MockState1>();
        
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
    
    private class MockState1
    {
        public IInnerStateValue<string> TestProperty1 { get; set; } = new InnerStateValue<string>("");
        [InitialValue("milton")]
        public IInnerStateValue<string> TestProperty2 { get; set; } = null!;
        public IState<MockState2> InnerState1 { get; set; } = new State<MockState2>(new MockState2());
        public IState<MockState2> InnerState2 { get; set; } = null!;
    }

    private class MockState2
    {
        [InitialValue(0)] 
        public IInnerStateValue<int> TestProperty { get; set; } = null!;
    }
}