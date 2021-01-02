/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Underscore76/SDVPracticeMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpeedrunPractice.Framework
{
    public class Reflector
    {
        public static Dictionary<string, FieldInfo> FieldInfos = new Dictionary<string, FieldInfo>();
        public static Dictionary<string, PropertyInfo> PropertyInfos = new Dictionary<string, PropertyInfo>();
        public static Dictionary<string, MethodInfo> MethodInfos = new Dictionary<string, MethodInfo>();

        public const BindingFlags AllFlags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        public const BindingFlags HiddenFlags = (BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo GetField<T>(T type, string field, BindingFlags flags = HiddenFlags)
        {
            string key = typeof(T).Name + "__" + field;
            if (FieldInfos.ContainsKey(key))
            {
                return FieldInfos[key];
            }
            FieldInfo info = typeof(T).GetField(field, flags);
            FieldInfos.Add(key, info);
            return info;
        }

        public static PropertyInfo GetProperty<T>(T type, string field, BindingFlags flags = HiddenFlags)
        {
            string key = typeof(T).Name + "__" + field;
            if (PropertyInfos.ContainsKey(key))
            {
                return PropertyInfos[key];
            }
            PropertyInfo info = typeof(T).GetProperty(field, flags);
            PropertyInfos.Add(key, info);
            return info;
        }
        public static MethodInfo GetMethod<T>(T type, string field, BindingFlags flags = HiddenFlags)
        {
            string key = nameof(type) + "__" + field;
            if (MethodInfos.ContainsKey(key))
            {
                return MethodInfos[key];
            }
            MethodInfo info = typeof(T).GetMethod(field, flags);
            MethodInfos.Add(key, info);
            return info;
        }


        public static V GetValue<T, V>(T obj, string field, BindingFlags flags = HiddenFlags)
        {
            FieldInfo info = GetField(obj, field, flags);
            if (info != null)
            {
                V value = (V)info.GetValue(obj);
                return value;
            }
            else
            {
                PropertyInfo pinfo = GetProperty(obj, field, flags);
                if (pinfo != null)
                {
                    V value = (V)pinfo.GetValue(obj, null);
                    return value;
                }
            }
            return default(V);
        }

        public static Type GetValueType<T>(T obj, string field, BindingFlags flags = AllFlags)
        {
            FieldInfo info = GetField(obj, field, flags);
            if (info != null)
            {
                return info.FieldType;
            }
            else
            {
                PropertyInfo pinfo = GetProperty(obj, field, flags);
                if (pinfo != null)
                {
                    return pinfo.PropertyType;
                }
            }
            return null;
        }

        public static object GetValue<T>(T obj, string field, BindingFlags flags = AllFlags)
        {
            FieldInfo info = GetField(obj, field, flags);
            if (info != null)
            {
                return info.GetValue(obj);
            }
            else
            {
                PropertyInfo pinfo = GetProperty(obj, field, flags);
                if (pinfo != null)
                {
                    return pinfo.GetValue(obj, null);
                }
            }
            return null;
        }

        public static void SetValue<T, V>(T obj, string field, V val, BindingFlags flags = HiddenFlags)
        {
            FieldInfo info = GetField(obj, field, flags);
            if (info != null)
            {
                info.SetValue(obj, val);
            }
            else
            {
                PropertyInfo pinfo = GetProperty(obj, field, flags);
                if (pinfo != null)
                {
                    pinfo.SetValue(obj, val, null);
                }
            }
        }

        public static void InvokeMethod<T>(T obj, string field, object[] args = null, BindingFlags flags = HiddenFlags)
        {
            MethodInfo info = GetMethod(obj, field, flags);
            if (info != null)
            {
                info.Invoke(obj, args);
                return;
            }

            throw new MethodAccessException(string.Format("Method does not exist: {0}::{1}", typeof(T).Name, field));
        }

        public static V InvokeMethod<T, V>(T obj, string field, object[] args = null, BindingFlags flags = HiddenFlags)
        {
            MethodInfo info = GetMethod(obj, field, flags);
            if (info != null)
            {
                return (V)info.Invoke(obj, args);
            }

            throw new MethodAccessException(string.Format("Method does not exist: {0}::{1}", typeof(T).Name, field));
        }

        public static Type[] GetTypesInNamespace(Assembly assembly, string space)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, space, StringComparison.Ordinal)).ToArray();
        }

        public static Type[] AllTypesInAssembly(Assembly assembly)
        {
            return assembly.GetTypes().ToArray();
        }
    }
}
