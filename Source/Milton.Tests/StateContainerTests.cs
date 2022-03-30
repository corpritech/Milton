using Milton.Abstractions;
using Xunit;

namespace Milton.Tests;

public class StateContainerTests
{
    [Fact]
    public void StateContainerCanBeInstantiated()
    {
        var mockState = new MockState();
        var stateContainer = new StateContainer<MockState>(mockState);
        
        Assert.NotNull(stateContainer);
        Assert.NotNull(stateContainer.CurrentState);
        Assert.True(ReferenceEquals(mockState, stateContainer.CurrentState));
    }

    [Fact]
    public void StateContainerPreviousStateIsInitiallyNull()
    {
        var stateContainer = new StateContainer<MockState>(new MockState());
        
        Assert.Null(stateContainer.PreviousState);
    }

    [Fact]
    public void StateContainerEmitsEventWhenStateValuesUpdate()
    {
        var stateContainer = new StateContainer<MockState>(new MockState());
        var containerEmittedEvent = false;
        
        stateContainer.OnChange(_ => containerEmittedEvent = true);
        stateContainer.CurrentState.TestProperty1.SetValue("milton");
        
        Assert.True(containerEmittedEvent);
    }

    [Fact]
    public void StateContainerEmitsEventWhenNestedStateContainersEmitEvent()
    {
        var stateContainer = new StateContainer<MockState>(new MockState());
        var containerEmittedEvent = false;
        
        stateContainer.OnChange(_ => containerEmittedEvent = true);
        stateContainer.CurrentState.InnerState1.CurrentState.TestProperty.SetValue(1);
        
        Assert.True(containerEmittedEvent);
    }

    [Fact]
    public void StateContainerRotatesCurrentStateToPreviousStateOnValueChanges()
    {
        var stateContainer = new StateContainer<MockState>(new MockState());
        
        stateContainer.CurrentState.TestProperty1.SetValue("milton");
        
        Assert.NotNull(stateContainer.PreviousState);
        Assert.False(ReferenceEquals(stateContainer.CurrentState, stateContainer.PreviousState));
        Assert.Equal("", stateContainer.PreviousState!.TestProperty1.Value);
        Assert.Equal("milton", stateContainer.CurrentState.TestProperty1.Value);
    }

    private class MockState
    {
        public IStateValue<string> TestProperty1 { get; set; } = new StateValue<string>("");
        public IStateValue<string> TestProperty2 { get; set; } = null!;
        public IStateContainer<InnerMockState> InnerState1 { get; set; } = new StateContainer<InnerMockState>(new InnerMockState());
        public IStateContainer<InnerMockState> InnerState2 { get; set; } = null!;
    }

    private class InnerMockState
    {
        public IStateValue<int> TestProperty { get; set; } = new StateValue<int>(0);
    }
}