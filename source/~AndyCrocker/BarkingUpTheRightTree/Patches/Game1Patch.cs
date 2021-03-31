/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Reflection.Emit;

namespace BarkingUpTheRightTree.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="Game1"/> class.</summary>
    internal static class Game1Patch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="Game1.createRadialDebris(GameLocation, int, int, int, int, bool, int, bool, int)"/> method.</summary>
        /// <param name="debrisType">The type of debris that is about to be spawned.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (depending on if <paramref name="debrisType"/> is 21).</returns>
        /// <remarks>This is used for ignoring debris spawning for debris that has an id of 21.<br/>The reason for 21 is because <see cref="OpCodes.Ldc_I4_S"/> only accepts an <see langword="sbyte"/> and <see cref="sbyte.MaxValue"/> isn't big enough to be outside of the game object ids range. As such 21 was used as it's an unused id and isn't an 'aliased' id (check the <see langword="switch"/> in <see cref="Debris(int, int, Vector2, Vector2, float)"/> constructor for the 'aliased' types).</remarks>
        internal static bool CreateRadialDebrisPrefix(int debrisType) => debrisType != 21;

        /// <summary>The prefix for the <see cref="Game1.createObjectDebris(int, int, int, int, int, float, GameLocation)"/> and <see cref="Game1.createObjectDebris(int, int, int, long, StardewValley.GameLocation))"/> methods.</summary>
        /// <param name="debrisType">The type of debris that is about to be spawned.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (depending on if <paramref name="objectIndex"/> is 21).</returns>
        /// <remarks>This is used for ignoring debris spawning for debris that has an id of 21.<br/>The reason for 21 is because <see cref="OpCodes.Ldc_I4_S"/> only accepts an <see langword="sbyte"/> and <see cref="sbyte.MaxValue"/> isn't big enough to be outside of the game object ids range. As such 21 was used as it's an unused id and isn't an 'aliased' id (check the <see langword="switch"/> in <see cref="Debris(int, int, Vector2, Vector2, float)"/> constructor for the 'aliased' types).</remarks>
        internal static bool CreateObjectDebrisPrefix(int objectIndex) => objectIndex != 21;
    }
}
