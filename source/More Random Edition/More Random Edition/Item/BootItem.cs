using System;

namespace Randomizer
{
	/// <summary>
	/// Represents a boot - doesn't use the same IDs as normal items, but we'll treat it like an item
	/// </summary>
	public class BootItem : Item
	{
		public string Description { get; set; }
		public int NotActuallyPrice { get; set; }
		public int Defense { get; set; }
		public int Immunity { get; set; }
		public int ColorSheetIndex { get; set; }

		public BootItem(
			int id,
			string name,
			int notActuallyPrice,
			int defense,
			int immunity,
			int colorSheetIndex) : base(id)
		{
			DifficultyToObtain = ObtainingDifficulties.NonCraftingItem;
			CanStack = false;
			ShouldBeForagable = false;

			OverrideName = name;
			Description = Globals.GetTranslation($"boots-{id}-description");
			NotActuallyPrice = notActuallyPrice;
			Defense = defense;
			Immunity = immunity;
			ColorSheetIndex = colorSheetIndex;
		}

		/// <summary>
		/// Gets the sale price of the boot
		/// Equal to the sell price * 8
		/// </summary>
		/// <returns />
		public int GetBuyPrice()
		{
			return Math.Max(50 * 8 * (Defense + Immunity), 100);
		}

		/// <summary>
		/// The string to use for asset replacement
		/// </summary>
		/// <returns />
		public override string ToString()
		{
			return $"{Name}/{Description}/{NotActuallyPrice}/{Defense}/{Immunity}/{ColorSheetIndex}/{Name}";
		}
	}
}
