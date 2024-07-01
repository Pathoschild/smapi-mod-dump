/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using StardewValley;
using StardewValley.GameData;

namespace OSTPlayer
{
    public class LogicUtils
    {
        private static Dictionary<string, string> gameSongs = new Dictionary<string, string>(){
            {"MainTheme", "Stardew Valley Overture"},
            {"title_night", "Load Game"},
            {"grandpas_theme", "Granpa's Theme"},

            {"spring1", "(Spring) It's A Big World Outside"},
            {"spring2", "(Spring) The Valley Comes Alive"},
            {"spring3", "(Spring) Wild Horseradish Jam"},
            {"summer1", "(Summer) Nature's Crescendo"},
            {"summer2", "(Summer) The Sun Can Bend An Orange Sky"},
            {"summer3", "(Summer) Tropicala"},
            {"fall1", "(Fall) The Smell of Mushroom"},
            {"fall2", "(Fall) Ghost Synth"},
            {"fall3", "(Fall) Raven's Descent"},
            {"winter1", "(Winter) Nocturne Of Ice"},
            {"winter2", "(Winter) The Wind Can Be Still"},
            {"winter3", "(Winter) Ancient"},

            {"AbigailFlute", "(Abigail's Melody) A Stillness in the Rain Solo"},
            {"AbigailFluteDuet", "(Abigail's Melody) A Stillness in the Rain Duet"},
            {"desolate", "(Alex's Theme) A Sad Song"},
            {"elliottPiano", "(Elliot's Theme) Piano Solo"},
            {"EmilyTheme", "(Emily's Theme) Song of Feathers"},
            {"ragtime", "(Haley's Theme) Pickle Jar Rag"},
            {"kidadumbautumn", "(Dr. Harvey's Theme) Grapefruit Sky"},
            {"breezy", "(Leah's Theme) Land of the Green and Gold"},
            {"spaceMusic", "(Maru's Theme) Starwatcher"},
            {"50s", "(Penny's Theme) Pleasant Memory"},
            {"echos", "(Sebastian's Theme) Echos"},
            {"shaneTheme", "(Shane's Theme) Frozen Pizza and Eggs"},
            {"WizardSong", "(Wizard's Theme) A Glimpse Of The Other World"},

            {"sampractice", "Band Practice"},
            {"honkytonky", "Sam's Band (Bluegrass Version)"},
            {"shimmeringbastion", "Sam's Band (Electronic Version)"},
            {"heavy", "Sam's Band (Heavy Version)"},
            {"poppy", "Sam's Band (Pop Version)"},
            
            {"junimoKart", "(Junimo Kart) Title Theme"},
            {"junimoKart_ghostMusic", "(Junimo Kart) Ghastly Galleon"},
            {"junimoKart_mushroomMusic", "(Junimo Kart) Glowshroom Grotto"},
            {"junimoKart_slimeMusic", "(Junimo Kart) Slomp's Stomp"},
            {"junimoKart_whaleMusic", "(Junimo Kart) The Gem Sea Giant"},

            {"Cavern", "(Mines) A Flicker In The Deep"},
            {"Cloth", "(Mines) Cloth"},
            {"Crystal Bells", "(Mines) Crystal Bells"},
            {"tribal", "(Mines) Danger!"},
            {"Icicles", "(Mines) Icicles"},
            {"Overcast", "(Mines) Magical Shoes"},
            {"XOR", "(Mines) Marimba Of Frozen Bones"},
            {"Secret Gnomes", "(Mines) Star Lumpy"},
            {"Of Dwarves", "(Mines) The Lava Dwellers"},
            {"Near The Planet Core", "(Mines) Visitor To The Unknown"},
            {"VolcanoMines2", "(Volcano Mines) Forgotten World"},
            {"VolcanoMines1", "(Volcano Mines) Molten Jelly"},
            
            {"Cowboy_singing", "(Journey Of The Prairie King) Ending"},
            {"cowboy_boss", "(Journey Of The Prairie King) Final Boss"},
            {"Cowboy_OVERWORLD", "(Journey Of The Prairie King) Overworld"},
            {"cowboy_outlawsong", "(Journey Of The Prairie King) The Outlaw"},

            {"movieTheater", "Movie Theater"},
            {"movieTheaterAfter", "Movie Theater (Closing Time)"},
            {"movie_nature", "(Movie Theme) Exploring Our Vibrant World"},
            {"movie_classic", "(Movie Theme) The Zuzu City Express"},
            {"movie_wumbus", "(Movie Theme) Wumbus"},
            
            {"sadpiano", "A Dark Corner To The Past"},
            {"junimoStarSong", "A Golden Star Is Born"},
            {"tinymusicbox", "Alex's Keepsake"},
            {"sweet", "Buttercup Melody"},
            {"wavy", "Calico Desert"},
            {"CloudCountry", "Cloud Country"},
            {"marnieShop", "Country Shop"},
            {"crane_game", "Crane Game"},
            {"crane_game_fast", "Crane Game Fast"},
            {"moonlightJellies", "Dance of the Moonlight Jellies"},
            {"distantBanjo", "Distant Banjo"},
            {"EmilyDream", "Dreamscape"},
            {"EmilyDance", "Emily's Dance"},
            {"tickTock", "Festival Game"},
            {"FlowerDance", "Flower Dance"},
            {"event1", "Fun Festival"},
            {"IslandMusic", "Ginger Island"},
            {"harveys_theme_jazz", "Grapefruit Sky (Pasta Primavera Mix)"},
            {"woodsTheme", "In The Deep Woods"},
            {"jaunty", "Jaunty"},
            {"sad_kid", "Leo's Song"},
            {"event2", "Luau Festival"},
            {"mermaidSong", "Mermaid Song"},
            {"musicboxsong", "Music Box Song"},
            {"caldera", "Mystery Of The Caldera"},
            {"night_market", "Night Market"},
            {"springtown", "Pelican Town"},
            {"PIRATE_THEME", "Pirate Theme"},
            {"playful", "Playful"},
            {"fieldofficeTentMusic", "Professor Snail's Radio"},
            {"fallFest", "Stardew Valley Fair Theme"},
            {"Saloon1", "Stardrop Saloon"},
            {"MarlonsTheme", "The Adventure Guild"},
            {"FrogCave", "The Gourmand's Cave"},
            {"sam_acoustic1", "The Happy Junimo Show"},
            {"libraryTheme", "The Library And Museum"},
            {"SettlingIn", "Settling In"},
            {"spirits_eve", "Spirit's Eve Festival"},
            {"submarine_song", "Submarine Theme"},
            {"SunRoom", "Sun Room (Alone With Relaxing Tea)"},
            {"gusviolin", "Violin Solo"},
            {"wedding", "Wedding Celebration"},
            {"christmasTheme", "Winter Festival"},

            {"end_credits", "(Credits) Summit Celebration"},

            {"archaeo", null},
            {"Cowboy_undead", null},
            {"sappypiano", null},
            {"starshoot", null}
        };
        public static List<Song> GetAllSongs()
        {
            List<Song> songs = new List<Song>();
            foreach(KeyValuePair<string, string> item in gameSongs){
                songs.Add(new Song(item.Key, item.Value));
            }
            return songs;
        }

