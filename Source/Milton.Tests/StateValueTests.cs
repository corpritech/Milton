using System;
using System.Threading.Tasks;
using Milton.Abstractions;
using Xunit;

namespace Milton.Tests;

public class StateValueTests
{
    [Theory]
    [ClassData(typeof(MockData))]
    public void StateValueCanBeInstantiated(object value)
    {
        var stateValueType = typeof(StateValue<>).MakeGenericType(value.GetType());
        var stateValue = Activator.CreateInstance(stateValueType, value);

        Assert.NotNull(stateValue);
    }

    [Fact]
    public async Task SetValueAsyncReturnAccurateStateValue()
    {
        const string value = "";
        const string newValue = "milton";

        var stateValue = new StateValue<string>(value);
        var newStateValue = await stateValue.SetValueAsync(newValue);

        Assert.False(ReferenceEquals(stateValue, newStateValue));
        Assert.Equal(newValue, newStateValue.Value);
        Assert.Equal(value, stateValue.Value);
    }

    [Fact]
    public async Task SetValueAsyncNotifiesHandlers()
    {
        const string value = "";
        const string newValue = "milton";

        var handlerWasInvoked = false;
        var stateValue = new StateValue<string>(value);

        stateValue.OnChange((_, _) => handlerWasInvoked = true);
        _ = await stateValue.SetValueAsync(newValue);

        Assert.True(handlerWasInvoked);
    }

    [Fact]
    public async Task SetValueAsyncEmitsAccurateEventData()
    {
        const string value = "";
        const string newValue = "milton";

        var handlerWasInvoked = false;
        var stateValue = new StateValue<string>(value);

        IStateValue<string>? emittedNewState = null;
        IStateValue<string>? emittedOldState = null;

        stateValue.OnChange((newStateValue, oldStateValue) =>
        {
            emittedNewState = newStateValue;
            emittedOldState = oldStateValue;
        });

        _ = await stateValue.SetValueAsync(newValue);

        Assert.NotNull(emittedNewState);
        Assert.NotNull(emittedOldState);
        Assert.False(ReferenceEquals(emittedNewState, emittedOldState));
        Assert.NotEqual(emittedNewState, emittedOldState);
        Assert.Equal(newValue, emittedNewState!.Value);
        Assert.Equal(value, emittedOldState!.Value);
    }
    
    [Fact]
    public async Task SetValueAsyncHonorsCloneableValueObjects()
    {
        var value = new MockCloneableValueObject("");
        var stateValue = new StateValue<MockCloneableValueObject>(value) as IStateValue<MockCloneableValueObject>;
        
        stateValue = await stateValue.SetValueAsync(stateValue.Value);
        
        Assert.True(stateValue.Value.CreatedAsClone);
    }
    
    [Fact]
    public async Task SetValueAsyncWorksWithNonCloneableValueObjects()
    {
        var value = new MockValueObject("");
        var stateValue = new StateValue<MockValueObject>(value) as IStateValue<MockValueObject>;
        var newStateValue = await stateValue.SetValueAsync(stateValue.Value);
        
        Assert.True(ReferenceEquals(stateValue.Value, newStateValue.Value));
    }

    [Fact]
    public void StateValuesWithTheSameValueAreEqual()
    {
        const string value = "milton";

        var stateValue1 = new StateValue<string>(value);
        var stateValue2 = new StateValue<string>(value);

        Assert.True(stateValue1.Equals(stateValue2));
    }

    [Fact]
    public void StateValuesWithDifferentValuesAreNotEqual()
    {
        const string value1 = "";
        const string value2 = "milton";

        var stateValue1 = new StateValue<string>(value1);
        var stateValue2 = new StateValue<string>(value2);

        Assert.False(stateValue1.Equals(stateValue2));
    }

    [Fact]
    public void StateValuesWithNullValuesAreEqual()
    {
        var stateValue1 = new StateValue<string>(null);
        var stateValue2 = new StateValue<string>(null);

        Assert.True(stateValue1.Equals(stateValue2));
    }

    [Fact]
    public void StateValuesWithDifferentNullValuesAreNotEqual()
    {
        var stateValue1 = new StateValue<string>("");
        var stateValue2 = new StateValue<string>(null);

        Assert.False(stateValue1.Equals(stateValue2));
    }

    [Fact]
    public void StateValuesWithTheSameValueHaveTheSameHashCode()
    {
        const string value = "milton";

        var stateValue1 = new StateValue<string>(value);
        var stateValue2 = new StateValue<string>(value);

        Assert.Equal(stateValue1.GetHashCode(), stateValue2.GetHashCode());
    }

    [Fact]
    public void StateValuesWithDifferentValuesHaveDifferentHashCodes()
    {
        const string value1 = "";
        const string value2 = "milton";

        var stateValue1 = new StateValue<string>(value1);
        var stateValue2 = new StateValue<string>(value2);

        Assert.NotEqual(stateValue1.GetHashCode(), stateValue2.GetHashCode());
    }

    [Fact]
    public void StateValuesWithNullValuesHaveTheSameHashCode()
    {
        var stateValue1 = new StateValue<string>(null);
        var stateValue2 = new StateValue<string>(null);

        Assert.Equal(stateValue1.GetHashCode(), stateValue2.GetHashCode());
    }

    [Fact]
    public void StateValuesWithDifferentNullValuesHaveDifferentHashCodes()
    {
        var stateValue1 = new StateValue<string>("");
        var stateValue2 = new StateValue<string>(null);

        Assert.NotEqual(stateValue1.GetHashCode(), stateValue2.GetHashCode());
    }

    private class MockCloneableValueObject : ICloneable
    {
        public bool CreatedAsClone { get; private init; }
        public string Value { get; }

        public MockCloneableValueObject(string value)
        {
            Value = value;
        }

        public object Clone()
            => new MockCloneableValueObject(Value)
            {
                CreatedAsClone = true
            };
    }
    
    private class MockValueObject
    {
        public string Value { get; }

        public MockValueObject(string value)
        {
            Value = value;
        }
    }

    private class MockData : TheoryData<object>
    {
        public MockData()
        {
            Add("");
            Add(123);
            Add(true);
            Add(new object());
        }
    }
}