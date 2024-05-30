/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Framework;
using HarmonyLib;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HappyHomeDesigner.Patches
{
	internal class ItemCloneFix
	{
		public static bool suppress_reduce = false;

		public static void Apply(Harmony harmony)
		{
			harmony.TryPatch(
				typeof(Farmer).GetMethod(nameof(Farmer.removeItemFromInventory)), 
				prefix: new(typeof(ItemCloneFix), nameof(RemoveTempItem))
			);
			harmony.TryPatch(
				typeof(Farmer).GetMethod(nameof(Farmer.reduceActiveItemByOne)), 
				prefix: new(typeof(ItemCloneFix), nameof(CheckReduceItem))
			);
			harmony.TryPatch(
				typeof(Utility).GetMethod(nameof(Utility.tryToPlaceItem)), 
				postfix: new(typeof(ItemCloneFix), nameof(AfterTryPlace)),
				prefix: new(typeof(ItemCloneFix), nameof(BeforeTryPlace))
			);
		}

		private static void BeforeTryPlace(Item item)
		{
			suppress_reduce = item is Furniture;
		}

		private static void AfterTryPlace()
		{
			suppress_reduce = false;
		}

		private static bool CheckReduceItem()
		{
			return !suppress_reduce;
		}

		private static bool RemoveTempItem(Farmer __instance, Item which)
		{
			if (__instance.TemporaryItem != which)
				return true;
			else
				__instance.TemporaryItem = null;

			return false;
		}
	}
}
