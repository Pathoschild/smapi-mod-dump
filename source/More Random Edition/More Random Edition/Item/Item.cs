using System;
using System.Text.RegularExpressions;

namespace Randomizer
{
	/// <summary>
	/// Represents an item in the game
	/// </summary>
	public class Item
	{
		public int Id { get; }
		public string Name
		{
			get { return GetName(); }
		}
		public string DisplayName
		{
			get
			{
				if (!string.IsNullOrEmpty(OverrideName) || !string.IsNullOrEmpty(OverrideDisplayName))
				{
					bool isRandomizedCookedItem = Globals.Config.RandomizeCrops && IsCooked;
					bool isRandomizedCropOrSeedItem = Globals.Config.RandomizeCrops && (IsCrop || IsSeed);
					bool isRandomizedFishItem = Globals.Config.RandomizeFish && IsFish;
					bool useOriginalName = isRandomizedCookedItem || isRandomizedCropOrSeedItem || isRandomizedFishItem;

					if (useOriginalName)
					{
						return Name;
					}
				}

				if (!string.IsNullOrEmpty(OverrideDisplayName))
				{
					return OverrideDisplayName;
				}

				return Globals.GetTranslation($"item-{Id}-display-name");
			}
		}
		public string OverrideName { get; set; }
		public string OverrideDisplayName { get; set; } // Used in the xnb string if it is populated
		public LocationData ForagableLocationData { get; } = new LocationData();
		public bool ShouldBeForagable { get; set; }
		public bool IsForagable
		{
			get { return ShouldBeForagable || ForagableLocationData.HasData(); }
		}

		public bool IsTrash { get; set; }
		public bool IsCraftable { get; set; }
		public bool IsSmelted { get; set; }
		public bool IsAnimalProduct { get; set; }
		public bool IsMonsterItem { get; set; }
		public bool IsFish { get; set; }
		public bool IsArtifact { get; set; }
		public bool IsMayonaisse { get; set; }
		public bool IsGeodeMineral { get; set; }
		public bool IsCrabPotItem { get; set; }
		public bool IsCrop { get; set; }
		public virtual bool IsFlower { get; set; }
		public bool IsSeed { get; set; }
		public bool IsCooked { get; set; }
		public bool IsRing { get; set; }
		public bool IsFruit { get; set; }
		public bool RequiresOilMaker { get; set; }
		public bool RequiresBeehouse { get; set; }
		public bool RequiresKeg { get; set; }
		public bool CanStack { get; set; } = true;

		public bool IsResource { get; set; }
		public Range ItemsRequiredForRecipe { get; set; } = new Range(1, 1);
		public double RequiredItemMultiplier = 1;

		public string CoffeeIngredient { get; set; }

		/// <summary>
		/// The difficulty that this item is to obtain
		/// Will return values appropriate to foragable items - they are never impossible
		/// </summary>
		public ObtainingDifficulties DifficultyToObtain
		{
			get
			{
				if (_difficultyToObtain == ObtainingDifficulties.Impossible && IsForagable)
				{
					return ObtainingDifficulties.LargeTimeRequirements;
				}

				return _difficultyToObtain;
			}
			set
			{
				_difficultyToObtain = value;
			}
		}
		private ObtainingDifficulties _difficultyToObtain { get; set; } = ObtainingDifficulties.Impossible;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The item ID</param>
		public Item(int id)
		{
			Id = id;
			CanStack = id >= -4;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The item ID</param>
		/// <param name="difficultyToObtain">The difficulty to obtain this item</param>
		public Item(int id, ObtainingDifficulties difficultyToObtain)
		{
			Id = id;
			DifficultyToObtain = difficultyToObtain;
		}


		/// <summary>
		/// Returns the string version of this item to use in crafting recipes
		/// Will NOT return the same value each time this is called!
		/// </summary>
		/// <returns></returns>
		public string GetStringForCrafting()
		{
			return $"{Id} {GetAmountRequiredForCrafting()}";
		}

		/// <summary>
		/// Gets a randomly generated amount of this item required for a crafting recipe
		/// Will always return a value of at least 1
		/// </summary>
		/// <returns />
		public int GetAmountRequiredForCrafting()
		{
			int baseAmount = ItemsRequiredForRecipe.GetRandomValue();
			return Math.Max((int)(baseAmount * RequiredItemMultiplier), 1);
		}

		/// <summary>
		/// Gets the name of an item from
		/// The putLastWordOfNameInParens parameter won't work if not passed in, no matter the item
		/// </summary>
		/// <returns>
		/// Splits apart the name from the ObjectIndexes name - WildHorseradish -> Wild Horseradish
		/// Uses the override name if there is one and the item type in question actually has a new name
		/// </returns>
		private string GetName()
		{
			bool ignoreOverrideName =
				(!Globals.Config.RandomizeCrops && (IsCrop || IsSeed)) ||
				(!Globals.Config.RandomizeFish && IsFish);
			if (!ignoreOverrideName && !string.IsNullOrEmpty(OverrideName))
			{
				return OverrideName;
			}

			string enumName = ((ObjectIndexes)Id).ToString();
			return Regex.Replace(enumName, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1").Trim();
		}

		/// <summary>
		/// Gets what a price for an item might be just based on its difficulty to obtain
		/// </summary>
		/// <param name="multiplier">The multiplier for the price - will add or subtract this as a percentage</param>
		/// <returns>The computed price</returns>
		public int GetPriceForObtainingDifficulty(double multiplier)
		{
			int basePrice = 0;
			switch (DifficultyToObtain)
			{
				case ObtainingDifficulties.NoRequirements:
					basePrice = 1000;
					break;
				case ObtainingDifficulties.SmallTimeRequirements:
					basePrice = 5000;
					break;
				case ObtainingDifficulties.MediumTimeRequirements:
					basePrice = 7500;
					break;
				case ObtainingDifficulties.LargeTimeRequirements:
					basePrice = 10000;
					break;
				case ObtainingDifficulties.UncommonItem:
					basePrice = 2500;
					break;
				case ObtainingDifficulties.RareItem:
					basePrice = 20000;
					break;
				case ObtainingDifficulties.EndgameItem:
					basePrice = 20000;
					break;
				case ObtainingDifficulties.NonCraftingItem:
					basePrice = 5000;
					break;
				default:
					Globals.ConsoleError($"Tried to get a base price for an item with an unrecognized ObtainingDifficulty: {Name}");
					return 100;
			}

			int smallerBasePrice = basePrice / 10; // Guarantees that the price will be an even number
			Range range = new Range(
				(int)(smallerBasePrice - (smallerBasePrice * multiplier)),
				(int)(smallerBasePrice * (multiplier + 1))
			);
			return range.GetRandomValue() * 10;
		}

		/// <summary>
		/// Not used normally - but when it is, used for the ObjectInformation string
		/// </summary>
		/// <returns />
		public override string ToString()
		{
			if (Id == (int)ObjectIndexes.Coffee)
			{
				return $"{Name}/150/1/Crafting/{Globals.GetTranslation("item-coffee-name", new { itemName = CoffeeIngredient })}/{Globals.GetTranslation("item-coffee-description")}/drink/0 0 0 0 0 0 0 0 0 1 0/120";
			}

			Globals.ConsoleError($"Called the ToString of unexpected item {Id}: {Name}");
			return "";
		}
	}
}
