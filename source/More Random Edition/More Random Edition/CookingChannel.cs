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
	/// <summary>
	/// Updates the cooking channel data to match the current randomizer settings
	/// </summary>
	public class CookingChannel
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

			public ShowData(int id, int recipeItemId = 0, int item1Id = 0, int item2Id = 0)
			{
				ID = id;
				Recipe = recipeItemId > 0 ? ItemList.Items[recipeItemId].OverrideDisplayName : "";
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
			Dictionary<string, string> replacements = new Dictionary<string, string>();
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
				new ShowData(2, 0, (int)ObjectIndexes.RedCabbage),
				new ShowData(3, (int)ObjectIndexes.RadishSalad, (int)ObjectIndexes.Radish),
				new ShowData(7, 0, (int)ObjectIndexes.Rice),
				new ShowData(10, (int)ObjectIndexes.TroutSoup, (int)ObjectIndexes.RainbowTrout),
				new ShowData(11, (int)ObjectIndexes.GlazedYams, (int)ObjectIndexes.Yam),
				new ShowData(12, (int)ObjectIndexes.ArtichokeDip, (int)ObjectIndexes.Artichoke),
				new ShowData(15, (int)ObjectIndexes.PumpkinPie, (int)ObjectIndexes.Pumpkin),
				new ShowData(16, (int)ObjectIndexes.CranberryCandy, (int)ObjectIndexes.Cranberries),
				new ShowData(17, 0, (int)ObjectIndexes.Tomato),
				new ShowData(18, 0, (int)ObjectIndexes.Potato),
				new ShowData(21, (int)ObjectIndexes.CarpSurprise, (int)ObjectIndexes.Carp),
				new ShowData(23, 0, (int)ObjectIndexes.Melon),
				new ShowData(24, (int)ObjectIndexes.FruitSalad),
				new ShowData(29, (int)ObjectIndexes.PoppyseedMuffin, (int)ObjectIndexes.Poppy, ((CropItem)ItemList.Items[(int)ObjectIndexes.Poppy]).MatchingSeedItem.Id),
				new ShowData(31, 0, (int)ObjectIndexes.Tomato),
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
	}
}


