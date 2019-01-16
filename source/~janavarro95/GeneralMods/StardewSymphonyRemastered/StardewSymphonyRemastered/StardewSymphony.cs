using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewSymphonyRemastered.Framework;
using StardewValley;
using StardustCore.UIUtilities;

namespace StardewSymphonyRemastered
{
    // TODO:
    //
    // Fixed Farm building glitch,
    // Added underground mine support
    // Added seasonal selection support
    // added just location support
    // added in write all config option
    // 
    // Add mod config to have silent rain option.
    // Add in shuffle song button that just selects music but probably plays a different song. same as musicManager.selectmusic(getConditionalString);
    // Add in a save button to save settings in the menu.
    // 
    // Notes:
    // All mods must add events/locations/festivals/menu information to this mod during the Entry function of their mod because once the player is loaded that's when all of the packs are initialized with all of their music.
    public class StardewSymphony : Mod
    {
        /*********
        ** Accessors
        *********/
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static MusicManager musicManager;
        public static bool menuChangedMusic;
        public static Config Config;
        public static TextureManager textureManager;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = this.Monitor;
            Config = helper.ReadConfig<Config>();
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

            helper.Events.Player.Warped += this.OnPlayerWarped;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.Saving += this.OnSaving;

            helper.Events.Display.MenuChanged += this.OnMenuChanged;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;


            musicManager = new MusicManager();
            textureManager = new TextureManager();
            this.LoadTextures();

            menuChangedMusic = false;


            //Initialize all of the lists upon creation during entry.
            SongSpecifics.initializeMenuList();
            SongSpecifics.initializeFestivalsList();

            this.LoadMusicPacks();
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                musicManager.selectMusic(SongSpecifics.getCurrentConditionalString());
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Ran once all of the entry methods are ran. This will ensure that all custom music from other mods has been properly loaded in.

            musicManager.initializeMenuMusic(); //Initialize menu music that has been added to SongSpecifics.menus from all other mods during their Entry function.
            musicManager.initializeFestivalMusic(); //Initialize festival music that has been added to SongSpecifics.menus from all other mods during their Entry function.
            musicManager.initializeEventMusic(); //Initialize event music that has been added to SongSpecifics.menus from all other mods during their Entry function.
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //Locaion initialization MUST occur after load. Anything else can occur before.
            SongSpecifics.initializeLocationsList(); //Gets all Game locations once the player has loaded the game, and all buildings on the player's farm and adds them to a location list.
            musicManager.initializeSeasonalMusic(); //Initialize the seasonal music using all locations gathered in the location list.
            musicManager.initializeMenuMusic();
            musicManager.initializeFestivalMusic();
            musicManager.initializeEventMusic();

            foreach (var musicPack in musicManager.MusicPacks)
                musicPack.Value.LoadSettings();

            SongSpecifics.menus.Sort();
            SongSpecifics.locations.Sort();
            SongSpecifics.festivals.Sort();
            SongSpecifics.events.Sort();

            musicManager.selectMusic(SongSpecifics.getCurrentConditionalString());
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // menu closed
            if (e.NewMenu == null)
            {
                if (menuChangedMusic)
                    musicManager.selectMusic(SongSpecifics.getCurrentConditionalString());
            }

