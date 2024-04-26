/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/Catalogue-Framework
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace CatalogueFramework {

	internal class FurniturePatches {

		internal static bool checkForAction_prefix(
			Furniture __instance, ref bool __result,
			Farmer who, bool justCheckingForActivity = false
		)
		{
			try
			{
				// This only applies to defined furniture
				if (ModEntry.shops.ContainsKey(__instance.ItemId))
				{
					if (__instance.Location == null || justCheckingForActivity)
						return true;	// run original logic
					
					Utility.TryOpenShopMenu(ModEntry.shops[__instance.ItemId], __instance.Location);
					__result = true;	// action found

					return false; // don't run original logic
				}
				return true; // run original logic
			}
			catch (Exception ex)
			{
				ModEntry.log($"Failed in {nameof(checkForAction_prefix)}:\n{ex}", LogLevel.Error);
				return true; // run original logic
			}
		}
	}

}