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
            RNG rng = BundleRandomizer.Rng;
            int moneyAmount;
			int bundleNameFlavorID;

			BundleType = rng.GetAndRemoveRandomValueFromList(RoomBundleTypes);
			switch (BundleType)
			{
				case BundleTypes.Vault2500:
					bundleNameFlavorID = rng.NextIntWithinRange(1, 7);
					moneyAmount = rng.NextIntWithinRange(500, 3500);
					break;
				case BundleTypes.Vault5000:
					bundleNameFlavorID = rng.NextIntWithinRange(1, 6);
					moneyAmount = rng.NextIntWithinRange(4000, 7000);
					break;
				case BundleTypes.Vault10000:
					bundleNameFlavorID = rng.NextIntWithinRange(1, 6);
					moneyAmount = rng.NextIntWithinRange(7500, 12500);
					break;
				case BundleTypes.Vault25000:
					bundleNameFlavorID = rng.NextIntWithinRange(1, 7);
					moneyAmount = rng.NextIntWithinRange(20000, 30000);
					break;
				default:
					return;
			}

			RequiredItems = new List<RequiredBundleItem> { new() { MoneyAmount = moneyAmount } };
            SetVaultBundleName(moneyAmount, bundleNameFlavorID);
			ImageNameSuffix = $"-{bundleNameFlavorID}";

			Color = rng.GetRandomValueFromList(
				Enum.GetValues(typeof(BundleColors)).Cast<BundleColors>().ToList());
		}

		/// <summary>
		/// Generates the reward for the bundle
		/// </summary>
		protected override void GenerateReward()
		{
            RNG rng = BundleRandomizer.Rng;

            List<RequiredBundleItem> potentialRewards = new List<RequiredBundleItem>
			{
				new(ObjectIndexes.GoldBar, rng.NextIntWithinRange(5, 25)),
				new(ObjectIndexes.IridiumBar, rng.NextIntWithinRange(1, 5)),
				new(BigCraftableIndexes.SolidGoldLewis),
				new(BigCraftableIndexes.HMTGF),
				new(BigCraftableIndexes.PinkyLemon),
				new(BigCraftableIndexes.Foroguemon),
				new(ObjectIndexes.GoldenPumpkin),
				new(ObjectIndexes.GoldenMask),
				new(ObjectIndexes.GoldenRelic),
				new(BigCraftableIndexes.GoldBrazier),
				new(ObjectIndexes.TreasureChest),
				new(ObjectIndexes.Lobster, rng.NextIntWithinRange(5, 25)),
				new(ObjectIndexes.LobsterBisque, rng.NextIntWithinRange(5, 25))
			};

			Reward = rng.GetRandomValueFromList(potentialRewards);
		}
	}
}
