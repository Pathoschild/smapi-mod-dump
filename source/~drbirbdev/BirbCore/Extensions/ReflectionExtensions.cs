/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

#nullable enable
using System;
using System.Reflection;

namespace BirbCore.Extensions;
public static class ReflectionExtensions
{
    public const BindingFlags AllDeclared = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    public static MemberInfo? GetMemberOfType(this Type type, Type memberType)
    {
        foreach (FieldInfo fieldInfo in type.GetFields(AllDeclared))
        {
            if (fieldInfo.FieldType == memberType)
            {
                return fieldInfo;
            }
        }
        foreach (PropertyInfo propertyInfo in type.GetProperties(AllDeclared))
        {
            if (propertyInfo.PropertyType == memberType)
            {
                return propertyInfo;
            }
        }
        return null;
    }

    public static MemberInfo? GetMemberOfName(this Type type, string name)
    {
        foreach (FieldInfo fieldInfo in type.GetFields(AllDeclared))
        {
            if (fieldInfo.Name == name)
            {
                return fieldInfo;
            }
        }
        foreach (PropertyInfo propertyInfo in type.GetProperties(AllDeclared))
        {
            if (propertyInfo.Name == name)
            {
                return propertyInfo;
            }
        }
        return null;
    }

    public static MemberInfo? GetMemberWithCustomAttribute(this Type type, Type attributeType)
    {
        foreach (FieldInfo fieldInfo in type.GetFields(AllDeclared))
        {
            foreach (Attribute attribute in fieldInfo.GetCustomAttributes())
            {
                if (attribute.GetType() == attributeType)
                {
                    return fieldInfo;
                }
            }
        }
        foreach (PropertyInfo propertyInfo in type.GetProperties(AllDeclared))
        {
            foreach (Attribute attribute in propertyInfo.GetCustomAttributes())
            {
                if (attribute.GetType() == attributeType)
                {
                    return propertyInfo;
                }
            }
        }
        return null;
    }

    public static Type? GetReflectedType(this MemberInfo member)
    {
        if (member is FieldInfo field)
        {
            return field.FieldType;
        }
        else if (member is PropertyInfo property)
        {
            return property.PropertyType;
        }
        return null;
    }

    public static Func<object?, object?>? GetGetter(this MemberInfo member)
    {
        if (member is FieldInfo field)
        {
            return field.GetValue;
        }
        else if (member is PropertyInfo property)
        {
            return property.GetValue;
        }
        return null;
    }

    public static Action<object?, object?>? GetSetter(this MemberInfo member)
    {
        if (member is FieldInfo field)
        {
            return field.SetValue;
        }
        else if (member is PropertyInfo property)
        {
            return property.SetValue;
        }
        return null;
    }

    public static T InitDelegate<T>(this MethodInfo method, object? instance = null) where T : Delegate
    {
        if (method.IsStatic)
        {
            return (T)Delegate.CreateDelegate(typeof(T), method);
        }
        else
        {
            return (T)Delegate.CreateDelegate(typeof(T), instance, method);
        }
    }
}
