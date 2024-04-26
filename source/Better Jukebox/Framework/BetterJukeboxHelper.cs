/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaphodil/BetterJukebox
**
*************************************************/

using System;
using System.Collections.Generic;

namespace Gaphodil.BetterJukebox.Framework
{
    static class BetterJukeboxHelper
    {
        public static List<string> SoundtrackOrderIds =
        [
            "title_day",
            "MainTheme",
            "CloudCountry",
            "grandpas_theme",
            "SettlingIn",
            "springsongs",
            "spring1",
            "spring2",
            "spring3",
            "springtown",
            "FlowerDance",
            "event1",
            "distantBanjo",
            "WizardSong",
            "summer1",
            "summer2",
            "summer3",
            "MarlonsTheme",
            "Saloon1",
            "event2",
            "moonlightJellies",
            "fall1",
            "fall2",
            "fall3",
            "libraryTheme",
            "fallFest",
            "tickTock",
            "spirits_eve",
            "winter1",
            "winter2",
            "winter3",
            "christmasTheme",
            "junimoStarSong",
            "marnieShop",
            "wavy",
            "playful",
            "sweet",
            "50s",
            "elliottPiano",
            "breezy",
            "AbigailFlute",
            "AbigailFluteDuet",
            "spaceMusic",
            "desolate",
            "ragtime",
            "echos",
            "kindadumbautumn",
            "shaneTheme",
            "EmilyTheme",
            "EmilyDream",
            "EmilyDance",
            "tinymusicbox",
            "sampractice",
            "shimmeringbastion",
            "poppy",
            "honkytonky",
            "heavy",
            "sadpiano",
            "musicboxsong",
            "jaunty",
            "gusviolin",
            "wedding",
            "EarthMine",
            "Crystal Bells",
            "Cavern",
            "Secret Gnomes",
            "FrostMine",
            "Icicles",
            "XOR",
            "Cloth",
            "LavaMine",
            "Near The Planet Core",
            "Of Dwarves",
            "Overcast",
            "tribal",
            "woodsTheme",
            "Cowboy_OVERWORLD",
            "cowboy_outlawsong",
            "Cowboy_undead",
            "cowboy_boss",
            "Cowboy_singing",
            "title_night",
            "night_market",
            "submarine_song",
            "mermaidSong",
            "SunRoom",
            "harveys_theme_jazz",
            "sam_acoustic2",
            "sam_acoustic1",
            "movieTheater",
            "crane_game",
            "crane_game_fast",
            "movie_wumbus",
            "movie_nature",
            "movie_classic",
            "movieTheaterAfter",
            "junimoKart",
            "junimoKart_whaleMusic",
            "junimoKart_slimeMusic",
            "junimoKart_ghostMusic",
            "junimoKart_mushroomMusic",
            "IslandMusic",
            "fieldofficeTentMusic",
            "VolcanoMines",
            "VolcanoMines1",
            "VolcanoMines2",
            "caldera",
            "FrogCave",
            "PIRATE_THEME",
            "PIRATE_THEME(muffled)",
            "sad_kid",
            "end_credits"
        ];

        // also used to determine locked tracks (minus cowboy)
        public static List<string> UnheardSoundtrack =
        [
            // "title_day",
            "MainTheme",
            "CloudCountry",
            "grandpas_theme",
            "SettlingIn",
            // "springsongs",
            "spring1",
            "spring2",
            "spring3",
            "springtown",
            "FlowerDance",
            "event1",
            "distantBanjo",
            "WizardSong",
            // "summersongs",
            "summer1",
            "summer2",
            "summer3",
            "MarlonsTheme",
            "Saloon1",
            "event2",
            "moonlightJellies",
            // "fallsongs",
            "fall1",
            "fall2",
            "fall3",
            "libraryTheme",
            "fallFest",
            "tickTock",
            "spirits_eve",
            // "wintersongs",
            "winter1",
            "winter2",
            "winter3",
            "christmasTheme",
            "junimoStarSong",
            "marnieShop",
            "wavy",
            "playful",
            "sweet",
            "50s",
            "elliottPiano",
            "breezy",
            "AbigailFlute",
            "AbigailFluteDuet",
            "spaceMusic",
            "desolate",
            "ragtime",
            "echos",
            "kindadumbautumn",
            "shaneTheme",
            "EmilyTheme",
            "EmilyDream",
            "EmilyDance",
            "tinymusicbox",
            "sampractice",
            "shimmeringbastion",
            "poppy",
            "honkytonky",
            "heavy",
            "sadpiano",
            "musicboxsong",
            "jaunty",
            "gusviolin",
            "wedding",
            // "EarthMine",
            "Crystal Bells",
            "Cavern",
            "Secret Gnomes",
            // "FrostMine",
            "Icicles",
            "XOR",
            "Cloth",
            // "LavaMine",
            "Near The Planet Core",
            "Of Dwarves",
            "Overcast",
            "tribal",
            "woodsTheme",
            "Cowboy_OVERWORLD",
            "cowboy_outlawsong",
            "Cowboy_undead",
            "cowboy_boss",
            "Cowboy_singing",
            "title_night",
            "night_market",
            "submarine_song",
            "mermaidSong",
            "SunRoom",
            "harveys_theme_jazz",
            "sam_acoustic2",
            "sam_acoustic1",
            "movieTheater",
            "crane_game",
            "crane_game_fast",
            "movie_wumbus",
            "movie_nature",
            "movie_classic",
            "movieTheaterAfter",
            "junimoKart",
            "junimoKart_whaleMusic",
            "junimoKart_slimeMusic",
            "junimoKart_ghostMusic",
            "junimoKart_mushroomMusic",
            "IslandMusic",
            "fieldofficeTentMusic",
            // "VolcanoMines",
            "VolcanoMines1",
            "VolcanoMines2",
            "caldera",
            "FrogCave",
            "PIRATE_THEME",
            "PIRATE_THEME(muffled)",
            "sad_kid",
            "end_credits"
        ];

