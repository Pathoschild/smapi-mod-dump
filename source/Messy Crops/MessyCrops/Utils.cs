/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/MessyCrops
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace MessyCrops
{
    internal static class Utils
    {
        private static MethodInfo addItemMethod = typeof(Utils).MethodNamed("AddItem");
        public static MethodInfo MethodNamed(this Type type, string name)
        {
            return AccessTools.Method(type, name);
        }
        public static MethodInfo MethodNamed(this Type type, string name, Type[] args)
        {
            return AccessTools.Method(type, name, args);
        }
        public static FieldInfo FieldNamed(this Type type, string name)
        {
            return AccessTools.Field(type, name);
        }
        public static CodeInstruction WithLabels(this CodeInstruction code, params Label[] labels)
        {
            foreach (Label label in labels)
                code.labels.Add(label);

            return code;
        }
        public static void AddDictionaryEntry<T>(IAssetData asset, object key, string path)
        {
            if (!typeof(T).IsGenericType || typeof(T).GetGenericTypeDefinition() != typeof(Dictionary<,>))
                return;

            Type[] types = typeof(T).GetGenericArguments();
            addItemMethod.MakeGenericMethod(types).Invoke(null, new object[] { asset, key, path });
        }
        public static void AddItem<k, v>(IAssetData asset, k key, string path)
        {
            var model = asset.AsDictionary<k, v>().Data;
            var entry = ModEntry.helper.Content.Load<v>($"assets/{path}");
            model.Add(key, entry);
        }

        public static v GetOrAdd<k, v>(this IDictionary<k, v> dict, k key, Func<k, v> getter)
        {
            if (dict.TryGetValue(key, out v val))
                return val;

            val = getter(key);
            dict[key] = val;
            return val;
        }
    }
}
