/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/HandyHeadphones
**
*************************************************/

using HandyHeadphones.API.Interfaces;
using HandyHeadphones.Patches;
using HandyHeadphones.UI;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HandyHeadphones
{
    public class ModEntry : Mod
    {
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static ModConfig config;
        internal static readonly string hatsPath = Path.Combine("assets", "HeadphonesPack");

        private string cachedRequestedSong;
        private bool waitingForEventToFinishToResumeCachedSong;

        // Debug related
        private bool debugMode = false;
        private List<string> testedSongs;

        // All songs manually listed
        public static readonly string[] allSongs =
        {
            "50s",
            "AbigailFlute",
            "AbigailFluteDuet",
            "aerobics",
            "breezy",
            "bugLevelLoop",
            "caldera",
            "Cavern",
            "christmasTheme",
            "Cloth",
            "CloudCountry",
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
            "end_credits",
            "event1",
            "event2",
            "fall1",
            "fall2",
            "fall3",
            "fallFest",
            "fieldofficeTentMusic",
            "FlowerDance",
            "FrogCave",
            "grandpas_theme",
            "gusviolin",
            "harveys_theme_jazz",
            "heavy",
            "honkytonky",
            "Icicles",
            "IslandMusic",
            "jaunty",
            "jojaOfficeSoundscape",
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
            "PIRATE_THEME",
            "PIRATE_THEME(muffled)",
            "playful",
            "poppy",
            "ragtime",
            "sad_kid",
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
            "VolcanoMines1",
            "VolcanoMines2",
            "wavy",
            "wedding",
            "winter1",
            "winter2",
            "winter3",
            "WizardSong",
            "woodsTheme",
            "XOR"
        };

        // All sounds manually listed
        public static readonly string[] allSounds =
        {
            "axe",
            "bigDeSelect",
            "bigSelect",
            "boulderCrack",
            "coin",
            "cut",
            "dialogueCharacter",
            "dialogueCharacterClose",
            "doorClose",
            "dropItemInWater",
            "ghost",
            "grunt",
            "hoeHit",
            "openBox",
            "pickUpItem",
            "sandyStep",
            "seeds",
            "shadowpeep",
            "shiny4",
            "smallSelect",
            "throwDownITem",
            "grassyStep",
            "stoneStep",
            "thudStep",
            "eat",
            "fishBite",
            "toolSwap",
            "pullItemFromWater",
            "woodyStep",
            "bob",
            "cluck",
            "cluck",
            "cluck",
            "dwoop",
            "explosion",
            "crafting",
            "detector",
            "furnace",
            "powerup",
            "fuse",
            "hitEnemy",
            "slime",
            "swordswipe",
            "slimedead",
            "Duggy",
            "money",
            "toolCharge",
            "ow",
            "death",
            "fireball",
            "rabbit",
            "stoneCrack",
            "stoneCrack",
            "woodyHit",
            "goat",
            "goat",
            "cow",
            "cow",
            "cow",
            "breathout",
            "breathin",
            "wind",
            "SpringBirds",
            "SpringBirds",
            "SpringBirds",
            "SpringBirds",
            "SpringBirds",
            "Ship",
            "Ship",
            "boop",
            "boop",
            "boop",
            "boop",
            "clubloop",
            "achievement",
            "drumkit6",
            "drumkit5",
            "drumkit3",
            "drumkit2",
            "drumkit4",
            "drumkit0",
            "drumkit1",
            "flute",
            "phone",
            "thunder",
            "rain",
            "crickets",
            "wand",
            "questcomplete",
            "cavedrip",
            "pig",
            "pig",
            "objectiveComplete",
            "backpackIN",
            "hammer",
            "rainsound",
            "rainsound",
            "rainsound",
            "croak",
            "treethud",
            "treecrack",
            "axchop",
            "leafrustle",
            "crystal",
            "purchase",
            "purchaseClick,purchase",
            "potterySmash",
            "select",
            "flameSpellHit",
            "flameSpell",
            "debuffHit",
            "debuffSpell",
            "wateringCan",
            "wateringCan",
            "wateringCan",
            "slimedead",
            "slimedead",
            "monsterdead",
            "clubhit",
            "clubswipe",
            "openChest",
            "swordswipe",
            "daggerswipe",
            "flybuzzing",
            "rockGolemSpawn",
            "rockGolemDie",
            "rockGolemHit",
            "darkCaveLoop",
            "bugLevelLoop",
            "batFlap",
            "batScreech",
            "parry,clank",
            "clubSmash",
            "ocean",
            "seagulls",
            "seagulls",
            "seagulls",
            "spring_day_ambient",
            "toyPiano",
            "skeletonStep",
            "skeletonDie",
            "skeletonHit",
            "slimeHit",
            "dustMeep",
            "throw",
            "minecartLoop",
            "roadnoise",
            "busDoorOpen",
            "sipTea",
            "shadowDie",
            "shadowHit",
            "healSound",
            "coldSpell",
            "Upper_Ambient",
            "Frost_Ambient",
            "Lava_Ambient",
            "yoba",
            "clank",
            "warrior",
            "slingshot",
            "discoverMineral",
            "trashcan",
            "trashcanlid",
            "newArtifact",
            "reward",
            "newRecord",
            "newRecipe",
            "stumpCrack",
            "secret1",
            "trainWhistle",
            "distantTrain",
            "distantTrain",
            "trainLoop",
            "getNewSpecialItem",
            "nightTime",
            "Meteorite",
            "UFO",
            "owl",
            "dogs",
            "scissors",
            "Milking",
            "Duck",
            "sheep",
            "killAnimal",
            "dwop",
            "bubbles",
            "bubbles",
            "moneyDial",
            "boulderBreak",
            "gulp",
            "gulp",
            "purchase,purchaseRepeat",
            "sell",
            "dirtyHit",
            "dirtyHit",
            "SinWave",
            "cast",
            "slowReel",
            "fastReel",
            "tinyWhip",
            "button1",
            "FishHit",
            "jingle1",
            "fishEscape",
            "fishingRodBend",
            "fishingRodBend",
            "fishingRodBend",
            "waterSlosh",
            "waterSlosh",
            "waterSlosh",
            "fishSlap",
            "Cowboy_Secret",
            "Cowboy_monsterDie",
            "Cowboy_monsterDie",
            "Cowboy_gunshot",
            "cowboy_dead",
            "Cowboy_Footstep",
            "Cowboy_undead",
            "cowboy_powerup",
            "cowboy_gunload",
            "Pickup_Coin15",
            "cowboy_monsterhit",
            "cowboy_gopher",
            "cowboy_explosion",
            "Hospital_Ambient",
            "breakingGlass",
            "robotSoundEffects",
            "robotBLASTOFF",
            "pool_ambient",
            "slosh",
            "slosh",
            "slosh",
            "cameraNoise",
            "keyboardTyping",
            "mouseClick",
            "whistle",
            "junimoMeep1",
            "communityCenter",
            "busDriveOff",
            "barrelBreak",
            "woodWhack",
            "woodWhack",
            "stairsdown",
            "serpentHit",
            "serpentDie",
            "dogWhining",
            "shwip",
            "fallDown",
            "doorCreak",
            "doorOpen",
            "doorCreakReverse",
            "cacklingWitch",
            "crow",
            "glug",
            "harvest",
            "thunder_small",
            "thunder_small",
            "rooster",
            "dog_pant",
            "dog_bark",
            "cat",
            "cat",
            "jojaOfficeSoundscape",
            "fall_day_ambient",
            "summer_day_ambient",
            "snowyStep",
            "cracklingFire",
            "babblingBrook",
            "heavyEngine",
            "spring_night_ambient",
            "cricketsAmbient",
            "give_gift",
            "stardrop",
            "crit",
            "cancel",
            "winter_day_ambient",
            "Stadium_cheer",
            "Stadium_ambient",
            "parrot",
            "miniharp_note",
            "submarine_landing"
        };

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor, helper and config
            monitor = Monitor;
            modHelper = helper;
            config = helper.ReadConfig<ModConfig>();

            // Load our Harmony patches
            try
            {
                var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patch: {e}", LogLevel.Error);
                return;
            }

            // Hook into the game launch
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            // Hook into the player warping
            helper.Events.Player.Warped += this.OnWarped;

            // Hook into the save loading
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

            // Hook into the player exiting to title
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            // Unhook into the 1 second tick
            Helper.Events.GameLoop.OneSecondUpdateTicked -= this.OnOneSecondUpdateTicked;

            cachedRequestedSong = null;
            waitingForEventToFinishToResumeCachedSong = false;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (debugMode)
            {
                testedSongs = new List<string>();
            }

            // Hook into the 1 second tick
            Helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (debugMode && e.IsMultipleOf(120))
            {
                VerifyAllSongs();
            }

            GameLocation location = Game1.player.currentLocation;
            if (location is null)
            {
                return;
            }

            if (location.currentEvent != null && !String.IsNullOrEmpty(location.miniJukeboxTrack.Value))
            {
                cachedRequestedSong = location.miniJukeboxTrack.Value;
                waitingForEventToFinishToResumeCachedSong = true;
            }
            else if (waitingForEventToFinishToResumeCachedSong)
            {
                location.miniJukeboxTrack.Value = cachedRequestedSong;

                cachedRequestedSong = null;
                waitingForEventToFinishToResumeCachedSong = false;
            }
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            Hat playerHat = e.Player.hat;

            if (playerHat is null || (playerHat.Name != "Headphones" && playerHat.Name != "Earbuds" && playerHat.Name != "Studio Headphones"))
            {
                return;
            }

            e.NewLocation.miniJukeboxTrack.Value = e.OldLocation.miniJukeboxTrack.Value;
            e.OldLocation.miniJukeboxTrack.Value = "";
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Check if spacechase0's JsonAssets is in the current mod list
            if (Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            {
                Monitor.Log("Attempting to hook into spacechase0.JsonAssets.", LogLevel.Debug);
                ApiManager.HookIntoJsonAssets(Helper);

                // Add the headphones asset
                ApiManager.GetJsonAssetInterface().LoadAssets(Path.Combine(Helper.DirectoryPath, hatsPath));
            }
        }

        private void VerifyAllSongs()
        {
            string song = allSongs.FirstOrDefault(s => !testedSongs.Contains(s));
            if (String.IsNullOrEmpty(song))
            {
                return;
            }
            testedSongs.Add(song);

            Monitor.Log($"Playing {song}...", LogLevel.Debug);
            if (Game1.player.currentLocation == null)
            {
                return;
            }
            if (song == "turn_off")
            {
                Game1.player.currentLocation.miniJukeboxTrack.Value = "";
                return;
            }
            if (song == "random")
            {
                Game1.player.currentLocation.SelectRandomMiniJukeboxTrack();
            }
            Game1.player.currentLocation.miniJukeboxTrack.Value = song;
        }

        public static void ShowMusicMenu()
        {
            List<string> list = config.ShowAllMusicTracks ? allSongs.ToList() : Game1.player.songsHeard.Distinct().ToList();
            list = list.OrderBy(s => Utility.getSongTitleFromCueName(s)).ToList();
            list.Insert(0, "turn_off");
            list.Insert(1, "random");

            Game1.activeClickableMenu = new MusicMenu(list, OnSongChosen, isJukebox: true, Game1.player.currentLocation.miniJukeboxTrack.Value);
        }

        public static void OnSongChosen(string selection)
        {
            if (Game1.player.currentLocation == null)
            {
                return;
            }

            if (selection == "turn_off")
            {
                Game1.player.currentLocation.miniJukeboxTrack.Value = "";
                return;
            }

            Game1.player.currentLocation.miniJukeboxTrack.Value = selection;
            Game1.updateMusic();
        }

        public static void ShowSoundMenu()
        {
            List<string> list = allSounds.ToList();
            list = list.Distinct().OrderBy(s => s).ToList();
            list.Insert(0, "turn_off");

            Game1.player.currentLocation.miniJukeboxTrack.Value = "none";

            Game1.activeClickableMenu = new SoundMenu(list, OnSoundChosen);
        }

        public static void OnSoundChosen(string selection)
        {
            if (Game1.player.currentLocation == null)
            {
                return;
            }

            if (selection == "turn_off")
            {
                Game1.playSound("cancel");
                Game1.player.currentLocation.miniJukeboxTrack.Value = "";
                return;
            }

            Game1.playSound(selection);
        }
    }
}
