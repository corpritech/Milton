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
        Assert.NotNull(state.CurrentState);
        Assert.True(ReferenceEquals(mockState, state.CurrentState));
    }

    [Fact]
    public void StatePreviousStateIsInitiallyNull()
    {
        var state = new State<MockState1>(new MockState1());
        
        Assert.Null(state.PreviousState);
    }

    [Fact]
    public void StateEmitsEventWhenStateValuesUpdate()
    {
        var state = new State<MockState1>(new MockState1());
        var emittedEvent = false;
        
        state.OnChange(_ => emittedEvent = true);
        state.CurrentState.TestProperty1.SetValue("milton");
        
        Assert.True(emittedEvent);
    }

    [Fact]
    public void StateEmitsEventWhenNestedStatesEmitEvent()
    {
        var state = new State<MockState1>(new MockState1());
        var emittedEvent = false;
        
        state.OnChange(_ => emittedEvent = true);
        state.CurrentState.InnerState1.CurrentState.TestProperty.SetValue(1);
        
        Assert.True(emittedEvent);
    }

    [Fact]
    public void StateRotatesCurrentStateToPreviousStateOnValueChanges()
    {
        var state = new State<MockState1>(new MockState1());
        
        state.CurrentState.TestProperty1.SetValue("milton");
        
        Assert.NotNull(state.PreviousState);
        Assert.False(ReferenceEquals(state.CurrentState, state.PreviousState));
        Assert.Equal("", state.PreviousState!.TestProperty1.Value);
        Assert.Equal("milton", state.CurrentState.TestProperty1.Value);
    }

    private class MockState1
    {
        public IInnerStateValue<string> TestProperty1 { get; set; } = new InnerStateValue<string>("");
        public IInnerStateValue<string> TestProperty2 { get; set; } = null!;
        public IState<MockState2> InnerState1 { get; set; } = new State<MockState2>(new MockState2());
        public IState<MockState2> InnerState2 { get; set; } = null!;
    }

    private class MockState2
    {
        public IInnerStateValue<int> TestProperty { get; set; } = new InnerStateValue<int>(0);
    }
}