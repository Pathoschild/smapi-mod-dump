namespace DynamicChecklist.Graph.Vertices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework;
    using StardewValley;

    public class MovableVertex : StardewVertex
    {
        public MovableVertex(GameLocation location, Vector2 position)
            : base(location, position)
        {
        }

        public void SetPosition(Vector2 position)
        {
            this.Position = position;
        }
    }
}
