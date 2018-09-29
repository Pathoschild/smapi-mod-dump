using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Services.Internal
{
    public sealed class Cloner
    {
        #region Private Data

        private static readonly Dictionary<Tuple<Type, Type>, Type> TypeIntersectionCache = new Dictionary<Tuple<Type, Type>, Type>();
        private static readonly Dictionary<Type, List<PropertyInfo>> PropertyInfoCache = new Dictionary<Type, List<PropertyInfo>>();
        private static readonly Dictionary<Type, List<FieldInfo>> FieldInfoCache = new Dictionary<Type, List<FieldInfo>>();

        #endregion

        #region Singleton Access

        private Cloner() { }

        private static Cloner _instance;

        public static Cloner Instance => _instance ?? (_instance = new Cloner());

        #endregion

        #region	Public Methods

        public void CopyData(object from, object to)
        {
            var pair = Tuple.Create(from.GetType(), to.GetType());
            Type commonType;
            if (!TypeIntersectionCache.TryGetValue(pair, out commonType))
            {
                commonType = from.GetType().Intersect(to.GetType());
                TypeIntersectionCache.Add(pair, commonType);
            }
            CopyData(from, to, commonType);
            (to as Object).SetColor((from as Object).GetColor());
        }

        public void CopyData(object from, object to, Type type)
        {
            List<PropertyInfo> properties;
            if (!PropertyInfoCache.TryGetValue(type, out properties))
            {
                properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite).ToList();
                PropertyInfoCache.Add(type, properties);
            }            
            foreach (var property in properties)
            {
                property.SetValue(to, property.GetValue(from));
            }

            List<FieldInfo> fields;
            if (!FieldInfoCache.TryGetValue(type, out fields))
            {
                fields = type.GetAllFields().Where(f => !f.IsInitOnly && !f.IsLiteral).ToList();
                FieldInfoCache.Add(type, fields);
            }   
            foreach (var field in fields)
            {
                field.SetValue(to, field.GetValue(from));
            }
        }

        #endregion
    }
}