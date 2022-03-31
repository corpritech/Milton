using System.Reflection;
using Milton.Abstractions;

namespace Milton.Extensions;

public static class PropertyInfoExtensions
{
    public static bool IsStateProperty(this PropertyInfo propertyInfo)
        => propertyInfo.GetSetMethod() != null && 
           propertyInfo.GetGetMethod() != null &&
           propertyInfo.PropertyType.IsInterface &&
           propertyInfo.PropertyType.IsGenericType &&
           propertyInfo.PropertyType.GetGenericArguments().Length == 1 &&
           propertyInfo.PropertyType.IsAssignableFrom(typeof(IStateProperty<>).MakeGenericType(propertyInfo.PropertyType.GetGenericArguments()[0]));

    public static bool IsState(this PropertyInfo propertyInfo)
        => propertyInfo.GetSetMethod() != null &&
           propertyInfo.GetGetMethod() != null &&
           propertyInfo.PropertyType.IsInterface &&
           propertyInfo.PropertyType.IsGenericType &&
           propertyInfo.PropertyType.GetGenericArguments().Length == 1 &&
           propertyInfo.PropertyType.GetGenericArguments()[0].IsClass &&
           propertyInfo.PropertyType.IsAssignableFrom(typeof(IState<>).MakeGenericType(propertyInfo.PropertyType.GetGenericArguments()[0]));
}