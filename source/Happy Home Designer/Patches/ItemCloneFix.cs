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
using StardewValley.Objects;
using System;

namespace HappyHomeDesigner.Patches
{
	internal class ItemCloneFix
	{
		public static void Apply(Harmony harmony)
		{
			harmony.Patch(
				typeof(Farmer).GetMethod(nameof(Farmer.reduceActiveItemByOne)), 
				prefix: new(typeof(ItemCloneFix), nameof(Prefix)));

			harmony.Patch(
				typeof(Furniture).GetMethod(nameof(Furniture.performObjectDropInAction)), 
				prefix: new(typeof(ItemCloneFix), nameof(BeforeDropIn)), 
				postfix: new(typeof(ItemCloneFix), nameof(AfterDropIn)));
		}

		private static bool Prefix(Farmer __instance)
		{
			if (__instance.TemporaryItem is null)
				return true;

			if (--__instance.TemporaryItem.Stack is <= 0)
				__instance.TemporaryItem = null;

			return false;
		}

		private static void BeforeDropIn(Item dropInItem, Farmer who, out Furniture __state)
		{
			__state = who.TemporaryItem as Furniture;
		}

		private static void AfterDropIn(Item dropInItem, Farmer who, Furniture __state)
		{
			if (__state is not null && who.TemporaryItem != __state)
			{
				__state.Stack = 1;
				who.TemporaryItem = __state;
			}
		}
	}
}
