/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace SpaceCore.Events
{
    public class EventArgsBombExploded
    {
        internal EventArgsBombExploded(Vector2 tileLocation, int radius)
        {
            this.Position = tileLocation;
            this.Radius = radius;
        }

        public Vector2 Position { get; }
        public int Radius { get; }
    }
}
