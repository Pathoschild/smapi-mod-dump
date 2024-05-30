/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/FurnitureFramework
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using System.Reflection;

namespace FurnitureFramework
{
	enum PatchType {
		Prefix,
		Postfix,
		Transpiler
	}

	class HarmonyPatcher
	{
		public static Harmony? harmony;

		public static void patch()
		{
			if (harmony == null)
				throw new NullReferenceException("Harmony was not set");
			
			var types = Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(
					t => t.Namespace is not null &&
					t.Namespace.StartsWith("FurnitureFramework.Patches") &&
					!t.IsEnum
				);

			foreach (Type type in types)
			{
				#region Get Identification Fields

				var prop = type.GetField("patch_type",
					BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic
				);
				if (prop is null)
				{
					ModEntry.log($"No patch_type in {type}", LogLevel.Trace);
					continue;
				}
				PatchType? patch_type = (PatchType?)prop.GetValue(null);
				if (patch_type is null)
				{
					ModEntry.log($"patch_type is invalid {type}", LogLevel.Trace);
					continue;
				}

				prop = type.GetField("base_type",
					BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic
				);
				if (prop is null)
				{
					ModEntry.log($"No base_type in {type}", LogLevel.Trace);
					continue;
				}
				Type? base_type = (Type?)prop.GetValue(null);
				if (base_type is null)
				{
					ModEntry.log($"base_type is invalid in {type}", LogLevel.Trace);
					continue;
				}

				#endregion

				#region Patch

				foreach (MethodInfo method in type.GetMethods(
					BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic
				))
				{
					ModEntry.log($"Patching {patch_type} for {base_type.Name}.{method.Name}", LogLevel.Trace);

					MethodInfo original = AccessTools.DeclaredMethod(
						base_type,
						method.Name
					);

					HarmonyMethod? prefix = null, postfix = null, transpiler = null;

					switch (patch_type)
					{
						case PatchType.Prefix:
							prefix = new(method, Priority.High);
							break;
						case PatchType.Transpiler:
							transpiler = new(method);
							break;
						case PatchType.Postfix:
							postfix = new(method);
							break;
					}

					harmony.Patch(
						original: original,
						prefix: prefix,
						transpiler: transpiler,
						postfix: postfix
					);
				}

				#endregion
			}
		}
	}
}