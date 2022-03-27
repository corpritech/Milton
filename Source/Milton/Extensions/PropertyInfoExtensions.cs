using System.Reflection;
using Milton.Abstractions;

namespace Milton.Extensions;

public static class PropertyInfoExtensions
{
    public static bool IsStateValue(this PropertyInfo propertyInfo)
        => propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericArguments().Length == 1 && typeof(IStateValue<>)
            .MakeGenericType(propertyInfo.PropertyType.GetGenericArguments()[0]).IsAssignableFrom(propertyInfo.PropertyType);
    
    public static bool IsStateContainer(this PropertyInfo propertyInfo)
        => propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericArguments().Length == 1 && propertyInfo.PropertyType.GetGenericArguments()[0].IsClass 
           && typeof(IStateContainer<>).MakeGenericType(propertyInfo.PropertyType.GetGenericArguments()[0]).IsAssignableFrom(propertyInfo.PropertyType);
}