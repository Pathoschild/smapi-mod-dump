using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Igorious.StardewValley.DynamicApi2.Extensions
{
    public static class ReflectionExtensions
    {
        public static Type Intersect(this Type firstType, Type secondType)
        {
            var counts = GetTypeHierarchy(firstType).Concat(GetTypeHierarchy(secondType))
                .GroupBy(t => t)
                .ToDictionary(g => g.Key, g => g.Count());

            return GetTypeHierarchy(firstType).First(t => counts[t] == 2);
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            if (type == null) return Enumerable.Empty<FieldInfo>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.Static | BindingFlags.Instance |
                                       BindingFlags.DeclaredOnly;
            return type.GetFields(flags).Concat(GetAllFields(type.BaseType));
        }

        public static MethodInfo GetInstanceMethod(this Type type, string methodName)
        {
            var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null) throw new InvalidOperationException($"Can't get instance method \"{methodName}\" of type {type.Name}");
            return method;
        }

        public static Lazy<FieldInfo> GetLazyInstanceField(this Type type, string fieldName) 
            => new Lazy<FieldInfo>(() => type.GetInstanceField(fieldName));

        public static FieldInfo GetInstanceField(this Type type, string fieldName) 
            => GetFieldInfo(type, fieldName, isStatic: false);

        public static T GetFieldValue<T>(this object o, Lazy<FieldInfo> fieldInfo) where T : class 
            => o.GetFieldValue<T>(fieldInfo.Value);

        public static T GetFieldValue<T>(this object o, FieldInfo fieldInfo) where T : class 
            => fieldInfo.GetValue(o) as T;

        public static void SetFieldValue<T>(this object o, Lazy<FieldInfo> fieldInfo, T value)
            => o.SetFieldValue(fieldInfo.Value, value);

        public static void SetFieldValue<T>(this object o, FieldInfo fieldInfo, T value) 
            => fieldInfo.SetValue(o, value);

        public static bool IsImplementGenericInterface(this Type t, Type genericInterface) => 
            t.GetInterfaces().Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == genericInterface);

        private static IEnumerable<Type> GetTypeHierarchy(Type type)
        {
            do
            {
                yield return type;
                type = type.BaseType;
            } while (type != null);
        }

        private static FieldInfo GetFieldInfo(Type type, string fieldName, bool isStatic)
        {
            while (type != typeof(object))
            {
                var fieldInfo = type.GetField(fieldName, (isStatic? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public | BindingFlags.NonPublic);
                if (fieldInfo != null) return fieldInfo;
                type = type.BaseType;
            }
            throw new InvalidOperationException($"Can't get field \"{fieldName}\" of type {type.Name}");
        }
    }
}