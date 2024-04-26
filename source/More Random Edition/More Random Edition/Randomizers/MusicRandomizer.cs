/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System.Collections.Generic;

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
		private static Dictionary<string, string> MusicReplacements { get; set; } = new();

		/// <summary>
		/// The list of songs
		/// </summary>
		private readonly static List<string> MusicList = new()
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
            "fall1",
            "fall2",
            "fall3",
            "fallFest",
            "FlowerDance",
            "grandpas_theme",
            "gusviolin",
            "harveys_theme_jazz",
            "heavy",
            "honkytonky",
            "Hospital_Ambient",
            "Icicles",
            "jaunty",
            "junimoKart",
            "junimoKart_ghostMusic",
            "junimoKart_mushroomMusic",
            "junimoKart_slimeMusic",
            "junimoKart_whaleMusic",
            "junimoStarSong",
            "kindadumbautumn",
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
            "spring1",
            "spring2",
            "spring3",
            "springtown",
            "starshoot",
            "submarine_song",
            "summer1",
            "summer2",
            "summer3",
            "SunRoom",
            "sweet",
            "tickTock",
            "tinymusicbox",
            "title_night",
            "tribal",
            "wavy",
            "wedding",
            "winter1",
            "winter2",
            "winter3",
            "WizardSong",
            "woodsTheme",
            "XOR",
            "VolcanoMines"
        };

		/// <summary>
		/// Randomizes all the music to another song
		/// </summary>
		/// <returns>A dictionary of song names to their alternatives</returns>
		public static void Randomize()
        {
            if (!Globals.Config.Music.Randomize ||
                Globals.Config.Music.RandomSongEachChange) 
            { 
                return; 
            }

            RNG rng = RNG.GetFarmRNG(nameof(MusicRandomizer));
			List<string> musicReplacementPool = new(MusicList);
			MusicReplacements = new Dictionary<string, string>();

			foreach (string song in MusicList)
			{
				string replacementSong = rng.GetAndRemoveRandomValueFromList(musicReplacementPool);
				MusicReplacements.Add(song, replacementSong);
			}

			WriteToSpoilerLog();
		}

		/// <summary>
		/// Intercepts Game1.changeMusicTrack so that we can call it with our remapped song
        /// This instance of harmony is pretty safe, as were just changing the input parameter
		/// </summary>
		public static void PatchChangeMusicTrack()
		{
            var harmony = new Harmony(Globals.ModRef.ModManifest.UniqueID);
            harmony.Patch(
               original: AccessTools.Method(typeof(Game1), nameof(Game1.changeMusicTrack)),
               prefix: new HarmonyMethod(typeof(MusicRandomizer), nameof(PatchChangeMusicTrack_Prefix))
            );
        }

        /// <summary>
        /// Grabs the mapped song, and changes the input parameter if a match is found
        /// </summary>
        /// <param name="newTrackName">The input parameter - MUST have this name!</param>
        /// <returns>True always - as we always want to call the original Game1.changeMusicTrack</returns>
        [HarmonyPatch(typeof(Game1))]
        internal static bool PatchChangeMusicTrack_Prefix(ref string newTrackName)
        {
            var mappedSong = GetMappedSong(newTrackName);
            if (Globals.Config.Music.Randomize && !string.IsNullOrEmpty(mappedSong))
            {
                newTrackName = mappedSong;
            }

            return true;
        }

        /// <summary>
        /// Gets the song that's mapped to the given song
        /// </summary>
        /// <param name="currentSong">The song to look up</param>
        /// <returns />
        private static string GetMappedSong(string currentSong)
		{
            if (Globals.Config.Music.RandomSongEachChange)
            {
                return GetRandomSong();
            }

			if (MusicReplacements.TryGetValue(currentSong ?? "", out string value))
			{
				return value;
			}
			return string.Empty;
		}

		/// <summary>
		/// Gets a random song seeded by Stardew's RNG
		/// </summary>
		/// <returns />
		private static string GetRandomSong()
		{
            return RNG.GetRandomValueFromListUsingRNG(MusicList, Game1.random);
		}

        /// <summary>
        /// Writes the music info to the spoiler log
        /// </summary>
        /// <param name="musicList">The music replacement list</param>
        private static void WriteToSpoilerLog()
		{
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