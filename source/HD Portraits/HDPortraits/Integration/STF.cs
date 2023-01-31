/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using AeroCore;
using AeroCore.Utils;
using HarmonyLib;
using HDPortraits.Patches;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;

namespace HDPortraits.Integration
{
	[ModInit(WhenHasMod = "Cherry.ShopTileFramework")]
	internal class STF
	{
		private static Type STFShop;
		private static MethodInfo ShopName;
		internal static void Init()
		{
			ModEntry.monitor.Log("Patching STF...");
			STFShop = AccessTools.TypeByName("ShopTileFramework.Shop.ItemShop");
			ShopName = AccessTools.PropertyGetter(STFShop, "ShopName");

			ModEntry.harmony.Patch(STFShop.MethodNamed("DisplayShop"), postfix: new(typeof(STF), nameof(AfterShopOpened)));
		}
		private static void AfterShopOpened(object __instance)
		{
			string name = (string)ShopName.Invoke(__instance, Array.Empty<object>());
			if (name is null || Game1.activeClickableMenu is not ShopMenu shop || shop.portraitPerson is null)
				return;

			name = "STF." + name;

			if (ModEntry.TryGetMetadata(name, PortraitDrawPatch.GetSuffix(shop.portraitPerson), out var meta))
			{
				PortraitDrawPatch.lastLoaded.Value.Add(meta);
				PortraitDrawPatch.currentMeta.Value = meta;
				meta.Animation?.Reset();
			}
		}
	}
}
