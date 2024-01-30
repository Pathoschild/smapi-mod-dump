/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;

namespace HappyHomeDesigner.Patches
{
	internal class ItemCloneFix
	{
		public static void Apply(Harmony harmony)
		{
			harmony.Patch(typeof(Farmer).GetMethod(nameof(Farmer.reduceActiveItemByOne)), new(typeof(ItemCloneFix), nameof(Prefix)));
		}

		private static bool Prefix(Farmer __instance)
		{
			if (__instance.TemporaryItem is null)
				return true;

			if (--__instance.TemporaryItem.Stack is <= 0)
				__instance.TemporaryItem = null;

			return false;
		}
	}
}
