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
using System.Linq;

namespace SimpleSoundManager.Framework
{
    /// <summary>A class that keeps track of the trigger and the list of songs associated with that trigger.</summary>
    internal class SongListNode
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The trigger name for the list of songs.</summary>
        public string Trigger { get; }

        /// <summary>The list of songs associated with a trigger.</summary>
        public string[] SongList { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="trigger">The trigger name for the list of songs.</param>
        /// <param name="songList">The list of songs associated with a trigger.</param>
        public SongListNode(string trigger, IEnumerable<string> songList)
        {
            this.Trigger = trigger;
            this.SongList = songList.ToArray();
        }
    }
}
