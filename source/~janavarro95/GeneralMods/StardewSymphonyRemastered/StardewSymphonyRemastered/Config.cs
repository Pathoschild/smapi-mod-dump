/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace StardewSymphonyRemastered
{
    /// <summary>A class that handles all of the config files for this mod.</summary>
    public class Config
    {

        /// <summary>
        /// If the current song hasn't finished playing just ignore a music swap until it finishes.
        /// </summary>
        public bool WaitForSongToFinishBeforeMusicSwap { get; set; } = true;

        /// <summary>The minimum delay between songs in milliseconds.</summary>
        public int MinimumDelayBetweenSongsInMilliseconds { get; set; } = 5000;

        /// <summary>The maximum delay between songs in milliseconds.</summary>
        public int MaximumDelayBetweenSongsInMilliseconds { get; set; } = 60000;

        /// <summary>The key binding to open the menu music.</summary>
        public SButton KeyBinding { get; set; } = SButton.L;

        /// <summary>Whether to completely disable the Stardew Valley OST.</summary>
        public bool DisableStardewMusic { get; set; } = false;

        /// <summary>Whether to show debug logs in the SMAPI console.</summary>
        public bool EnableDebugLog { get; set; } = false;

        /// <summary>
        /// Loctions that ignore the warp music change so that the music will continue to play from the previous location.
        /// </summary>
        public SortedDictionary<string, bool> LocationsToIgnoreWarpMusicChange { get; set; } = new SortedDictionary<string, bool>();
    }
}
