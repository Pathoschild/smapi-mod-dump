/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Slothsoft.Challenger.Common;

internal static class CommonHelpers {
    /// <summary>
    /// Gets the map tile the cursor is over.
    /// "Inspired" by <a href="https://github.com/ImJustMatt/StardewMods">ImJustMatt</a>.
    /// </summary>
    /// <param name="radius">The tile distance from the player.</param>
    /// <param name="fallback">Fallback to grab tile if cursor tile is out of range.</param>
    /// <returns>Returns the tile position.</returns>
    public static Vector2 GetCursorTile(int radius = 0, bool fallback = true) {
        if (radius == 0) {
            return Game1.lastCursorTile;
        }

        var pos = Game1.GetPlacementGrabTile();
        pos.X = (int)pos.X;
        pos.Y = (int)pos.Y;

        if (fallback && !Utility.tileWithinRadiusOfPlayer((int)pos.X, (int)pos.Y, radius, Game1.player)) {
            pos = Game1.player.GetGrabTile();
        }

        return pos;
    }

    /// <summary>
    /// Joins the list in the way it is displayed in <code>ChallengeMenu</code>.
    /// </summary>
    /// <param name="strings"></param>
    /// <returns></returns>
    
    public static string ToListString(IEnumerable<string> strings) {
        return strings.Aggregate("", (current, s) => current + ToListString(s));
    }
    
    /// <summary>
    /// Prepares the string in the way it is displayed in <code>ChallengeMenu</code>.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    
    public static string ToListString(string s) {
        return "-  " + s + "\n";
    }
}