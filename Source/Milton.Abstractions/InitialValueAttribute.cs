namespace Milton.Abstractions;

[AttributeUsage(AttributeTargets.Property)]
public class InitialValueAttribute : Attribute
{
    public object Value { get; }
    public InitialValueAttribute(object value)
    {
        Value = value;
    }
}