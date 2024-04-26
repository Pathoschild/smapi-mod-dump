/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

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
			RNG rng = BundleRandomizer.Rng;

			BundleType = rng.GetAndRemoveRandomValueFromList(RoomBundleTypes);

			switch (BundleType)
			{
				case BundleTypes.JojaMissing:
					SetBundleName("bundle-joja-missing");

					RequiredItems = new List<RequiredBundleItem>
					{
						new(ItemList.GetRandomItemAtDifficulty(rng, ObtainingDifficulties.EndgameItem)),
						new(ItemList.GetRandomItemAtDifficulty(rng, ObtainingDifficulties.RareItem)),
						new(ItemList.GetRandomItemAtDifficulty(rng, ObtainingDifficulties.LargeTimeRequirements)),
						new(ItemList.GetRandomItemAtDifficulty(rng, ObtainingDifficulties.MediumTimeRequirements)),
						new(
							rng.GetRandomValueFromList(
								ItemList.GetItemsBelowDifficulty(
									ObtainingDifficulties.Impossible))
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
