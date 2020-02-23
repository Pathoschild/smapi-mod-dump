using System.Collections.Generic;

namespace Randomizer
{
	public class JojaBundle : Bundle
	{
		public static List<BundleTypes> RoomBundleTypes { get; set; }

		/// <summary>
		/// Creates a bundle for the Joja mart
		/// </summary>
		protected override void Populate()
		{
			BundleType = Globals.RNGGetAndRemoveRandomValueFromList(RoomBundleTypes);
			List<RequiredItem> potentialItems = new List<RequiredItem>();

			switch (BundleType)
			{
				case BundleTypes.JojaMissing:
					Name = Globals.GetTranslation("bundle-joja-missing");

					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.EndgameItem)),
						new RequiredItem(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.RareItem)),
						new RequiredItem(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.LargeTimeRequirements)),
						new RequiredItem(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements)),
						new RequiredItem(
							Globals.RNGGetRandomValueFromList(ItemList.GetItemsBelowDifficulty(ObtainingDifficulties.Impossible, new List<int> { (int)ObjectIndexes.AnyFish }))
						)
					};
					MinimumRequiredItems = 5;
					Color = BundleColors.Blue;
					break;
			}
		}

		/// <summary>
		/// There actually no item reward for the Joja bundle, so leaving blank
		/// </summary>
		protected override void GenerateReward()
		{
		}
	}
}
