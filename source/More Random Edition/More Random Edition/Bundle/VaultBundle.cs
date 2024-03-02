/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Randomizer
{
	public class VaultBundle : Bundle
	{
		public static List<BundleTypes> RoomBundleTypes { get; set; }

		/// <summary>
		/// Populates the bundle with the name, required items, minimum required, and color
		/// </summary>
		protected override void Populate()
		{
			int moneyAmount = 0;
			BundleType = Globals.RNGGetAndRemoveRandomValueFromList(RoomBundleTypes);
			int bundleNameFlavorID = 1;

			switch (BundleType)
			{
				case BundleTypes.Vault2500:
					bundleNameFlavorID = Range.GetRandomValue(1, 7);
					moneyAmount = Range.GetRandomValue(500, 3500);
					break;
				case BundleTypes.Vault5000:
					bundleNameFlavorID = Range.GetRandomValue(1, 6);
					moneyAmount = Range.GetRandomValue(4000, 7000);
					break;
				case BundleTypes.Vault10000:
					bundleNameFlavorID = Range.GetRandomValue(1, 6);
					moneyAmount = Range.GetRandomValue(7500, 12500);
					break;
				case BundleTypes.Vault25000:
					bundleNameFlavorID = Range.GetRandomValue(1, 7);
					moneyAmount = Range.GetRandomValue(20000, 30000);
					break;
				default:
					return;
			}

			RequiredItems = new List<RequiredItem> { new RequiredItem() { MoneyAmount = moneyAmount } };
            SetVaultBundleName(moneyAmount, bundleNameFlavorID);
			ImageNameSuffix = $"-{bundleNameFlavorID}";

			Color = Globals.RNGGetRandomValueFromList(
				Enum.GetValues(typeof(BundleColors)).Cast<BundleColors>().ToList());
		}

		/// <summary>
		/// Generates the reward for the bundle
		/// </summary>
		protected override void GenerateReward()
		{
			List<RequiredItem> potentialRewards = new List<RequiredItem>
			{
				new(ObjectIndexes.GoldBar, Range.GetRandomValue(5, 25)),
				new(ObjectIndexes.IridiumBar, Range.GetRandomValue(1, 5)),
				new(BigCraftableIndexes.SolidGoldLewis),
				new(BigCraftableIndexes.HMTGF),
				new(BigCraftableIndexes.PinkyLemon),
				new(BigCraftableIndexes.Foroguemon),
				new(ObjectIndexes.GoldenPumpkin),
				new(ObjectIndexes.GoldenMask),
				new(ObjectIndexes.GoldenRelic),
				new(BigCraftableIndexes.GoldBrazier),
				new(ObjectIndexes.TreasureChest),
				new(ObjectIndexes.Lobster, Range.GetRandomValue(5, 25)),
				new(ObjectIndexes.LobsterBisque, Range.GetRandomValue(5, 25))
			};

			Reward = Globals.RNGGetRandomValueFromList(potentialRewards);
		}
	}
}
