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
