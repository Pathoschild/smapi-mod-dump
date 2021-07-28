/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace SleepyEye.Framework
{
    /// <summary>Metadata about where and when the player camped.</summary>
    internal class CampData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the game location.</summary>
        public string Location { get; set; }

        /// <summary>The player's map pixel position.</summary>
        public Vector2 Position { get; set; }

        /// <summary>The mine level, if applicable.</summary>
        public int? MineLevel { get; set; }

        /// <summary>The <see cref="WorldDate.TotalDays"/> value when the player slept.</summary>
        public int DaysPlayed { get; set; }
    }
}
