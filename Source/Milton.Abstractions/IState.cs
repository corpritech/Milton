using System.Linq.Expressions;

namespace Milton.Abstractions;

public interface IState<TInnerState> where TInnerState : class
{
    public TInnerState CurrentState { get; }
    public Task UpdateAsync<TProperty>(Expression<Func<TInnerState, TProperty>> propertyExpression, TProperty value);
    public Task UpdateAsync(Action<IStateUpdateBuilder<TInnerState>> updateAction);
    public void OnChange(Action<IState<TInnerState>> onChangeAction);
    public void OnChange<TProperty>(Expression<Func<TInnerState, TProperty>> propertyExpression, Action<TProperty> onChangeAction);
    public void OnChange<TProperty>(Expression<Func<TInnerState, TProperty>> propertyExpression, Action<TProperty, TProperty> onChangeAction);
}