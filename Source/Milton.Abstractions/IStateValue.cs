﻿namespace Milton.Abstractions;

public interface IStateValue<T>
{
    public T Value { get; }
    public IStateValue<T> SetValue(T value);
    public void OnChange(Action<IStateValue<T>, IStateValue<T>> handler);
}