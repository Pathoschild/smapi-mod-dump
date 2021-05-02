/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace LoveOfCooking.Core.HarmonyPatches
{
	public static class BushPatches
	{
		public static void Patch(HarmonyInstance harmony)
		{
			var type = typeof(Bush);
			var prefixes = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>(nameof(InBloom_Prefix), nameof(Bush.inBloom)),
				new KeyValuePair<string, string>(nameof(IsDestroyable_Prefix), nameof(Bush.isDestroyable)),
				new KeyValuePair<string, string>(nameof(EffectiveSize_Prefix), "getEffectiveSize"),
				new KeyValuePair<string, string>(nameof(Shake_Prefix), "shake"),
			};

			foreach (var pair in prefixes)
			{
				var prefix = pair.Key;
				var original = pair.Value;
				Log.D($"Applying prefix: {type.Name}.{original}",
					ModEntry.Instance.Config.DebugMode);
				harmony.Patch(
					original: AccessTools.Method(type, original),
					prefix: new HarmonyMethod(typeof(BushPatches), prefix));
			}
		}
		
		public static bool InBloom_Prefix(Bush __instance, ref bool __result, string season, int dayOfMonth)
		{
			if (!(__instance is CustomBush bush))
				return true;
			__result = CustomBush.InBloomBehaviour(bush, season, dayOfMonth);
			return false;
		}

		public static bool IsDestroyable_Prefix(Bush __instance, ref bool __result)
		{
			if (!(__instance is CustomBush bush))
				return true;
			__result = CustomBush.IsDestroyableBehaviour(bush);
			return false;
		}

		public static bool EffectiveSize_Prefix(Bush __instance, ref int __result)
		{
			if (!(__instance is CustomBush bush))
				return true;
			__result = CustomBush.GetEffectiveSizeBehaviour(bush);
			return false;
		}

		public static bool Shake_Prefix(Bush __instance, Vector2 tileLocation)
		{
			if (!(__instance is CustomBush bush))
				return true;
			CustomBush.ShakeBehaviour(bush, tileLocation);
			return true;
		}
	}
}
