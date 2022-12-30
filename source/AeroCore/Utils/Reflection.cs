/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.ReflectedValue;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace AeroCore.Utils
{
    [ModInit]
    public static class Reflection
    {
        private static readonly MethodInfo addItemMethod = typeof(Reflection).MethodNamed("AddItem");
        private static readonly MethodInfo addItemsMethod = typeof(Reflection).MethodNamed("AddItems");
        private static readonly Dictionary<KeyValuePair<Type, Type>, MethodInfo> addItemCache = new();
        private static readonly Dictionary<KeyValuePair<Type, Type>, MethodInfo> addItemsCache = new();
        internal static Multiplayer mp = null;
        public static Multiplayer Multiplayer => mp;
        public static readonly Dictionary<Type, JsonConverter> KnownConverters = new();

        internal static void Init()
        {
            var ns = "StardewModdingAPI.Framework.Serialization.";
            KnownConverters.Add(typeof(Vector2), (JsonConverter)TypeNamed(ns + "Vector2Converter").New());
            KnownConverters.Add(typeof(Point), (JsonConverter)TypeNamed(ns + "PointConverter").New());
            KnownConverters.Add(typeof(Rectangle), (JsonConverter)TypeNamed(ns + "RectangleConverter").New());
            KnownConverters.Add(typeof(Keybind), (JsonConverter)TypeNamed(ns + "KeybindConverter").New());
            KnownConverters.Add(typeof(Color), new Framework.ColorConverter());
        }

        public static Type TypeNamed(string name) => AccessTools.TypeByName(name);
        public static MethodInfo MethodNamed(this Type type, string name, Type[] args = null) => AccessTools.Method(type, name, args);
        public static MethodInfo DeclaredMethod(this Type type, string name, Type[] args = null) => AccessTools.DeclaredMethod(type, name, args);
        public static MethodInfo PropertyGetter(this Type type, string name) => AccessTools.PropertyGetter(type, name);
        public static MethodInfo PropertySetter(this Type type, string name) => AccessTools.PropertySetter(type, name);
        public static FieldInfo FieldNamed(this Type type, string name) => AccessTools.Field(type, name);
        public static object New(this Type type, params object[] args)
            => Activator.CreateInstance(type, args);
        public static IValue<T> ValueNamed<T>(this Type type, string name)
            => (AccessTools.Property(type, name) is not null) ? new InstanceProperty<T>(type, name) : 
                (AccessTools.Field(type, name) is not null) ? new InstanceField<T>(type, name) : 
            throw new NullReferenceException($"Type '{type.FullName}' does not have a property or field named '{name}'.");
        public static IStaticValue<T> StaticValueNamed<T>(this Type type, string name)
        {
            FieldInfo field;
            var prop = AccessTools.PropertyGetter(type, name);
            return (prop is not null && prop.IsStatic) ? new StaticProperty<T>(type, name) :
                ((field = AccessTools.Field(type, name)) is not null && field.IsStatic) ? new StaticField<T>(type, name) :
            throw new NullReferenceException($"Type '{type.FullName}' does not have a static property or field named '{name}'.");
        }
        public static ValueChain ValueRef(this Type type, string name)
            => new(type, name);
        public static ValueChain MethodRef(this Type type, string name, Type[] arg_types, params object[] args)
        {
            var method = arg_types is null ? type.MethodNamed(name) : type.MethodNamed(name, arg_types);
            if (method is null)
                throw new NullReferenceException($"Type '{type.FullName}' does not contain method '{name}'.");
            else
                return new(type, method, args);
        }
        public static CodeInstruction WithLabels(this CodeInstruction code, params Label[] labels)
        {
            foreach (Label label in labels)
                code.labels.Add(label);

            return code;
        }
        public static bool AddDictionaryEntry(IModContentHelper helper, IAssetData asset, object key, string path)
        {
            Type T = asset.DataType;

            if (!T.IsGenericType || T.GetGenericTypeDefinition() != typeof(Dictionary<,>))
                return false;

            Type[] types = T.GetGenericArguments();
            if(key.GetType().IsAssignableTo(types[0]))
                return false;

            KeyValuePair<Type, Type> typ = new(types[0], types[1]);
            if (!addItemCache.TryGetValue(typ, out var method))
                addItemCache.Add(typ, method = addItemMethod.MakeGenericMethod(types));
            method.Invoke(null, new object[] {helper, asset, key, path});
            return true;
        }
        public static void AddItem<k, v>(IModContentHelper helper, IAssetData asset, k key, string path)
        {
            var model = asset.AsDictionary<k, v>().Data;
            var entry = helper.Load<v>(path);
            model.Add(key, entry);
        }
        public static bool AddDictionaryEntries(IModContentHelper helper, IAssetData asset, string path)
        {
            Type T = asset.DataType;

            if (!T.IsGenericType || T.GetGenericTypeDefinition() != typeof(Dictionary<,>))
                return false;

            Type[] types = T.GetGenericArguments();

            KeyValuePair<Type, Type> typ = new(types[0], types[1]);
            if (!addItemsCache.TryGetValue(typ, out var method))
                addItemsCache.Add(typ, method = addItemsMethod.MakeGenericMethod(types));
            method.Invoke(null, new object[] { helper, asset, path });
            return true;
        }
        public static void AddItems<k, v>(IModContentHelper helper, IAssetData asset, string path)
        {
            var model = asset.AsDictionary<k, v>().Data;
            foreach ((k key, v val) in helper.Load<Dictionary<k, v>>(path))
                model[key] = val;
        }
        public static bool TryConvertType(object what, Type to, out object converted)
        {
            if (what is null)
            {
                converted = null;
                return !to.IsValueType; // value types can't be null
            }

            var type = what.GetType();
            var op_implicit = to.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(mi =>
                {
                    var pi = mi.GetParameters().FirstOrDefault();
                    return mi.Name == "op_Implicit" &&
                    mi.ReturnType == to &&
                    pi != null && pi.ParameterType == type;
                }).FirstOrDefault();
            if (op_implicit is not null)
            {
                converted = op_implicit.Invoke(to, new[] { what });
                return true;
            }
            try
            {
                converted = Convert.ChangeType(what, to);
                return true;
            }
            catch (Exception)
            {
                converted = default;
                return false;
            }
        }
        public static T MapTo<T>(this IDictionary<string, object> dict, T obj)
        {
            if (dict is IDictionary<string, JToken> jdict)
                return jdict.MapTo(obj);

            var type = obj.GetType();
            var allflag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            var props = new Dictionary<string, MemberInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var f in type.GetFields(allflag))
                if (!f.IsInitOnly && !f.IsLiteral) // const and readonly
                    props[f.Name] = f;
            foreach (var p in type.GetProperties(allflag))
                if (p.CanWrite) // writeable
                    props[p.Name] = p;

            foreach((var name, var prop) in dict)
            {
                if(props.TryGetValue(name, out var member))
                {
                    object val = (member.MemberType == MemberTypes.Field) ? ((FieldInfo)member).GetValue(obj) : ((PropertyInfo)member).GetValue(obj);
                    var targetType = (member.MemberType == MemberTypes.Field) ? ((FieldInfo)member).FieldType : ((PropertyInfo)member).PropertyType;

                    if (!prop.GetType().IsAssignableTo(targetType)) // if it can't be directly assigned
                        if (TryConvertType(val, targetType, out var con)) // implicit conversion
                            val = con;
                        else // try map
                            val = MapTo(prop ?? Activator.CreateInstance(targetType), val);
                    if (member is FieldInfo field)
                        field.SetValue(obj, val);
                    else
                        ((PropertyInfo)member).SetValue(obj, val);
                }
            }

            return obj;
        }
        public static T MapTo<T>(this IDictionary<string, JToken> dict, T obj)
        {
            Type type = obj.GetType();
            foreach ((var k, var v) in dict)
            {
                var p = type.GetProperty(k);
                if (p is null)
                    continue;
                p.SetValue(obj, v.ToObject(p.PropertyType));
            }
            return obj;
        }
        public static T MapTo<T>(T obj, object args)
        {
            if (args is JObject jo)
                return (T)jo.ToObject(obj.GetType());
            else if(args.GetType() == obj.GetType())
                return (T)args;
            else if(args is IDictionary<string, object> dict)
                return dict.MapTo(obj);

            var type = obj.GetType();
            var allflag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            var props = new Dictionary<string, MemberInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var f in type.GetFields(allflag))
                if (!f.IsInitOnly && !f.IsLiteral) // const and readonly
                    props[f.Name] = f;
            foreach (var p in type.GetProperties(allflag))
                if (p.CanWrite) // writeable
                    props[p.Name] = p;

            foreach (var member in args.GetType().GetMembers(allflag))
            {
                if (member.MemberType is not (MemberTypes.Field or MemberTypes.Property))
                    continue;
                if (member.MemberType == MemberTypes.Property && !((PropertyInfo)member).CanRead)
                    continue;
                if (!props.TryGetValue(member.Name, out var target))
                    continue;

                object val = (member.MemberType == MemberTypes.Field) ? ((FieldInfo)member).GetValue(obj) : ((PropertyInfo)member).GetValue(obj);
                object current = (target is FieldInfo) ? ((FieldInfo)target).GetValue(obj) : ((PropertyInfo)target).GetValue(obj);
                var sourceType = (member.MemberType == MemberTypes.Field) ? ((FieldInfo)member).FieldType : ((PropertyInfo)member).PropertyType;
                var targetType = (target is FieldInfo) ? ((FieldInfo)target).FieldType : ((PropertyInfo)target).PropertyType;

                if (!sourceType.IsAssignableTo(targetType)) // if it can't be directly assigned
                    if (TryConvertType(val, targetType, out var con)) // implicit conversion
                        val = con;
                    else // try map
                        val = MapTo(current ?? Activator.CreateInstance(targetType), val);
                if (target is FieldInfo field)
                    field.SetValue(obj, val);
                else
                    ((PropertyInfo)target).SetValue(obj, val);
            }
            return obj;
        }
        public static T ValueIgnoreCase<T>(this JObject obj, string fieldName)
        {
            JToken token = obj.GetValue(fieldName, StringComparison.OrdinalIgnoreCase);
            return token != null
                ? token.Value<T>()
                : default;
        }

        public static IEnumerable<Type> GetAllKnownTypes()
            => AppDomain.CurrentDomain.GetAssemblies().SelectMany((ass) => {
                    try{return ass.GetTypes();}
                    catch{return Array.Empty<Type>();}
                });

        public static bool TryPatch(this Harmony harmony, MethodBase original, HarmonyMethod prefix = null, 
            HarmonyMethod postfix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null)
        {
            if (original is null)
                return false;
            harmony.Patch(original, prefix, postfix, transpiler, finalizer);
            return true;
        }
    }
}
