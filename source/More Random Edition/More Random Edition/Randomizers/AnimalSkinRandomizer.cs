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

namespace Randomizer
{
    // TODO: this class needs to be removed - just need to fix critter replacements
    public class AnimalSkinRandomizer
	{
        private static Dictionary<string, string> _replacements;
		private readonly static List<string> PossibleCritterReplacements = new()
		{
			"crittersBears",
			"crittersseagullcrow",
			"crittersWsquirrelPseagull",
			"crittersBlueBunny"
		};

		private static string _critterSpoilerString;

		/// <summary>
		/// Randomizes animal skins
		/// Replaces the critters tilesheet and replaces a random animal with a bear
		/// </summary>
		/// <returns>The dictionary for the AssetLoader to use</returns>
		public static Dictionary<string, string> Randomize()
		{
			_replacements = new Dictionary<string, string>();

			AddCritterReplacement(Globals.RNGGetRandomValueFromList(PossibleCritterReplacements));

			if (!Globals.Config.RandomizeAnimalSkins)
			{
				return new Dictionary<string, string>();
			}

			WriteToSpoilerLog();
			return _replacements;
		}

        /// <summary>
        /// Adds the critter replacement to the dictionary
        /// </summary>
        /// <param name="critterName">The critter asset name</param>
        private static void AddCritterReplacement(string critterName)
		{
			_critterSpoilerString = $"Critter sheet replaced with {critterName}";
			_replacements.Add("TileSheets/critters", $"assets/TileSheets/{critterName}");
		}

		/// <summary>
		/// Writes the NPC replacements to the spoiler log
		/// </summary>
		private static void WriteToSpoilerLog()
		{
			Globals.SpoilerWrite("==== CRITTERS ====");
			Globals.SpoilerWrite(_critterSpoilerString);
			Globals.SpoilerWrite("");
		}
	}
}
