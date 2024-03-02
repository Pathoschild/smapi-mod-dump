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
using System.Linq;

namespace Randomizer
{
	public class SecretNotesRandomizer
	{
		private static Dictionary<string, string> prefs;

		/// <summary>
		/// For each secret note, generate a random number of NPCs for whom to reveal loved items.
		/// </summary>
		/// <returns><c>Dictionary&lt;int, string&gt;</c> containing the secret note IDs and strings to replace.</returns>
		public static Dictionary<int, string> FixSecretNotes(Dictionary<string, string> preferenceReplacements)
		{
			prefs = preferenceReplacements;
			Dictionary<int, string> _replacements = new();
			Dictionary<int, string> secretNoteData = Globals.ModRef.Helper.GameContent
                .Load<Dictionary<int, string>>("Data/SecretNotes");

			// The data dictionary has more entries than we want to change - we do only want indexes 1-9
            for (int noteIndex = 1; noteIndex < 9; noteIndex++)
			{
				// Pick from 1-3 random items to reveal loved items for, choosing more items for fewer characters
				int characterNum = Range.GetRandomValue(1, 3);
				int itemNum = Range.GetRandomValue(5, 6) - characterNum;

				List<string> NoteNPCs = Globals.RNGGetRandomValuesFromList(
					PreferenceRandomizer.GiftableNPCs.Values.ToList(), characterNum);
				string NPCLovesString = FormatRevealString(NoteNPCs, itemNum);

				string dataWithoutReveal = secretNoteData[noteIndex].Split("%revealtaste")[0];
                string noteText = $"{dataWithoutReveal}{NPCLovesString}";
				_replacements.Add(noteIndex, noteText);
			}

			WriteToSpoilerLog(_replacements);

			return _replacements;
		}

		/// <summary>
		/// Formats set of reveal commands (e.g. <c>"%revealtasteSam270%revealtasteMaru113..."</c>).
		/// </summary>
		/// <param name="NPCs">NPCs to reveal items for</param>
		/// <param name="itemNum">Number of items to reveal for each NPC</param>
		/// <returns><c>String</c> containing NPCs' preferences to reveal.</returns>
		private static string FormatRevealString(List<string> NPCs, int itemNum)
		{
			string lovesString = "";

			foreach (string NPC in NPCs)
			{
				string[] tokens = prefs[NPC].Split('/');
				List<string> items = tokens[1].Trim().Split(' ') 
					.Where(x => int.Parse(x) > 0)            
					.ToList();                               

				for (int num = itemNum; num > 0; num--)
				{
					lovesString += GetItemRevealString(NPC, items);
				}
			}

			return lovesString;
		}

		/// <summary>
		/// Builds a single item reveal string (e.g. <c>%revealtasteAbigail206</c>).
		/// </summary>
		/// <param name="NPC">Name of the NPC to build the item reveal string for.</param>
		/// <returns>String representing item reveal command.</returns>
		private static string GetItemRevealString(string name, List<string> items)
		{
			return items.Any()
				? "%revealtaste" + name + Globals.RNGGetAndRemoveRandomValueFromList(items)
				: "";
		}

		/// <summary>
		/// Log the results
		/// </summary>
		/// <param name="replacements">The results</param>
		private static void WriteToSpoilerLog(Dictionary<int, string> replacements)
		{
			if (!Globals.Config.NPCs.RandomizeIndividualPreferences) 
			{ 
				return; 
			}

			Globals.SpoilerWrite("===== SECRET NOTES =====");
			foreach (KeyValuePair<int, string> pair in replacements)
			{
				Globals.SpoilerWrite($"{pair.Key}: {pair.Value}");
			}
			Globals.SpoilerWrite("");
		}
	}
}
