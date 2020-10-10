/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewValley;

namespace Pathoschild.Stardew.LookupAnything.Framework.Constants
{
    /// <summary>The direction a player is facing.</summary>
    internal enum FacingDirection
    {
        /// <summary>The player is facing the top of the screen.</summary>
        Up = Game1.up,

        /// <summary>The player is facing the right side of the screen.</summary>
        Right = Game1.right,

        /// <summary>The player is facing the bottom of the screen.</summary>
        Down = Game1.down,

        /// <summary>The player is facing the left side of the screen.</summary>
        Left = Game1.left
    }
}
