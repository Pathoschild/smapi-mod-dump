/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace ParrotPerch
{
    internal class PerchParrot : EmilysParrot
    {
        public Vector2 tile;

        public PerchParrot(Vector2 location, Vector2 tile) : base(location)
        {
            this.tile = tile;
            layerDepth = 1;
        }
    }
}