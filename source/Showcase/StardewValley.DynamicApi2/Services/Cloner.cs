using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Igorious.StardewValley.DynamicApi2.Extensions;

namespace Igorious.StardewValley.DynamicApi2.Services
{
    public sealed class Cloner
    {
        public static Cloner Instance { get; } = new Cloner();

        private static readonly Dictionary<(Type, Type), Type> TypeIntersectionCache = new Dictionary<(Type, Type), Type>();
        private static readonly Dictionary<Type, List<PropertyInfo>> PropertyInfoCache = new Dictionary<Type, List<PropertyInfo>>();
        private static readonly Dictionary<Type, List<FieldInfo>> FieldInfoCache = new Dictionary<Type, List<FieldInfo>>();

        public void CopyData(object from, object to)
        {
            var pair = (from.GetType(), to.GetType());
            if (!TypeIntersectionCache.TryGetValue(pair, out var commonType))
            {
                commonType = from.GetType().Intersect(to.GetType());
                TypeIntersectionCache.Add(pair, commonType);
            }
            CopyData(from, to, commonType);
        }

        private static void CopyData(object from, object to, Type type)
        {
            if (!PropertyInfoCache.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite).ToList();
                PropertyInfoCache.Add(type, properties);
            }
            foreach (var property in properties)
            {
                property.SetValue(to, property.GetValue(from));
            }

            if (!FieldInfoCache.TryGetValue(type, out var fields))
            {
                fields = type.GetAllFields().Where(f => !f.IsInitOnly && !f.IsLiteral).ToList();
                FieldInfoCache.Add(type, fields);
            }
            foreach (var field in fields)
            {
                field.SetValue(to, field.GetValue(from));
            }
        }
    }
}