/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace DecidedlyShared.Utilities
{
    public class Locations
    {
        public static bool IsTileEmpty(GameLocation location, Vector2 tile)
        {
            return !(location.Objects.ContainsKey(tile) || location.terrainFeatures.ContainsKey(tile));
        }
    }
}
