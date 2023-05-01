/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using BetterBeehouses.integration;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using FastExpressionCompiler.LightExpression;

namespace BetterBeehouses
{
	static class Utils
	{
		private static MethodInfo addItemMethod = typeof(Utils).MethodNamed("AddItem");
		public static MethodInfo PropertyGetter(this Type type, string name) => AccessTools.PropertyGetter(type, name);
		public static MethodInfo PropertySetter(this Type type, string name) => AccessTools.PropertySetter(type, name);
		public static MethodInfo MethodNamed(this Type type, string name)
			=> AccessTools.Method(type, name);
		public static MethodInfo MethodNamed(this Type type, string name, Type[] args)
			=> AccessTools.Method(type, name, args);
		public static FieldInfo FieldNamed(this Type type, string name)
			=> AccessTools.Field(type, name);
		public static CodeInstruction WithLabels(this CodeInstruction code, params Label[] labels)
		{
			foreach (Label label in labels)
				code.labels.Add(label);

			return code;
		}
		public static bool GetProduceHere(GameLocation loc, Config.ProduceWhere where)
			=> where is not Config.ProduceWhere.Never && (!loc.IsOutdoors || where is Config.ProduceWhere.Always);
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
		// copied from AtraBase. Thanks atravita!
		public static Action<TObject, TField> GetInstanceFieldSetter<TObject, TField>(this FieldInfo field)
		{
			if (field is null)
				return null;
			if (!typeof(TObject).IsAssignableTo(field.DeclaringType))
				throw new ArgumentException($"{typeof(TObject).FullName} is not assignable to {field.DeclaringType?.FullName}");
			if (!typeof(TField).IsAssignableTo(field.FieldType))
				throw new ArgumentException($"{typeof(TField).FullName} is not assignable to {field.FieldType.FullName}");
			if (field.IsStatic)
				throw new ArgumentException($"Expected a non-static field");

			ParameterExpression objparam = Expression.ParameterOf<TObject>("obj");
			ParameterExpression fieldval = Expression.ParameterOf<TField>("fieldval");
			UnaryExpression convertfield = Expression.Convert(fieldval, field.FieldType);
			MemberExpression fieldsetter = Expression.Field(objparam, field);
			BinaryExpression assignexpress = Expression.Assign(fieldsetter, convertfield);

			return Expression.Lambda<Action<TObject, TField>>(assignexpress, objparam, fieldval).CompileFast();
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
		internal static void AddQuickBool(this IGMCMAPI api, object inst, IManifest manifest, string prop)
		{
			var p = inst.GetType().GetProperty(prop);
			var cfname = prop.Decap();
			api.AddBoolOption(manifest,
				p.GetGetMethod().CreateDelegate<Func<bool>>(inst),
				p.GetSetMethod().CreateDelegate<Action<bool>>(inst),
				() => ModEntry.i18n.Get($"config.{cfname}.name"),
				() => ModEntry.i18n.Get($"config.{cfname}.desc")
			);
		}
		internal static void AddQuickFloat(this IGMCMAPI api, object inst, IManifest manifest, string prop, float? min = null, float? max = null, float? inc = null)
		{
			var p = inst.GetType().GetProperty(prop);
			var cfname = prop.Decap();
			api.AddNumberOption(manifest,
				p.GetGetMethod().CreateDelegate<Func<float>>(inst),
				p.GetSetMethod().CreateDelegate<Action<float>>(inst),
				() => ModEntry.i18n.Get($"config.{cfname}.name"),
				() => ModEntry.i18n.Get($"config.{cfname}.desc"),
				min, max, inc
			);
		}
		internal static void AddQuickInt(this IGMCMAPI api, object inst, IManifest manifest, string prop, int? min = null, int? max = null, int? inc = null)
		{
			var p = inst.GetType().GetProperty(prop);
			var cfname = prop.Decap();
			api.AddNumberOption(manifest,
				p.GetGetMethod().CreateDelegate<Func<int>>(inst),
				p.GetSetMethod().CreateDelegate<Action<int>>(inst),
				() => ModEntry.i18n.Get($"config.{cfname}.name"),
				() => ModEntry.i18n.Get($"config.{cfname}.desc"),
				min, max, inc
			);
		}
		internal static void AddQuickEnum<TE>(this IGMCMAPI api, object inst, IManifest manifest, string prop) where TE : Enum
		{
			var p = inst.GetType().GetProperty(prop);
			var cfname = prop.Decap();
			var tenum = typeof(TE);
			var tname = tenum.Name.Decap();
			api.AddTextOption(manifest,
				() => p.GetValue(inst).ToString(),
				(s) => p.SetValue(inst, (TE)Enum.Parse(tenum, s)),
				() => ModEntry.i18n.Get($"config.{cfname}.name"),
				() => ModEntry.i18n.Get($"config.{cfname}.desc"),
				Enum.GetNames(tenum),
				(s) => ModEntry.i18n.Get($"config.{tname}.{s}")
			);
		}
		internal static void AddQuickLink(this IGMCMAPI api, string id, IManifest manifest)
			=> api.AddPageLink(manifest, id, () => ModEntry.i18n.Get($"config.{id}.name"), () => ModEntry.i18n.Get($"config.{id}.desc"));
		internal static string Decap(this string src)
			=> src.Length > 0 ? char.ToLower(src[0]) + src[1..] : string.Empty;
		internal static float Next(this Random rand, float max)
			=> (float)rand.NextDouble() * max;
	}
}
