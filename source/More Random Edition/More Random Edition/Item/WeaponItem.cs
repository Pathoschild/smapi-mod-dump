using System;

namespace Randomizer
{
	/// <summary>
	/// Represents a weapon - doesn't use the same IDs as normal items, but we'll treat it like an item
	/// </summary>
	public class WeaponItem : Item
	{
		public string Description { get; set; }
		public Range Damage { get; set; }
		public double Knockback { get; set; }
		public int Speed { get; set; }
		public int AddedPrecision { get; set; }
		public int AddedDefense { get; set; }
		public WeaponType Type { get; set; }
		public int BaseMineLevelDrop { get; set; }
		public int MinMineLevelDrop { get; set; }
		public int AddedAOE { get; set; }
		public double CritChance { get; set; }
		public double CritMultiplier { get; set; }

		public WeaponItem(int id) : base(id)
		{
			DifficultyToObtain = ObtainingDifficulties.NonCraftingItem;
			CanStack = false;
			ShouldBeForagable = false;
		}

		/// <summary>
		/// Whether to randomize drop info - currently based on whether it is a galaxy weapon
		/// </summary>
		/// <returns />
		public bool ShouldRandomizeDropInfo()
		{
			return Id != (int)WeaponIndexes.GalaxyDagger &&
				Id != (int)WeaponIndexes.GalaxyHammer &&
				Id != (int)WeaponIndexes.GalaxySlingshot &&
				Id != (int)WeaponIndexes.GalaxySword;
		}

		/// <summary>
		/// Gets the buy price of the weapon - 8 * 100 * the weapon level
		/// THe original sell price was 100 * the level
		/// </summary>
		/// <returns />
		public int GetBuyPrice()
		{
			if (!Globals.Config.RandomizeWeapons) { return GetOriginalBuyPrice(); }

			return GetLevel() * 100 * 4;
		}

		/// <summary>
		/// Gets the level of the weapon
		/// </summary>
		/// <param name="weapon">The weapon level</param>
		/// <returns />
		public int GetLevel()
		{
			if (Name == "Unusual Pie Shieldbreaker of the Forest")
			{
				int hi = 5;
				hi++;
			}

			int averageDamage = (Damage.MaxValue + Damage.MinValue) / 2;
			double speedBonus = 1d + 0.1d * (Math.Max(0, Speed) + (Type == WeaponType.Dagger ? 15 : 0));
			int statsBonus = (AddedPrecision / 2) + AddedDefense;
			double critChanceBonus = (CritChance - 0.02) * 100d;
			double critMultBonus = (CritMultiplier - 3) * 20d;

			int baseLevel = (int)(averageDamage * speedBonus) + (int)(statsBonus + critChanceBonus + critMultBonus);
			if (Type == WeaponType.ClubOrHammer)
			{
				baseLevel /= 2;
			}

			return baseLevel / 5 + 1;
		}

		/// <summary>
		/// Gets the original buy price of the weapon, taken from the getAdventureShopStock method
		/// of Utility.cs
		/// </summary>
		/// <returns />
		public int GetOriginalBuyPrice()
		{
			switch (Id)
			{
				case 12: return 250;
				case 17: return 500;
				case 1: return 750;
				case 43: return 850;
				case 44: return 1500;
				case 27: return 2000;
				case 10: return 2000;
				case 7: return 4000;
				case 5: return 6000;
				case 50: return 9000;
				case 9: return 25000;
				case 4: return 50000;
				case 23: return 35000;
				case 29: return 75000;
				case 13: return 10000;
			}

			return 9999; // Shouldn't be hitting this
		}

		/// <summary>
		/// The string to use for asset replacement
		/// </summary>
		/// <returns />
		public override string ToString()
		{
			return $"{Name}/{Description}/{Damage.MinValue}/{Damage.MaxValue}/{Knockback}/{Speed}/{AddedPrecision}/{AddedDefense}/{(int)Type}/{BaseMineLevelDrop}/{MinMineLevelDrop}/{AddedAOE}/{CritChance}/{CritMultiplier}/{Name}";
		}
	}
}
