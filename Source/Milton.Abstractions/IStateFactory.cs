﻿namespace Milton.Abstractions;

public interface IStateFactory
{
    public IState<TInnerState> CreateState<TInnerState>() where TInnerState : class;
    public TInnerState CreateInnerState<TInnerState>() where TInnerState : class;
    public IStateProperty<TValue> CreateStateProperty<TValue>(TValue? initialValue);
}