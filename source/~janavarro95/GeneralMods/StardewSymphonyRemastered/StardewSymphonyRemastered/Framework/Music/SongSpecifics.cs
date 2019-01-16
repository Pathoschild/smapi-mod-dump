using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using static System.String;

namespace StardewSymphonyRemastered.Framework
{
    /// <summary>Stores information about what songs play when.</summary>
    public class SongSpecifics
    {
        public SortedDictionary<string, List<string>> listOfSongsWithTriggers; //triggerName, <songs>. Seasonal music

        public List<string> listOfSongsWithoutTriggers;

        public List<string> festivalSongs;
        public List<string> eventSongs;

        public static List<string> locations = new List<string>();
        public static List<string> festivals = new List<string>();
        public static List<string> events = new List<string>();

        /// <summary>Keeps track of the menus that support custom music with this mod.</summary>
        public static List<string> menus = new List<string>();

        private readonly string[] seasons;
        private readonly string[] weather;
        private readonly string[] daysOfWeek;
        private readonly string[] timesOfDay;
        public static char seperator = '_';

        /// <summary>Construct an instance.</summary>
        public SongSpecifics()
        {
            this.seasons = new[]
            {
                "spring",
                "summer",
                "fall",
                "winter"
            };

            this.weather = new[]
            {
                "sunny",
                "rain",
                "debris",
                "lightning",
                "snow",
                "festival",
                "wedding"
            };
            this.daysOfWeek = new[]
            {
                "sunday",
                "monday",
                "tuesday",
                "wednesday",
                "thursday",
                "friday",
                "saturday"
            };
            this.timesOfDay = new[]
            {
                "day",
                "night",
                "12A.M.",
                "1A.M.",
                "2A.M.",
                "3A.M.",
                "4A.M.",
                "5A.M.",
                "6A.M.",
                "7A.M.",
                "8A.M.",
                "9A.M.",
                "10A.M.",
                "11A.M.",
                "12P.M.",
                "1P.M.",
                "2P.M.",
                "3P.M.",
                "4P.M.",
                "5P.M.",
                "6P.M.",
                "7P.M.",
                "8P.M.",
                "9P.M.",
                "10P.M.",
                "11P.M.",
            };


            this.listOfSongsWithTriggers = new SortedDictionary<string, List<string>>();
            this.listOfSongsWithoutTriggers = new List<string>();
            this.eventSongs = new List<string>();
            this.festivalSongs = new List<string>();

        }



        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //                         Static Methods                       //
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        #region

        /// <summary>Sum up some conditionals to parse the correct string key to access the songs list.</summary>
        public static string getCurrentConditionalString(bool getJustLocation = false)
        {
            string key;
            //Event id's are the number found before the : for the event in Content/events/<location>.yaml file where location is the name of the stardew valley location.

            if (!getJustLocation)
            {
                if (Game1.eventUp && !Game1.CurrentEvent.isFestival)
                {
                    //Get the event id an hijack it with some different music
                    //String key="Event_EventName";

                    var reflected = StardewSymphony.ModHelper.Reflection.GetField<int>(Game1.CurrentEvent, "id");

                    int id = reflected.GetValue();
                    key = id.ToString(); //get the event id. Really really messy.
                    return key;

                }
                else if (Game1.isFestival())
                {
                    //hijack the date of the festival and load some different songs
                    // string s="Festival name"
                    key = Game1.CurrentEvent.FestivalName;
                    return key;
                }
                else if (Game1.activeClickableMenu != null)
                {
                    string name = Game1.activeClickableMenu.GetType().ToString().Replace('.', seperator);
                    //Iterate through all of the potential menu options and check if it is valid.
                    foreach (string menuNamespaceName in menus)
                    {
                        if (name == menuNamespaceName)
                        {
                            key = name;
                            StardewSymphony.menuChangedMusic = true;
                            return key;
                        }
                    }
                    return ""; //No menu found so don't event try to change the music.

                }
                else
                {
                    key = getSeasonNameString() + seperator + getWeatherString() + seperator + getTimeOfDayString(true) + seperator + getLocationString() + seperator + getDayOfWeekString();

                    if (StardewSymphony.musicManager.GetApplicableMusicPacks(key).Count == 0)
                    {
                        key = getSeasonNameString() + seperator + getWeatherString() + seperator + getTimeOfDayString(false) + seperator + getLocationString() + seperator + getDayOfWeekString();
                    }
                }
            }
            else
                key = getLocationString();

            return key;
        }



