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
		/// The default data
		/// </summary>
		private readonly static List<string> DefaultNPCDispositionData = new List<string>
		{
			"teen/rude/outgoing/neutral/female/datable/Sebastian/Town/fall 13/Caroline 'mom' Pierre 'dad'/SeedShop 1 9/Abigail",
			"adult/polite/neutral/neutral/female/not-datable/Pierre/Town/winter 7/Pierre 'husband' Abigail ''/SeedShop 22 5/Caroline",
			"adult/rude/shy/negative/male/not-datable/Emily/Town/winter 26/Emily ''/Blacksmith 3 13/Clint",
			"adult/polite/neutral/positive/male/not-datable/Robin/Town/summer 19/Robin 'wife' Maru ''/ScienceHouse 19 4/Demetrius",
			"adult/neutral/neutral/neutral/male/not-datable/null/Town/summer 24/Elliott ''/FishShop 5 4/Willy",
			"adult/polite/neutral/neutral/male/datable/Leah/Town/fall 5/Willy ''/ElliottHouse 1 5/Elliott",
			"adult/polite/outgoing/positive/female/datable/null/Town/spring 27/Haley 'sister'/HaleyHouse 16 5/Emily",
			"adult/polite/outgoing/positive/female/not-datable/George/Town/winter 20/George 'husband' Alex 'grandson'/JoshHouse 2 17/Evelyn",
			"adult/rude/neutral/negative/male/not-datable/Evelyn/Town/fall 24/Evelyn 'wife' Alex 'grandson'/JoshHouse 16 22/George",
			"adult/neutral/outgoing/positive/male/not-datable/null/Town/summer 8/Emily '' Pam ''/Saloon 18 6/Gus",
			"adult/rude/outgoing/neutral/female/datable/Alex/Town/spring 14/Emily 'sister'/HaleyHouse 8 7/Haley",
			"adult/polite/shy/positive/male/datable/Maru/Town/winter 14//HarveyRoom 13 4/Harvey",
			"child/neutral/shy/positive/female/not-datable/Vincent/Town/summer 4/Vincent ''/AnimalShop 4 6/Jas",
			"adult/polite/neutral/neutral/female/not-datable/Kent/Town/fall 11/Sam 'eldest_son' Vincent 'youngest_son' Kent 'husband'/SamHouse 4 5/Jodi",
			"adult/rude/outgoing/positive/male/datable/Haley/Town/summer 13/George 'grandpa' Evelyn 'grandma'/JoshHouse 19 5/Alex",
			"adult/neutral/shy/negative/male/not-datable/Jodi/Town/spring 4/Jodi 'wife' Sam 'eldest_son' Vincent 'youngest_boy'/SamHouse 22 5/Kent",
			"adult/polite/neutral/positive/female/datable/Elliott/Town/winter 23//LeahHouse 3 7/Leah",
			"adult/neutral/outgoing/positive/male/not-datable/null/Town/spring 7/Marnie ''/ManorHouse 8 5/Lewis",
			"adult/neutral/shy/positive/male/not-datable/null/Town/winter 3//Tent 2 2/Linus",
			"adult/neutral/outgoing/neutral/male/not-datable/Marnie/Town///AdventureGuild 5 11/Marlon",
			"adult/polite/outgoing/positive/female/not-datable/Lewis/Town/fall 18/Lewis '' Shane 'nephew' Jas 'niece'/AnimalShop 12 14/Marnie",
			"teen/neutral/outgoing/positive/female/datable/Harvey/Town/summer 10/Robin 'mom' Demetrius 'dad' Sebastian 'half-brother'/ScienceHouse 2 4/Maru",
			"adult/rude/outgoing/negative/female/not-datable/Gus/Town/spring 18/Penny 'little_baby-girl' Gus ''/Trailer 15 4/Pam",
			"teen/polite/shy/positive/female/datable/Sam/Town/fall 2/Pam 'mother'/Trailer 4 9/Penny",
			"adult/neutral/outgoing/positive/male/not-datable/Caroline/Town/spring 26/Abigail 'daughter' Caroline 'wife'/SeedShop 4 17/Pierre",
			"adult/neutral/outgoing/positive/female/not-datable/Demetrius/Town/fall 21/Demetrius 'husband' Maru 'daughter' Sebastian 'son'/ScienceHouse 21 4/Robin",
			"teen/neutral/outgoing/positive/male/datable/Penny/Town/summer 17/Vincent 'little_brother' Jodi 'mom' Kent 'dad' Sebastian ''/SamHouse 22 13/Sam",
			"teen/rude/shy/negative/male/datable/Abigail/Town/winter 10/Robin 'mom' Maru 'half-sister' Sam ''/SebastianRoom 10 9/Sebastian",
			"adult/rude/shy/negative/male/datable/null/Town/spring 20/Marnie 'aunt'/AnimalShop 25 6/Shane",
			"child/neutral/outgoing/positive/male/not-datable/Jas/Town/spring 10/Jas ''/SamHouse 10 23/Vincent",
			"adult/rude/neutral/negative/male/not-datable/null/Other/winter 17//WizardHouse 3 17/Wizard",
			"adult/neutral/outgoing/positive/undefined/not-datable/null/Other/summer 22//Mine 43 6/Dwarf",
			"adult/neutral/outgoing/positive/female/not-datable/null/Desert/fall 15/Emily ''/SandyHouse 2 5/Sandy",
			"adult/polite/shy/neutral/male/secret/null/Other/winter 1//Sewer 31 17/Krobus"

		};

		/// <summary>
		/// Holidays - don't assign birthdays to these days!
		/// </summary>
		private readonly static List<SDate> Holidays = new List<SDate>
		{
			new SDate(13, "spring", 1),
			new SDate(24, "spring", 1),
			new SDate(11, "summer", 1),
			new SDate(16, "fall", 1),
			new SDate(27, "fall", 1),
			new SDate(8, "winter", 1),
			new SDate(25, "winter", 1),
			new SDate(15, "winter", 1),
			new SDate(16, "winter", 1),
			new SDate(17, "winter", 1)
		};

		/// <summary>
		/// The string to use for holidays in the birthdays in use list
		/// </summary>
		private const string HolidayString = "HOLIDAY";

		private const int BirthdayIndex = 8;
		private const int RelationshipsIndex = 9;

		/// <summary>
		/// Does the birthday randomization
		/// </summary>
		/// <returns>The dictionary to use for replacements</returns>
		public static Dictionary<string, string> Randomize()
		{
			Dictionary<SDate, string> birthdaysInUse = InitBirthdaysInUse();
			Dictionary<string, string> replacements = new Dictionary<string, string>();

			foreach (string npcDisposition in DefaultNPCDispositionData)
			{
				string[] tokens = npcDisposition.Split('/');
				int nameIndex = tokens.Length - 1;
				string name = tokens[nameIndex];

				SDate addedDate = AddRandomBirthdayToNPC(birthdaysInUse, name);
				tokens[BirthdayIndex] = $"{addedDate.Season} {addedDate.Day}";
				tokens[RelationshipsIndex] = Globals.GetTranslation($"{name}-relationships");
				tokens[nameIndex] = Globals.GetTranslation($"{name}-name");

				replacements.Add(name, string.Join("/", tokens));
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
			Dictionary<SDate, string> birthdaysInUse = new Dictionary<SDate, string>();
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

			List<string> seasonStrings = new List<string> { "spring", "summer", "fall", "winter" };
			string season = Globals.RNGGetRandomValueFromList(seasonStrings);
			bool dateRetrieved = false;
			SDate date = null;

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
			SDate wizardBirthday = new SDate(day, "winter", 1);

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
			if (!Globals.Config.RandomizeNPCBirthdays) { return; }

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
