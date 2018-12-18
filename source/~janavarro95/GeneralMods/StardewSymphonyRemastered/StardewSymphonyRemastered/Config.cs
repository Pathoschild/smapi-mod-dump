using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSymphonyRemastered
{
    /// <summary>
    /// A class that handles all of the config files for this mod.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Whether or not to display debug log information on the SMAPI console for this mod.
        /// </summary>
        public bool EnableDebugLog { get; set; }=false;

        /// <summary>
        /// The minimum delay between songs in terms of milliseconds.
        /// </summary>
        public int MinimumDelayBetweenSongsInMilliseconds { get; set; }=5000;

        /// <summary>
        /// The maximum delay between songs in terms of milliseconds.
        /// </summary>
        public int MaximumDelayBetweenSongsInMilliseconds { get; set; }=60000;

        /// <summary>
        /// The key binding to open up the menu music.
        /// </summary>
        public string KeyBinding { get; set; }="L";
        
        /// <summary>
        /// Used to write a .json file for every possible option for a music pack. Use at your own risk!
        /// </summary>
        public bool writeAllConfigMusicOptions { get; set; } = false;

        /// <summary>
        /// Used to completely disable the Stardew Valley OST.
        /// </summary>
        public bool disableStardewMusic { get; set; } = false;
    }
}
