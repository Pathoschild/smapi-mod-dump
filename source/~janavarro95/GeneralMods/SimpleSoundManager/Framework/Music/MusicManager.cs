using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework.Audio;
using StardewValley;

namespace SimpleSoundManager.Framework
{
    /// <summary>Manages all music for the mod.</summary>
    public class MusicManager
    {
        /*********
        ** Fields
        *********/
        /// <summary>The RNG used to select music packs and songs.</summary>
        private readonly Random Random = new Random();

        /// <summary>The delay timer between songs.</summary>
        private readonly Timer Timer = new Timer();


        /*********
        ** Accessors
        *********/
        /// <summary>The loaded music packs.</summary>
        public IDictionary<string, MusicPack> MusicPacks { get; } = new Dictionary<string, MusicPack>();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public MusicManager()
        {
            
        }

        /// <summary>Adds a valid xwb music pack to the list of music packs available.</summary>
        /// <param name="musicPack">The music pack to add.</param>
        /// <param name="displayLogInformation">Whether or not to display the process to the console. Will include information from the pack's metadata. Default:False</param>
        /// <param name="displaySongs">If displayLogInformation is also true this will display the name of all of the songs in the music pack when it is added in.</param>
        public void addMusicPack(MusicPack musicPack, bool displayLogInformation = false, bool displaySongs = false)
        {
            if (displayLogInformation)
            {
                if (ModCore.Config.EnableDebugLog)
                {
                    ModCore.ModMonitor.Log("Adding music pack:");
                    ModCore.ModMonitor.Log($"   Name: {musicPack.Name}");
                    ModCore.ModMonitor.Log($"   Author: {musicPack.Manifest.Author}");
                    ModCore.ModMonitor.Log($"   Description: {musicPack.Manifest.Description}");
                    ModCore.ModMonitor.Log($"   Version Info: {musicPack.Manifest.Version}");
                }
                if (displaySongs && ModCore.Config.EnableDebugLog)
                {
                    ModCore.ModMonitor.Log("    Song List:");
                    foreach (string songName in musicPack.Sounds.Keys)
                        ModCore.ModMonitor.Log($"        {songName}");
                }
            }

            this.MusicPacks.Add(musicPack.Name, musicPack);
        }

        /// <summary>
        /// Plays the specified sound from the music pack.
        /// </summary>
        /// <param name="packName"></param>
        /// <param name="soundName"></param>
        public void playSound(string packName, string soundName)
        {
            if (this.MusicPacks.ContainsKey(packName))
            {
                this.MusicPacks[packName].PlaySound(soundName);
            }
            else
            {
                ModCore.DebugLog("No pack with specified key/name: " + packName);
            }
        }

        /// <summary>
        /// Stops a said sound from the music pack.
        /// </summary>
        /// <param name="packName"></param>
        /// <param name="soundName"></param>
        public void stopSound(string packName,string soundName)
        {
            if (this.MusicPacks.ContainsKey(packName))
            {
                this.MusicPacks[packName].StopSound();
            }
            else
            {
                ModCore.DebugLog("No pack with specified key/name: " + packName);
            }
        }

        /// <summary>
        /// Updates all music packs every so often.
        /// </summary>
        public void update()
        {
            foreach(MusicPack pack in this.MusicPacks.Values)
            {
                pack.update();
            }
        }
    }
}
