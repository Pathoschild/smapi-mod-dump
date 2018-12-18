using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;
using StardewSymphonyRemastered.Framework;
using System.IO;
using StardustCore.UIUtilities;
using NAudio.Wave;

namespace StardewSymphonyRemastered
{
    /// TODO:
    ///
    /// Fixed Farm building glitch,
    /// Added underground mine support
    /// Added seasonal selection support
    /// added just location support
    /// added in write all config option
    /// 
    /// Add mod config to have silent rain option.
    /// Add in shuffle song button that just selects music but probably plays a different song. same as musicManager.selectmusic(getConditionalString);
    /// Add in a save button to save settings in the menu.
    /// 
    /// Notes:
    /// All mods must add events/locations/festivals/menu information to this mod during the Entry function of their mod because once the player is loaded that's when all of the packs are initialized with all of their music.
    public class StardewSymphony : Mod
    {
        public static WaveBank DefaultWaveBank;
        public static ISoundBank DefaultSoundBank;


        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static IManifest Manifest;

        public static MusicManager musicManager;

        private string MusicPath;
        public static string WavMusicDirectory;
        public static string XACTMusicDirectory;
        public static string TemplateMusicDirectory;

        public bool musicPacksInitialized;



        public static bool festivalStart;
        public static bool eventStart;

        public static bool menuChangedMusic;

        public static Config Config;

        


        public static TextureManager textureManager;
        
        /// <summary>
        /// Entry point for the mod.
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            
            DefaultSoundBank = Game1.soundBank;
            DefaultWaveBank = Game1.waveBank;
            ModHelper = helper;
            ModMonitor = Monitor;
            Manifest = ModManifest;
            Config = helper.ReadConfig<Config>();
            StardewModdingAPI.Events.SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            // StardewModdingAPI.Events.EventArgsLocationsChanged += LocationEvents_CurrentLocationChanged;

            StardewModdingAPI.Events.PlayerEvents.Warped += PlayerEvents_Warped;
            StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;
            StardewModdingAPI.Events.ControlEvents.KeyPressed += ControlEvents_KeyPressed;
            StardewModdingAPI.Events.SaveEvents.BeforeSave += SaveEvents_BeforeSave;

            StardewModdingAPI.Events.MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            StardewModdingAPI.Events.MenuEvents.MenuClosed += MenuEvents_MenuClosed;

            StardewModdingAPI.Events.GameEvents.FirstUpdateTick += GameEvents_FirstUpdateTick;
            StardewModdingAPI.Events.GameEvents.OneSecondTick += GameEvents_OneSecondTick;


            musicManager = new MusicManager();

            MusicPath = Path.Combine(ModHelper.DirectoryPath, "Content", "Music");
            WavMusicDirectory = Path.Combine(MusicPath, "Wav");
            XACTMusicDirectory = Path.Combine(MusicPath, "XACT");
            TemplateMusicDirectory = Path.Combine(MusicPath, "Templates");


            textureManager = new TextureManager();
            this.createDirectories();
            this.createBlankXACTTemplate();
            this.createBlankWAVTemplate();

            musicPacksInitialized = false;
            menuChangedMusic = false;


            //Initialize all of the lists upon creation during entry.
            SongSpecifics.initializeMenuList();
            SongSpecifics.initializeEventsList();
            SongSpecifics.initializeFestivalsList();

            initializeMusicPacks();
        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            if (musicManager == null) return;
            else
            {
                musicManager.updateTimer();
            }
        }

        /// <summary>
        /// Raised when the player changes locations. This should determine the next song to play.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerEvents_Warped(object sender, StardewModdingAPI.Events.EventArgsPlayerWarped e)
        {
            musicManager.selectMusic(SongSpecifics.getCurrentConditionalString());

        }

        /// <summary>
        /// Ran once all of teh entry methods are ran. This will ensure that all custom music from other mods has been properly loaded in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameEvents_FirstUpdateTick(object sender, EventArgs e)
        {
            if (musicPacksInitialized == false)
            {
                musicManager.initializeMenuMusic(); //Initialize menu music that has been added to SongSpecifics.menus from all other mods during their Entry function.
                musicManager.initializeFestivalMusic();//Initialize festival music that has been added to SongSpecifics.menus from all other mods during their Entry function.
                musicManager.initializeEventMusic();//Initialize event music that has been added to SongSpecifics.menus from all other mods during their Entry function.
                musicPacksInitialized = true;
            }
        }

