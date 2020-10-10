/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.DebugMode.Framework
{
    /// <summary>The direction a player is facing.</summary>
    /// <remarks>Derived from <see cref="StardewValley.Character.setMovingInFacingDirection"/>.</remarks>
    public enum PlayerDirection
    {
        /// <summary>The player is facing up (with their back to the camera).</summary>
        Up = 0,

        /// <summary>The player is right (with their right side to the camera).</summary>
        Right = 1,

        /// <summary>The player is down (with their face to the camera).</summary>
        Down = 2,

        /// <summary>The player is left (with their left side to the camera).</summary>
        Left = 3
    }
}
