﻿using System.Linq.Expressions;

namespace CorpriTech.Milton.Abstractions;

public interface IStateUpdateBuilder<TInnerState> where TInnerState : class
{
    public IStateUpdateBuilder<TInnerState> Update<TProperty>(Expression<Func<TInnerState, TProperty>> propertyExpression, TProperty value);
}