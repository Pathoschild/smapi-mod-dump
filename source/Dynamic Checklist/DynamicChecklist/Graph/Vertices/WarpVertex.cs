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

    public class WarpVertex : StardewVertex
    {
        public WarpVertex(GameLocation location, Vector2 position, GameLocation targetLocation, Vector2 targetPosition)
            : base(location, position)
        {
            this.TargetLocation = targetLocation;
            this.TargetPosition = targetPosition;
        }

        public GameLocation TargetLocation { get; private set; }

        public Vector2 TargetPosition { get; private set; }

        public void SetTargetPosition(Vector2 targetPosition)
        {
            this.TargetPosition = targetPosition;
        }
    }
}
