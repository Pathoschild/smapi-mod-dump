/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;

namespace PyTK.Types
{
    public class TileLocationSelector
    {
        public Func<GameLocation, Vector2, bool> predicate = (l, v) => false;
        public GameLocation location;

        public TileLocationSelector(Func<GameLocation, Vector2,bool> predicate = null, GameLocation location = null)
        {
            this.location = location;

            if (predicate != null && location != null)
                this.predicate = (l, v) => l == location ? predicate.Invoke(l, v) : false;
            else if (location != null)
                this.predicate = (l, v) => l == location;
            else if(predicate != null)
                this.predicate = predicate;
        }
    }
}
