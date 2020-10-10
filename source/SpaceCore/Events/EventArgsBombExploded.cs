/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/SpaceCore_SDV
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceCore.Events
{
    public class EventArgsBombExploded
    {
        internal EventArgsBombExploded(Vector2 tileLocation, int radius)
        {
            Position = tileLocation;
            Radius = radius;
        }
        
        public Vector2 Position { get; }
        public int Radius { get; }
    }
}