        /// <summary>Initialize the location lists with the names of all of the major locations in the game.</summary>
        public static void initializeLocationsList()
        {
            //Give stardew symphony access to have unique music at any game location.
            foreach (var v in Game1.locations)
            {
                locations.Add(v.Name);
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("Adding in song triggers for location: " + v.Name);
            }

            locations.Add("UndergroundMine Floors 1-39");
            locations.Add("UndergroundMine Floors 40-69");
            locations.Add("UndergroundMine Floors 70-79");
            locations.Add("UndergroundMine Floors 80-120");

            //Try to get stardew symphony to recognize builds on the farm and try to give those buildings unique soundtracks as well.
            try
            {
                var farm = (Farm)Game1.getLocationFromName("Farm");
                foreach (var building in farm.buildings)
                {
                    if (IsNullOrEmpty(building.nameOfIndoors) || locations.Contains(building.nameOfIndoors))
                        continue;

                    locations.Add(building.nameOfIndoors);
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("Adding in song triggers for location: " + building.nameOfIndoors);
                }
            }
            catch (Exception err)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log(err.ToString());
            }
        }

        /// <summary>Initializes a list of the festivals included in vanilla stardew valley to be allowed to have custom music options. Initialized by festival name.</summary>
        public static void initializeFestivalsList()
        {
            addFestival("Egg Festival"); //Egg festival
            addFestival("Flower Dance"); //Flower dance
            addFestival("Luau"); //luau
            addFestival("Dance Of The Moonlight Jellies"); //moonlight jellies
            addFestival("Stardew Valley Fair"); //fall fair
            addFestival("Spirit's Eve"); //spirits eve
            addFestival("Festival of Ice"); //festival of ice
            addFestival("Feast of the Winter Star"); //festival of winter star
        }

        /// <summary>Add a specific new festival to the list. Must be in the format seasonDay. For example, spring13 or fall27.</summary>
        public static void addFestival(string name)
        {
            festivals.Add(name);
        }

        /// <summary>Custom way to add in event to hijack music.</summary>
        public static void addEvent(string id)
        {
            events.Add(id);
            //Do some logic here
        }

        /// <summary>Add a location to the loctaion list.</summary>
        public static void addLocation(string name)
        {
            locations.Add(name);
        }

        /// <summary>Get the name of the day of the week from what game day it is.</summary>
        public static string getDayOfWeekString()
        {
            int dayOfWeek = Game1.dayOfMonth % 7;
            switch (dayOfWeek)
            {
                case 0:
                    return "sunday";
                case 1:
                    return "monday";
                case 2:
                    return "tuesday";
                case 3:
                    return "wednesday";
                case 4:
                    return "thursday";
                case 5:
                    return "friday";
                case 6:
                    return "saturday";
                default:
                    return "";
            }
        }

        /// <summary>Get the name of the current season</summary>
        public static string getSeasonNameString()
        {
            return Game1.currentSeason.ToLower();
        }

        /// <summary>Get the name for the current weather outside.</summary>
        public static string getWeatherString()
        {

            if (Game1.isRaining && !Game1.isLightning)
                return "rain";
            if (Game1.isLightning)
                return "lightning";
            if (Game1.isDebrisWeather)
                return "debris"; //????
            if (Game1.isSnowing)
                return "snow";
            if (Game1.weddingToday)
                return "wedding";
            return "sunny"; //If none of the other weathers, make it sunny.
        }

        /// <summary>Get the name for the time of day that it currently is.</summary>
        public static string getTimeOfDayString(bool hourly)
        {
            if (!hourly)
            {
                return Game1.timeOfDay < Game1.getModeratelyDarkTime()
                    ? "day"
                    : "night";
            }
            else
            {
                int hour = Game1.timeOfDay / 100;
                string suffix = hour < 12 && hour >= 24 ? "A.M." : "P.M.";
                return hour + suffix;
            }
        }

