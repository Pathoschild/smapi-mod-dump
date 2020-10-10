/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gunnargolf/DynamicChecklist
**
*************************************************/

namespace DynamicChecklist.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework;
    using StardewValley;

    public class StardewVertex
    {
        public StardewVertex(GameLocation location, Vector2 position)
        {
            this.Location = location;
            this.Position = position;
        }

        public GameLocation Location { get; private set; }

        public Vector2 Position { get; protected set; }

        public static float Distance(StardewVertex vertex1, StardewVertex vertex2)
        {
            return Math.Abs(vertex1.Position.X - vertex2.Position.X) + Math.Abs(vertex1.Position.Y - vertex2.Position.Y);
        }
    }
}
