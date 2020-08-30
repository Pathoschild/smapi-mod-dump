using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmHouseRedone.Pathing
{
    public class Node
    {
        public Vector2 position;
        public bool traversible;

        public Node parent;

        public float weightCost;
        public int gCost;
        public int hCost;
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }
        public int x
        {
            get
            {
                return (int)position.X;
            }
        }
        public int y
        {
            get
            {
                return (int)position.Y;
            }
        }

        public Node(Vector2 position, bool traversible)
        {
            this.position = position;
            this.traversible = traversible;
        }

        
    }
}
