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
using System;
using System.Collections.Generic;
using System.Linq;
using SVObject = StardewValley.Object;

namespace Randomizer
{
	/// <summary>
	/// Represents an item you make in your kitchen
	/// </summary>
	public class CookedItem : Item
	{
		/// <summary>
		/// Set up a dictionary to link cooked items to their recipe names
		/// this will be used to fix the recipe tooltips of items in ObjectInformation
		/// that do not match from their display names
		/// </summary>
		private readonly static Dictionary<int, string> CookedItemsToRecipeNames = new();
		static CookedItem() {
            const int CookedItemIdIndex = 2;
            var cookingRecipeData = Globals.ModRef.Helper.GameContent
                .Load<Dictionary<string, string>>("Data/CookingRecipes");

            foreach (KeyValuePair<string, string> data in cookingRecipeData)
            {
                string[] tokens = data.Value.Split("/");
				int cookedItemId;
				try
				{
                    cookedItemId = int.Parse(tokens[CookedItemIdIndex]);
                }
                catch(Exception) 
				{
					Globals.ConsoleTrace($"Cannot parse cooked item id (it was likely modded, so skipping): {data.Key}");
					continue;
				}
                CookedItemsToRecipeNames[cookedItemId] = data.Key;
            }
        }

		/// <summary>
		/// The name this item goes by in the recipe list
		/// </summary>
		public string RecipeName { get => CookedItemsToRecipeNames[Id]; }

		/// <summary>
		/// The id of the special ingredient used to cook this item
		/// </summary>
		public int? IngredientId { get; set; } = null;

        /// <summary>
        /// The special ingredient used to cook this item
        /// </summary>
        public string IngredientName { 
			get 
			{
				return IngredientId == null
					? string.Empty
					: ItemList.Items[(ObjectIndexes)IngredientId.Value].Name;
			}
		}

		/// <summary>
		/// Whether this is a dish where the name was changed
		/// </summary>
		public bool IsCropOrFishDish { get; set; }

		/// <summary>
		/// True if it's a fish dish
		/// </summary>
		public bool IsFishDish { get; set; }

        /// <summary>
        /// True if it's a crop dish - based on the fish dish value
        /// </summary>
        public bool IsCropDish { 
			get
			{
				return IsCropOrFishDish && !IsFishDish;
			}
		}

		public CookedItem(int id) : base(id)
		{
			IsCooked = true;
			DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
        }

		public CookedItem(
			int id, 
			int? ingredientId,
			bool isFishDish = false): this(id)
		{
			IsCropOrFishDish = true;
			IngredientId = ingredientId;
            IsFishDish = isFishDish;
        }

		/// <summary>
		/// We can't do this on construction, since the randomized names have not happened yet
		/// </summary>
		public void CalculateOverrideName()
		{
			if (IsCropOrFishDish)
            {
                string nameAndDescription = Globals.GetTranslation($"item-{Id}-name-and-description", new { itemName = IngredientName });
                string name = nameAndDescription.Split("/")[0];
                OverrideName = string.Format(name, IngredientName);
            }
        }

        /// <summary>
        /// Gets all the fish dishes
        /// </summary>
        /// <returns />
		public static List<CookedItem> GetAllFishDishes()
		{
			return ItemList.Items.Values
				.Where(item => item is CookedItem cookedItem && cookedItem.IsFishDish)
				.Cast<CookedItem>()
				.ToList();
		}

        /// <summary>
        /// Gets all the crop dishes
        /// </summary>
        /// <returns />
        public static List<CookedItem> GetAllCropDishes()
        {
            return ItemList.Items.Values
                .Where(item => item is CookedItem cookedItem && cookedItem.IsCropDish)
                .Cast<CookedItem>()
                .ToList();
        }

		/// <summary>
		/// Gets the saliable object for this cooked item
		/// If it's a recipe, it replaces the name with the recipe name so that
		/// the tooltip will display correctly
		/// </summary>
		/// <param name="initialStack">How many are in the stack to buy</param>
		/// <param name="isRecipe">Whether this is a recipe</param>
		/// <param name="price">The price of the item (-1 for default price)</param>
		/// <returns>The object that can be sold</returns>
        public override ISalable GetSaliableObject(int initialStack = 1, bool isRecipe = false, int price = -1)
        {
			var svObject = new SVObject(Id, initialStack, isRecipe, price);
			if (isRecipe)
			{
				svObject.Name = RecipeName;
            }
			return svObject;
        }

        /// <summary>
        /// Returns the ToString representation to be used for the cooked item object
        /// </summary>
        /// <returns />
        public override string ToString()
		{
			if (!IsCropOrFishDish)
			{
				Globals.ConsoleWarn($"Unexpected ToString call of cooked item: {Id}: {EnglishName}");
				return "";
			}

            string[] itemData = ItemList.OriginalItemList[Id].Split("/");
            string[] nameAndDescription = Globals.GetTranslation($"item-{Id}-name-and-description", 
				new { itemName = IngredientName }).Split("/");
			itemData[(int)ObjectInformationIndexes.DisplayName] = nameAndDescription[0];
            itemData[(int)ObjectInformationIndexes.Description] = nameAndDescription[1];

            return string.Join("/", itemData);
		}
    }
}
