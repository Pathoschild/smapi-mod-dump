/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Randomizes the music in the game
	/// </summary>
	public class MusicRandomizer
	{
		/// <summary>
		/// The dictionary of music replacements
		/// </summary>
		public static Dictionary<string, string> MusicReplacements { get; set; } = new Dictionary<string, string>();

		/// <summary>
		/// The list of songs
		/// </summary>
		public static List<string> MusicList = new List<string>
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
				"XOR",
				"tropical_island_day_ambient",
				"VolcanoMines",
				"Volcano_Ambient"
			};

		/// <summary>
		/// The last song that played/is playing
		/// </summary>
		private static string _lastCurrentSong { get; set; }

		/// <summary>
		/// Randomizes all the music to another song
		/// </summary>
		/// <returns>A dictionary of song names to their alternatives</returns>
		public static void Randomize()
		{
			List<string> musicReplacementPool = new List<string>(MusicList);
			MusicReplacements = new Dictionary<string, string>();
			_lastCurrentSong = "";

			foreach (string song in MusicList)
			{
				string replacementSong = Globals.RNGGetAndRemoveRandomValueFromList(musicReplacementPool);
				MusicReplacements.Add(song, replacementSong);
			}

			WriteToSpoilerLog();
		}

		/// <summary>
		/// Attempts to replace the current song with a different one
		/// If the song was barely replaced, it doesn't do anything
		/// </summary>
		public static void TryReplaceSong()
		{
			string currentSong = Game1.currentSong?.Name;
			if (_lastCurrentSong == currentSong) { return; }

			string newSongToPlay = Globals.Config.Music.RandomSongEachTransition ? GetRandomSong() : GetMappedSong(currentSong);

			//TODO: get rid of this set if 3 if-statements in the next major release (includes removing it from MusicList)
			if (newSongToPlay == "Volcano_Ambient")
			{
				newSongToPlay = MusicReplacements["Volcano_Ambient"];
			}
			if (newSongToPlay == "Lava_Ambient")
			{
				newSongToPlay = MusicReplacements["Lava_Ambient"];
			}
			if (newSongToPlay == "Volcano_Ambient") // Hack in case Lava_Ambient was mapped to Volcano_Ambient
			{
				newSongToPlay = MusicReplacements["Volcano_Ambient"];
			}

			if (!string.IsNullOrWhiteSpace(newSongToPlay))
			{
				_lastCurrentSong = newSongToPlay;
				Game1.changeMusicTrack(newSongToPlay);

				//Game1.addHUDMessage(new HUDMessage($"Song: {currentSong} | Replaced with: {value}"));
			}
		}

		/// <summary>
		/// Gets the song that's mapped to the given song
		/// </summary>
		/// <param name="currentSong">The song to look up</param>
		/// <returns />
		private static string GetMappedSong(string currentSong)
		{
			if (MusicReplacements.TryGetValue(currentSong ?? "", out string value))
			{
				return value;
			}
			return string.Empty;
		}

		/// <summary>
		/// Gets a random song
		/// </summary>
		/// <returns />
		private static string GetRandomSong()
		{
			//TODO: remove the Volcano_Ambient check in the next major release
			return Globals.RNGGetRandomValueFromList(
				MusicList.Where(song => song != "Volcano_Ambient" && song != "Lava_Ambient").ToList(), true);
		}

		/// <summary>
		/// Writes the music info to the spoiler log
		/// </summary>
		/// <param name="musicList">The music replacement list</param>
		private static void WriteToSpoilerLog()
		{
			if (!Globals.Config.Music.Randomize || Globals.Config.Music.RandomSongEachTransition) { return; }

			Globals.SpoilerWrite("==== MUSIC ====");
			foreach (string song in MusicReplacements.Keys)
			{
				Globals.SpoilerWrite($"{song} is now {MusicReplacements[song]}");
			}

			Globals.SpoilerWrite("---");
			Globals.SpoilerWrite("");
		}
	}
}