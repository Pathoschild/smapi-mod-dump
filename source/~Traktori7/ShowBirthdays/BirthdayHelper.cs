/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;


namespace ShowBirthdays
{
	class BirthdayHelper
	{
		// Reference to the monitor to allow error logging
		private readonly IMonitor monitor;
		private readonly IGameContentHelper contentHelper;
		private readonly IModRegistry modRegistry;
		public List<Birthday>[] birthdays = new List<Birthday>[4];


		public BirthdayHelper(IMonitor m, IModRegistry mr, IGameContentHelper helper)
		{
			monitor = m;
			contentHelper = helper;
			modRegistry = mr;

			// Initialize the array of lists
			for (int i = 0; i < birthdays.Length; i++)
			{
				birthdays[i] = new List<Birthday>();
			}
		}


		internal void RecheckBirthdays()
		{
			// Reset the birthday lists
			Reset();

			// Load Custom NPC Exclusions exclusion rules if they exists
			bool exclusionRulesFound = false;
			Dictionary<string, string> exclusionRules = new Dictionary<string, string>();

			if (modRegistry.IsLoaded("Esca.CustomNPCExclusions"))
			{
				IModInfo modInfo = modRegistry.Get("Esca.CustomNPCExclusions")!;

				// Is the version new enough to contain the calendar exclusion
				if (modInfo.Manifest.Version.CompareTo(new SemanticVersion("1.4.0")) >= 0)
				{
					monitor.Log("Custom NPC Exclusions 1.4.0 or newer found.");

					try
					{
						exclusionRules = contentHelper.Load<Dictionary<string, string>>("Data/CustomNPCExclusions");
						exclusionRulesFound = true;
					}
					catch (Exception e)
					{
						monitor.Log("Loading Custom NPC Exclusion rules failed.", LogLevel.Error);
						monitor.Log(e.ToString(), LogLevel.Error);
					}
				}
			}

			// Loop through all of the NPCs, filter out characters that don't have a proper birthday
			// TODO: Switch to HarmonyPatch_BirthdayCalendar.IncludeBirthday in custom npc exclusions instead of straight from the exclusions rules?
			foreach (NPC n in Utility.getAllCharacters())
			{
				// Checking for 0 should eliminate a lot of the non-friendable NPCs, needs verification
				if (n.isVillager() && n.Birthday_Day > 0 && n.Birthday_Season is not null)
				{
					// It returns 1-4 for the base game seasons, and -1 if there were no matches
					if (Utility.getSeasonNumber(n.Birthday_Season) == -1)
					{
						monitor.Log($"Encountered an unexpected season for birthday {n.Birthday_Season} {n.Birthday_Day} for {n.Name}", LogLevel.Debug);
						continue;
					}

					bool hideBirthday = false;

					// Was Custom NPC Exclusions found
					if (exclusionRulesFound)
					{
						// Try if the NPC's name is in the rules
						if (exclusionRules.TryGetValue(n.Name, out string? s1))
						{
							// Entry found, split it into the different rules
							string[] rules = s1.Split(' ', ',', '/');

							for (int i = 0; i < rules.Length; i++)
							{
								// Check if it contains 'All', 'Calendar' or 'TownEvent'
								if (rules[i].Equals("All", StringComparison.OrdinalIgnoreCase)
									|| rules[i].Equals("Calendar", StringComparison.OrdinalIgnoreCase)
									|| rules[i].Equals("TownEvent", StringComparison.OrdinalIgnoreCase))
								{
									monitor.Log($"Custom NPC Exclusions wants to hide {n.Name} from the calendar. Complying...");
									hideBirthday = true;
									break;
								}
							}
						}
					}

					// This check needs further testing, especially with custom npcs
					if (!n.CanSocialize && !Game1.player.friendshipData.ContainsKey(n.Name))
					{
						hideBirthday = true;
					}

					if (!hideBirthday)
					{
						AddBirthday(n.Birthday_Season, n.Birthday_Day, n);
					}
					else
					{
						monitor.Log($"NPC: {n.Name} Birthday: {n.Birthday_Season} {n.Birthday_Day} was hidden from the calendar.");
					}
				}
			}
		}


		private void Reset()
		{
			// Reinitialize the array of lists
			for (int i = 0; i < birthdays.Length; i++)
			{
				birthdays[i] = new List<Birthday>();
			}
		}


		/// <summary>
		/// Adds birthday and the NPC to the correct list
		/// </summary>
		internal void AddBirthday(string season, int birthday, NPC n)
		{
			List<Birthday>? list = GetListOfBirthdays(season);

			if (list is null)
			{
				monitor.Log($"Failed to add birthday {season} {birthday} for {n.Name}", LogLevel.Error);
				return;
			}

			// null if birthday hasn't been added
			Birthday? day = list.Find(x => x.day == birthday);

			if (day is null)
			{
				// Add the birthday
				Birthday newDay = new Birthday(monitor, birthday, contentHelper);
				newDay.AddNPC(n);
				list.Add(newDay);
			}
			else
			{
				// Add the npc to the existing day
				day.AddNPC(n);
			}
		}


		/// <summary>
		/// Returns the list of birthday days for the season
		/// </summary>
		/// <param name="season">Wanted season</param>
		/// <param name="onlyShared">Return only shared birthday days</param>
		/// <returns></returns>
		internal List<int> GetDays(string season, bool onlyShared = false)
		{
			List<Birthday>? list = GetListOfBirthdays(season);

			List<int> days = new List<int>();

			if (list is not null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (!onlyShared || list[i].GetNPCs().Count > 1)
						days.Add(list[i].day);
				}
			}

			return days;
		}


		/// <summary>
		/// Returns the list of NPCs that have the given birthday. Returns null if no NPC matches the date.
		/// </summary>
		internal List<NPC>? GetNpcs(string season, int day)
		{
			Birthday? birthday = GetBirthday(season, day);

			if (birthday is null)
				return null;
			else
				return birthday.GetNPCs();
		}


		/// <summary>
		/// Returns the birthday object if it exists
		/// </summary>
		private Birthday? GetBirthday(string season, int day)
		{
			List<Birthday>? list = GetListOfBirthdays(season);

			if (list is null)
				return null;

			return list.Find(x => x.day == day);
		}


		/// <summary>
		/// Returns the list of Birthdays for the given season. Returns null if such list was not found.
		/// </summary>
		private List<Birthday>? GetListOfBirthdays(string season)
		{
			int index = Utility.getSeasonNumber(season);

			if (index >= 0 && index < birthdays.Length)
			{
				return birthdays[index];
			}
			else
			{
				monitor.Log($"Tried to get the list of birthdays for an unknown season {season}.", LogLevel.Error);
				return null;
			}
		}


		/// <summary>
		/// Returns the NPC for the day
		/// </summary>
		/// <param name="season">Season</param>
		/// <param name="day">Day</param>
		/// <param name="nextInCycle">Get next available sprite</param>
		internal Texture2D? GetSprite(string season, int day, bool nextInCycle)
		{
			Birthday? birthday = GetBirthday(season, day);

			if (birthday is null)
				return null;

			return birthday.GetSprite(nextInCycle);
		}
	}
}