            // menu changed
            else
                musicManager.SelectMenuMusic(SongSpecifics.getCurrentConditionalString());
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            // THIS IS WAY TO LONG to run. Better make it save individual lists when I am editing songs.
            foreach (var musicPack in musicManager.MusicPacks)
                musicPack.Value.SaveSettings();
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == Config.KeyBinding && Game1.activeClickableMenu == null)
                Game1.activeClickableMenu = new Framework.Menus.MusicManagerMenu(Game1.viewport.Width, Game1.viewport.Height);
        }


        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // update delay timer
            if (e.IsOneSecond)
                musicManager?.UpdateTimer();

            // initiate music packs
            if (musicManager != null)
            {
                if (Config.DisableStardewMusic)
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
                    if (musicManager.CurrentMusicPack == null) return;
                    if (Game1.currentSong != null && musicManager.CurrentMusicPack.IsPlaying())
                    {
                        //ModMonitor.Log("STOP THE MUSIC!!!");
                        Game1.currentSong.Stop(AudioStopOptions.Immediate); //stop the normal songs from playing over the new songs
                        Game1.currentSong.Stop(AudioStopOptions.AsAuthored);
                        //Game1.nextMusicTrack = "";  //same as above line
                    }
                }
            }
        }

        /// <summary>Load the textures needed by the mod.</summary>
        public void LoadTextures()
        {
            Texture2DExtended LoadTexture(string name)
            {
                return new Texture2DExtended(this.Helper.Content.Load<Texture2D>($"assets/{name}"));
            }

            //Generic Icons
            textureManager.addTexture("MusicNote", LoadTexture("MusicNote.png"));
            textureManager.addTexture("MusicDisk", LoadTexture("MusicDisk.png"));
            textureManager.addTexture("MusicCD", LoadTexture("MusicDisk.png"));
            textureManager.addTexture("OutlineBox", LoadTexture("OutlineBox.png"));
            textureManager.addTexture("AddIcon", LoadTexture("AddButton.png"));
            textureManager.addTexture("DeleteIcon", LoadTexture("DeleteButton.png"));
            textureManager.addTexture("GreenBallon", LoadTexture("GreenBallon.png"));
            textureManager.addTexture("RedBallon", LoadTexture("RedBallon.png"));
            textureManager.addTexture("StarIcon", LoadTexture("StarIcon.png"));
            textureManager.addTexture("MenuIcon", LoadTexture("MenuIcon.png"));

            //Time Icons
            textureManager.addTexture("DayIcon", LoadTexture("TimeIcon_Day.png"));
            textureManager.addTexture("NightIcon", LoadTexture("TimeIcon_Night.png"));

            //Fun Icons
            textureManager.addTexture("EventIcon", LoadTexture("EventIcon.png"));
            textureManager.addTexture("FestivalIcon", LoadTexture("FestivalIcon.png"));

            //WeatherIcons
            textureManager.addTexture("SunnyIcon", LoadTexture("WeatherIcon_Sunny.png"));
            textureManager.addTexture("RainyIcon", LoadTexture("WeatherIcon_Rainy.png"));
            textureManager.addTexture("DebrisSpringIcon", LoadTexture("WeatherIcon_DebrisSpring.png"));
            textureManager.addTexture("DebrisSummerIcon", LoadTexture("WeatherIcon_DebrisSummer.png"));
            textureManager.addTexture("DebrisFallIcon", LoadTexture("WeatherIcon_DebrisFall.png"));
            textureManager.addTexture("WeatherFestivalIcon", LoadTexture("WeatherIcon_Festival.png"));
            textureManager.addTexture("SnowIcon", LoadTexture("WeatherIcon_Snowing.png"));
            textureManager.addTexture("StormIcon", LoadTexture("WeatherIcon_Stormy.png"));
            textureManager.addTexture("WeddingIcon", LoadTexture("WeatherIcon_WeddingHeart.png"));

            //Season Icons
            textureManager.addTexture("SpringIcon", LoadTexture("SeasonIcon_Spring.png"));
            textureManager.addTexture("SummerIcon", LoadTexture("SeasonIcon_Summer.png"));
            textureManager.addTexture("FallIcon", LoadTexture("SeasonIcon_Fall.png"));
            textureManager.addTexture("WinterIcon", LoadTexture("SeasonIcon_Winter.png"));

            //Day Icons
            textureManager.addTexture("MondayIcon", LoadTexture("DayIcons_Monday.png"));
            textureManager.addTexture("TuesdayIcon", LoadTexture("DayIcons_Tuesday.png"));
            textureManager.addTexture("WednesdayIcon", LoadTexture("DayIcons_Wednesday.png"));
            textureManager.addTexture("ThursdayIcon", LoadTexture("DayIcons_Thursday.png"));
            textureManager.addTexture("FridayIcon", LoadTexture("DayIcons_Friday.png"));
            textureManager.addTexture("SaturdayIcon", LoadTexture("DayIcons_Saturday.png"));
            textureManager.addTexture("SundayIcon", LoadTexture("DayIcons_Sunday.png"));

            textureManager.addTexture("HouseIcon", LoadTexture("HouseIcon.png"));

            textureManager.addTexture("PlayButton", LoadTexture("PlayButton.png"));
            textureManager.addTexture("StopButton", LoadTexture("StopButton.png"));
            textureManager.addTexture("BackButton", LoadTexture("BackButton.png"));
        }

        /// <summary>Load the available music packs.</summary>
        public void LoadMusicPacks()
        {
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                MusicPack musicPack = new MusicPack(contentPack);
                musicPack.SongInformation.initializeMenuMusic();
                musicPack.LoadSettings();
                musicManager.addMusicPack(musicPack, true, true);
            }
        }

        public static void DebugLog(string s)
        {
            if (Config.EnableDebugLog)
                ModMonitor.Log(s);
        }
    }
}
