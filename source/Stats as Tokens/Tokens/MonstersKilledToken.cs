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
	internal class MonstersKilledToken : BaseToken
	{
		/*********
		** Fields
		*********/
		/// <summary>The game stats as of the last context update. No need to cache data since game stores it by monster.</summary>
		private readonly Dictionary<string, SerializableDictionary<string, int>> monsterStatsDict;

		/*********
		** Constructor
		*********/
		public MonstersKilledToken()
		{
			monsterStatsDict = new(StringComparer.OrdinalIgnoreCase)
			{
				[host] = InitializeMonstersKilledStats(),
				[loc] = InitializeMonstersKilledStats()
			};
		}

		/*********
		** Public methods
		*********/

		/****
		** Metadata
		****/

		public override bool TryValidateInput(string input, out string error)
		{
			error = "";
			string[] args = input.ToLower().Trim().Split('|');

			if (args.Length == 2)
			{
				if (!args[0].Contains("player="))
				{
					error += "Named argument 'player' not provided. ";
				}
				else if (args[0].IndexOf('=') == args[0].Length - 1)
				{
					error += "Named argument 'player' not provided a value. Must be one of the following values: 'hostPlayer', 'localPlayer'. ";
				}
				else
				{
					// accept hostplayer or host, localplayer or local
					string playerType = args[0].Substring(args[0].IndexOf('=') + 1).Trim();
					if (!(playerType.Equals(host) || playerType.Equals(loc)))
					{
						error += "Named argument 'player' must be one of the following values: 'hostPlayer', 'localPlayer'. ";
					}
				}

				if (!args[1].Contains("monster="))
				{
					error += "Named argument 'monster' not provided. ";
					return false;
				}
				else if (args[1].IndexOf('=') == args[1].Length - 1)
				{
					error += "Named argument 'monster' must be a string consisting of alphanumeric characters. ";
				}
			}
			else
			{
				error += "Incorrect number of arguments provided. A 'player' argument and 'monster' argument should be provided. ";
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

			SerializableDictionary<string, int> monStats = Game1.stats.specificMonstersKilled;
			SerializableDictionary<string, int> cachedMonStats = monsterStatsDict[pType];

			// check cached local player stats against Game1's local player stats
			// only needs to happen if player is local
			if (!Game1.IsMasterGame)
			{
				foreach (KeyValuePair<string, int> pair in monStats)
				{
					if (!cachedMonStats.ContainsKey(pair.Key))
					{
						hasChanged = true;
						cachedMonStats[pair.Key] = pair.Value;
					}
					else if (!cachedMonStats[pair.Key].Equals(pair.Value))
					{
						hasChanged = true;
						cachedMonStats[pair.Key] = pair.Value;
					}
				}
			}

			// check Game1's local player stats against cached data and reset cached data to 0 if stat is not present in the Game1 stats 
			if (!Game1.IsMasterGame)
			{
				foreach (KeyValuePair<string, int> pair in cachedMonStats)
				{
					if (!monStats.ContainsKey(pair.Key))
					{
						hasChanged = true;
						cachedMonStats[pair.Key] = 0;
					}
				}
			}

			pType = host;

			// check cached master player stats against Game1's master player stats
			// needs to happen whether player is host or local
			monStats = Game1.MasterPlayer.stats.specificMonstersKilled;
			cachedMonStats = monsterStatsDict[pType];

			foreach (KeyValuePair<string, int> pair in monStats)
			{
				if (!cachedMonStats.ContainsKey(pair.Key))
				{
					hasChanged = true;
					cachedMonStats[pair.Key] = pair.Value;
				}
				else if (!cachedMonStats[pair.Key].Equals(pair.Value))
				{
					hasChanged = true;
					cachedMonStats[pair.Key] = pair.Value;
				}
			}


			// check Game1's master player stats against cached data and reset cached data to 0 if stat is not present in the Game1 stats 
			foreach (KeyValuePair<string, int> pair in cachedMonStats)
			{
				if (!monStats.ContainsKey(pair.Key))
				{
					hasChanged = true;
					cachedMonStats[pair.Key] = 0;
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

			string playerType = args[0].Substring(args[0].IndexOf('=') + 1).Trim().ToLower().Replace(" ", "");
			string monster = args[1].Substring(args[1].IndexOf('=') + 1).Trim().ToLower().Replace(" ", "");

			if (playerType.Equals(host))
			{
				bool found = TryGetMonsterStat(monster, host, out string monsterNum);

				if (found)
				{
					output.Add(monsterNum);
				}
			}
			else
			{
				bool found = TryGetMonsterStat(monster, loc, out string monsterNum);

				if (found)
				{
					output.Add(monsterNum);
				}
			}

			return output;
		}
		/*********
		** Private methods
		*********/

		/// <summary>
		/// Initialize and return dictionary with all monsters set to 0 kills. Theoretically supports custom monsters if they are added to Data/Monsters.
		/// </summary>
		/// <returns>A dictionary containing all monster names as keys with value 0.</returns>
		private static SerializableDictionary<string, int> InitializeMonstersKilledStats()
		{
			SerializableDictionary<string, int> monstersKilled = new();
			Dictionary<string, string> monsterData = Globals.Helper.GameContent.Load<Dictionary<string, string>>("Data/Monsters");

			foreach (KeyValuePair<string, string> monster in monsterData)
			{
				monstersKilled[monster.Key] = 0;
			}

			return monstersKilled;
		}

		/// <summary>
		/// Attempts to find the number of specified monsters killed for the specified player type, and if located, passes the value out via <c>monsterNum</c>.
		/// </summary>
		/// <param name="monsterName">The monster to check kills of.</param>
		/// <param name="playerType">The player type to check - host or local.</param>
		/// <param name="monsterNum">The string to pass the value to if located.</param>
		/// <returns><c>True</c> if located, <c>False</c> otherwise.</returns>
		private bool TryGetMonsterStat(string monsterName, string playerType, out string monsterNum)
		{
			bool found = false;
			monsterNum = "";

			if (playerType.Equals(loc) && Game1.IsMasterGame)
			{
				playerType = host;
			}

			if (playerType.Equals(host) || playerType.Equals(loc))
			{
				foreach (string key in monsterStatsDict[playerType].Keys)
				{
					if (key.ToLower().Replace(" ", "").Equals(monsterName))
					{
						found = true;
						monsterNum = monsterStatsDict[playerType][key].ToString();
					}
				}
			}

			return found;
		}

	}
}
