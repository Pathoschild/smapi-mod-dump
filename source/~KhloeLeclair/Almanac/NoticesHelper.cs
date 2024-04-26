/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;
using StardewValley.Locations;

namespace Leclair.Stardew.Almanac;

public static class NoticesHelper {

	public static bool CanVisitIsland(NPC who, WorldDate date) {
		if (who is null)
			return false;

		if (date is null)
			return IslandSouth.CanVisitIslandToday(who);

		int oldDay = Game1.dayOfMonth;
		string oldSeason = Game1.currentSeason;

		bool result;

		try {
			Game1.dayOfMonth = date.DayOfMonth;
			Game1.currentSeason = date.SeasonKey;

			result = IslandSouth.CanVisitIslandToday(who);

		} finally {
			Game1.dayOfMonth = oldDay;
			Game1.currentSeason = oldSeason;
		}

		return result;
	}

	public static List<string>? GetIslandVisitors(IList<NPC>? chars, WorldDate date) {

		if (Utility.isFestivalDay(date.DayOfMonth, date.Season))
			return null;

		if (date.SeasonKey == "winter" && date.DayOfMonth >= 15 && date.DayOfMonth <= 17)
			return null;

		if (Game1.getLocationFromName("IslandSouth") is not IslandSouth isle)
			return null;

		//if (!isle.resortRestored.Value)
		//	return null;

		if (chars is null)
			return null;

		// TODO: Island weather check.
		// TODO: resortOpenToday?

		Random rnd = new Random((int) (Game1.uniqueIDForThisGame * 1.21f) + (int) ((date.TotalDays + 1) * 2.5f));
		List<string> valid = new();
		List<string> young = new();

		int oldDay = Game1.dayOfMonth;
		string oldSeason = Game1.currentSeason;

		try {
			Game1.dayOfMonth = date.DayOfMonth;
			Game1.currentSeason = date.SeasonKey;

			foreach (NPC npc in chars) {
				if (IslandSouth.CanVisitIslandToday(npc)) {
					//if (CanVisitIsland(npc, date))
					valid.Add(npc.Name);
					if (npc.Age == 2)
						young.Add(npc.Name);
				}

			}

		} finally {
			Game1.dayOfMonth = oldDay;
			Game1.currentSeason = oldSeason;
		}

		List<string> visitors = new();
		if (rnd.NextDouble() < 0.4) {
			for (int i = 0; i < 5; i++) {
				string who = valid[rnd.Next(valid.Count)];
				if (who != null && ! young.Contains(who)) { 
					valid.Remove(who);
					visitors.Add(who);
				}
			}
		} else {
			string[][] groups = new string[][] {
				new string[] { "Sebastian", "Sam", "Abigail" },
				new string[] { "Jodi", "Kent", "Vincent", "Sam" },
				new string[] { "Jodi", "Vincent", "Sam" },
				new string[] { "Pierre", "Caroline", "Abigail" },
				new string[] { "Robin", "Demetrius", "Maru", "Sebastian" },
				new string[] { "Lewis", "Marnie" },
				new string[] { "Marnie", "Shane", "Jas" },
				new string[] { "Penny", "Jas", "Vincent" },
				new string[] { "Pam", "Penny" },
				new string[] { "Caroline", "Marnie", "Robin", "Jodi" },
				new string[] { "Haley", "Penny", "Leah", "Emily", "Maru", "Abigail" },
				new string[] { "Alex", "Sam", "Sebastian", "Elliott", "Shane", "Harvey" },
			};

			string[] group = groups[rnd.Next(groups.Length)];

			bool failed = false;
			foreach(string who in group) {
				if (!valid.Contains(who)) {
					failed = true;
					break;
				}
			}

			if (!failed) {
				foreach(string who in group) {
					valid.Remove(who);
					visitors.Add(who);
				}
			}

			for(int i = 0; i < 5 - visitors.Count; i++) {
				string who = valid[rnd.Next(valid.Count)];
				if (who != null && ! young.Contains(who)) { 
					valid.Remove(who);
					visitors.Add(who);
				}
			}
		}

		return visitors;
	}

}
