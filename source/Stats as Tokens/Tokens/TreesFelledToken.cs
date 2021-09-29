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

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatsAsTokens
{
	internal class TreesFelledToken : BaseToken
	{

		/*********
		** Fields
		*********/
		/// <summary>Dictionary of player-type dictionaries, which store tree name as Key, number felled as Value.</summary>
		public static TreesFelledData treesFelledDict, cachedTreesFelledDict;
		/// <summary>Internal cache of hardcoded tree types</summary>
		public static readonly Dictionary<string, string> treeTypeDict;

		/*********
		** Public methods
		*********/

		/****
		** Static Constructor
		****/

		/// <summary>
		/// Constructor needs to be static to initialize static fields properly. The dictionary which stores how trees have been felled has to be static in order to be accessed by the Harmony patch triggered upon a tree falling.
		/// </summary>
		static TreesFelledToken()
		{

			treesFelledDict = new();
			cachedTreesFelledDict = new();

			treeTypeDict = new()
			{
				["1"] = "oak",
				["2"] = "maple",
				["3"] = "pine",
				["6"] = "palm",
				["7"] = "mushroom",
				["8"] = "mahogany",
				//[9] = "palm2"
			};

			HarmonyPatches.TreeFelledPatch();
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

				if (!args[1].Contains("type="))
				{
					error += "Named argument 'type' not provided. Must be a string consisting of alphanumeric characters. ";
					return false;
				}
				else if (args[1].IndexOf('=') == args[1].Length - 1)
				{
					error += "Named argument 'type' must be a string consisting of alphanumeric characters. ";
				}
			}
			else
			{
				error += "Incorrect number of arguments provided. A 'player' argument and 'type' argument should be provided. ";
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

			Dictionary<string, int> treesFelled = treesFelledDict.Value[pType];
			Dictionary<string, int> cachedTreesFelled = cachedTreesFelledDict.Value[pType];

			// check cached local player stats against Game1's local player stats
			// only needs to happen if player is local
			if (!Game1.IsMasterGame)
			{
				foreach (KeyValuePair<string, int> pair in treesFelled)
				{
					if (!cachedTreesFelled.ContainsKey(pair.Key))
					{
						hasChanged = true;
						cachedTreesFelled[pair.Key] = pair.Value;
					}
					else if (!cachedTreesFelled[pair.Key].Equals(pair.Value))
					{
						hasChanged = true;
						cachedTreesFelled[pair.Key] = pair.Value;
					}
				}
			}

			pType = host;

			// check cached master player stats against Game1's master player stats
			// needs to happen whether player is host or local
			treesFelled = treesFelledDict.Value[pType];
			cachedTreesFelled = cachedTreesFelledDict.Value[pType];

			foreach (KeyValuePair<string, int> pair in treesFelled)
			{
				if (!cachedTreesFelled.ContainsKey(pair.Key))
				{
					hasChanged = true;
					cachedTreesFelled[pair.Key] = pair.Value;
				}
				else if (!cachedTreesFelled[pair.Key].Equals(pair.Value))
				{
					hasChanged = true;
					cachedTreesFelled[pair.Key] = pair.Value;
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
			string treeType = args[1].Substring(args[1].IndexOf('=') + 1).Trim().ToLower().Replace(" ", "");

			string pType = playerType.Equals("host") ? host : loc;

			if (TryGetTreesFelled(treeType, pType, out string treeNum))
			{
				output.Add(treeNum);
			}

			return output;
		}

		/*********
		** Private methods
		*********/

		/// <summary>
		/// Initializes internal dictionary to 0.
		/// </summary>
		/// <returns>A dictionary with all tree types as keys, initialized to value 0.</returns>
		private static Dictionary<string, int> InitializeTreesFelledStats()
		{
			return new Dictionary<string, int>()
			{
				["1"] = 0,
				["2"] = 0,
				["3"] = 0,
				["6"] = 0,
				["7"] = 0,
				["8"] = 0,
				//[9] = 0
			};
		}

		/// <summary>
		/// Attempts to find the number of the specified tree type felled for the specified player type, and if located, passes the value out via <c>treeNum</c>.
		/// </summary>
		/// <param name="treeNameOrId"></param>
		/// <param name="playerType"></param>
		/// <param name="treeNum"></param>
		/// <returns></returns>
		private bool TryGetTreesFelled(string treeNameOrId, string playerType, out string treeNum)
		{
			string pType = playerType;
			string treeTypeNum = "";
			treeNum = "";

			bool found = false;
			bool isNumericId = int.TryParse(treeNameOrId, out _);

			// string passed in is not a number - try matching with tree type dict to find type number
			if (!isNumericId)
			{
				// "any" is special case - otherwise, try to match
				if (!treeNameOrId.Equals("any"))
				{
					string treeName = treeNameOrId.Trim().Replace(" ", "").Replace("2", "").ToLower() ?? treeNameOrId;

					// logging
					Globals.Monitor.Log($"Parsed 'type' value {treeNameOrId} to {treeName}");

					foreach (KeyValuePair<string, string> pair in treeTypeDict)
					{
						if (pair.Value.Equals(treeName))
						{
							treeTypeNum = pair.Key.ToString();
							break;
						}
					}
				}
			}
			else
			{
				treeTypeNum = treeNameOrId;
			}

			if (playerType.Equals(loc) && Game1.IsMasterGame)
			{
				pType = host;
			}

			if (pType.Equals(host) || pType.Equals(loc))
			{
				if (treeNameOrId.Equals("any"))
				{
					found = true;
					treeNum = cachedTreesFelledDict.Value[pType].Values.Sum().ToString();
				}
				else
				{
					foreach (string key in cachedTreesFelledDict.Value[pType].Keys)
					{
						if (key.Equals(treeTypeNum))
						{
							found = true;
							treeNum = cachedTreesFelledDict.Value[pType][key].ToString();
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
		public class TreesFelledData
		{
			public Dictionary<string, Dictionary<string, int>> Value;

			public TreesFelledData()
			{
				Value = new(StringComparer.OrdinalIgnoreCase)
				{
					[host] = InitializeTreesFelledStats(),
					[loc] = InitializeTreesFelledStats()
				};
			}
		}
	}
}
