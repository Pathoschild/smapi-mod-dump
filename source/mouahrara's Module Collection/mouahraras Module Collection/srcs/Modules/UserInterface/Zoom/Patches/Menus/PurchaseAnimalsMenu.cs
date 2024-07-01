/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using HarmonyLib;
using StardewValley.Menus;
using mouahrarasModuleCollection.UserInterface.Zoom.Utilities;

namespace mouahrarasModuleCollection.UserInterface.Zoom.Patches
{
	internal class PurchaseAnimalsMenuPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.setUpForAnimalPlacement)),
				postfix: new HarmonyMethod(typeof(MenusPatchUtility), nameof(MenusPatchUtility.EnterFarmViewPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.setUpForReturnToShopMenu)),
				postfix: new HarmonyMethod(typeof(MenusPatchUtility), nameof(MenusPatchUtility.LeaveFarmViewPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.setUpForReturnAfterPurchasingAnimal)),
				postfix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(SetUpForReturnAfterPurchasingAnimalPostfix))
			);
		}

		private static void SetUpForReturnAfterPurchasingAnimalPostfix()
		{
			if (ModEntry.Config.ShopsBetterAnimalPurchase)
				return;

			MenusPatchUtility.LeaveFarmViewPostfix();
		}
	}
}
