namespace Microsoft.AspNetCore.Components;

public class PropertyUpdateEvent<TProperty>
{
    public TProperty NewValue { get; }
    public TProperty OldValue { get; }

    public PropertyUpdateEvent(TProperty newValue, TProperty oldValue)
    {
        NewValue = newValue;
        OldValue = oldValue;
    }
}