/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ObjectLoadDisplayNamePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectLoadDisplayNamePatch()
		{
			Original = typeof(SObject).MethodNamed(name: "loadDisplayName");
			Postfix = new HarmonyMethod(GetType(), nameof(ObjectLoadDisplayNamePostfix));
		}

		/// <summary>Patch to add honey-specific mead names.</summary>
		[HarmonyPostfix]
		private static void ObjectLoadDisplayNamePostfix(SObject __instance, ref string __result)
		{
			try
			{
				if (!__instance.name.Contains("Mead") || __instance.preservedParentSheetIndex.Value <= 0) return;

				string prefix = Game1.objectInformation[__instance.preservedParentSheetIndex.Value].Split('/')[4];
				__result = prefix + " " + __result;
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}
	}
}
