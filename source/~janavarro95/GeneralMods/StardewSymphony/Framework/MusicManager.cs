using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;

namespace Omegasis.StardewSymphony.Framework
{
    /// <summary>Data and metadata for a music pack.</summary>
    internal class MusicManager
    {
        /*********
        ** Properties
        *********/
        /// <summary>Writes messages to the console and log file.</summary>
        private readonly IMonitor Monitor;

        /// <summary>The valid season values.</summary>
        private readonly Season[] Seasons = Enum.GetValues(typeof(Season)).Cast<Season>().ToArray();


        /*********
        ** Accessors
        *********/
        /// <summary>The directory path containing the music.</summary>
        public string Directory { get; }

        /// <summary>The name of the wavebank file.</summary>
        public string WavebankName { get; }

        /// <summary>The name of the soundbank file.</summary>
        public string SoundbankName { get; }

        /// <summary>The loaded wavebank (if any).</summary>
        public WaveBank Wavebank { get; }

        /// <summary>The loaded soundbank (if any).</summary>
        public ISoundBank Soundbank { get; }

        /// <summary>The default music to play if there isn't a more specific option.</summary>
        public IDictionary<Season, List<Cue>> DefaultSongs = MusicManager.CreateSeasonalSongList();

        /// <summary>The music to play at night.</summary>
        public IDictionary<Season, List<Cue>> NightSongs = MusicManager.CreateSeasonalSongList();

        /// <summary>The music to play on rainy days.</summary>
        public IDictionary<Season, List<Cue>> RainySongs = MusicManager.CreateSeasonalSongList();

        /// <summary>The music to play on rainy nights.</summary>
        public IDictionary<Season, List<Cue>> RainyNightSongs = MusicManager.CreateSeasonalSongList();

        /// <summary>Songs that play in specific locations.</summary>
        public Dictionary<string, List<Cue>> LocationSongs { get; } = new Dictionary<string, List<Cue>>();

        /// <summary>Songs that play in specific locations on rainy days.</summary>
        public Dictionary<string, List<Cue>> LocationRainSongs { get; } = new Dictionary<string, List<Cue>>();

        /// <summary>Songs that play in specific locations at night.</summary>
        public Dictionary<string, List<Cue>> LocationNightSongs { get; } = new Dictionary<string, List<Cue>>();

        /// <summary>Songs that play in specific locations on rainy nights.</summary>
        public Dictionary<string, List<Cue>> LocationRainNightSongs { get; } = new Dictionary<string, List<Cue>>();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">Writes messages to the console and log file.</param>
        /// <param name="wavebank">The name of the wavebank file.</param>
        /// <param name="soundbank">The name of the soundbank file.</param>
        /// <param name="directory">The directory path containing the music.</param>
        public MusicManager(IMonitor monitor, string wavebank, string soundbank, string directory)
        {
            // init data
            this.Monitor = monitor;
            this.Directory = directory;
            this.WavebankName = wavebank + ".xwb";
            this.SoundbankName = soundbank + ".xsb";

            // init banks
            string wavePath = Path.Combine(this.Directory, this.WavebankName);
            string soundPath = Path.Combine(this.Directory, this.SoundbankName);

            this.Monitor.Log(wavePath);
            this.Monitor.Log(soundPath);

            if (File.Exists(wavePath))
                this.Wavebank = new WaveBank(Game1.audioEngine, wavePath);
            if (File.Exists(Path.Combine(this.Directory, this.SoundbankName)))
                this.Soundbank = (ISoundBank)new SoundBankWrapper(new SoundBank(Game1.audioEngine, soundPath));

            // update audio
            Game1.audioEngine.Update();
        }