        public static List<string> UnheardNamed =
        [
            "aerobics",
            "bugLevelLoop",
            "jojaOfficeSoundscape",
            // "random",
            "starshoot",
        ];

        public static List<string> UnheardRandom =
        [
            "springsongs",
            "EarthMine",
            "FrostMine",
            "LavaMine",
            "VolcanoMines"
        ];

        public static List<string> UnheardMisc =
        [
            // "springsongs",
            // "EarthMine",
            // "FrostMine",
            // "LavaMine",
            // "VolcanoMines",
            "archaeo",
            "bigDrums",
            "clubloop",
            "communityCenter",
            // "Cyclops",
            "fall_day_ambient",
            "Frost_Ambient",
            // "Ghost Synth",
            "Hospital_Ambient",
            "jungle_ambience",
            "Lava_Ambient",
            // "Majestic",
            "Meteorite",
            "movieScreenAmbience",
            // "New Snow",
            "nightTime",
            "ocean",
            // "Orange",
            // "Pink Petals",
            // "Plums",
            "pool_ambient",
            // "rain",
            "sappypiano",
            "secret1",
            "spring_day_ambient",
            "spring_night_ambient",
            "Stadium_ambient",
            "stardrop",
            "summer_day_ambient",
            // "Tropical Jam",
            "tropical_island_day_ambient",
            "Upper_Ambient",
            "Volcano_Ambient",
            "winter_day_ambient"
        ];

        public static List<string> UnheardDupes =
        [
            "Cyclops",
            "Ghost Synth",
            "Majestic",
            "New Snow",
            "Orange",
            "Pink Petals",
            "Plums",
            "Tropical Jam"
        ];
        
        public static List<string> UnheardMusical =
        [
            "aerobics",
            "starshoot",
            "archaeo",
            "bigDrums",
            "sappypiano",
            "secret1",
            "stardrop"
        ];


        /// <summary>
        /// Remove ambient tracks from the list of songs available in the jukebox.
        /// Copied from the ChooseFromListMenu constructor.
        /// </summary>
        /// <param name="trackList"></param>
        public static void RemoveAmbience(List<string> trackList, List<string> blacklist, List<string> whitelist)
        {
            for (int index = trackList.Count - 1; index >= 0; --index)
            {
                // always handle list parameters
                if (whitelist.Contains(trackList[index]))
                {
                    continue;
                }
                else if (blacklist.Contains(trackList[index]))
                {
                    trackList.RemoveAt(index);
                }
                // vanilla
                else if (trackList[index].ToLower().Contains("ambient") || trackList[index].ToLower().Contains("bigdrums") || trackList[index].ToLower().Contains("clubloop") || trackList[index].ToLower().Contains("ambience"))
                {
                    trackList.RemoveAt(index);
                }
                else
                {
                    switch (trackList[index])
                    {
                        case "buglevelloop": // vanilla bug: should be "bugLevelLoop"
                        case "coin":
                        case "communityCenter":
                        case "jojaOfficeSoundscape":
                        case "nightTime":
                        case "ocean":
                            trackList.RemoveAt(index);
                            continue;
                        default:
                            continue;
                    }
                }
            }
        }

        public static void TrueRemoveAmbience(List<string> trackList, List<string> blacklist, List<string> whitelist)
        {
            for (int i = trackList.Count - 1; i >= 0; --i)
            {
                string track = trackList[i];

                // always handle list parameters
                if (whitelist.Contains(trackList[i]))
                {
                    continue;
                }
                else if (blacklist.Contains(trackList[i]))
                {
                    trackList.RemoveAt(i);
                }

                // keep these in
                switch (track)
                {
                    case "aerobics":
                    case "starshoot":
                    case "archaeo":
                    case "bigDrums":
                    case "sappypiano":
                    case "secret1":
                    case "stardrop":
                        continue;
                    default:
                        break;
                }

                // specifically remove these
                switch (track)
                {
                    case "bugLevelLoop": // vanilla bug fixed
                    case "jojaOfficeSoundscape":
                    case "coin":
                        trackList.RemoveAt(i);
                        continue;
                    default:
                        break;
                }
                // and the rest
                if (!UnheardMisc.IndexOf(track).Equals(-1))
                {
                    trackList.RemoveAt(i);
                }
            }
        }

