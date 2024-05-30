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
using Microsoft.Xna.Framework;
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

			harmony.TryPatch(
				typeof(Furniture).GetMethod(nameof(Furniture.performObjectDropInAction)),
				prefix: new(typeof(FurnitureAction), nameof(ApplyFairyDust))
			);
		}

		private static bool ApplyFairyDust(Furniture __instance, Item dropInItem, bool probe, 
			bool returnFalseIfItemConsumed, Farmer who, ref bool __result)
		{
			if (__instance.QualifiedItemId != "(F)" + AssetManager.DELUXE_ID || 
				dropInItem.QualifiedItemId != "(O)872")
				return true;

			__result = true;
			if (probe)
				return false;

			if (who is not null)
			{
				__result = !returnFalseIfItemConsumed;
				who.reduceActiveItemByOne();
			}

			var location = __instance.Location;
			var tile = __instance.TileLocation;
			var size = __instance.sourceRect.Value.Size;
			var Region = new Rectangle(
				(int)tile.X + size.X / 32,
				(int)tile.Y + ((size.Y / 16) - __instance.getTilesHigh()) / 2,
				size.X / 16,
				size.Y / 16
			);

			Game1.playSound("secret1");
			int FlashDelay = 1500;

			Utility.addStarsAndSpirals(location, Region.X, Region.Y + Region.Height - 2, Region.Width, 1, FlashDelay, 50, Color.Magenta);

			DelayedAction.screenFlashAfterDelay(.5f, FlashDelay, "wand");
			DelayedAction.functionAfterDelay(() => {
				Utility.addSprinklesToLocation(location, Region.X, Region.Y, Region.Width, Region.Height, 400, 40, Color.White);
				location.furniture.Remove(__instance);
				Game1.createItemDebris(
					ItemRegistry.Create("(T)" + AssetManager.PORTABLE_ID),
					(tile + new Vector2(.5f)) * Game1.tileSize,
					-1,
					location
				);
			}, FlashDelay);

			return false;
		}

		private static CatalogType GetCatalogTypeOf(string FurnitureID)
		{
			return FurnitureID switch
			{
				"(F)1308" => CatalogType.Wallpaper,
				"(F)1226" => CatalogType.Furniture,
				"(F)" + AssetManager.CATALOGUE_ID => CatalogType.Furniture | CatalogType.Wallpaper,
				"(F)" + AssetManager.COLLECTORS_ID => CatalogType.Collector,
				"(F)" + AssetManager.DELUXE_ID or
				"(O)" + AssetManager.PORTABLE_ID or
				"(T)" + AssetManager.PORTABLE_ID
					=> CatalogType.Furniture | CatalogType.Wallpaper | CatalogType.Collector,
				_ => CatalogType.None
			};
		}

		private static bool CheckAction(Furniture __instance, ref bool __result)
		{
			const CatalogType Deluxe = CatalogType.Collector | CatalogType.Furniture | CatalogType.Wallpaper;

			var heldType = GetCatalogTypeOf(__instance.heldObject.Value?.QualifiedItemId);
			var baseType = GetCatalogTypeOf(__instance.QualifiedItemId);

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

					// something else holding the portable catalogue
					else if (__instance.heldObject.Value?.ItemId == AssetManager.PORTABLE_ID)
						Catalog.ShowCatalog(GenerateCombined(Deluxe), Deluxe.ToString());

					// other
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
