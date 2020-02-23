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
			string bundleNameFlavor = "";

			switch (BundleType)
			{
				case BundleTypes.Vault2500:
					bundleNameFlavor = Globals.RNGGetRandomValueFromList(new List<string>
					{
						Globals.GetTranslation("Vault2500-1"),
						Globals.GetTranslation("Vault2500-2"),
						Globals.GetTranslation("Vault2500-3"),
						Globals.GetTranslation("Vault2500-4"),
						Globals.GetTranslation("Vault2500-5"),
						Globals.GetTranslation("Vault2500-6"),
						Globals.GetTranslation("Vault2500-7")
					});
					moneyAmount = Range.GetRandomValue(500, 3500);
					break;
				case BundleTypes.Vault5000:
					bundleNameFlavor = Globals.RNGGetRandomValueFromList(new List<string>
					{
						Globals.GetTranslation("Vault5000-1"),
						Globals.GetTranslation("Vault5000-2"),
						Globals.GetTranslation("Vault5000-3"),
						Globals.GetTranslation("Vault5000-4"),
						Globals.GetTranslation("Vault5000-5"),
						Globals.GetTranslation("Vault5000-6")
					});
					moneyAmount = Range.GetRandomValue(4000, 7000);
					break;
				case BundleTypes.Vault10000:
					bundleNameFlavor = Globals.RNGGetRandomValueFromList(new List<string>
					{
						Globals.GetTranslation("Vault10000-1"),
						Globals.GetTranslation("Vault10000-2"),
						Globals.GetTranslation("Vault10000-3"),
						Globals.GetTranslation("Vault10000-4"),
						Globals.GetTranslation("Vault10000-5"),
						Globals.GetTranslation("Vault10000-6")
					});
					moneyAmount = Range.GetRandomValue(7500, 12500);
					break;
				case BundleTypes.Vault25000:
					bundleNameFlavor = Globals.RNGGetRandomValueFromList(new List<string>
					{
						Globals.GetTranslation("Vault25000-1"),
						Globals.GetTranslation("Vault25000-2"),
						Globals.GetTranslation("Vault25000-3"),
						Globals.GetTranslation("Vault25000-4"),
						Globals.GetTranslation("Vault25000-5"),
						Globals.GetTranslation("Vault25000-6"),
						Globals.GetTranslation("Vault25000-7")
					});
					moneyAmount = Range.GetRandomValue(20000, 30000);
					break;
				default:
					return;
			}

			RequiredItems = new List<RequiredItem> { new RequiredItem() { MoneyAmount = moneyAmount } };
			string moneyString = moneyAmount.ToString("N0", new CultureInfo(Globals.ModRef.Helper.Translation.Locale));
			Name = $"{Globals.GetTranslation("vault-money-format", new { moneyString })}: {bundleNameFlavor}";
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
				new RequiredItem((int)ObjectIndexes.GoldBar, Range.GetRandomValue(5, 25)),
				new RequiredItem((int)ObjectIndexes.IridiumBar, Range.GetRandomValue(1, 5)),
				new RequiredItem((int)ObjectIndexes.SolidGoldLewis),
				new RequiredItem((int)ObjectIndexes.HMTGF),
				new RequiredItem((int)ObjectIndexes.PinkyLemon),
				new RequiredItem((int)ObjectIndexes.Foroguemon),
				new RequiredItem((int)ObjectIndexes.GoldenPumpkin),
				new RequiredItem((int)ObjectIndexes.GoldenMask),
				new RequiredItem((int)ObjectIndexes.GoldenRelic),
				new RequiredItem((int)ObjectIndexes.GoldBrazier),
				new RequiredItem((int)ObjectIndexes.TreasureChest),
				new RequiredItem((int)ObjectIndexes.Lobster, Range.GetRandomValue(5, 25)),
				new RequiredItem((int)ObjectIndexes.LobsterBisque, Range.GetRandomValue(5, 25))
			};

			Reward = Globals.RNGGetRandomValueFromList(potentialRewards);
		}
	}
}
