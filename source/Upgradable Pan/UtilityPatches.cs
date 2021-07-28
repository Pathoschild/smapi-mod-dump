/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/UpgradeablePan
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using StardewValley.Tools;

namespace UpgradablePan
{
	public class UtilityPatches
	{
		private static int _priceForToolUpgradeLevel(int level)
		{
			switch (level)
			{
				default:
				case 1:
					return 2000;
				case 2:
					return 5000;
				case 3:
					return 10000;
				case 4:
					return 25000;
			}
		}

		private static int _indexOfExtraMaterialForToolUpgrade(int level)
		{
			switch (level)
			{
				default:
				case 1:
					return StardewValley.Object.copperBar;
				case 2:
					return StardewValley.Object.ironBar;
				case 3:
					return StardewValley.Object.goldBar;
				case 4:
					return StardewValley.Object.iridiumBar;
			}
		}

		public static void getBlacksmithUpgradeStock_Postfix(Farmer who, ref Dictionary<ISalable, int[]> __result)
		{
			Tool pan = who.getToolFromName("Pan");
			if (pan != null && pan.UpgradeLevel < 4)
			{
				Tool shopPan = new Pan();
				shopPan.UpgradeLevel = (int)pan.upgradeLevel + 1;
				__result.Add(shopPan, new int[3]
				{
					_priceForToolUpgradeLevel(shopPan.UpgradeLevel),
					1,
					_indexOfExtraMaterialForToolUpgrade(shopPan.UpgradeLevel)
				});
			}
		}

		public static void getFishShopStock_Postfix(Farmer who, ref Dictionary<ISalable, int[]> __result)
		{
			Pan foundPan = null;

			foreach (ISalable item in __result.Keys)
			{
				if (item is Pan pan)
				{
					foundPan = pan;
					break;
				}
			}

			if (foundPan != null)
			{
				__result.Remove(foundPan);
			}
		}

		public static bool PerformSpecialItemPlaceReplacement_Prefix(Item placedItem, ref Item __result)
		{
			if (placedItem != null && placedItem is Pan pan)
			{
				__result = new PanHat(pan);
				return false;
			}
			return true;
		}

		public static bool PerformSpecialItemGrabReplacement_Prefix(Item heldItem, ref Item __result)
		{
			if (heldItem != null && heldItem is PanHat panHat)
			{
				Pan pan = new Pan();
				pan.UpgradeLevel = panHat.UpgradeLevel;
				__result = pan;
				return false;
			}
			return true;
		}
	}
}