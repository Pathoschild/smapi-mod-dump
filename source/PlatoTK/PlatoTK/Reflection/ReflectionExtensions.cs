/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;
using System.Linq;
using System.Reflection;

namespace PlatoTK.Reflection
{
    public static class ReflectionExtensions
    {
        public static IPrivateFields GetPrivateFields(this object obj)
        {
            return new PrivateFields(obj);
        }

        public static IPrivateFields GetPrivateProperties(this object obj)
        {
            return new PrivateProperties(obj);
        }

        public static IPrivateMethods GetPrivateMethods(this object obj)
        {
            return new PrivateMethods(obj);
        }

        public static object GetFieldValue(this object obj, string field, bool isStatic = false)
        {
            Type t = obj is Type ? (Type)obj : obj.GetType();
            if (obj is Type)
                isStatic = true;
            return t.GetField(field, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(isStatic ? null : obj);
        }

        public static T GetFieldValue<T>(this object obj, string field, bool isStatic = false)
        {
            Type t = obj is Type ? (Type)obj : obj.GetType();
            if (obj is Type)
                isStatic = true;
            return (T)t.GetField(field, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(isStatic ? null : obj);
        }

        public static void SetFieldValue(this object obj, string field, object value, bool isStatic = false)
        {
            Type t = obj is Type ? (Type)obj : obj.GetType();
            if (obj is Type)
                isStatic = true;
            t.GetField(field, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(isStatic ? null : obj, value);
        }

        public static object GetPropertyValue(this object obj, string property, bool isStatic = false)
        {
            Type t = obj is Type ? (Type)obj : obj.GetType();
            if (obj is Type)
                isStatic = true;
            return t.GetProperty(property, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(isStatic ? null : obj);
        }

        public static void SetPropertyValue(this object obj, object value, string property, bool isStatic = false)
        {
            if (obj is Type)
                isStatic = true;
            Type t = obj is Type ? (Type)obj : obj.GetType();
            t.GetProperty(property, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(isStatic ? null : obj, value);
        }

        public static void CallAction(this object obj, string action, params object[] args)
        {
            bool isStatic = false;

            Type t = obj is Type ? (Type)obj : obj.GetType();
            if (obj is Type)
                isStatic = true;
            t.GetMethod(
                action,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                Type.DefaultBinder,
                args.Select(o => o.GetType()).ToArray(),
                new ParameterModifier[0])
                ?.Invoke(isStatic ? null : obj, args);
        }

        public static T CallFunction<T>(this object obj, string action, params object[] args)
        {
            bool isStatic = false;
            if (obj is Type)
                isStatic = true;

            Type t = obj is Type ? (Type)obj : obj.GetType();

            return (T)t.GetMethod(
                            action,
                            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                            Type.DefaultBinder,
                            args.Select(o => o.GetType()).ToArray(),
                            new ParameterModifier[0])
                            ?.Invoke(isStatic ? null : obj, args);
        }

    }
}
