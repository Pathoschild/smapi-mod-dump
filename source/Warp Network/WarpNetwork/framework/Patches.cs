/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using StardewValley;
using SObject = StardewValley.Object;
using System;
using HarmonyLib;

namespace WarpNetwork.framework
{
	internal class Patches
	{
		internal static void Patch(Harmony harmony)
		{
			harmony.Patch(typeof(SObject).GetMethod(nameof(SObject.performUseAction)), new(typeof(Patches), nameof(UsePrefix)));
			harmony.Patch(typeof(SObject).GetMethod(nameof(SObject.checkForAction)), new(typeof(Patches), nameof(ActionPrefix)));
		}
		private static bool UsePrefix(GameLocation location, SObject __instance, ref bool __result)
		{
			if (!Game1.player.canMove || __instance.isTemporarilyInvisible)
				return true;

			if (!Game1.eventUp && !Game1.isFestival() &&
				!Game1.fadeToBlack && !Game1.player.swimming.Value &&
				!Game1.player.bathingClothes.Value && !Game1.player.onBridge.Value)
				return true;

			if (!ItemHandler.TryUseTotem(Game1.player, __instance))
				return true;

			__result = false;
			return false;
		}
		private static bool ActionPrefix(Farmer who, bool justCheckingForActivity, SObject __instance, ref bool __result)
		{
			if (__instance.isTemporarilyInvisible || who is null)
				return true;

			if (!ItemHandler.ActivateObject(__instance, justCheckingForActivity, who))
				return true;

			__result = true;
			return false;
		}
	}
}