        public static int CompareSongsByPos(Song s1, Song s2){

            if(s1.HasName && !s2.HasName)
                return 1;
            if(!s1.HasName && s2.HasName)
                return -1;

            foreach(KeyValuePair<string, string> item in gameSongs){
                if(item.Key == s1.Id)
                    return 1;
                if(item.Key == s2.Id)
                    return -1;
            }

            return 0;
        }

        // Removes song added by default while playing a track
        public static void removeHeardSong(string songName)
        {

            HashSet<string> songsHeard = Game1.player.songsHeard;
            switch (songName)
            {
                case "EarthMine":
                    songsHeard.Remove("Crystal Bells");
                    songsHeard.Remove("Cavern");
                    songsHeard.Remove("Secret Gnomes");
                    break;
                case "FrostMine":
                    songsHeard.Remove("Cloth");
                    songsHeard.Remove("Icicles");
                    songsHeard.Remove("XOR");
                    break;
                case "LavaMine":
                    songsHeard.Remove("Of Dwarves");
                    songsHeard.Remove("Near The Planet Core");
                    songsHeard.Remove("Overcast");
                    songsHeard.Remove("tribal");
                    break;
                case "VolcanoMines":
                    songsHeard.Remove("VolcanoMines1");
                    songsHeard.Remove("VolcanoMines2");
                    break;
                default:
                    songsHeard.Remove(songName);

                    break;
            }
        }
    }
}
