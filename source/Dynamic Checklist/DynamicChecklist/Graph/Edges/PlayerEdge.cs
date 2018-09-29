namespace DynamicChecklist.Graph.Edges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DynamicChecklist.Graph.Vertices;
    using Microsoft.Xna.Framework;

    public class PlayerEdge : StardewEdge
    {
        public PlayerEdge(MovableVertex source, StardewVertex target)
            : base(source, target, "Player")
        {
        }
    }
}
