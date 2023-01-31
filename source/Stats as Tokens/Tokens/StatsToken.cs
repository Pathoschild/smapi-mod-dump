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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StatsAsTokens
{
	internal class StatsToken : BaseToken
	{
		/*********
		** Fields
		*********/

		/// <summary>The game stats as of the last context update.</summary>
		private readonly Dictionary<string, Stats> statsDict = new();
		/// <summary>Array of public fields in the type StardewValley.Stats.</summary>
		private readonly FieldInfo[] statFields;

		private bool statsInitialized = false;

		/*********
		** Constructor
		*********/

		public StatsToken()
		{
			statFields = typeof(Stats).GetFields();
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
					// accepts only hostPlayer or localPlayer
					string playerType = args[0].Substring(args[0].IndexOf('=') + 1).Trim();
					if (!(playerType.Equals(host) || playerType.Equals(loc)))
					{
						error += "Named argument 'player' must be one of the following values: 'hostPlayer', 'localPlayer'. ";
					}
				}

				if (!args[1].Contains("stat="))
				{
					error += "Named argument 'stat' not provided. ";
					return false;
				}
				else if (args[1].IndexOf('=') == args[1].Length - 1)
				{
					error += "Named argument 'stat' must be a string consisting of alphanumeric values. ";
				}
				else
				{
					string statArg = args[1].Substring(args[1].IndexOf('=') + 1);
					if (statArg.Any(ch => !char.IsLetterOrDigit(ch) && ch != ' '))
					{
						error += "Only alphanumeric values may be provided to 'stat' argument. ";
					}
				}
			}
			else
			{
				error += "Incorrect number of arguments provided. A 'player' argument and 'stat' argument should be provided. ";
			}

			return error.Equals("");
		}

		/****
		** State
		****/

		protected override bool DidStatsChange()
		{
			bool hasChanged = false;

			string pType;

			// on the very first context update, need to initialize statsDict and any non-initialized stats
			if (!statsInitialized)
			{
				Game1.player.stats = InitializeOtherStatFields(Game1.player.stats);

				// it's fine if these resolve to the same player
				statsDict[host] = Game1.MasterPlayer.stats;
				statsDict[loc] = Game1.player.stats;

				statsInitialized = true;
			}

			// check cached local player stats against Game1's local player stats
			// only needs to happen if player is local and not master
			if (!Game1.IsMasterGame)
			{
				pType = loc;

				foreach (FieldInfo field in statFields)
				{
					if (field.FieldType.Equals(typeof(uint)))
					{
						if (!field.GetValue(Game1.stats).Equals(field.GetValue(statsDict[pType])))
						{
							hasChanged = true;
							field.SetValue(statsDict[pType], field.GetValue(Game1.stats));
						}
					}
					else if (field.FieldType.Equals(typeof(SerializableDictionary<string, uint>)))
					{
						SerializableDictionary<string, uint> otherStats = (SerializableDictionary<string, uint>)field.GetValue(Game1.stats);
						SerializableDictionary<string, uint> cachedOtherStats = statsDict[pType].stat_dictionary;

						foreach (KeyValuePair<string, uint> pair in otherStats)
						{
							if (!cachedOtherStats.ContainsKey(pair.Key))
							{
								hasChanged = true;
								cachedOtherStats[pair.Key] = pair.Value;
							}
							else if (!cachedOtherStats[pair.Key].Equals(pair.Value))
							{
								hasChanged = true;
								cachedOtherStats[pair.Key] = pair.Value;
							}
						}
					}
				}
			}

			pType = host;

			// check cached master player stats against Game1's master player stats
			// needs to happen whether player is host or local
			foreach (FieldInfo field in statFields)
			{
				if (field.FieldType.Equals(typeof(uint)))
				{
					if (!field.GetValue(Game1.MasterPlayer.stats).Equals(field.GetValue(statsDict[pType])))
					{
						hasChanged = true;
						field.SetValue(statsDict[pType], field.GetValue(Game1.MasterPlayer.stats));
					}
				}
				else if (field.FieldType.Equals(typeof(SerializableDictionary<string, uint>)))
				{
					SerializableDictionary<string, uint> otherStats = (SerializableDictionary<string, uint>)field.GetValue(Game1.MasterPlayer.stats);
					SerializableDictionary<string, uint> cachedOtherStats = statsDict[pType].stat_dictionary;

					foreach (KeyValuePair<string, uint> pair in otherStats)
					{
						if (!cachedOtherStats.ContainsKey(pair.Key))
						{
							hasChanged = true;
							cachedOtherStats[pair.Key] = pair.Value;
						}
						else if (!cachedOtherStats[pair.Key].Equals(pair.Value))
						{
							hasChanged = true;
							cachedOtherStats[pair.Key] = pair.Value;
						}
					}
				}
			}

			return hasChanged;
		}

		public override IEnumerable<string> GetValues(string input)
		{
			List<string> output = new();

			string[] args = input.Split('|');

			string playerType = args[0].Substring(args[0].IndexOf('=') + 1).Trim().ToLower().Replace(" ", "");
			string stat = args[1].Substring(args[1].IndexOf('=') + 1).Trim().ToLower().Replace(" ", "");

			if (playerType.Equals(host))
			{
				bool found = TryGetField(stat, host, out string hostStat);

				if (found)
				{
					output.Add(hostStat);
				}
			}
			else if (playerType.Equals(loc))
			{
				bool found = TryGetField(stat, loc, out string hostStat);

				if (found)
				{
					output.Add(hostStat);
				}
			}

			return output;
		}

		/*********
		** Private methods
		*********/

		/// <summary>
		/// Initializes stat fields for internal stat dictionary. These stats are not fields in the <c>Stats</c> object and so do not show up normally until they have been incremented at least once.
		/// </summary>
		/// <param name="stats">The <c>Stats</c> object to initialize the internal stat dictionary of.</param>
		private static Stats InitializeOtherStatFields(Stats stats)
		{
			List<string> otherStats = new()
			{
				"timesEnchanted",
				"beachFarmSpawns",
				"childrenTurnedToDoves",
				"boatRidesToIsland",
				"hardModeMonstersKilled",
				"trashCansChecked",
				"ostrichEggsLayed",
				"dinosaurEggsLayed",
				"rabbitsFeetDropped",
				"duckFeathersDropped",
				"mayonnaiseMade",
				"duckMayonnaiseMade",
				"voidMayonnaiseMade",
				"dinosaurMayonnaiseMade",
				"treesPlanted"
			};

			foreach (string stat in otherStats)
			{
				if (!stats.stat_dictionary.ContainsKey(stat))
				{
					stats.stat_dictionary[stat] = 0;
				}
			}

			return stats;
		}

		/// <summary>
		/// Attempts to find the specified stat field for the specified player type, and if located, passes the value out via <c>foundStat</c>.
		/// </summary>
		/// <param name="statField">The stat to look for</param>
		/// <param name="playerType">The player type to check - hostPlayer or localPlayer</param>
		/// <param name="foundStat">The string to pass the value to if located.</param>
		/// <returns><c>True</c> if located, <c>False</c> otherwise.</returns>
		private bool TryGetField(string statField, string playerType, out string foundStat)
		{
			bool found = false;
			foundStat = "";

			if (playerType.Equals(loc) && Game1.IsMasterGame)
			{
				playerType = host;
			}

			foreach (FieldInfo field in statFields)
			{
				if (field.Name.ToLower().Equals(statField))
				{
					found = true;
					foundStat = field.GetValue(statsDict[playerType]).ToString();

#if DEBUG
					Globals.Monitor.Log($"Matched {statField} to {field.Name}");
					Globals.Monitor.Log($"Expected value: {field.GetValue(Game1.stats)}");
					Globals.Monitor.Log($"Actual value: {foundStat}");
#endif
				}
			}

			if (found) return true;

			foreach (string key in statsDict[playerType].stat_dictionary.Keys)
			{
				if (key.ToLower().Replace(" ", "").Equals(statField))
				{
					found = true;
					foundStat = statsDict[playerType].stat_dictionary[key].ToString();

#if DEBUG
						Globals.Monitor.Log($"Matched {statField} to {key}");
						Globals.Monitor.Log($"Expected value: {Game1.stats.stat_dictionary[key]}");
						Globals.Monitor.Log($"Actual value: {foundStat}");
#endif
				}
			}

			return found;
		}
	}
}
