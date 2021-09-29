/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/StatsAsTokens
**
*************************************************/

// Copyright (C) 2021 Vertigon
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatsAsTokens
{
	internal class FoodEatenToken : BaseToken
	{
		/*********
		** Fields
		*********/
		/// <summary>Stores item ID number as Key, number eaten as Value.</summary>
		public static FoodEatenData foodEatenDict, cachedFoodEatenDict;
		/// <summary>Internal cache of data from ObjectInformation.json. Used to determine which items are considered food and which are not.</summary>
		public static readonly Dictionary<int, string> objectData;

		/*********
		** Public methods
		*********/

		/****
		** Static Constructor
		****/

		/// <summary>
		/// Constructor needs to be static to initialize static fields properly. The dictionary which stores how much food is eaten has to be static in order to be accessed by the Harmony patch triggered upon eating.
		/// </summary>
		static FoodEatenToken()
		{
			objectData = Globals.Helper.Content.Load<Dictionary<int, string>>("Data/ObjectInformation", ContentSource.GameContent);

			foodEatenDict = new();
			cachedFoodEatenDict = new();

			HarmonyPatches.FoodEatenPatch();
		}

		/****
		** Metadata
		****/

		public override bool TryValidateInput(string input, out string error)
		{
			error = "";
			string[] args = input.ToLower().Trim().Split('|');

			if (args.Count() == 2)
			{
				if (!args[0].Contains("player="))
				{
					error += "Named argument 'player' not provided. ";
				}
				else if (args[0].IndexOf('=') == args[0].Length - 1)
				{
					error += "Named argument 'player' not provided a value. Must be one of the following values: 'host', 'local'. ";
				}
				else
				{
					// accept hostplayer or host, localplayer or local
					string playerType = args[0].Substring(args[0].IndexOf('=') + 1).Trim().Replace("player", "");
					if (!(playerType.Equals("host") || playerType.Equals("local")))
					{
						error += "Named argument 'player' must be one of the following values: 'host', 'local'. ";
					}
				}

				if (!args[1].Contains("food="))
				{
					error += "Named argument 'food' not provided. Must be a string consisting of alphanumeric characters. ";
					return false;
				}
				else if (args[1].IndexOf('=') == args[1].Length - 1)
				{
					error += "Named argument 'food' must be a string consisting of alphanumeric characters. ";
				}
			}
			else
			{
				error += "Incorrect number of arguments provided. A 'player' argument and 'food' argument should be provided. ";
			}

			return error.Equals("");
		}

		/****
		** State
		****/

		protected override bool DidStatsChange()
		{
			bool hasChanged = false;

			string pType = loc;

			Dictionary<string, int> foodEaten = foodEatenDict.Value[pType];
			Dictionary<string, int> cachedFoodEaten = cachedFoodEatenDict.Value[pType];

			// check cached local player stats against Game1's local player stats
			// only needs to happen if player is local
			if (!Game1.IsMasterGame)
			{
				foreach (KeyValuePair<string, int> pair in foodEaten)
				{
					if (!cachedFoodEaten.ContainsKey(pair.Key))
					{
						hasChanged = true;
						cachedFoodEaten[pair.Key] = pair.Value;
					}
					else if (!cachedFoodEaten[pair.Key].Equals(pair.Value))
					{
						hasChanged = true;
						cachedFoodEaten[pair.Key] = pair.Value;
					}
				}
			}

			pType = host;

			// check cached master player stats against Game1's master player stats
			// needs to happen whether player is host or local
			foodEaten = foodEatenDict.Value[pType];
			cachedFoodEaten = cachedFoodEatenDict.Value[pType];

			foreach (KeyValuePair<string, int> pair in foodEaten)
			{
				if (!cachedFoodEaten.ContainsKey(pair.Key))
				{
					hasChanged = true;
					cachedFoodEaten[pair.Key] = pair.Value;
				}
				else if (!cachedFoodEaten[pair.Key].Equals(pair.Value))
				{
					hasChanged = true;
					cachedFoodEaten[pair.Key] = pair.Value;
				}
			}

			return hasChanged;
		}

		/// <summary>Get the current values.</summary>
		/// <param name="input">The input arguments, if applicable.</param>
		public override IEnumerable<string> GetValues(string input)
		{
			List<string> output = new();

			string[] args = input.Split('|');

			// sanitize inputs
			string playerType = args[0].Substring(args[0].IndexOf('=') + 1).Trim().ToLower().Replace("player", "").Replace(" ", "");
			string food = args[1].Substring(args[1].IndexOf('=') + 1).Trim().ToLower().Replace(" ", "");

			string pType = playerType.Equals("host") ? host : loc;

			if (TryGetFoodEaten(food, pType, out string foodEatenNum))
			{
				output.Add(foodEatenNum);
			}

			return output;
		}

		/*********
		** Private methods
		*********/

		/// <summary>
		/// Initializes internal dictionary to 0. Scrapes ObjectInformation to locate all edible items (edibility != -300).
		/// </summary>
		/// <returns>A dictionary with all food items as keys, initialized to value 0.</returns>
		private static SerializableDictionary<string, int> InitializeFoodEatenStats()
		{
			SerializableDictionary<string, int> foodEaten = new();

			foreach (KeyValuePair<int, string> obj in objectData)
			{
				string[] objDescription = obj.Value.Split('/');

				// anything with edibility that is not -300 is edible
				if (objDescription.Length > 2 && !objDescription[2].Equals("-300"))
				{
					foodEaten[obj.Key.ToString()] = 0;
				}
			}

			// check for DGA food?
			// can't be done without hard dependency - mention to users that they will need to check if FoodEaten has value for DGA food before querying with it

			return foodEaten;
		}

		/// <summary>
		/// Attempts to find the number of the specified food eaten for the specified player type, and if located, passes the value out via <c>foodEatenNum</c>.
		/// </summary>
		/// <param name="foodNameOrId"></param>
		/// <param name="playerType"></param>
		/// <param name="foodEatenNum"></param>
		/// <returns></returns>
		private bool TryGetFoodEaten(string foodNameOrId, string playerType, out string foodEatenNum)
		{
			string pType = playerType;
			string foodId = "";
			foodEatenNum = "";

			bool found = false;
			bool isNumericId = int.TryParse(foodNameOrId, out _);

			// string passed in is not a number - try matching with object entry to find ID
			if (!isNumericId)
			{
				// "any" is special case - otherwise, try to match
				if (!foodNameOrId.Equals("any"))
				{
					string fuzzyName = Utility.fuzzyItemSearch(foodNameOrId)?.Name.Trim().Replace(" ", "").ToLower() ?? foodNameOrId;

					// logging
					Globals.Monitor.Log($"Parsed 'food' value {foodNameOrId} to {fuzzyName}");

					foreach (KeyValuePair<int, string> pair in objectData)
					{
						if (pair.Value.Split('/')[0].Replace(" ", "").ToLower().Equals(fuzzyName))
						{
							foodId = pair.Key.ToString();
							break;
						}
					}
				}
			}
			else
			{
				foodId = foodNameOrId;
			}

			if (playerType.Equals(loc) && Game1.IsMasterGame)
			{
				pType = host;
			}

			if (pType.Equals(host) || pType.Equals(loc))
			{
				if (foodNameOrId.Equals("any"))
				{
					found = true;
					foodEatenNum = cachedFoodEatenDict.Value[pType].Values.Sum().ToString();
				}
				else
				{
					foreach (string key in cachedFoodEatenDict.Value[pType].Keys)
					{
						if (key.Equals(foodId))
						{
							found = true;
							foodEatenNum = cachedFoodEatenDict.Value[pType][key].ToString();
						}
					}
				}
			}

			return found;
		}

		/*********
		** Subclass
		*********/

		/// <summary>
		/// Required in order to read/write data to save file properly. Just stores the host and local dicts (food = key, number eaten = value).
		/// </summary>
		public class FoodEatenData
		{
			public Dictionary<string, Dictionary<string, int>> Value;

			public FoodEatenData()
			{
				Value = new(StringComparer.OrdinalIgnoreCase)
				{
					[host] = InitializeFoodEatenStats(),
					[loc] = InitializeFoodEatenStats()
				};
			}
		}

	}
}
