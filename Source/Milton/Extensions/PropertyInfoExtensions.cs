using System.Reflection;

namespace CorpriTech.Milton.Extensions;

#pragma warning disable CS1591
public static class PropertyInfoExtensions
#pragma warning restore CS1591
{
    /// <summary>
    /// Identifies whether or not the specified <see cref="PropertyInfo"/> can be used as a valid state property.
    /// </summary>
    /// <param name="propertyInfo">The property to check.</param>
    /// <returns>Whether or not the specified property can be used as a valid state property.</returns>
    public static bool IsStateProperty(this PropertyInfo propertyInfo)
        => propertyInfo.CanRead &&
           propertyInfo.CanWrite &&
           propertyInfo.GetSetMethod() != null &&
           propertyInfo.GetGetMethod() != null &&
           !propertyInfo.PropertyType.IsGenericType &&
           !propertyInfo.PropertyType.IsAbstract;
}