        /// <summary>Get the name of the location of where I am at.</summary>
        public static string getLocationString()
        {
            try
            {
                string locName = Game1.currentLocation.Name;
                if (locName.StartsWith("UndergroundMine"))
                {
                    StardewSymphony.DebugLog("LOC VALUE:" + locName);
                    string splits = locName.Replace("UndergroundMine", "");
                    StardewSymphony.DebugLog("DEBUG VALUE:" + splits);
                    int number = Convert.ToInt32(splits);
                    if (number >= 1 && number <= 39)
                        return "UndergroundMine" + " Floors 1-39";
                    if (number >= 40 && number <= 69)
                        return "UndergroundMine" + " Floors 40-69";
                    if (number >= 70 && number <= 79)
                        return "UndergroundMine" + " Floors 70-79";
                    if (number >= 80 && number <= 120)
                        return "UndergroundMine" + " Floors 80-120";
                }

                if (locName.Contains("Cabin") || Game1.currentLocation.isFarmBuildingInterior())
                    locName = Game1.currentLocation.uniqueName;

                return locName;
            }
            catch (Exception err)
            {
                StardewSymphony.ModMonitor.Log(err.ToString());
                return "";
            }
        }

        #endregion

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //                         Non-Static Methods                   //
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//


        #region
        /// <summary>Initialize a basic list of menus supported.</summary>
        public static void initializeMenuList()
        {
            addMenu(typeof(StardewValley.Menus.TitleMenu)); //Of course!
            addMenu(typeof(StardewValley.Menus.AboutMenu)); //Sure, though I doubt many people look at this menu.
            addMenu(typeof(StardewValley.Menus.Billboard));  //The billboard in town.
            addMenu(typeof(StardewValley.Menus.BlueprintsMenu)); // the crafting menu.
            //addMenu(typeof(StardewValley.Menus.BobberBar)); //Fishing.
            addMenu(typeof(StardewValley.Menus.Bundle)); //Definitely could be fun. Custom bundle menu music.
            addMenu(typeof(StardewValley.Menus.CarpenterMenu)); //Building a thing with robbin
            addMenu(typeof(StardewValley.Menus.CataloguePage)); //???
            addMenu(typeof(StardewValley.Menus.CharacterCustomization)); //Yea!
            addMenu(typeof(StardewValley.Menus.CollectionsPage));
            addMenu(typeof(StardewValley.Menus.CoopMenu));
            addMenu(typeof(StardewValley.Menus.CraftingPage));
            addMenu(typeof(StardewValley.Menus.Fish)); //Music when fishing
            addMenu(typeof(StardewValley.Menus.GameMenu)); //Err default inventory page?
            addMenu(typeof(StardewValley.Menus.GeodeMenu));  //Flint
            addMenu(typeof(StardewValley.Menus.LoadGameMenu)); //Loading the game.
            addMenu(typeof(StardewValley.Menus.LevelUpMenu)); //Leveling up
            addMenu(typeof(StardewValley.Menus.LetterViewerMenu)); //Viewing your mail
            addMenu(typeof(StardewValley.Menus.MapPage)); //Looking at the map
            addMenu(typeof(StardewValley.Menus.MuseumMenu)); //Arranging things in the museum
            addMenu(typeof(StardewValley.Menus.NamingMenu)); //Naming an animal
            addMenu(typeof(StardewValley.Menus.PurchaseAnimalsMenu)); //Buying an animal.
            addMenu(typeof(StardewValley.Menus.SaveGameMenu)); //Saving the game / end of night
            addMenu(typeof(StardewValley.Menus.ShippingMenu)); //Shipping screen.
            addMenu(typeof(StardewValley.Menus.ShopMenu)); //Buying things

        }

        /// <summary>Add amenu to stardew symphony so that it may have unique music.</summary>
        /// <param name="menuType">The type of menu to add in. Typically this is typeof(MyMenuClass)</param>
        public static void addMenu(Type menuType)
        {
            try
            {
                string name = menuType.ToString().Replace('.', seperator); //Sanitize the name passed in to use my parsing conventions.
                menus.Add(name); //Add the sanitized menu name to the list of menus that have custom music.
            }
            catch { }
        }

        /// <summary>Initialize the music packs with music from all passed in menus.</summary>
        public void initializeMenuMusic()
        {
            foreach (string v in menus)
            {
                try
                {
                    this.listOfSongsWithTriggers.Add(v, new List<string>());
                }
                catch { }
            }
        }

        public void initializeFestivalMusic()
        {
            foreach (string v in festivals)
            {
                try
                {
                    this.listOfSongsWithTriggers.Add(v, new List<string>());
                }
                catch { }
            }
        }

        public void initializeEventMusic()
        {
            foreach (string v in events)
            {
                try
                {
                    this.listOfSongsWithTriggers.Add(v, new List<string>());
                }
                catch { }
            }
        }

