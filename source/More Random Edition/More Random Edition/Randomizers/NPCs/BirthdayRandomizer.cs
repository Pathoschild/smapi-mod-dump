/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Randomizes the birthdays of all NPCs
	/// </summary>
	public class BirthdayRandomizer
	{
		/// <summary>
		/// Holidays - don't assign birthdays to these days!
		/// </summary>
		private readonly static List<SDate> Holidays = new()
		{
			new(13, "spring", 1),
			new(24, "spring", 1),
			new(11, "summer", 1),
			new(16, "fall", 1),
			new(27, "fall", 1),
			new(8, "winter", 1),
			new(25, "winter", 1),
			new(15, "winter", 1),
			new(16, "winter", 1),
			new(17, "winter", 1)
		};

		/// <summary>
		/// The string to use for holidays in the birthdays in use list
		/// </summary>
		private const string HolidayString = "HOLIDAY";

		/// <summary>
		/// Does the birthday randomization
		/// </summary>
		/// <returns>The dictionary to use for replacements</returns>
		public static Dictionary<string, string> Randomize()
		{
			Dictionary<SDate, string> birthdaysInUse = InitBirthdaysInUse();
			Dictionary<string, string> replacements = new();
			Dictionary<string, string> npcDispositionData = Globals.ModRef.Helper.GameContent
                .Load<Dictionary<string, string>>("Data/NPCDispositions");

            foreach (KeyValuePair<string, string> dispositionData in npcDispositionData)
			{
				string npcName = dispositionData.Key;
				string[] data = dispositionData.Value.Split('/');

				// Don't replace a non-existant birthday
				if (string.IsNullOrWhiteSpace(data[(int)NPCDispositionIndexes.Birthday]))
				{
					continue;
				}

                SDate addedDate = AddRandomBirthdayToNPC(birthdaysInUse, npcName);
                data[(int)NPCDispositionIndexes.Birthday] = $"{addedDate.Season} {addedDate.Day}";

				replacements.Add(npcName, string.Join("/", data));
			}

			WriteToSpoilerLog(birthdaysInUse);
			return replacements;
		}

		/// <summary>
		/// Initializes the birthdays in use - adds all the holidays to it so that they can't
		/// be picked
		/// </summary>
		/// <returns />
		private static Dictionary<SDate, string> InitBirthdaysInUse()
		{
			Dictionary<SDate, string> birthdaysInUse = new();
			foreach (SDate holidayDate in Holidays)
			{
				birthdaysInUse.Add(holidayDate, HolidayString);
			}
			return birthdaysInUse;
		}

		/// <summary>
		/// Gets a random Stardew Valley date - excludes the holidays and birthdays already in use
		/// </summary>
		/// <param name="birthdaysInUse">The birthdays in use - this function adds the date to it</param>
		/// <returns>The date added</returns>
		private static SDate AddRandomBirthdayToNPC(Dictionary<SDate, string> birthdaysInUse, string npcName)
		{
			if (npcName == "Wizard")
			{
				return GetWizardBirthday(birthdaysInUse);
			}

			List<string> seasonStrings = new() { "spring", "summer", "fall", "winter" };
			string season = Globals.RNGGetRandomValueFromList(seasonStrings);
			bool dateRetrieved = false;
			SDate date;

			do
			{
				date = new SDate(Range.GetRandomValue(1, 28), season, 1);
				if (!birthdaysInUse.ContainsKey(date))
				{
					birthdaysInUse.Add(date, npcName);
					dateRetrieved = true;
				}
			} while (!dateRetrieved);

			return date;
		}

		/// <summary>
		/// Gets the wizard's birthday - must be from the 15-17, as the game hard-codes the "Night Market" text
		/// to the billboard
		/// </summary>
		/// <param name="birthdaysInUse">The birthdays in use - this function adds the date to it</param>
		/// <returns />
		private static SDate GetWizardBirthday(Dictionary<SDate, string> birthdaysInUse)
		{
			int day = Range.GetRandomValue(15, 17);
			SDate wizardBirthday = new(day, "winter", 1);

			birthdaysInUse.Remove(wizardBirthday);
			birthdaysInUse.Add(wizardBirthday, "Wizard");
			return wizardBirthday;
		}

		/// <summary>
		/// Write to the spoiler log
		/// </summary>
		/// <param name="replacements">The replacements made - need to filter out the "HOLIDAY" entries</param>
		private static void WriteToSpoilerLog(Dictionary<SDate, string> replacements)
		{
			if (!Globals.Config.NPCs.RandomizeBirthdays) { return; }

			Globals.SpoilerWrite("===== NPC BIRTHDAYS =====");
			foreach (SDate date in replacements.Keys)
			{
				string npcName = replacements[date];
				if (npcName == HolidayString) { continue; }

				Globals.SpoilerWrite($"{npcName}: {date.Season} {date.Day}");
			}
			Globals.SpoilerWrite("");
		}
	}
}
