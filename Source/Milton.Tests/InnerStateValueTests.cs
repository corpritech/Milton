using System;
using System.Threading.Tasks;
using Milton.Abstractions;
using Xunit;

namespace Milton.Tests;

public class InnerStateValueTests
{
    [Theory]
    [ClassData(typeof(MockData))]
    public void InnerStateValueCanBeInstantiated(object value)
    {
        var stateValueType = typeof(StateProperty<>).MakeGenericType(value.GetType());
        var stateValue = Activator.CreateInstance(stateValueType, value);

        Assert.NotNull(stateValue);
    }

    [Fact]
    public async Task InnerSetValueAsyncReturnAccurateStateValue()
    {
        const string value = "";
        const string newValue = "milton";

        var stateValue = new StateProperty<string>(value);
        var newStateValue = await stateValue.SetValueAsync(newValue);

        Assert.False(ReferenceEquals(stateValue, newStateValue));
        Assert.Equal(newValue, newStateValue.Value);
        Assert.Equal(value, stateValue.Value);
    }

    [Fact]
    public async Task InnerSetValueAsyncNotifiesHandlers()
    {
        const string value = "";
        const string newValue = "milton";

        var handlerWasInvoked = false;
        var stateValue = new StateProperty<string>(value);

        stateValue.OnChange((_, _) => handlerWasInvoked = true);
        _ = await stateValue.SetValueAsync(newValue);

        Assert.True(handlerWasInvoked);
    }

    [Fact]
    public async Task InnerSetValueAsyncEmitsAccurateEventData()
    {
        const string value = "";
        const string newValue = "milton";

        var handlerWasInvoked = false;
        var stateValue = new StateProperty<string>(value);

        IStateProperty<string>? emittedNewState = null;
        IStateProperty<string>? emittedOldState = null;

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
    public async Task InnerSetValueAsyncHonorsCloneableValueObjects()
    {
        var value = new MockCloneableValueObject("");
        var stateValue = new StateProperty<MockCloneableValueObject>(value) as IStateProperty<MockCloneableValueObject>;
        
        stateValue = await stateValue.SetValueAsync(stateValue.Value);
        
        Assert.True(stateValue.Value.CreatedAsClone);
    }
    
    [Fact]
    public async Task InnerSetValueAsyncWorksWithNonCloneableValueObjects()
    {
        var value = new MockValueObject("");
        var stateValue = new StateProperty<MockValueObject>(value) as IStateProperty<MockValueObject>;
        var newStateValue = await stateValue.SetValueAsync(stateValue.Value);
        
        Assert.True(ReferenceEquals(stateValue.Value, newStateValue.Value));
    }

    [Fact]
    public void StatePropertiesWithTheSameValueAreEqual()
    {
        const string value = "milton";

        var stateValue1 = new StateProperty<string>(value);
        var stateValue2 = new StateProperty<string>(value);

        Assert.True(stateValue1.Equals(stateValue2));
    }

    [Fact]
    public void StatePropertiesWithDifferentValuesAreNotEqual()
    {
        const string value1 = "";
        const string value2 = "milton";

        var stateValue1 = new StateProperty<string>(value1);
        var stateValue2 = new StateProperty<string>(value2);

        Assert.False(stateValue1.Equals(stateValue2));
    }

    [Fact]
    public void StatePropertiesWithNullValuesAreEqual()
    {
        var stateValue1 = new StateProperty<string>(null);
        var stateValue2 = new StateProperty<string>(null);

        Assert.True(stateValue1.Equals(stateValue2));
    }

    [Fact]
    public void StatePropertiesWithDifferentNullValuesAreNotEqual()
    {
        var stateValue1 = new StateProperty<string>("");
        var stateValue2 = new StateProperty<string>(null);

        Assert.False(stateValue1.Equals(stateValue2));
    }

    [Fact]
    public void StatePropertiesWithTheSameValueHaveTheSameHashCode()
    {
        const string value = "milton";

        var stateValue1 = new StateProperty<string>(value);
        var stateValue2 = new StateProperty<string>(value);

        Assert.Equal(stateValue1.GetHashCode(), stateValue2.GetHashCode());
    }

    [Fact]
    public void StatePropertiesWithDifferentValuesHaveDifferentHashCodes()
    {
        const string value1 = "";
        const string value2 = "milton";

        var stateValue1 = new StateProperty<string>(value1);
        var stateValue2 = new StateProperty<string>(value2);

        Assert.NotEqual(stateValue1.GetHashCode(), stateValue2.GetHashCode());
    }

    [Fact]
    public void StatePropertiesWithNullValuesHaveTheSameHashCode()
    {
        var stateValue1 = new StateProperty<string>(null);
        var stateValue2 = new StateProperty<string>(null);

        Assert.Equal(stateValue1.GetHashCode(), stateValue2.GetHashCode());
    }

    [Fact]
    public void StatePropertiesWithDifferentNullValuesHaveDifferentHashCodes()
    {
        var stateValue1 = new StateProperty<string>("");
        var stateValue2 = new StateProperty<string>(null);

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