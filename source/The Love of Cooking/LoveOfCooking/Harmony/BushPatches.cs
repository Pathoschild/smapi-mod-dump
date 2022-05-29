/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace LoveOfCooking.HarmonyPatches
{
	public static class BushPatches
	{
		public static void Patch(Harmony harmony)
		{
			System.Type type = typeof(Bush);
			var prefixes = new List<(string prefix, string original)>
			{
				(prefix: nameof(InBloom_Prefix), original: nameof(Bush.inBloom)),
				(prefix: nameof(IsDestroyable_Prefix), original: nameof(Bush.isDestroyable)),
				(prefix: nameof(EffectiveSize_Prefix), original: "getEffectiveSize"),
				(prefix: nameof(Shake_Prefix), original: "shake"),
			};

			foreach ((string prefix, string original) in prefixes)
			{
				Log.D($"Applying prefix: {type.Name}.{original}",
					ModEntry.Config.DebugMode);
				harmony.Patch(
					original: AccessTools.Method(type, original),
					prefix: new HarmonyMethod(typeof(BushPatches), prefix));
			}
		}
		
		public static bool InBloom_Prefix(Bush __instance, ref bool __result)
		{
			if (__instance is not CustomBush bush)
				return true;
			__result = CustomBush.InBloomBehaviour(bush);
			return false;
		}

		public static bool IsDestroyable_Prefix(Bush __instance, ref bool __result)
		{
			if (__instance is not CustomBush bush)
				return true;
			__result = CustomBush.IsDestroyableBehaviour(bush);
			return false;
		}

		public static bool EffectiveSize_Prefix(Bush __instance, ref int __result)
		{
			if (__instance is not CustomBush bush)
				return true;
			__result = CustomBush.GetEffectiveSizeBehaviour(bush);
			return false;
		}

		public static bool Shake_Prefix(Bush __instance, Vector2 tileLocation)
		{
			if (__instance is not CustomBush bush)
				return true;
			CustomBush.ShakeBehaviour(bush, tileLocation);
			return true;
		}
	}
}
