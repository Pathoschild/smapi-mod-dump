using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSymphonyRemastered.Framework
{
    /// <summary>
    /// Stores information about what songs play when.
    /// </summary>
    public class SongSpecifics
    {
        public SortedDictionary<string, List<Song>> listOfSongsWithTriggers; //triggerName, <songs>. Seasonal music


        public List<Song> listOfSongsWithoutTriggers;


        public List<Song> festivalSongs;
        public List<Song> eventSongs;

        public static List<string> locations = new List<string>();
        public static List<string> festivals = new List<string>();
        public static List<string> events = new List<string>();

        /// <summary>
        /// Keeps track of the menus that support custom music with this mod.
        /// </summary>
        public static List<string> menus = new List<string>();

        string[] seasons;
        string[] weather;
        string[] daysOfWeek;
        string[] timesOfDay;
        public static char seperator = '_';

        /// <summary>
        /// Constructor.
        /// </summary>
        public SongSpecifics()
        {
            seasons = new string[]
            {
                "spring",
                "summer",
                "fall",
                "winter"
            };

            weather = new string[]
            {
                "sunny",
                "rainy",
                "debris",
                "lightning",
                "snow",
                "wedding"
            };
            daysOfWeek = new string[]
            {
                "sunday",
                "monday",
                "tuesday",
                "wednesday",
                "thursday",
                "friday",
                "saturday"
            };
            timesOfDay = new string[]
            {
                "day",
                "night"
            };


            listOfSongsWithTriggers = new SortedDictionary<string, List<Song>>();
            this.listOfSongsWithoutTriggers = new List<Song>();
            this.eventSongs = new List<Song>();
            this.festivalSongs = new List<Song>();

        }



        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //                         Static Methods                       //
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        #region

        /// <summary>
        /// Sum up some conditionals to parse the correct string key to access the songs list.
        /// </summary>
        /// <returns></returns>
        public static string getCurrentConditionalString()
        {
            string key = "";
            bool foundMenuString = false;
            //Event id's are the number found before the : for the event in Content/events/<location>.yaml file where location is the name of the stardew valley location.
            if (Game1.eventUp == true && Game1.CurrentEvent.isFestival==false)
            {
                //Get the event id an hijack it with some different music
                //String key="Event_EventName";

                var reflected = StardewSymphony.ModHelper.Reflection.GetField<int>(Game1.CurrentEvent, "id", true);

                int id = reflected.GetValue();
                key= id.ToString(); //get the event id. Really really messy.
                return key;
               
            }
            else if (Game1.isFestival())
            {
                //hijack the date of the festival and load some different songs
                // string s="Festival name"
                key = Game1.CurrentEvent.FestivalName;
                return key;
            }
            else if (Game1.activeClickableMenu!=null)
            {
                String name = Game1.activeClickableMenu.GetType().ToString().Replace('.', seperator);
                //Iterate through all of the potential menu options and check if it is valid.
                foreach (var menuNamespaceName in menus)
                {
                    if (name == menuNamespaceName)
                    {
                        key =name;
                        foundMenuString = true;
                        StardewSymphony.menuChangedMusic = true;
                        return key;
                    }
                }
                return ""; //No menu found so don't event try to change the music.

            }
            else
            {
                key = getSeasonNameString() + seperator + getWeatherString() + seperator + getTimeOfDayString() + seperator + getLocationString() + seperator + getDayOfWeekString();
            }

            if(foundMenuString==false && key == "")
            {
                key = getSeasonNameString() + seperator + getWeatherString() + seperator + getTimeOfDayString() + seperator + getLocationString() + seperator + getDayOfWeekString();
            }

            return key;
        }



        /// <summary>
        /// Initialize the location lists with the names of all of the major locations in the game.
        /// </summary>
        public static void initializeLocationsList()
        {
            //Give stardew symphony access to have unique music at any game location.
            foreach (var v in Game1.locations)
            {
                locations.Add(v.Name);
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("Adding in song triggers for location: " + v.Name);
            }

            //Try to get stardew symphony to recognize builds on the farm and try to give those buildings unique soundtracks as well.
            try
            {
                var farm = (Farm)Game1.getLocationFromName("Farm");
                foreach(var building in farm.buildings)
                {
                    locations.Add(building.nameOfIndoors);
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("Adding in song triggers for location: " + building.nameOfIndoors);
                }
            }
            catch(Exception err)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log(err.ToString());
            }

        }

        /// <summary>
        /// Initializes a list of the festivals included in vanilla stardew valley to be allowed to have custom music options.
        /// Initialized by festival name
        /// </summary>
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

        /// <summary>
        /// Add a specific new festival to the list. Must be in the format seasonDay.
        /// Ex) spring13
        /// Ex) fall27
        /// </summary>
        public static void addFestival(string name)
        {
            festivals.Add(name);
        }

        /// <summary>
        /// TODO: Get a list of all of the vanilla events in the game. But how to determine what event is playing is the question.
        /// </summary>
        /// <param name="name"></param>
        public static void initializeEventsList()
        {
            //Do some logic here
            //addEvent(12345.ToString());
        }

        /// <summary>
        /// TODO: Custom way to add in event to hijack music.
        /// </summary>
        /// <param name="name"></param>
        public static void addEvent(string id)
        {
            events.Add(id);
            //Do some logic here
        }

        /// <summary>
        /// Add a location to the loctaion list.
        /// </summary>
        /// <param name="name"></param>
        public static void addLocation(string name)
        {
            locations.Add(name);
        }

        /// <summary>
        /// Get the name of the day of the week from what game day it is.
        /// </summary>
        /// <returns></returns>
        public static string getDayOfWeekString()
        {
            int day = Game1.dayOfMonth;
            int dayOfWeek = day % 7;
            if (dayOfWeek == 0)
            {
                return "sunday";
            }
            if (dayOfWeek == 1)
            {
                return "monday";
            }
            if (dayOfWeek == 2)
            {
                return "tuesday";
            }
            if (dayOfWeek == 3)
            {
                return "wednesday";
            }
            if (dayOfWeek == 4)
            {
                return "thursday";
            }
            if (dayOfWeek == 5)
            {
                return "friday";
            }
            if (dayOfWeek == 6)
            {
                return "saturday";
            }
            return "";
        }

        /// <summary>
        /// Get the name of the current season
        /// </summary>
        /// <returns></returns>
        public static string getSeasonNameString()
        {
            return Game1.currentSeason.ToLower();
        }

        /// <summary>
        /// Get the name for the current weather outside.
        /// </summary>
        /// <returns></returns>
        public static string getWeatherString()
        {

            if (Game1.isRaining && Game1.isLightning==false) return "rainy";
            if (Game1.isLightning) return "lightning";
            if (Game1.isDebrisWeather) return "debris"; //????
            if (Game1.isSnowing) return "snow";
            if (Game1.weddingToday) return "wedding";
            
            return "sunny"; //If none of the other weathers, make it sunny.
        }

        /// <summary>
        /// Get the name for the time of day that it currently is.
        /// </summary>
        /// <returns></returns>
        public static string getTimeOfDayString()
        {
            if (Game1.timeOfDay < Game1.getModeratelyDarkTime()) return "day";
            else return "night";
        }

        /// <summary>
        /// Get the name of the location of where I am at.
        /// </summary>
        /// <returns></returns>
        public static string getLocationString()
        {
            try
            {
                return Game1.currentLocation.Name;
            }
            catch(Exception err)
            {
                err.ToString();
                return "";
            }
        }

        public static string getCurrentMenuString()
        {
            if (Game1.activeClickableMenu == null) return "";
            else return Game1.activeClickableMenu.GetType().ToString();
        }
        #endregion

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //                         Non-Static Methods                   //
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//


      
        
        #region
        /// <summary>
        /// Adds the song's reference to a music pack.
        /// </summary>
        /// <param name="songName">The FULL namespace of the menu. Example is StardewValley.Menus.TitleMenu</param>
        public void addSongToMusicPack(Song song)
        {
            this.listOfSongsWithoutTriggers.Add(song);
        }

        /// <summary>
        /// Initialize a basic list of menus supported.
        /// </summary>
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

        /// <summary>
        /// Add a menu to stardew symphony so that it may have unique music.
        /// </summary>
        /// <param name="name"></param>
        public static void addMenu(string name)
        {
            try
            {
                name = name.Replace('.', seperator); //Sanitize the name passed in to use my parsing conventions.
                menus.Add(name);
            }
            catch(Exception err)
            {
                err.ToString();
            }
        }

        /// <summary>
        /// Add amenu to stardew symphony so that it may have unique music.
        /// </summary>
        /// <param name="menuType">The type of menu to add in. Typically this is typeof(MyMenuClass)</param>
        public static void addMenu(Type menuType)
        {
            try
            {
                string name = menuType.ToString().Replace('.', seperator); //Sanitize the name passed in to use my parsing conventions.
                menus.Add(name); //Add the sanitized menu name to the list of menus that have custom music.
            }
            catch(Exception err)
            {
                err.ToString();
            }
        }

        /// <summary>
        /// Initialize the music packs with music from all passed in menus.
        /// </summary>
        public void initializeMenuMusic()
        {
            foreach(var v in menus)
            {
                try
                {
                    this.listOfSongsWithTriggers.Add(v, new List<Song>());
                }
                catch(Exception err)
                {
                    //err.ToString();
                }
            }
        }

        public void initializeFestivalMusic()
        {
            foreach (var v in festivals)
            {
                try
                {
                    this.listOfSongsWithTriggers.Add(v, new List<Song>());
                }
                catch (Exception err)
                {
                    err.ToString();
                }
            }
        }

        public void initializeEventMusic()
        {
            foreach (var v in events)
            {
                try
                {
                    this.listOfSongsWithTriggers.Add(v, new List<Song>());
                }
                catch (Exception err)
                {
                    err.ToString();
                }
            }
        }

   

        /// <summary>
        /// Checks if the song exists at all in this music pack.
        /// </summary>
        /// <param name="songName"></param>
        /// <returns></returns>
        public bool isSongInList(string songName)
        {
            Song s = getSongFromList(listOfSongsWithoutTriggers, songName);
            if (s == null)
            {
                return false;
            }
            return listOfSongsWithoutTriggers.Contains(s);
        }

        /// <summary>
        /// A pretty big function to add in all of the specific songs that play at certain locations_seasons_weather_dayOfWeek_times. 
        /// </summary>
        public void initializeSeasonalMusic()
        {
            foreach(var season in seasons)
            {
                listOfSongsWithTriggers.Add(season, new List<Song>());
                foreach(var weather in this.weather)
                {
                    listOfSongsWithTriggers.Add(season + seperator + weather,new List<Song>());
                    foreach(var time in this.timesOfDay)
                    {
                        listOfSongsWithTriggers.Add(season + seperator + weather+seperator+time, new List<Song>());
                        foreach(var loc in locations)
                        {
                            listOfSongsWithTriggers.Add(season + seperator + weather + seperator + time+seperator+loc, new List<Song>());
                            foreach(var day in this.daysOfWeek)
                            {
                                listOfSongsWithTriggers.Add(season + seperator + weather + seperator + time + seperator + loc+seperator+day, new List<Song>());
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Used to access the master list of songs this music pack contains.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public KeyValuePair<string,List<Song>>getSongList(string key)
        {
            string keyPhrase = "";
            string keyPhraseInfo = "";
            try
            {
                 keyPhrase= key.Split(seperator).ElementAt(0);
                 keyPhraseInfo= key.Split(seperator).ElementAt(1);
            }
            catch(Exception err)
            {
                err.ToString();
                 keyPhrase = key;
            }
            
                //This is just the plain song name with no extra info.
                foreach(KeyValuePair<string,List<Song>> pair in listOfSongsWithTriggers)
                {
                    //StardewSymphony.ModMonitor.Log(pair.Key);
                    if (pair.Key == key) return pair;
                }

            //return new KeyValuePair<string, List<string>>(key, listOfSongsWithTriggers[key]);

            return new KeyValuePair<string, List<Song>>("",null);
        }

        public List<Song> getFestivalMusic()
        {
            return this.festivalSongs;
        }

        public List<Song> getEventMusic()
        {
            return this.eventSongs;
        }

        public List<Song> getMenuMusic()
        {
            return null;
            //return this.menuSongs();
        }

        /// <summary>
        /// Add a song name to a specific list of songs to play that will play under certain conditions.
        /// </summary>
        /// <param name="songListKey"></param>
        /// <param name="songName"></param>
        public void addSongToTriggerList(string songListKey,string songName)
        {

            var songKeyPair = getSongList(songListKey); //Get the trigger list

            var song = getSongFromList(listOfSongsWithoutTriggers, songName); //Get the song from the master song pool
            if (song == null)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("For some reason you are trying to add a song that is null. The name of the song is "+ songName);
                return;
            }
            songKeyPair.Value.Add(song); //add the song from master pool to the trigger list
        }

        public void addSongToFestivalList(string songName)
        {

            var songKeyPair = this.festivalSongs;

            var song = getSongFromList(listOfSongsWithoutTriggers, songName); //Get the song from the master song pool
            if (song == null)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("For some reason you are trying to add a song that is null. The name of the song is " + songName);
                return;
            }
            songKeyPair.Add(song); //add the song from master pool to the trigger list
        }

        public void addSongToEventList(string songName)
        {

            var songKeyPair = this.eventSongs;

            var song = getSongFromList(listOfSongsWithoutTriggers, songName); //Get the song from the master song pool
            if (song == null)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("For some reason you are trying to add a song that is null. The name of the song is " + songName);
                return;
            }
            songKeyPair.Add(song); //add the song from master pool to the trigger list
        }

        /// <summary>
        /// Remove a song name from a specific list of songs to play that will play under certain conditions.
        /// </summary>
        /// <param name="songListKey"></param>
        /// <param name="songName"></param>
        public void removeSongFromTriggerList(string songListKey,string songName)
        {
            var songKeyPair = getSongList(songListKey);
            var song = getSongFromList(songKeyPair.Value, songName);
            songKeyPair.Value.Remove(song);
        }

        /// <summary>
        /// Get the Song instance that is referenced with the song's name.
        /// </summary>
        /// <param name="songList"></param>
        /// <param name="songName"></param>
        /// <returns></returns>
        public Song getSongFromList(List<Song> songList,string songName)
        {
            foreach (var song in songList)
            {
                if (song.name == songName)
                {
                    return song;
                }
            }
            return null;
        }
        #endregion
    }
}
