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
    public const BindingFlags ALL_DECLARED = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    public static bool TryGetMemberOfType(this Type type, Type memberType, out MemberInfo memberInfo)
    {
        foreach (FieldInfo fieldInfo in type.GetFields(ALL_DECLARED))
        {
            if (fieldInfo.FieldType != memberType)
            {
                continue;
            }

            memberInfo = fieldInfo;
            return true;
        }
        foreach (PropertyInfo propertyInfo in type.GetProperties(ALL_DECLARED))
        {
            if (propertyInfo.PropertyType != memberType)
            {
                continue;
            }

            memberInfo = propertyInfo;
            return true;
        }

        memberInfo = typeof(int);
        return false;
    }

    public static bool TryGetMemberOfName(this Type type, string name, out MemberInfo memberInfo)
    {
        foreach (FieldInfo fieldInfo in type.GetFields(ALL_DECLARED))
        {
            if (fieldInfo.Name != name)
            {
                continue;
            }

            memberInfo = fieldInfo;
            return true;
        }
        foreach (PropertyInfo propertyInfo in type.GetProperties(ALL_DECLARED))
        {
            if (propertyInfo.Name != name)
            {
                continue;
            }

            memberInfo = propertyInfo;
            return true;
        }

        memberInfo = typeof(int);
        return false;
    }

    public static bool TryGetGetterOfName(this Type type, string name, out Func<object?, object?> getter)
    {
        if (!TryGetMemberOfName(type, name, out MemberInfo memberInfo))
        {
            getter = null!;
            return false;
        }

        getter = memberInfo.GetGetter();
        return true;
    }

    public static bool TryGetSetterOfName(this Type type, string name, out Action<object?, object?> setter)
    {
        if (!TryGetMemberOfName(type, name, out MemberInfo memberInfo))
        {
            setter = null!;
            return false;
        }

        setter = memberInfo.GetSetter();
        return true;
    }

    public static bool TryGetMemberWithCustomAttribute(this Type type, Type attributeType, out MemberInfo memberInfo)
    {
        foreach (FieldInfo fieldInfo in type.GetFields(ALL_DECLARED))
        {
            foreach (Attribute attribute in fieldInfo.GetCustomAttributes())
            {
                if (attribute.GetType() != attributeType)
                {
                    continue;
                }

                memberInfo = fieldInfo;
                return true;
            }
        }
        foreach (PropertyInfo propertyInfo in type.GetProperties(ALL_DECLARED))
        {
            foreach (Attribute attribute in propertyInfo.GetCustomAttributes())
            {
                if (attribute.GetType() != attributeType)
                {
                    continue;
                }

                memberInfo = propertyInfo;
                return true;
            }
        }

        memberInfo = typeof(int);
        return false;
    }

    public static Type GetReflectedType(this MemberInfo member)
    {
        return member switch
        {
            FieldInfo field => field.FieldType,
            PropertyInfo property => property.PropertyType,
            _ => typeof(int) // default case shouldn't happen
        };
    }

    public static Func<object?, object?> GetGetter(this MemberInfo member)
    {
        return member switch
        {
            FieldInfo field => field.GetValue,
            PropertyInfo property => property.GetValue,
            _ => a => a // default case shouldn't happen
        };
    }

    public static Action<object?, object?> GetSetter(this MemberInfo member)
    {
        return member switch
        {
            FieldInfo field => field.SetValue,
            PropertyInfo property => property.SetValue,
            _ => (a, b) => {} // default case shouldn't happen
        };
    }

    public static T InitDelegate<T>(this MethodInfo method, object? instance = null) where T : Delegate
    {
        if (method.IsStatic)
        {
            return (T)Delegate.CreateDelegate(typeof(T), method);
        }
        return (T)Delegate.CreateDelegate(typeof(T), instance, method);
    }
}
