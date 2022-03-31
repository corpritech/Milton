using System.Reflection;
using Milton.Abstractions;

namespace Milton.Extensions;

internal static class PropertyInfoExtensions
{
    internal static bool IsInnerStateValue(this PropertyInfo propertyInfo)
        => propertyInfo.GetSetMethod() != null && 
           propertyInfo.GetGetMethod() != null &&
           propertyInfo.PropertyType.IsInterface &&
           propertyInfo.PropertyType.IsGenericType &&
           propertyInfo.PropertyType.GetGenericArguments().Length == 1 &&
           propertyInfo.PropertyType.IsAssignableFrom(typeof(IInnerStateValue<>).MakeGenericType(propertyInfo.PropertyType.GetGenericArguments()[0]));

    internal static bool IsState(this PropertyInfo propertyInfo)
        => propertyInfo.GetSetMethod() != null &&
           propertyInfo.GetGetMethod() != null &&
           propertyInfo.PropertyType.IsInterface &&
           propertyInfo.PropertyType.IsGenericType &&
           propertyInfo.PropertyType.GetGenericArguments().Length == 1 &&
           propertyInfo.PropertyType.GetGenericArguments()[0].IsClass &&
           propertyInfo.PropertyType.IsAssignableFrom(typeof(IState<>).MakeGenericType(propertyInfo.PropertyType.GetGenericArguments()[0]));
}