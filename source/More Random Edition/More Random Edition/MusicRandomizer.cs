using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Randomizes the music in the game
	/// </summary>
	public class MusicRandomizer
	{
		/// <summary>
		/// Randomizes all the music to another song
		/// </summary>
		/// <returns>A dictionary of song names to their alternatives</returns>
		public static Dictionary<string, string> Randomize()
		{
			List<string> musicList = new List<string>
			{
				"50s",
				"AbigailFlute",
				"AbigailFluteDuet",
				"aerobics",
				"breezy",
				"bugLevelLoop",
				"Cavern",
				"christmasTheme",
				"Cloth",
				"CloudCountry",
				"clubloop",
				"coin",
				"communityCenter",
				"cowboy_boss",
				"cowboy_outlawsong",
				"Cowboy_OVERWORLD",
				"Cowboy_singing",
				"Cowboy_undead",
				"crane_game",
				"crane_game_fast",
				"Crystal Bells",
				"desolate",
				"distantBanjo",
				"echos",
				"elliottPiano",
				"EmilyDance",
				"EmilyDream",
				"EmilyTheme",
				"event1",
				"event2",
				"fall_day_ambient",
				"fall1",
				"fall2",
				"fall3",
				"fallFest",
				"FlowerDance",
				"Frost_Ambient",
				"grandpas_theme",
				"gusviolin",
				"harveys_theme_jazz",
				"heavy",
				"honkytonky",
				"Hospital_Ambient",
				"Icicles",
				"jaunty",
				"jojaOfficeSoundscape",
				"junimoKart",
				"junimoKart_ghostMusic",
				"junimoKart_mushroomMusic",
				"junimoKart_slimeMusic",
				"junimoKart_whaleMusic",
				"junimoStarSong",
				"kindadumbautumn",
				"Lava_Ambient",
				"libraryTheme",
				"MainTheme",
				"MarlonsTheme",
				"marnieShop",
				"mermaidSong",
				"moonlightJellies",
				"movie_classic",
				"movie_nature",
				"movie_wumbus",
				"movieTheater",
				"movieTheaterAfter",
				"musicboxsong",
				"Near The Planet Core",
				"night_market",
				"Of Dwarves",
				"Overcast",
				"playful",
				"poppy",
				"ragtime",
				"sadpiano",
				"Saloon1",
				"sam_acoustic1",
				"sam_acoustic2",
				"sampractice",
				"Secret Gnomes",
				"SettlingIn",
				"shaneTheme",
				"shimmeringbastion",
				"spaceMusic",
				"spirits_eve",
				"spring_day_ambient",
				"spring_night_ambient",
				"spring1",
				"spring2",
				"spring3",
				"springtown",
				"starshoot",
				"submarine_song",
				"summer_day_ambient",
				"summer1",
				"summer2",
				"summer3",
				"SunRoom",
				"sweet",
				"tickTock",
				"tinymusicbox",
				"title_night",
				"tribal",
				"Upper_Ambient",
				"wavy",
				"wedding",
				"winter_day_ambient",
				"winter1",
				"winter2",
				"winter3",
				"WizardSong",
				"woodsTheme",
				"XOR"
			};
			List<string> musicReplacementPool = new List<string>(musicList);
			Dictionary<string, string> musicReplacements = new Dictionary<string, string>();

			foreach (string song in musicList)
			{
				string replacementSong = Globals.RNGGetAndRemoveRandomValueFromList(musicReplacementPool);
				musicReplacements.Add(song.ToLower(), replacementSong);
			}

			WriteToSpoilerLog(musicReplacements);
			return musicReplacements;
		}

		/// <summary>
		/// Writes the music info to the spoiler log
		/// </summary>
		/// <param name="musicList">The music replacement list</param>
		private static void WriteToSpoilerLog(Dictionary<string, string> replacementList)
		{
			if (!Globals.Config.RandomizeMusic) { return; }

			Globals.SpoilerWrite("==== MUSIC ====");
			foreach (string song in replacementList.Keys)
			{
				Globals.SpoilerWrite($"{song} is now {replacementList[song]}");
			}

			Globals.SpoilerWrite("---");
			Globals.SpoilerWrite("");
		}
	}
}