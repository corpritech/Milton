using System;
using Xunit;

namespace CorpriTech.Milton.Tests;

public class StateTests
{
    [Fact]
    public void StateCanBeInstantiated()
    {
        var state = new State<MockState>();
        
        Assert.NotNull(state);
        Assert.NotNull(state.CurrentState);
    }
    
    [Fact]
    public void StateCanBeInstantiatedWithDefinedState()
    {
        var mockState = new MockState();
        var state = new State<MockState>(mockState);
        
        Assert.NotNull(state);
        Assert.NotNull(state.CurrentState);
        Assert.True(ReferenceEquals(mockState, state.CurrentState));
    }

    [Fact]
    public void StateCanBeUpdated()
    {
        var state = new State<MockState>();
        const string newStringValue = "Milton";

        state.UpdateAsync(x => x.String, newStringValue);
        
        Assert.Equal(newStringValue, state.CurrentState.String);
    }
    
    [Fact]
    public void StateCanBeUpdatedInBulk()
    {
        var state = new State<MockState>();
        const string newStringValue = "Milton";
        const int newIntValue = 12345;
        
        state.UpdateAsync(x =>
        {
            x.Update(s => s.String, newStringValue);
            x.Update(s => s.Number, newIntValue);
        });
        
        Assert.Equal(newStringValue, state.CurrentState.String);
        Assert.Equal(newIntValue, state.CurrentState.Number);
    }
    
    [Fact]
    public void BulkStateUpdatesUseLastPropertyValue()
    {
        var state = new State<MockState>();
        const string newStringValue = "Milton";
        const string secondNewStringValue = "Box";
        
        state.UpdateAsync(x =>
        {
            x.Update(s => s.String, newStringValue);
            x.Update(s => s.String, secondNewStringValue);
        });
        
        Assert.Equal(secondNewStringValue, state.CurrentState.String);
    }

    [Fact]
    public void StateIsNotMutatedOnUpdate()
    {
        var state = new State<MockState>();
        var oldState = state.CurrentState;
        const string newStringValue = "Milton";

        state.UpdateAsync(x => x.String, newStringValue);
        
        Assert.False(ReferenceEquals(oldState, state.CurrentState));
    }
    
    [Fact]
    public void CloneableStatesAreInvokedOnUpdate()
    {
        var state = new State<MockCloneableState>();
        const string newStringValue = "Milton";

        state.UpdateAsync(x => x.String, newStringValue);
        
        Assert.True(state.CurrentState.WasCloned);
    }
    
    [Fact]
    public void StateEmitsOnChangeWhenPropertiesAreUpdated()
    {
        var state = new State<MockState>();
        var emittedEvent = false;
        
        state.OnChange(_ => emittedEvent = true);
        state.UpdateAsync(x => x.Number, 12345);
        
        Assert.True(emittedEvent);
    }
    
    [Fact]
    public void StateOnChangeFiltersEmitWhenSelectedPropertiesAreUpdated()
    {
        var state = new State<MockState>();
        var emittedEvent = false;
        
        state.OnChange(x => x.Number, _ => emittedEvent = true);
        state.UpdateAsync(x => x.Number, 12345);
        
        Assert.True(emittedEvent);
    }
    
    [Fact]
    public void StateOnChangeFiltersDoNotEmitWhenUnselectedPropertiesAreUpdated()
    {
        var state = new State<MockState>();
        var emittedEvent = false;
        
        state.OnChange(x => x.String, _ => emittedEvent = true);
        state.UpdateAsync(x => x.Number, 12345);
        
        Assert.False(emittedEvent);
    }
    
    [Fact]
    public void StateOnChangeEmitsOnceForBulkChanges()
    {
        var state = new State<MockState>();
        var totalOnChangeEmits = 0;

        state.OnChange(_ => totalOnChangeEmits++);
        state.UpdateAsync(x =>
        {
            x.Update(s => s.String, "Milton");
            x.Update(s => s.Number, 12345);
        });

        Assert.Equal(1, totalOnChangeEmits);
    }

    private class MockCloneableState : MockState, ICloneable
    {
        public bool WasCloned { get; private init; }= false;
        public object Clone()
        {
            return new MockCloneableState()
            {
                WasCloned = true
            };
        }
    }
    
    private class MockState
    {
        public int Number { get; init; } = 0;
        public string String { get; init; } = "";
    }
}