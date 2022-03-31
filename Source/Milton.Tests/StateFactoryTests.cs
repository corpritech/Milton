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
        
        Assert.NotNull(stateContainer.Properties);
        Assert.NotNull(stateContainer.Properties.TestProperty1);
        Assert.NotNull(stateContainer.Properties.TestProperty1.Value);
        Assert.Equal("", stateContainer.Properties.TestProperty1.Value);
        Assert.NotNull(stateContainer.Properties.TestProperty2);
        Assert.NotNull(stateContainer.Properties.TestProperty2.Value);
        Assert.Equal("milton", stateContainer.Properties.TestProperty2.Value);
        Assert.NotNull(stateContainer.Properties.InnerState1);
        Assert.NotNull(stateContainer.Properties.InnerState1.Properties);
        Assert.Null(stateContainer.Properties.InnerState1.Properties.TestProperty);
        Assert.NotNull(stateContainer.Properties.InnerState2);
        Assert.Equal(0, stateContainer.Properties.InnerState2.Properties.TestProperty.Value);
    }
    
    private class MockState1
    {
        public IStateProperty<string> TestProperty1 { get; set; } = new StateProperty<string>("");
        [InitialValue("milton")]
        public IStateProperty<string> TestProperty2 { get; set; } = null!;
        public IState<MockState2> InnerState1 { get; set; } = new State<MockState2>(new MockState2());
        public IState<MockState2> InnerState2 { get; set; } = null!;
    }

    private class MockState2
    {
        [InitialValue(0)] 
        public IStateProperty<int> TestProperty { get; set; } = null!;
    }
}