using Milton.Abstractions;
using Xunit;

namespace Milton.Tests;

public class StateTests
{
    [Fact]
    public void StateCanBeInstantiated()
    {
        var mockState = new MockState1();
        var state = new State<MockState1>(mockState);
        
        Assert.NotNull(state);
        Assert.NotNull(state.Properties);
        Assert.True(ReferenceEquals(mockState, state.Properties));
    }
    
    [Fact]
    public void StateEmitsEventWhenStateValuesUpdate()
    {
        var state = new State<MockState1>(new MockState1());
        var emittedEvent = false;
        
        state.OnChange(_ => emittedEvent = true);
        state.Properties.TestProperty1.Value = "milton";
        
        Assert.True(emittedEvent);
    }

    [Fact]
    public void StateEmitsEventWhenNestedStatesEmitEvent()
    {
        var state = new State<MockState1>(new MockState1());
        var emittedEvent = false;
        
        state.OnChange(_ => emittedEvent = true);
        state.Properties.InnerState1.Properties.TestProperty.Value = 1;
        
        Assert.True(emittedEvent);
    }

    private class MockState1
    {
        public IStateProperty<string> TestProperty1 { get; set; } = new StateProperty<string>("");
        public IStateProperty<string> TestProperty2 { get; set; } = null!;
        public IState<MockState2> InnerState1 { get; set; } = new State<MockState2>(new MockState2());
        public IState<MockState2> InnerState2 { get; set; } = null!;
    }

    private class MockState2
    {
        public IStateProperty<int> TestProperty { get; set; } = new StateProperty<int>(0);
    }
}