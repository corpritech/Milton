using System.Reflection;

namespace CorpriTech.Milton.Extensions;

public static class PropertyInfoExtensions
{
    public static bool IsStateProperty(this PropertyInfo propertyInfo)
        => propertyInfo.CanRead &&
           propertyInfo.CanWrite &&
           propertyInfo.GetSetMethod() != null && 
           propertyInfo.GetGetMethod() != null &&
           !propertyInfo.PropertyType.IsGenericType &&
           !propertyInfo.PropertyType.IsAbstract;
}