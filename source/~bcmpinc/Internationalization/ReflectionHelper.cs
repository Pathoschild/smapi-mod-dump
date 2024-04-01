/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Internationalization
{
    static class ReflectionHelper {
        private static Dictionary<(System.Type, string), PropertyInfo> cache_property = new Dictionary<(System.Type, string), PropertyInfo>();
        public static T Property<T>(object o, string name) {
            var main_type = o.GetType();
            var key = (main_type,name);
            if (!cache_property.TryGetValue(key, out PropertyInfo property)) {
                for (var type = main_type; type != null; type = type.BaseType) {
                    property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (property != null) {
                        cache_property[key] = property;
                        ModEntry.Log($"Found property {name} in {main_type.FullName}");
                        goto found;
                    }
                }
                throw new System.Exception($"Could not find property {name} in {main_type.FullName}");
            }
            found:
            return (T)property.GetValue(o);
        }

        private static Dictionary<(System.Type, string), FieldInfo> cache_field = new Dictionary<(System.Type, string), FieldInfo>();
        public static T Field<T>(object o, string name) {
            var main_type = o.GetType();
            var key = (main_type,name);
            if (!cache_field.TryGetValue(key, out FieldInfo field)) {
                for (var type = main_type; type != null; type = type.BaseType) {
                    field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null) {
                        cache_field[key] = field;
                        ModEntry.Log($"Found field {name} in {main_type.FullName}");
                        goto found;
                    }
                }
                throw new System.Exception($"Could not find field {name} in {main_type.FullName}");
            }
            found:
            return (T)field.GetValue(o);
        }

        private static Dictionary<(System.Type, ImmutableArray<System.Type>), ConstructorInfo> cache_constructor = new Dictionary<(System.Type, ImmutableArray<System.Type>), ConstructorInfo>();
        public static T Constructor<T>(params object[] args) {
            var types = args.Select(x=>x.GetType()).ToArray();
            var key = (typeof(T), types.ToImmutableArray());
            if (!cache_constructor.TryGetValue(key, out ConstructorInfo method)) {
                method = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, types);
                if (method != null) {
                    cache_constructor[key] = method;
                    ModEntry.Log($"Found constructor for {typeof(T)}");
                    goto found;
                }
                throw new System.Exception($"Could not find constructor for {typeof(T)}");
            }
            found:
            return (T)method.Invoke(args);
        }

        private static Dictionary<(System.Type, string, ImmutableArray<System.Type>), MethodInfo> cache_method = new Dictionary<(System.Type, string, ImmutableArray<System.Type>), MethodInfo>();
        public static T Method<T>(object o, string name, params object[] args) {
            var types = args.Select(x=>x.GetType()).ToArray();
            var main_type = o.GetType();
            var key = (main_type,name,types.ToImmutableArray());
            if (!cache_method.TryGetValue(key, out MethodInfo method)) {
                for (var type = main_type; type != null; type = type.BaseType) {
                    method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (method != null) {
                        cache_method[key] = method;
                        ModEntry.Log($"Found field {name} in {main_type.FullName}");
                        goto found;
                    }
                }
                throw new System.Exception($"Could not find method {name} in {main_type.FullName}");
            }
            found:
            return (T)method.Invoke(o, args);
        }
    }
}