        public static void FilterTracksFromList(List<string> trackList, int ambienceType, string config_blist = "", string config_wlist = "")
        {
            FilterListConfig blacklist = new(config_blist);
            FilterListConfig whitelist = new(config_wlist);

            switch (ambienceType)
            {
                case 1:
                    RemoveAmbience(trackList, blacklist.content, whitelist.content);
                    break;
                case 2:
                    TrueRemoveAmbience(trackList, blacklist.content, whitelist.content);
                    break;
                case 0:
                    // always handle list parameters
                    for (int i = trackList.Count - 1; i >= 0; --i)
                    {
                        if (whitelist.content.Contains(trackList[i]))
                        {
                            continue;
                        }
                        else if (blacklist.content.Contains(trackList[i]))
                        {
                            trackList.RemoveAt(i);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        // from List to HashSet
        public static void AddUnheardTracks(HashSet<string> trackList, bool soundtrack, bool named, bool random, bool misc, bool dupes, bool musical)
        {
            // O(n^2) every time oh yeah
            if (soundtrack)
                AddOneListIteratively(trackList, UnheardSoundtrack);
            if (named)
                AddOneListIteratively(trackList, UnheardNamed);
            if (random)
                AddOneListIteratively(trackList, UnheardRandom);
            if (misc)
                AddOneListIteratively(trackList, UnheardMisc);
            if (dupes)
                AddOneListIteratively(trackList, UnheardDupes);
            if (musical)
                AddOneListIteratively(trackList, UnheardMusical);
        }

        public static void AddOneListIteratively(HashSet<string> trackList, List<string> toAdd)
        {
            string track;
            for (int i = 0; i < toAdd.Count; i++)
            {
                track = toAdd[i];
                /*if (trackList.IndexOf(track).Equals(-1))
                {
                    trackList.Add(track);
                }*/
                trackList.Add(track); // no duplication prevention necessary here
            }
        }

        public static void AddLockedTracks(List<BetterJukeboxItem> trackList)
        {
            for (int i = 0; i < UnheardSoundtrack.Count; i++)
            {
                string toAdd = UnheardSoundtrack[i];
                bool exists = false;
                for (int j = 0; j < trackList.Count; j++)
                {
                    if (trackList[j].Name.Equals(toAdd))
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists)
                    continue;

                trackList.Add(new BetterJukeboxItem(toAdd, true));
            }
        }

        public static string BandcampTitleFromCue(string cue, Func<string, StardewModdingAPI.Translation> GetTranslation)
        {
            switch (cue)
            {
                case "WizardSong":
                case "MarlonsTheme":
                case "libraryTheme":
                case "50s":
                case "elliottPiano":
                case "breezy":
                case "AbigailFlute":
                case "spaceMusic":
                case "desolate":
                case "ragtime":
                case "echos":
                case "kindadumbautumn":
                case "shaneTheme":
                case "EmilyTheme":
                case "tinymusicbox":
                case "shimmeringbastion":
                case "poppy":
                case "honkytonky":
                case "heavy":
                case "wedding":
                case "Cowboy_OVERWORLD":
                case "cowboy_outlawsong":
                case "Cowboy_undead":
                case "cowboy_boss":
                case "Cowboy_singing":
                case "title_night":
                case "submarine_song":
                case "mermaidSong":
                case "SunRoom":
                case "movie_wumbus":
                case "movie_nature":
                case "movie_classic":
                case "junimoKart":
                case "junimoKart_whaleMusic":
                case "junimoKart_slimeMusic":
                case "junimoKart_ghostMusic":
                case "junimoKart_mushroomMusic":
                case "end_credits":
                    return GetTranslation("BetterJukebox:" + cue);
                default:
                    break;
            }
            return "";
        }

        public static int SortBySoundtrackOrder(BetterJukeboxItem item1, BetterJukeboxItem item2)
        {
            string cue1 = item1.Name;
            string cue2 = item2.Name;
            int ind1 = SoundtrackOrderIds.IndexOf(cue1);
            int ind2 = SoundtrackOrderIds.IndexOf(cue2);
            if (ind1.Equals(ind2)) return 0;
            if ((ind1.Equals(-1) ? short.MaxValue : ind1) < (ind2.Equals(-1) ? short.MaxValue : ind2)) return -1;
            return 1;
        }
    }
}
