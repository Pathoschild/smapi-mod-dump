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
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.TokenizableStrings;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// Randomizes the birthdays of all NPCs
    /// </summary>
    public class BirthdayRandomizer
	{
		private static RNG Rng { get; set; }

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
		public static Dictionary<string, CharacterData> Randomize()
		{
            Dictionary<string, CharacterData> replacements = new();
            if (!Globals.Config.NPCs.RandomizeBirthdays) 
			{ 
				return replacements; 
			}

			Globals.SpoilerWrite("===== NPC BIRTHDAYS =====");

			Rng = RNG.GetFarmRNG(nameof(BirthdayRandomizer));
			Dictionary<string, CharacterData> characterData = DataLoader.Characters(Game1.content);
			List<SDate> assignableBirthdays = GetAssignableBirthdayList();

			foreach (KeyValuePair<string, CharacterData> dispositionData in characterData)
			{
				string npcName = dispositionData.Key;
				CharacterData npc = dispositionData.Value;

				// Don't replace a non-existant birthday
				if (npc.BirthSeason == null)
				{
					continue;
				}

                AddRandomBirthdayToNPC(npcName, npc, assignableBirthdays);
				replacements.Add(npcName, npc);
			}

			Globals.SpoilerWrite("");

			return replacements;
		}

		/// <summary>
		/// Gets a random Stardew Valley date - excludes the holidays and birthdays already in use
		/// </summary>
		/// <param name="npcName">The name of the NPC - used as the key</param>
		/// <param name="npc">The npc to modify</param>
		private static void AddRandomBirthdayToNPC(
            string npcName,
            CharacterData npc,
			List<SDate> assignableBirthdays)
		{
			if (npcName == "Wizard")
			{
				SetWizardBirthday(npc);
				return;
			}

			if (!assignableBirthdays.Any())
			{
				assignableBirthdays = GetAssignableBirthdayList();
				Globals.ConsoleWarn("There are more NPCs than there are allowable days in the year - note that some days will have multiple birthdays!");
			}

			SDate randomBirthday = Rng.GetAndRemoveRandomValueFromList(assignableBirthdays);
			SetNpcBirthday(npc, randomBirthday);
		}

		/// <summary>
		/// Gets the entire list of days that a birthday can take place, excluding holidays
		/// </summary>
		/// <returns>The fresh list of birthdays</returns>
		private static List<SDate> GetAssignableBirthdayList()
		{
			const int DaysInMonth = 28;
			List<string> seasonStrings = new() { "spring", "summer", "fall", "winter" };
			List<SDate> allDays = new();

			foreach (string season in seasonStrings) 
			{
				for (int i = 1; i < DaysInMonth + 1; i++)
				{
					allDays.Add(new SDate(i, season));
				}
			}

			return allDays.Where(day => !Holidays.Contains(day)).ToList();
		}

		/// <summary>
		/// Sets the wizard's birthday - must be from the 15-17, as the game hard-codes the "Night Market" text
		/// to the billboard
		/// </summary>
		/// <param name="wizard">The wizard to modify</param>
		/// <returns />
		private static void SetWizardBirthday(CharacterData wizard)
		{
			int day = Rng.NextIntWithinRange(15, 17);
			SDate wizardBirthday = new(day, "winter", 1);
			SetNpcBirthday(wizard, wizardBirthday);
		}

		/// <summary>
		/// Sets the birthday of the NPC to the given value
		/// </summary>
		/// <param name="npc">The NPC's character data</param>
		/// <param name="birthday">The birthday to set them to</param>
		private static void SetNpcBirthday(CharacterData npc, SDate birthday)
		{
			npc.BirthSeason = birthday.Season;
			npc.BirthDay = birthday.Day;

			Globals.SpoilerWrite($"{TokenParser.ParseText(npc.DisplayName)}: {birthday.Season} {birthday.Day}");
		}
	}
}
