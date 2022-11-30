/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BetterBeehouses
{
    static class Utils
    {
        private static MethodInfo addItemMethod = typeof(Utils).MethodNamed("AddItem");
		public static MethodInfo PropertyGetter(this Type type, string name) => AccessTools.PropertyGetter(type, name);
		public static MethodInfo PropertySetter(this Type type, string name) => AccessTools.PropertySetter(type, name);
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
        public static bool GetProduceHere(GameLocation loc, Config.ProduceWhere where)
        {
            return where is not Config.ProduceWhere.Never && (!loc.IsOutdoors || where is Config.ProduceWhere.Always);
        }
        public static void AddDictionaryEntry(IAssetData asset, object key, string path)
        {
            Type T = asset.DataType;
            if (!T.IsGenericType || T.GetGenericTypeDefinition() != typeof(Dictionary<,>))
                return;

            Type[] types = T.GetGenericArguments();
            addItemMethod.MakeGenericMethod(types).Invoke(null, new object[] {asset, key, path});
        }
        public static void AddItem<k, v>(IAssetData asset, k key, string path)
        {
            var model = asset.AsDictionary<k, v>().Data;
            var entry = ModEntry.helper.ModContent.Load<v>($"assets/{path}");
            model.Add(key, entry);
        }
        public static string Uniformize(this string str)
        {
            var s = str.AsSpan();
            var r = new Span<char>(new char[s.Length]);
            int len = 0;
            int last = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsWhiteSpace(s[i]))
                    continue;

                if (i - last <= 1)
                {
                    last = i + 1;
                    continue;
                }

                s[last..i].CopyTo(r[len..]);
                len += i - last;
                last = i + 1;
            }
            if (last < s.Length)
            {
                s[last..].CopyTo(r[len..]);
                len += s.Length - last;
            }
            return new string(r[..len]).ToLowerInvariant();
        }
		public static string GetChunk(this string str, char delim, int which)
		{
			int i = 0;
			int n = 0;
			int z = 0;
			while (i < str.Length)
			{
				if (str[i] == delim)
				{
					if (n == which)
						return str[z..i];
					n++;
					z = i + 1;
				}
				i++;
			}
			if (n == which)
				return str[z..i];
			return "";
		}
	}
}
