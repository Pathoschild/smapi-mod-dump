using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Igorious.StardewValley.DynamicAPI.Extensions
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

        public static IReadOnlyList<Delegate> GetEventHandlers(this Type type, string eventName)
        {
            var fieldInfo = type.GetField(eventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var field = fieldInfo?.GetValue(null) as Delegate;
            return field?.GetInvocationList() ?? new Delegate[] { };
        }

        public static void RemoveEventHandler(this Type type, string eventName, Delegate handler)
        {
            var eventInfo = type.GetEvent(eventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            eventInfo?.RemoveEventHandler(null, handler);
        }

        public static void AddEventHandler(this Type type, string eventName, Delegate handler)
        {
            var eventInfo = type.GetEvent(eventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            eventInfo?.AddEventHandler(null, handler);
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            if (type == null) return Enumerable.Empty<FieldInfo>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.Static | BindingFlags.Instance |
                                       BindingFlags.DeclaredOnly;
            return type.GetFields(flags).Concat(GetAllFields(type.BaseType));
        }

        public static T GetField<T>(this object o, string fieldName) where T : class
        {
            return GetField<T>(o, o.GetType(), fieldName);
        }

        public static void SetField<T>(this object o, string fieldName, T value) where T : class
        {
            var fieldInfo = o.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            fieldInfo?.SetValue(o, value);
        }

        public static bool IsImplementGenericInterface(this Type t, Type genericInterface)
        {
            return t.GetInterfaces().Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == genericInterface);
        }

        private static IEnumerable<Type> GetTypeHierarchy(Type type)
        {
            do
            {
                yield return type;
                type = type.BaseType;
            } while (type != null);
        }

        private static T GetField<T>(object o, Type type, string fieldName) where T : class
        {
            while (type != typeof(object))
            {
                var fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fieldInfo != null) return fieldInfo.GetValue(o) as T;
                type = type.BaseType;
            }
            return null;
        }
    }
}
