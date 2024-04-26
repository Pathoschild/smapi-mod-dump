/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using SVObject = StardewValley.Object;
using SVRing = StardewValley.Objects.Ring;

namespace Randomizer
{
	/// <summary>
	/// Represents an item in the game
	/// </summary>
	public class Item
    {
        public const string ObjectIdPrefix = "(O)";
		public const string DefaultTexture = "Maps/springobjects";

		public string Id { get; }

		/// <summary>
		/// Gets the corresponding object index
		/// Intended to only be used with objects!
		/// </summary>
		public ObjectIndexes ObjectIndex
		{
			get
			{
				if (IsBigCraftable)
				{
					Globals.ConsoleWarn($"Tried to get the object index of big craftable (using Acorn instead): {Id}");
					return ObjectIndexes.Acorn;
				}
				return ObjectIndexesExtentions.GetObjectIndex(Id);
			}
		}

        /// <summary>
        /// Gets the corresponding object index
        /// Intended to only be used with big craftables!
        /// </summary>
        public BigCraftableIndexes BigCraftableIndex
		{
            get
            {
                if (!IsBigCraftable)
                {
                    Globals.ConsoleWarn($"Tried to get the big craftable index of a non-bigcraftable (using Chest instead): {Id}");
                    return BigCraftableIndexes.Chest;
                }
                return BigCraftableIndexesExtentions.GetBigCraftableIndex(Id);
            }
        }

        /// <summary>
		/// This is the QualifiedItemId in Stardew's code a prefix before the integer id
        /// BigCraftables (BC), 
		/// Boots (B), 
		/// Farmhouse Flooring (FL), 
		/// Furniture (F), 
		/// Hats (H), 
		/// Objects (O),
		/// Pants (P), 
		/// Shirts (S),
		/// Tools (T), 
		/// Wallpaper (WP),
		/// Weapons (W)
        /// </summary>
        public string QualifiedId { 
			get
			{
                // We currently only define objects and big craftables here
                string itemType = IsBigCraftable ? "BC" : "O";
				return $"({itemType}){Id}";
			} 
		}

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
					bool isRandomizedCookedItem = Globals.Config.Crops.Randomize && IsCooked;
					bool isRandomizedCropOrSeedItem = Globals.Config.Crops.Randomize && (IsCrop || IsSeed);
					bool isRandomizedFishItem = Globals.Config.Fish.Randomize && IsFish;
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

