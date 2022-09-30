/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewSymphonyRemastered.Framework;
using StardewSymphonyRemastered.Framework.V2;
using StardewValley;
using StardustCore.UIUtilities;

namespace StardewSymphonyRemastered
{
    // TODO:
    // 
    // Add mod config to have silent rain option.
    // Added in option to tie in wav sound volume to game sound options.
    //Add in way to see all selected options for a song in the menu?
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
        public static MusicManagerV2 musicManager;
        public static bool menuChangedMusic;
        public static Config Config;
        public static TextureManager textureManager;


        private float oldVolume=-5f;
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
            helper.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;

            helper.Events.Display.MenuChanged += this.OnMenuChanged;

            


            musicManager = new MusicManagerV2();
            textureManager = new TextureManager("StardewSymphony");
            this.LoadTextures();

            menuChangedMusic = false;


            //Initialize all of the lists upon creation during entry.
            SongSpecificsV2.initializeMenuList();
            SongSpecificsV2.initializeFestivalsList();


            this.LoadMusicPacks();
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (Game1.timeOfDay % 100 != 0) return; //Only check on the hour.
            if (musicManager.CurrentMusicPack != null)
            {
                //If there isn't another song already playing. Meaning a new song will play only if a different conditional is hit or this currently playing song finishes.
                if (musicManager.CurrentMusicPack.IsPlaying() == false)
                {
                    musicManager.selectMusic(SongSpecificsV2.getCurrentConditionalString());
                }
            }
            else
            {
                musicManager.selectMusic(SongSpecificsV2.getCurrentConditionalString());
            }
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                musicManager.selectMusic(SongSpecificsV2.getCurrentConditionalString(), true);
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //Locaion initialization MUST occur after load. Anything else can occur before.
            SongSpecificsV2.initializeLocationsList(); //Gets all Game locations once the player has loaded the game, and all buildings on the player's farm and adds them to a location list.

            foreach (var musicPack in musicManager.MusicPacks)
                musicPack.Value.LoadSettings();

            SongSpecificsV2.menus.Sort();
            SongSpecificsV2.locations.Sort();
            SongSpecificsV2.festivals.Sort();
            SongSpecificsV2.events.Sort();

            musicManager.selectMusic(SongSpecificsV2.getCurrentConditionalString());
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
                {
                    musicManager.selectMusic(SongSpecificsV2.getCurrentConditionalString());
                    menuChangedMusic = false;
                }
            }

            // menu changed
            else
                musicManager.SelectMenuMusic(SongSpecificsV2.getCurrentConditionalString());
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
                Game1.activeClickableMenu = new Framework.Menus.MusicManagerMenuV2(Game1.viewport.Width, Game1.viewport.Height);
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
                        Game1.requestedMusicTrack = "";  //same as above line
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

            //Update volume.0.
            
            if (this.oldVolume < 0f)
            {
                this.oldVolume = Game1.options.musicVolumeLevel;
            }
            if (this.oldVolume != Game1.options.musicVolumeLevel)
            {
                this.oldVolume = Game1.options.musicVolumeLevel;
                if (musicManager.CurrentMusicPack != null)
                {
                    if (musicManager.CurrentMusicPack.CurrentSound != null)
                    {
                        musicManager.CurrentMusicPack.CurrentSound.Volume = this.oldVolume;
                    }
                }
            }
        }

        /// <summary>Load the textures needed by the mod.</summary>
        public void LoadTextures()
        {
            Texture2DExtended LoadTexture(string name)
            {
                return new Texture2DExtended(this.Helper.DirectoryPath,this.ModManifest,Path.Combine(ModHelper.DirectoryPath,"assets",name));
            }

            textureManager.searchForTextures(this.Helper,this.ModManifest,Path.Combine("assets","Locations"));

            textureManager.addTexture("SaveIcon", LoadTexture("SaveIcon.png"));
            textureManager.addTexture("LastPage", LoadTexture("lastPageButton.png"));
            textureManager.addTexture("NextPage", LoadTexture("nextPageButton.png"));
            textureManager.addTexture("QuestionMark", LoadTexture("QuestionMark.png"));

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
            textureManager.addTexture("WeatherIcon", LoadTexture("WeatherIcon.png"));
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
            textureManager.addTexture("CalendarMonday", LoadTexture("CalendarMonday.png"));
            textureManager.addTexture("CalendarTuesday", LoadTexture("CalendarTuesday.png"));
            textureManager.addTexture("CalendarWednesday", LoadTexture("CalendarWednesday.png"));
            textureManager.addTexture("CalendarThursday", LoadTexture("CalendarThursday.png"));
            textureManager.addTexture("CalendarFriday", LoadTexture("CalendarFriday.png"));
            textureManager.addTexture("CalendarSaturday", LoadTexture("CalendarSaturday.png"));
            textureManager.addTexture("CalendarSunday", LoadTexture("CalendarSunday.png"));
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


            textureManager.addTexture("DayNightIcon", LoadTexture("DayNightIcon.png"));
            textureManager.addTexture("12AM", LoadTexture("12AM.png"));
            textureManager.addTexture("1AM", LoadTexture("1AM.png"));
            textureManager.addTexture("2AM", LoadTexture("2AM.png"));
            textureManager.addTexture("3AM", LoadTexture("3AM.png"));
            textureManager.addTexture("4AM", LoadTexture("4AM.png"));
            textureManager.addTexture("5AM", LoadTexture("5AM.png"));
            textureManager.addTexture("6AM", LoadTexture("6AM.png"));
            textureManager.addTexture("7AM", LoadTexture("7AM.png"));
            textureManager.addTexture("8AM", LoadTexture("8AM.png"));
            textureManager.addTexture("9AM", LoadTexture("9AM.png"));
            textureManager.addTexture("10AM", LoadTexture("10AM.png"));
            textureManager.addTexture("11AM", LoadTexture("11AM.png"));
            textureManager.addTexture("12PM", LoadTexture("12PM.png"));
            textureManager.addTexture("1PM", LoadTexture("1PM.png"));
            textureManager.addTexture("2PM", LoadTexture("2PM.png"));
            textureManager.addTexture("3PM", LoadTexture("3PM.png"));
            textureManager.addTexture("4PM", LoadTexture("4PM.png"));
            textureManager.addTexture("5PM", LoadTexture("5PM.png"));
            textureManager.addTexture("6PM", LoadTexture("6PM.png"));
            textureManager.addTexture("7PM", LoadTexture("7PM.png"));
            textureManager.addTexture("8PM", LoadTexture("8PM.png"));
            textureManager.addTexture("9PM", LoadTexture("9PM.png"));
            textureManager.addTexture("10PM", LoadTexture("10PM.png"));
            textureManager.addTexture("11PM", LoadTexture("11PM.png"));
        }

        /// <summary>Load the available music packs.</summary>
        public void LoadMusicPacks()
        {
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                MusicPackV2 musicPack = new MusicPackV2(contentPack);
                //musicPack.SongInformation.initializeMenuMusic();
                musicPack.LoadSettings();
                musicManager.addMusicPack(musicPack, true, true);
            }


        }

        /// <summary>
        /// Used to print messages to the SMAPI console.
        /// </summary>
        /// <param name="s"></param>
        public static void DebugLog(string s)
        {
            if (Config.EnableDebugLog)
                ModMonitor.Log(s);
        }
    }
}
