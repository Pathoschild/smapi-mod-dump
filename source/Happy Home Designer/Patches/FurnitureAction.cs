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
using StardewValley.Objects;
using System.Reflection;
using static HappyHomeDesigner.Framework.ModUtilities;

namespace HappyHomeDesigner.Patches
{
	internal class FurnitureAction
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.TryPatch(
				typeof(Furniture).GetMethod(nameof(Furniture.checkForAction)),
				prefix: new(typeof(FurnitureAction), nameof(CheckAction))
			);

			harmony.TryPatch(
				typeof(Furniture).GetMethod("loadDescription", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public),
				postfix: new(typeof(FurnitureAction), nameof(EditDescription))
			);
		}

		private static CatalogType GetCatalogTypeOf(string FurnitureID)
		{
			return FurnitureID switch
			{
				"1308" => CatalogType.Wallpaper,
				"1226" => CatalogType.Furniture,
				AssetManager.CATALOGUE_ID => CatalogType.Furniture | CatalogType.Wallpaper,
				AssetManager.COLLECTORS_ID => CatalogType.Collector,
				AssetManager.DELUXE_ID => CatalogType.Furniture | CatalogType.Wallpaper | CatalogType.Collector,
				_ => CatalogType.None
			};
		}

		private static bool CheckAction(Furniture __instance, ref bool __result)
		{
			string HeldID = (__instance.heldObject.Value as Furniture)?.ItemId;
			var heldType = GetCatalogTypeOf(HeldID);
			var baseType = GetCatalogTypeOf(__instance.ItemId);

			var combined = baseType | heldType;

			switch (__instance.ItemId)
			{
				// Furniture catalogue
				case "1226":
					if (heldType is not CatalogType.None)
						Catalog.ShowCatalog(GenerateCombined(combined), combined.ToString());
					else
						return true;
					break;

				// Wallpaper catalogue
				case "1308":
					if (heldType is not CatalogType.None)
						Catalog.ShowCatalog(GenerateCombined(combined), combined.ToString());
					else
						return true;
					break;

				// All others
				default:
					// one of my catalogues
					if (baseType is not CatalogType.None)
						Catalog.ShowCatalog(GenerateCombined(combined), combined.ToString());
					else
						return true;
					break;
			}

			__result = true;
			return false;
		}

		private static string EditDescription(string original, Furniture __instance)
		{
			if (ItemRegistry.GetDataOrErrorItem(__instance.ItemId).IsErrorItem)
				return original;

			return __instance.ItemId switch
			{
				AssetManager.CATALOGUE_ID => ModEntry.i18n.Get("furniture.Catalogue.desc"),
				AssetManager.COLLECTORS_ID => ModEntry.i18n.Get("furniture.CollectorsCatalogue.desc"),
				AssetManager.DELUXE_ID => ModEntry.i18n.Get("furniture.DeluxeCatalogue.desc"),
				_ => original
			};
		}
	}
}