				return ItemRegistry.GetData(QualifiedId).DisplayName;
            }
		}
		public string OverrideName { get; set; }
		public string OverrideDisplayName { get; set; } // Used in the xnb string if it is populated
		/// <summary>
		/// The Name field in the object data is the English name
		/// </summary>
		public string EnglishName => IsBigCraftable
			? Name
			: Game1.objectData[Id].Name;
		/// <summary>
		/// The default texture is in sprint objects - it's unfortunately not defined by default
		/// </summary>
		public string Texture => Game1.objectData[Id].Texture ?? DefaultTexture;
		public int SpriteIndex => Game1.objectData[Id].SpriteIndex;
		public bool ShouldBeForagable { get; set; }
		public bool IsForagable => ShouldBeForagable;
		public bool IsTrash { get; set; }
		public bool IsCraftable { get; set; }
		public bool IsBigCraftable { get; set; }

		/// <summary>
		/// BigCraftables have no price, so we need to define our own
		/// Note that this is the price that we want to buy it at, so we need to split it in half when setting it as a value
		/// </summary>
		public int BigCraftablePrice { get; set; }

		public bool IsSmelted { get; set; }
		public bool IsAnimalProduct { get; set; }
		public bool IsMonsterItem { get; set; }
		public bool IsFish { get; set; }
		public bool IsArtifact { get; set; }
		public bool IsMayonaisse { get =>
            new List<string>() {
                ObjectIndexes.Mayonnaise.GetId(),
				ObjectIndexes.DuckMayonnaise.GetId(),
				ObjectIndexes.VoidMayonnaise.GetId()
            }.Contains(Id);
        }
        public bool IsMilk { get => Category == ItemCategories.Milk; }
        public bool IsEgg { get => Category == ItemCategories.Eggs; }
        public bool IsGeodeMineral { get; set; }
		public bool IsCrabPotItem { get =>
			this is CrabPotItem ||
            new List<string>() {
                ObjectIndexes.Clam.GetId(),
                ObjectIndexes.Cockle.GetId(),
                ObjectIndexes.Mussel.GetId(),
                ObjectIndexes.Oyster.GetId()
            }.Contains(Id);
        }
        public bool IsTapperItem { get =>
			new List<string>() {
				ObjectIndexes.PineTar.GetId(),
				ObjectIndexes.OakResin.GetId(),
				ObjectIndexes.MapleSyrup.GetId(),
				ObjectIndexes.MysticSyrup.GetId()
			}.Contains(Id);
        }
        public bool IsCrop { get; set; }
		public virtual bool IsFlower { get; set; }
		public bool IsSeed { get; set; }
		public bool IsCooked { get; set; }
		public bool IsRing { get; set; }
		public bool IsFruit { get; set; }
		public bool IsRandomizedFruitTree {
            get => FruitTreeRandomizer.RandomizedFruitTreeIds.Contains(Id);
        }
		public bool IsTotem { get =>
            new List<string>() {
                ObjectIndexes.WarpTotemFarm.GetId(),
                ObjectIndexes.WarpTotemBeach.GetId(),
                ObjectIndexes.WarpTotemMountains.GetId(),
                ObjectIndexes.WarpTotemDesert.GetId(),
                ObjectIndexes.RainTotem.GetId(),
				ObjectIndexes.TreasureTotem.GetId()
            }.Contains(Id);
		}
		public bool RequiresOilMaker { get; set; }
		public bool RequiresBeehouse { get; set; }
		public bool RequiresKeg { get; set; }
		public bool CanStack { get; set; } = true;

		public bool IsResource { get; set; }
		public Range ItemsRequiredForRecipe { get; set; } = new Range(1, 1);
		public double RequiredItemMultiplier = 1;

		public string CoffeeIngredient { get; set; }

        /// <summary>
        /// Gets the category that belongs to the item
        /// Null if we don't have the category defined
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The ItemCategories value found, or null if none was found</returns>
        public ItemCategories? Category
        {
			get 
			{
                int categoryId = ItemRegistry.GetData(QualifiedId).Category;
                return Enum.IsDefined(typeof(ItemCategories), categoryId)
                    ? (ItemCategories)categoryId
                    : null;
            }
        }

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
        public Item(string id)
        {
            Id = id;
            CanStack = !IsBigCraftable;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The item ID</param>
        public Item(ObjectIndexes index)
		{
			Id = index.GetId();
			CanStack = true;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The item ID</param>
        public Item(BigCraftableIndexes index)
        {
            Id = index.GetId();
            CanStack = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The item ID - assumed to have used GetId</param>
        /// <param name="isBigCraftable">Whether the item is a big craftable</param>
        /// <param name="difficultyToObtain">The difficulty to obtain this item</param>
        public Item(string id, ObtainingDifficulties difficultyToObtain, bool isBigCraftable = false)
		{
			Id = id;
			IsBigCraftable = isBigCraftable;
			DifficultyToObtain = difficultyToObtain;
			if (isBigCraftable) 
			{ 
				CanStack = false; 
			}
		}

        /// <summary>
        /// Returns whether the given qualified id is for a Stardew Valley object
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns>True if the given id is for a Stardew Valley object, false otherwise</returns>
        public static bool IsQualifiedIdForObject(string id)
        {
            return id.StartsWith(ObjectIdPrefix);
        }

        /// <summary>
        /// Gets a randomly generated amount of this item required for a crafting recipe
        /// Will always return a value of at least 1
        /// </summary>
        /// <param name="rng">The RNG to use</param>
        /// <returns />
        public int GetAmountRequiredForCrafting(RNG rng)
		{
			int baseAmount = ItemsRequiredForRecipe.GetRandomValue(rng);
			return Math.Max((int)(baseAmount * RequiredItemMultiplier), 1);
		}

		/// <summary>
		/// Gets the name of an item from
		/// </summary>
		/// <returns>
		/// Splits apart the name from the ObjectIndexes name - WildHorseradish -> Wild Horseradish
		/// Uses the override name if there is one and the item type in question actually has a new name
		/// </returns>
		private string GetName()
		{
            bool ignoreOverrideName =
				(!Globals.Config.Crops.Randomize && (IsCrop || IsSeed)) ||
				(!Globals.Config.Fish.Randomize && IsFish);

            if (!ignoreOverrideName && !string.IsNullOrEmpty(OverrideName))
            {
                return OverrideName;
            }

			return ItemRegistry.GetData(QualifiedId).InternalName;
		}

		public virtual ISalable GetSaliableObject(int initialStack = 1, bool isRecipe = false, int price = -1)
		{
			if (IsRing)
			{
                return new SVRing(Id.ToString())
                {
                    Stack = initialStack
                };
            }

            return IsBigCraftable 
				? new SVObject(Vector2.Zero, Id.ToString(), isRecipe)
					{
						Stack = initialStack,
						Price = price == -1 
							? (BigCraftablePrice / 2) // We want the sell price, not the buy price
							: price
					}
				: new SVObject(Id.ToString(), initialStack, isRecipe, price);
        }

		/// <summary>
		/// Not used, so log when it's called
		/// </summary>
		/// <returns />
		public override string ToString()
		{
			Globals.ConsoleError($"Called the ToString of unexpected item {Id}: {Name}");
			return "";
		}
	}
}
