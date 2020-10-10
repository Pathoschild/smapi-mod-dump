/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

namespace Randomizer
{
	/// <summary>
	/// Represents a crop
	/// </summary>
	public class CropItem : Item
	{
		public int Price { get; set; }
		public string CategoryString { get; set; }
		public string Description { get; set; }

		public override bool IsFlower
		{
			get
			{
				return CategoryString == "18/Basic -80";
			}
		}

		public SeedItem MatchingSeedItem
		{
			get
			{
				return ItemList.GetSeedFromCrop(this);
			}
		}

		public CropItem(int id, string categoryString) : base(id)
		{
			IsCrop = true;
			DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
			CategoryString = categoryString;
		}

		public override string ToString()
		{
			return $"{Name}/{Price}/{CategoryString}/{Name}/{Description}";
		}
	}
}
