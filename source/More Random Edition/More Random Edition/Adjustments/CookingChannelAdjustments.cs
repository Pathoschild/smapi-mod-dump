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
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Updates the cooking channel data to match the current randomizer settings
	/// </summary>
	public class CookingChannelAdjustments
	{
		/// <summary>
		/// A class consisting of all the data needed to replace one of the cooking channel recipes
		/// </summary>
		private class ShowData
		{
			public int ID { get; set; }
			public string Recipe { get; set; }
			public string Item1 { get; set; }
			public string Item2 { get; set; }

			public ShowData(int id, ObjectIndexes recipeItemId = 0, ObjectIndexes item1Id = 0, ObjectIndexes item2Id = 0)
			{
				ID = id;
				Recipe = recipeItemId > 0 ? recipeItemId.GetItem().OverrideDisplayName : "";
				Item1 = item1Id > 0 ? ItemList.GetItemName(item1Id) : "";
				Item2 = item2Id > 0 ? ItemList.GetItemName(item2Id) : "";
			}

			public object GetTokenObject()
			{
				return new
				{
					recipe = Recipe,
					item1 = Item1,
					item2 = Item2
				};
			}
		}

		/// <summary>
		/// Gets the text edits for the cooking channel so it makes sense with the randomized items
		/// </summary>
		public static Dictionary<string, string> GetTextEdits()
		{
			FixCookingRecipeDisplayNames();

            Dictionary<string, string> replacements = new();
			if (!Globals.Config.Crops.Randomize && !Globals.Config.Fish.Randomize) { return replacements; }

			foreach (ShowData showData in GetCookingChannelData())
			{
				AddReplacement(replacements, showData.ID, showData.GetTokenObject());
			}

			return replacements;
		}

		/// <summary>
		/// Gets the cooking channel data
		/// </summary>
		/// <returns>A list of the ShowData</returns>
		private static List<ShowData> GetCookingChannelData()
		{
			return new List<ShowData>
			{
				new(2, 0, ObjectIndexes.RedCabbage),
				new(3, ObjectIndexes.RadishSalad, ObjectIndexes.Radish),
				new(7, 0, ObjectIndexes.Rice),
				new(10, ObjectIndexes.TroutSoup, ObjectIndexes.RainbowTrout),
				new(11, ObjectIndexes.GlazedYams, ObjectIndexes.Yam),
				new(12, ObjectIndexes.ArtichokeDip, ObjectIndexes.Artichoke),
				new(15, ObjectIndexes.PumpkinPie, ObjectIndexes.Pumpkin),
				new(16, ObjectIndexes.CranberryCandy, ObjectIndexes.Cranberries),
				new(17, 0, ObjectIndexes.Tomato),
				new(18, 0, ObjectIndexes.Potato),
				new(21, ObjectIndexes.CarpSurprise, ObjectIndexes.Carp),
				new(23, 0, ObjectIndexes.Melon),
				new(24, ObjectIndexes.FruitSalad),
				new(29, 
					ObjectIndexes.PoppyseedMuffin, 
					ObjectIndexes.Poppy, 
					((CropItem)ObjectIndexes.Poppy.GetItem()).MatchingSeedItem.ObjectIndex),
				new(31, 0, ObjectIndexes.Tomato),
			};
		}

		/// <summary>
		/// Adds a replacement to the dictionary of replacements
		/// </summary>
		/// <param name="replacements">The replacement dictionary</param>
		/// <param name="id">The id of the cooking show</param>
		/// <param name="tokenObject">The object containing all the replacements</param>
		private static void AddReplacement(Dictionary<string, string> replacements, int id, object tokenObject)
		{
			string replacementText = Globals.GetTranslation($"cooking-channel-{id}", tokenObject);
			replacements.Add(id.ToString(), replacementText);
		}

        /// <summary>
        /// Fix the cooking recipe display names so that the queen of sauce shows
        /// can actually display the correct thing
        /// </summary>
        public static void FixCookingRecipeDisplayNames()
		{
			ItemList.GetCookedItems()
				.Where(item => item is CookedItem cookedItem && cookedItem.IsCropOrFishDish)
				.ToList()
				.ForEach(dish =>
				{
                    ObjectIndexes index = dish.ObjectIndex;
                    CookedItem item = (CookedItem)index.GetItem();
                    item.OverrideDisplayName = dish.DisplayName;
                });
        }
    }
}