        /// <summary>Read cue names from a text file and adds them to a specific list. Morphs with specific conditional name. Conditionals are hardcoded.</summary>
        /// <param name="conditionalName">The conditional file name to read.</param>
        /// <param name="cues">The music list to update.</param>
        public void LoadSeasonalSongs(string conditionalName, IDictionary<string, MusicManager> cues)
        {
            string path = Path.Combine(this.Directory, "Music_Files", "Seasons", conditionalName + ".txt");
            if (!File.Exists(path))
            {
                this.Monitor.Log($"Stardew Symohony:The specified music file could not be found. That music file is {conditionalName} which should be located at {path} but don't worry I'll create it for you right now. It's going to be blank though");
                string[] text = new string[3];
                text[0] = conditionalName + " music file. This file holds all of the music that will play when there is no music for this game location, or simply put this is default music. Simply type the name of the song below the wall of equal signs.";
                text[1] = "========================================================================================";

                File.WriteAllLines(path, text);
            }
            else
            {
                this.Monitor.Log($"Stardew Symphony:The music pack located at: {this.Directory} is processing the song info for the game location: {conditionalName}");

                string[] text = File.ReadAllLines(path);
                int i = 2;
                var lineCount = File.ReadLines(path).Count();

                while (i < lineCount) //the ordering seems bad, but it works.
                {
                    if (Convert.ToString(text[i]) == "")
                        break;
                    if (Convert.ToString(text[i]) == "\n")
                        break;

                    foreach (Season season in this.Seasons)
                    {
                        // default music
                        if ($"{season}".Equals(conditionalName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string cueName = Convert.ToString(text[i]);
                            i++;

                            this.DefaultSongs[season].Add(this.Soundbank.GetCue(cueName));
                            if (!cues.Keys.Contains(cueName))
                                cues.Add(cueName, this);
                        }

                        // night music
                        if ($"{season}_night".Equals(conditionalName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string cueName = Convert.ToString(text[i]);
                            i++;

                            this.NightSongs[season].Add(this.Soundbank.GetCue(cueName));
                            if (!cues.Keys.Contains(cueName))
                                cues.Add(cueName, this);
                        }

                        // rain music
                        if ($"{season}_rain".Equals(conditionalName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string cueName = Convert.ToString(text[i]);
                            i++;

                            this.RainySongs[season].Add(this.Soundbank.GetCue(cueName));
                            if (!cues.Keys.Contains(cueName))
                                cues.Add(cueName, this);
                        }

                        // rainy night
                        if ($"{season}_rain_night".Equals(conditionalName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string cueName = Convert.ToString(text[i]);
                            i++;

                            this.RainyNightSongs[season].Add(this.Soundbank.GetCue(cueName));
                            if (!cues.Keys.Contains(cueName))
                                cues.Add(cueName, this);
                        }
                    }
                }
                if (i == 2)
                {
                    //  Monitor.Log("Just thought that I'd let you know that there are no songs associated with the music file located at " + mylocation3 +" this may be intentional, but just incase you were wanted music, now you knew which ones were blank.");
                    return;
                }
                this.Monitor.Log($"Stardew Symohony:The music pack located at: {this.Directory} has successfully processed the song info for the game location {conditionalName}");
            }
        }

        /// <summary>Read cue names from a text file and adds them to a specific list. Morphs with specific conditional name. Conditionals are hardcoded.</summary>
        /// <param name="conditionalName">The conditional file name to read.</param>
        /// <param name="cues">The music list to update.</param>
        public void Music_Loader_Locations(string conditionalName, IDictionary<string, MusicManager> cues)
        {
            List<Cue> locationSongs = new List<Cue>();

            string path = Path.Combine(this.Directory, "Music_Files", "Locations", conditionalName + ".txt");
            if (!File.Exists(path))
            {
                this.Monitor.Log("StardewSymohony:A music list for the location " + conditionalName + " does not exist for the music pack located at " + path + " which isn't a problem, I just thought I'd let you know since this may have been intentional. Also I'm creating it for you just incase. Cheers.");

                string[] text = new string[3];//seems legit.
                text[0] = conditionalName + " music file. This file holds all of the music that will play when at this game location. Simply type the name of the song below the wall of equal signs.";
                text[1] = "========================================================================================";

                File.WriteAllLines(path, text);
            }
            else
            {
                this.Monitor.Log("Stardew Symphony:The music pack located at: " + this.Directory + " is processing the song info for the game location: " + conditionalName);
                string[] readtext = File.ReadAllLines(path);
                int i = 2;
                var lineCount = File.ReadLines(path).Count();
                while (i < lineCount) //the ordering seems bad, but it works.
                {
                    if (Convert.ToString(readtext[i]) == "")
                        break;
                    if (Convert.ToString(readtext[i]) == "\n")
                        break;
                    string cueName = Convert.ToString(readtext[i]);
                    i++;
                    if (!cues.Keys.Contains(cueName))
                    {
                        locationSongs.Add(this.Soundbank.GetCue(cueName));
                        cues.Add(cueName, this);
                    }
                    else
                        locationSongs.Add(this.Soundbank.GetCue(cueName));
                }
                if (i == 2)
                {
                    //  Monitor.Log("Just thought that I'd let you know that there are no songs associated with the music file located at " + mylocation3 + " this may be intentional, but just incase you were wanted music, now you knew which ones were blank.");
                    return;
                }
                if (locationSongs.Count > 0)
                {
                    this.LocationSongs.Add(conditionalName, locationSongs);
                    this.Monitor.Log("StardewSymhony:The music pack located at: " + this.Directory + " has successfully processed the song info for the game location: " + conditionalName);
                }
            }
        }

        /// <summary>Read cue names from a text file and adds them to a specific list. Morphs with specific conditional name. Conditionals are hardcoded.</summary>
        /// <param name="conditionalName">The conditional file name to read.</param>
        /// <param name="cues">The music list to update.</param>
        public void Music_Loader_Locations_Rain(string conditionalName, IDictionary<string, MusicManager> cues)
        {
            List<Cue> locationSongs = new List<Cue>();
            string path = Path.Combine(this.Directory, "Music_Files", "Locations", conditionalName + ".txt");
            if (!File.Exists(path))
            {
                this.Monitor.Log("StardewSymphony:A music list for the location " + conditionalName + " does not exist for the music pack located at " + path + " which isn't a problem, I just thought I'd let you know since this may have been intentional. Also I'm creating it for you just incase. Cheers.");
                string[] text = new string[3];//seems legit.
                text[0] = conditionalName + " music file. This file holds all of the music that will play when at this game location. Simply type the name of the song below the wall of equal signs.";
                text[1] = "========================================================================================";
                File.WriteAllLines(path, text);
            }
            else
            {
                // add in data here
                string[] readtext = File.ReadAllLines(path);
                int i = 2;
                var lineCount = File.ReadLines(path).Count();
                while (i < lineCount) //the ordering seems bad, but it works.
                {
                    if (Convert.ToString(readtext[i]) == "")
                        break;
                    if (Convert.ToString(readtext[i]) == "\n")
                        break;
                    string cueName = Convert.ToString(readtext[i]);
                    i++;
                    if (!cues.Keys.Contains(cueName))
                    {
                        locationSongs.Add(this.Soundbank.GetCue(cueName));
                        cues.Add(cueName, this);
                    }
                    else
                        locationSongs.Add(this.Soundbank.GetCue(cueName));
                }
                if (i == 2)
                {
                    // Monitor.Log("Just thought that I'd let you know that there are no songs associated with the music file located at " + mylocation3 + " this may be intentional, but just incase you were wanted music, now you knew which ones were blank.");
                    return;
                }
                if (locationSongs.Count > 0)
                {
                    this.LocationRainSongs.Add(conditionalName, locationSongs);
                    this.Monitor.Log("StardewSymohony:The music pack located at: " + this.Directory + " has successfully processed the song info for the game location: " + conditionalName);
                }
            }
        }

        /// <summary>Read cue names from a text file and adds them to a specific list. Morphs with specific conditional name. Conditionals are hardcoded.</summary>
        /// <param name="conditionalName">The conditional file name to read.</param>
        /// <param name="cues">The music list to update.</param>
        public void Music_Loader_Locations_Night(string conditionalName, IDictionary<string, MusicManager> cues)
        {
            List<Cue> locationSongs = new List<Cue>();
            string path = Path.Combine(this.Directory, "Music_Files", "Locations", conditionalName + ".txt");
            if (!File.Exists(path))
            {
                this.Monitor.Log("StardewSymphony:A music list for the location " + conditionalName + " does not exist for the music pack located at " + path + " which isn't a problem, I just thought I'd let you know since this may have been intentional. Also I'm creating it for you just incase. Cheers.");
                string[] text = new string[3];//seems legit.
                text[0] = conditionalName + " music file. This file holds all of the music that will play when at this game location. Simply type the name of the song below the wall of equal signs.";
                text[1] = "========================================================================================";
                File.WriteAllLines(path, text);
            }
            else
            {
                // add in data here
                string[] readtext = File.ReadAllLines(path);
                int i = 2;
                var lineCount = File.ReadLines(path).Count();

                while (i < lineCount) //the ordering seems bad, but it works.
                {
                    if (Convert.ToString(readtext[i]) == "")
                        break;
                    if (Convert.ToString(readtext[i]) == "\n")
                        break;
                    string cueName = Convert.ToString(readtext[i]);
                    i++;
                    if (!cues.Keys.Contains(cueName))
                    {
                        locationSongs.Add(this.Soundbank.GetCue(cueName));
                        cues.Add(cueName, this);
                    }
                    else
                        locationSongs.Add(this.Soundbank.GetCue(cueName));
                }
                if (i == 2)
                {
                    //  Monitor.Log("Just thought that I'd let you know that there are no songs associated with the music file located at " + mylocation3 + " this may be intentional, but just incase you were wanted music, now you knew which ones were blank.");
                    return;
                }
                if (locationSongs.Count > 0)
                {
                    this.LocationNightSongs.Add(conditionalName, locationSongs);
                    this.Monitor.Log("StardewSymphonyLThe music pack located at: " + this.Directory + " has successfully processed the song info for the game location: " + conditionalName);
                }
            }
        }

        /// <summary>Read cue names from a text file and adds them to a specific list. Morphs with specific conditional name. Conditionals are hardcoded.</summary>
        /// <param name="conditionalName">The conditional file name to read.</param>
        /// <param name="cues">The music list to update.</param>
        public void Music_Loader_Locations_Rain_Night(string conditionalName, IDictionary<string, MusicManager> cues)
        {
            List<Cue> locationSongs = new List<Cue>();
            var musicPath = this.Directory;

            string path = Path.Combine(musicPath, "Music_Files", "Locations", conditionalName + ".txt");
            if (!File.Exists(path))
            {
                this.Monitor.Log("StardewSymphony:A music list for the location " + conditionalName + " does not exist for the music pack located at " + path + " which isn't a problem, I just thought I'd let you know since this may have been intentional. Also I'm creating it for you just incase. Cheers.");
                string[] text = new string[3];//seems legit.
                text[0] = conditionalName + " music file. This file holds all of the music that will play when at this game location. Simply type the name of the song below the wall of equal signs.";
                text[1] = "========================================================================================";
                File.WriteAllLines(path, text);
            }
            else
            {
                //load in music stuff from the text files using the code below.
                string[] text = File.ReadAllLines(path);
                int i = 2;
                var lineCount = File.ReadLines(path).Count();

                while (i < lineCount) //the ordering seems bad, but it works.
                {
                    if (Convert.ToString(text[i]) == "") //if there is ever an empty line, stop processing the music file
                        break;
                    if (Convert.ToString(text[i]) == "\n")
                        break;
                    string cueName = Convert.ToString(text[i]);
                    i++;
                    if (!cues.Keys.Contains(cueName))
                    {
                        locationSongs.Add(this.Soundbank.GetCue(cueName));
                        cues.Add(cueName, this);
                    }
                    else
                        locationSongs.Add(this.Soundbank.GetCue(cueName));
                }
                if (i == 2)
                {
                    //  Monitor.Log("Just thought that I'd let you know that there are no songs associated with the music file located at " + mylocation3 + " this may be intentional, but just incase you were wanted music, now you knew which ones were blank.");
                    return;
                }

                if (locationSongs.Count > 0)
                {
                    this.LocationRainNightSongs.Add(conditionalName, locationSongs);
                    this.Monitor.Log("StardewSymohony:The music pack located at: " + this.Directory + " has successfully processed the song info for the game location: " + conditionalName);
                }
            }
        }

        /// <summary>Create a dictionary of seasonal songs.</summary>
        private static IDictionary<Season, List<Cue>> CreateSeasonalSongList()
        {
            IDictionary<Season, List<Cue>> dict = new Dictionary<Season, List<Cue>>();
            foreach (Season season in Enum.GetValues(typeof(Season)))
                dict[season] = new List<Cue>();
            return dict;
        }
    }
}
