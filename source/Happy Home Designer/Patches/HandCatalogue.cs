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
using HappyHomeDesigner.Menus;
using HarmonyLib;
using StardewValley;

namespace HappyHomeDesigner.Patches
{
	internal class HandCatalogue
	{
		public static void Apply(Harmony harmony)
		{
			harmony.TryPatch(
				typeof(Tool).GetMethod(nameof(Tool.DoFunction)),
				postfix: new(typeof(HandCatalogue), nameof(OpenIfCatalogue))
			);
		}

		private static void OpenIfCatalogue(Tool __instance, Farmer who)
		{
			if (__instance.ItemId != AssetManager.PORTABLE_ID)
				return;

			if (!who.IsLocalPlayer)
				return;

			var catalogues = 
				ModUtilities.CatalogType.Collector | 
				ModUtilities.CatalogType.Furniture | 
				ModUtilities.CatalogType.Wallpaper;

			Catalog.ShowCatalog(ModUtilities.GenerateCombined(catalogues), catalogues.ToString());
		}
	}
}