        /// <summary>
        /// Events to occur after the game has loaded in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {

            //Locaion initialization MUST occur after load. Anything else can occur before.
            SongSpecifics.initializeLocationsList(); //Gets all Game locations once the player has loaded the game, and all buildings on the player's farm and adds them to a location list.
            musicManager.initializeSeasonalMusic(); //Initialize the seasonal music using all locations gathered in the location list.
            musicManager.initializeMenuMusic();
            musicManager.initializeFestivalMusic();
            musicManager.initializeEventMusic();

            foreach (var musicPack in musicManager.musicPacks)
            {
                musicPack.Value.readFromJson();
            }

            SongSpecifics.menus.Sort();
            SongSpecifics.locations.Sort();
            SongSpecifics.festivals.Sort();
            SongSpecifics.events.Sort();

            musicManager.selectMusic(SongSpecifics.getCurrentConditionalString());

        }


        /// <summary>
        /// Choose new music when a menu is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuEvents_MenuClosed(object sender, StardewModdingAPI.Events.EventArgsClickableMenuClosed e)
        {
            if (menuChangedMusic == true)
            {
                musicManager.selectMusic(SongSpecifics.getCurrentConditionalString());
            }
        }

        /// <summary>
        /// Choose new music when a menu is opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuEvents_MenuChanged(object sender, StardewModdingAPI.Events.EventArgsClickableMenuChanged e)
        {
            //var ok = musicManager.currentMusicPack.getNameOfCurrentSong();
            musicManager.selectMenuMusic(SongSpecifics.getCurrentConditionalString());
        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            // THIS IS WAY TO LONG to run. Better make it save individual lists when I am editing songs.
            foreach(var musicPack in musicManager.musicPacks)
            {
                musicPack.Value.writeToJson();
            }
            
        }

        /// <summary>
        /// Fires when a key is pressed to open the music selection menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlEvents_KeyPressed(object sender, StardewModdingAPI.Events.EventArgsKeyPressed e)
        {
            if (e.KeyPressed.ToString() == Config.KeyBinding && Game1.activeClickableMenu==null)
            {
                Game1.activeClickableMenu = new Framework.Menus.MusicManagerMenu(Game1.viewport.Width,Game1.viewport.Height);
            }
        }


        /// <summary>
        /// Raised every frame. Mainly used just to initiate the music packs. Probably not needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (musicManager == null) return;
            
            if (Config.disableStardewMusic==true)
            {
                if (Game1.currentSong != null)
                {
                    Game1.currentSong.Stop(AudioStopOptions.Immediate); //stop the normal songs from playing over the new songs
                    Game1.currentSong.Stop(AudioStopOptions.AsAuthored);
                    Game1.nextMusicTrack = "";  //same as above line
                }
            }
            else
            {
                if (musicManager.currentMusicPack == null) return;
                if (Game1.currentSong != null && musicManager.currentMusicPack.isPlaying())
                {
                    //ModMonitor.Log("STOP THE MUSIC!!!");
                    Game1.currentSong.Stop(AudioStopOptions.Immediate); //stop the normal songs from playing over the new songs
                    Game1.currentSong.Stop(AudioStopOptions.AsAuthored);
                    //Game1.nextMusicTrack = "";  //same as above line
                }
            }
      
        }

        /// <summary>
        /// Load in the music packs to the music manager.
        /// </summary>
        public void initializeMusicPacks()
        {
            //load in all packs here.
            loadXACTMusicPacks();
            loadWAVMusicPacks();
        }

        /// <summary>
        /// Create the core directories needed by the mod.
        /// </summary>
        public void createDirectories()
        {
            string path = Path.Combine(ModHelper.DirectoryPath, "Content", "Graphics", "MusicMenu");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = Path.Combine("Content", "Graphics", "MusicMenu");
            //Generic Icons
            string musicNote = Path.Combine(path, "MusicNote.png");
            string musicCD = Path.Combine(path, "MusicDisk.png");
            string outlineBox = Path.Combine(path, "OutlineBox.png");

            string addIcon = Path.Combine(path, "AddButton.png");
            string deleteButton = Path.Combine(path, "DeleteButton.png");

            string greenBallon = Path.Combine(path, "GreenBallon.png");
            string redBallon = Path.Combine(path, "RedBallon.png");
            string starIcon = Path.Combine(path, "StarIcon.png");

            string menuIcon = Path.Combine(path, "MenuIcon.png");

            //Time Icons
            string dayIcon = Path.Combine(path, "TimeIcon_Day.png");
            string nightIcon = Path.Combine(path, "TimeIcon_Night.png");

            //Fun Icons
            string eventIcon = Path.Combine(path, "EventIcon.png");
            string festivalIcon = Path.Combine(path, "FestivalIcon.png");

            //WeatherIcons
            string sunnyIcon = Path.Combine(path, "WeatherIcon_Sunny.png");
            string rainyIcon = Path.Combine(path, "WeatherIcon_Rainy.png");
            string debrisIconSpring = Path.Combine(path, "WeatherIcon_DebrisSpring.png");
            string debrisIconSummer = Path.Combine(path, "WeatherIcon_DebrisSummer.png");
            string debrisIconFall = Path.Combine(path, "WeatherIcon_DebrisFall.png");
            string weatherFestivalIcon = Path.Combine(path, "WeatherIcon_Festival.png");
            string snowIcon = Path.Combine(path, "WeatherIcon_Snowing.png");
            string stormIcon = Path.Combine(path, "WeatherIcon_Stormy.png");
            string weddingIcon = Path.Combine(path, "WeatherIcon_WeddingHeart.png");

            //Season Icons
            string springIcon = Path.Combine(path, "SeasonIcon_Spring.png");
            string summerIcon = Path.Combine(path, "SeasonIcon_Summer.png");
            string fallIcon = Path.Combine(path, "SeasonIcon_Fall.png");
            string winterIcon = Path.Combine(path, "SeasonIcon_Winter.png");

            //Day Icons
            string mondayIcon = Path.Combine(path, "DayIcons_Monday.png");
            string tuesdayIcon = Path.Combine(path, "DayIcons_Tuesday.png");
            string wednesdayIcon = Path.Combine(path, "DayIcons_Wednesday.png");
            string thursdayIcon = Path.Combine(path, "DayIcons_Thursday.png");
            string fridayIcon = Path.Combine(path, "DayIcons_Friday.png");
            string saturdayIcon = Path.Combine(path, "DayIcons_Saturday.png");
            string sundayIcon = Path.Combine(path, "DayIcons_Sunday.png");

            string houseIcon = Path.Combine(path, "HouseIcon.png");
            string playButton = Path.Combine(path, "PlayButton.png");
            string stopButton = Path.Combine(path, "StopButton.png");
            string backButton = Path.Combine(path, "BackButton.png");

            textureManager.addTexture("MusicNote",new Texture2DExtended(ModHelper,Manifest.UniqueID,musicNote));
            textureManager.addTexture("MusicDisk", new Texture2DExtended(ModHelper, Manifest.UniqueID, musicCD));
            textureManager.addTexture("MusicCD", new Texture2DExtended(ModHelper, Manifest.UniqueID, musicCD));
            textureManager.addTexture("OutlineBox", new Texture2DExtended(ModHelper, Manifest.UniqueID, outlineBox));
            textureManager.addTexture("AddIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID, addIcon));
            textureManager.addTexture("DeleteIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID, deleteButton));
            textureManager.addTexture("GreenBallon", new Texture2DExtended(ModHelper, Manifest.UniqueID, greenBallon));
            textureManager.addTexture("RedBallon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  redBallon));
            textureManager.addTexture("StarIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  starIcon));
            textureManager.addTexture("MenuIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  menuIcon));
            textureManager.addTexture("DayIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  dayIcon));
            textureManager.addTexture("NightIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  nightIcon));
            textureManager.addTexture("EventIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  eventIcon));
            textureManager.addTexture("FestivalIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  festivalIcon));
            textureManager.addTexture("SunnyIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  sunnyIcon));
            textureManager.addTexture("RainyIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  rainyIcon));
            textureManager.addTexture("DebrisSpringIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  debrisIconSpring));
            textureManager.addTexture("DebrisSummerIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  debrisIconSummer));
            textureManager.addTexture("DebrisFallIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  debrisIconFall));
            textureManager.addTexture("WeatherFestivalIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  weatherFestivalIcon));
            textureManager.addTexture("SnowIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  snowIcon));
            textureManager.addTexture("StormIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  stormIcon));
            textureManager.addTexture("WeddingIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  weddingIcon));
            textureManager.addTexture("SpringIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  springIcon));
            textureManager.addTexture("SummerIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  summerIcon));
            textureManager.addTexture("FallIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  fallIcon));
            textureManager.addTexture("WinterIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  winterIcon));
            textureManager.addTexture("MondayIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  mondayIcon));
            textureManager.addTexture("TuesdayIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  tuesdayIcon));
            textureManager.addTexture("WednesdayIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  wednesdayIcon));
            textureManager.addTexture("ThursdayIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  thursdayIcon));
            textureManager.addTexture("FridayIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  fridayIcon));
            textureManager.addTexture("SaturdayIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  saturdayIcon));
            textureManager.addTexture("SundayIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  sundayIcon));

            textureManager.addTexture("HouseIcon", new Texture2DExtended(ModHelper, Manifest.UniqueID,  houseIcon));

            textureManager.addTexture("PlayButton", new Texture2DExtended(ModHelper, Manifest.UniqueID,  playButton));
            textureManager.addTexture("StopButton", new Texture2DExtended(ModHelper, Manifest.UniqueID,  stopButton));
            textureManager.addTexture("BackButton", new Texture2DExtended(ModHelper, Manifest.UniqueID,  backButton));


            if (!Directory.Exists(MusicPath)) Directory.CreateDirectory(MusicPath);
            if (!Directory.Exists(WavMusicDirectory)) Directory.CreateDirectory(WavMusicDirectory);
            if (!Directory.Exists(XACTMusicDirectory)) Directory.CreateDirectory(XACTMusicDirectory);
            if (!Directory.Exists(TemplateMusicDirectory)) Directory.CreateDirectory(TemplateMusicDirectory);
        }


        /// <summary>
        /// Used to create a blank XACT music pack example.
        /// </summary>
        public void createBlankXACTTemplate()
        {
            string path= Path.Combine(TemplateMusicDirectory, "XACT");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if(!File.Exists(Path.Combine(path, "MusicPackInformation.json"))){
                MusicPackMetaData blankMetaData = new MusicPackMetaData("Omegas's Music Data Example","Omegasis","Just a simple example of how metadata is formated for music packs. Feel free to copy and edit this one!","1.0.0 CoolExample","Icon.png");
                blankMetaData.writeToJson(Path.Combine(path, "MusicPackInformation.json"));
            }
            if (!File.Exists(Path.Combine(path, "readme.txt")))
            {
                string info = "Place the Wave Bank.xwb file and Sound Bank.xsb file you created in XACT in a similar directory in Content/Music/XACT/SoundPackName.\nModify MusicPackInformation.json as desire!\nRun the mod!";
                File.WriteAllText(Path.Combine(path, "readme.txt"),info);
            }
        }

        /// <summary>
        /// USed to create a blank WAV music pack example.
        /// </summary>
        public void createBlankWAVTemplate()
        {
            

            string path = Path.Combine(TemplateMusicDirectory, "WAV");
            string pathSongs = Path.Combine(path, "Songs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!Directory.Exists(pathSongs))
            {
                Directory.CreateDirectory(pathSongs);
            }
            if (!File.Exists(Path.Combine(path, "MusicPackInformation.json")))
            {
                MusicPackMetaData blankMetaData = new MusicPackMetaData("Omegas's Music Data Example", "Omegasis", "Just a simple example of how metadata is formated for music packs. Feel free to copy and edit this one!", "1.0.0 CoolExample","Icon");
                blankMetaData.writeToJson(Path.Combine(path, "MusicPackInformation.json"));
            }
            if (!File.Exists(Path.Combine(path, "readme.txt")))
            {
                string info = "Place the .wav song files in the Songs folder, modify the MusicPackInformation.json as desired, and then run!";
                File.WriteAllText(Path.Combine(path, "readme.txt"), info);
            }
        }

        /// <summary>
        /// Load in the XACT music packs.
        /// </summary>
        public void loadXACTMusicPacks()
        {
            string[] listOfDirectories= Directory.GetDirectories(XACTMusicDirectory);
            foreach(string folder in listOfDirectories)
            {
                //This chunk essentially allows people to name .xwb and .xsb files whatever they want.
                string[] xwb=Directory.GetFiles(folder, "*.xwb");
                string[] xsb = Directory.GetFiles(folder, "*.xsb");

                string[] debug = Directory.GetFiles(folder);
                if (xwb.Length == 0)
                {
                    if(Config.EnableDebugLog)
                        ModMonitor.Log("Error loading in attempting to load music pack from: " + folder + ". There is no wave bank music file: .xwb located in this directory. AKA there is no valid music here.", LogLevel.Error);
                    return;
                }
                if (xwb.Length >= 2)
                {
                    if (Config.EnableDebugLog)
                        ModMonitor.Log("Error loading in attempting to load music pack from: " + folder + ". There are too many wave bank music files or .xwbs located in this directory. Please ensure that there is only one music pack in this folder. You can make another music pack but putting a wave bank file in a different folder.", LogLevel.Error);
                    return;
                }

                if (xsb.Length == 0)
                {
                    if (Config.EnableDebugLog)
                        ModMonitor.Log("Error loading in attempting to load music pack from: " + folder + ". There is no sound bank music file: .xsb located in this directory. AKA there is no valid music here.", LogLevel.Error);
                    return;
                }
                if (xsb.Length >= 2)
                {
                    if (Config.EnableDebugLog)
                        ModMonitor.Log("Error loading in attempting to load music pack from: " + folder + ". There are too many sound bank music files or .xsbs located in this directory. Please ensure that there is only one sound reference file in this folder. You can make another music pack but putting a sound file in a different folder.", LogLevel.Error);
                    return;
                }

                string waveBank = xwb[0];
                string soundBank = xsb[0];
                string metaData = Path.Combine(folder, "MusicPackInformation.json");

                if (!File.Exists(metaData))
                {
                    if (Config.EnableDebugLog)
                        ModMonitor.Log("WARNING! Loading in a music pack from: " + folder + ". There is no MusicPackInformation.json associated with this music pack meaning that while songs can be played from this pack, no information about it will be displayed.", LogLevel.Error);
                }
                StardewSymphonyRemastered.Framework.XACTMusicPack musicPack = new XACTMusicPack(folder, waveBank,soundBank);

                musicPack.songInformation.initializeMenuMusic();
                musicPack.readFromJson();


                musicManager.addMusicPack(musicPack,true,true);
                
                
            }
        }

        /// <summary>
        /// Load in WAV music packs.
        /// </summary>
        public void loadWAVMusicPacks()
        {
            string[] listOfDirectories = Directory.GetDirectories(WavMusicDirectory);
            foreach (string folder in listOfDirectories)
            {
                string metaData = Path.Combine(folder, "MusicPackInformation.json");

                if (!File.Exists(metaData))
                {
                    if (Config.EnableDebugLog)
                        ModMonitor.Log("WARNING! Loading in a music pack from: " + folder + ". There is no MusicPackInformation.json associated with this music pack meaning that while songs can be played from this pack, no information about it will be displayed.", LogLevel.Error);
                }

                StardewSymphonyRemastered.Framework.WavMusicPack musicPack = new WavMusicPack(folder);

                musicPack.songInformation.initializeMenuMusic();
                musicPack.readFromJson();

                musicManager.addMusicPack(musicPack,true,true);
               
                
            }
        }


        /// <summary>
        /// Reset the music files for the game.
        /// </summary>
        public static void Reset()
        {
            Game1.waveBank = DefaultWaveBank;
            Game1.soundBank = DefaultSoundBank;
        }


        public static void DebugLog(string s)
        {
            if (Config.EnableDebugLog)
            {
                ModMonitor.Log(s);
            }
        }

    }
}
