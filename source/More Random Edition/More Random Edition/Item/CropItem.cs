/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Represents a crop
	/// </summary>
	public class CropItem : Item
	{
		public int Price { get; set; }
        public string Description { get; set; }

        public override bool IsFlower
		{
			get
			{
				return Game1.objectData[Id].Category == Object.flowersCategory;
			}
		}

		public SeedItem MatchingSeedItem
		{
			get
			{
				return ItemList.GetSeedFromCrop(this);
			}
		}

		public CropItem(ObjectIndexes index) : base(index)
		{
			IsCrop = true;
			DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
		}

		/// <summary>
		/// Gets all the crop items
		/// </summary>
		/// <param name="includeUnchangedCrops">Include unchanged crop items (ancient fruit)</param>
		/// <returns />
		public static List<CropItem> Get(bool includeUnchangedCrops = false)
		{
			return ItemList.Items.Values.Where(x =>
				x.IsCrop &&
				(includeUnchangedCrops || x.ObjectIndex != ObjectIndexes.AncientFruit))
			.Cast<CropItem>()
			.ToList();
		}
	}
}