        /// <summary>A pretty big function to add in all of the specific songs that play at certain locations_seasons_weather_dayOfWeek_times. </summary>
        public void initializeSeasonalMusic()
        {
            foreach (string loc in locations)
                this.listOfSongsWithTriggers.Add(loc, new List<string>());

            foreach (string season in this.seasons)
            {
                this.listOfSongsWithTriggers.Add(season, new List<string>());
                foreach (string weather in this.weather)
                {
                    this.listOfSongsWithTriggers.Add(season + seperator + weather, new List<string>());
                    foreach (string time in this.timesOfDay)
                    {
                        this.listOfSongsWithTriggers.Add(season + seperator + weather + seperator + time, new List<string>());
                        foreach (string loc in locations)
                        {
                            this.listOfSongsWithTriggers.Add(season + seperator + weather + seperator + time + seperator + loc, new List<string>());
                            foreach (string day in this.daysOfWeek)
                            {
                                this.listOfSongsWithTriggers.Add(season + seperator + weather + seperator + time + seperator + loc + seperator + day, new List<string>());
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Used to access the master list of songs this music pack contains.</summary>
        public KeyValuePair<string, List<string>> getSongList(string key)
        {
            if (!this.listOfSongsWithTriggers.ContainsKey(key))
                return new KeyValuePair<string, List<string>>("", null);

            //This is just the plain song name with no extra info.
            foreach (KeyValuePair<string, List<string>> pair in this.listOfSongsWithTriggers)
            {
                //StardewSymphony.ModMonitor.Log(pair.Key);
                if (pair.Key == key)
                    return pair;
            }

            return new KeyValuePair<string, List<string>>("", null);
        }

        public List<string> getFestivalMusic()
        {
            return this.festivalSongs;
        }

        public List<string> getEventMusic()
        {
            return this.eventSongs;
        }

        /// <summary>Add a song name to a specific list of songs to play that will play under certain conditions.</summary>
        public void addSongToTriggerList(string songListKey, string songName)
        {
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log(songListKey);

            var songKeyPair = this.getSongList(songListKey); //Get the trigger list
            if (songKeyPair.Value == null)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("For some reason you are trying to add a song to a list that is null. The name of the song list is " + songListKey);
                return;
            }
            string song = this.getSongFromList(this.listOfSongsWithoutTriggers, songName); //Get the song from the master song pool
            if (song == null)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("For some reason you are trying to add a song that is null. The name of the song is " + songName);
                return;
            }
            songKeyPair.Value.Add(song); //add the song from master pool to the trigger list
        }

        public void addSongToFestivalList(string songName)
        {
            string song = this.getSongFromList(this.listOfSongsWithoutTriggers, songName); //Get the song from the master song pool
            if (song == null)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("For some reason you are trying to add a song that is null. The name of the song is " + songName);
                return;
            }
            this.festivalSongs.Add(song); //add the song from master pool to the trigger list
        }

        public void addSongToEventList(string songName)
        {
            var songKeyPair = this.eventSongs;

            string song = this.getSongFromList(this.listOfSongsWithoutTriggers, songName); //Get the song from the master song pool
            if (song == null)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("For some reason you are trying to add a song that is null. The name of the song is " + songName);
                return;
            }
            songKeyPair.Add(song); //add the song from master pool to the trigger list
        }

        /// <summary>Remove a song name from a specific list of songs to play that will play under certain conditions.</summary>
        public void removeSongFromTriggerList(string songListKey, string songName)
        {
            var songKeyPair = this.getSongList(songListKey);
            string song = this.getSongFromList(songKeyPair.Value, songName);
            songKeyPair.Value.Remove(song);
        }

        /// <summary>Remove a song from the event list.</summary>
        public void removeSongFromEventList(string songName)
        {
            string song = this.getSongFromList(this.eventSongs, songName);
            this.eventSongs.Remove(song);
        }

        /// <summary>Remove a song from the festival list.</summary>
        public void removeSongFromFestivalList(string songName)
        {
            string song = this.getSongFromList(this.festivalSongs, songName);
            this.festivalSongs.Remove(song);
        }

        /// <summary>Get the Song instance that is referenced with the song's name.</summary>
        public string getSongFromList(List<string> songList, string songName)
        {
            return songList.FirstOrDefault(p => p == songName);
        }
        #endregion
    }
}
