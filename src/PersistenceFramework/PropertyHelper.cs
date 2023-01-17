using System.Collections;
using System.Reflection;

namespace Holize.PersistenceFramework;

/// <summary>
///     A class that helps to copy properties between two objects of the same type without changing reference of any nested
///     fields.
/// </summary>
internal static class PropertyHelper
{
    /// <summary>
    ///     Copies properties from the <paramref name="source" /> to the <paramref name="destination" /> without changing
    ///     instances contained in the <paramref name="destination" />.
    /// </summary>
    /// <param name="source">Object from which copy is going to be performed.</param>
    /// <param name="destination">Object whose properties are to be updated.</param>
    /// <exception cref="ArgumentNullException">Thrown when source or destination is <see langword="null" />.</exception>
    /// <typeparam name="T">The type to copy.</typeparam>
    public static void CopyProperties<T>(T source, T destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        var propertiesToCopy = GetProperties(source.GetType());
        // update properties
        foreach (var property in propertiesToCopy)
        {
            var typeOfProperty = PropertyType(property);
            CopyProperty(source, destination, typeOfProperty, property);
        }
    }


    /// <summary>
    ///     Copies properties from the <paramref name="source" /> to the <paramref name="destination" /> without changing
    ///     instances contained in the <paramref name="destination" />.
    /// </summary>
    /// <param name="source">Object from which copy is going to be performed.</param>
    /// <param name="destination">Object whose properties are to be updated.</param>
    /// <param name="typeOfProperty">The type of property.</param>
    /// <param name="property">The property info.</param>
    private static void CopyProperty(object source, object destination, ObjectType typeOfProperty,
        PropertyInfo property)
    {
        // Because we don't want to deal with all sorts problems (mainly event rewiring) that occurs when
        // changing property that is an object copy of parameters from the source to destination
        // will be performed only on the primitive types.
        // Any type that is not a primitive will be accessed recursively in order to lower it to primitive
        switch (typeOfProperty)
        {
            case ObjectType.Class:
            case ObjectType.Struct:
                SetReferenceProperty(source, destination, property);
                break;
            case ObjectType.Primitive:
            case ObjectType.Enum:
            default:
                SetPrimitiveProperty(source, destination, property);
                break;
        }
    }


    /// <summary>
    ///     Updates the property that is of a type <see cref="ObjectType.Class" /> or <see cref="ObjectType.Struct" />.
    /// </summary>
    /// <param name="source">Object from which copy is going to be performed.</param>
    /// <param name="destination">Object whose properties are to be updated.</param>
    /// <param name="property">The property to update.</param>
    private static void SetReferenceProperty(object source, object destination, PropertyInfo property)
    {
        switch (destination)
        {
            case Array:
                property.SetValue(destination, source);
                return;
            case IList destinationAsIList:
            {
                for (var i = destinationAsIList.Count - 1; i >= 0; i--) destinationAsIList.RemoveAt(i);

                foreach (var item in (IList)source) destinationAsIList.Add(item);

                return;
            }
        }

        var sourceValue = property.GetValue(source);
        var destinationValue = property.GetValue(destination);


        // destination object / sourceValue does not have any value therefor
        // we don't have to worry about keeping same instance of the object
        if (destinationValue is null || sourceValue is null)
        {
            property.SetValue(destination, sourceValue);
            return;
        }

        // update nested property
        CopyProperties(sourceValue, destinationValue);
    }


    /// <summary>
    ///     Updates the property that is of a type <see cref="ObjectType.Primitive" />.
    /// </summary>
    /// <param name="source">Object from which copy is going to be performed.</param>
    /// <param name="destination">Object whose properties are to be updated.</param>
    /// <param name="property">The property to update.</param>
    private static void SetPrimitiveProperty(object source, object destination, PropertyInfo property)
    {
        // updating a primitive value in the collection
        // updating a primitive value in the collection
        if (source is ICollection)
        {
            SetReferenceProperty(source, destination, property);
            return;
        }

        var val = property.GetValue(source);
        property.SetValue(destination, val);
    }


    /// <summary>
    ///     Get a type of the <see cref="property" />.
    /// </summary>
    /// <param name="property">The property whose type is to be retrieved.</param>
    private static ObjectType PropertyType(PropertyInfo property)
    {
        // for the purposes of copying values string should be represented as a primitive (copiable value)
        if (property.PropertyType == typeof(string))
            return ObjectType.Primitive;

        if (property.PropertyType.IsEnum)
            return ObjectType.Enum;

        if (property.PropertyType.IsClass)
            return ObjectType.Class;

        return property.PropertyType is { IsValueType: true, IsPrimitive: false }
            ? ObjectType.Struct
            : ObjectType.Primitive;
    }

    /// <summary>
    ///     Get collection of <see cref="PropertyInfo" /> of public properties.
    /// </summary>
    /// <param name="type">The type whose properties are to be retrieved.</param>
    private static IEnumerable<PropertyInfo> GetProperties(Type type)
    {
        return type.GetProperties();
    }
}