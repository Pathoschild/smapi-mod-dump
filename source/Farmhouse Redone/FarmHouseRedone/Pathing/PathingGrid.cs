using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmHouseRedone.Pathing
{
    public class PathingGrid
    {
        public Node[,] grid;
        public GameLocation location;
        public int width;
        public int height;

        public PathingGrid(GameLocation location, int width = -1, int height = -1)
        {
            if (width == -1)
                width = location.map.GetLayer("Back").LayerWidth;
            if (height == -1)
                height = location.map.GetLayer("Back").LayerHeight;
            this.location = location;
            this.width = width;
            this.height = height;

            grid = new Node[this.width, this.height];

            for(int x = 0; x < this.width; x++)
            {
                for(int y = 0; y < this.height; y++)
                {
                    bool traversible = location.isTilePassable(new xTile.Dimensions.Location(x, y), Game1.viewport);
                    grid[x, y] = new Node(new Vector2(x, y), traversible);
                }
            }

            updateWeights();
        }

        public void updateWeights()
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    Node node = this.getNode(x, y);
                    node.weightCost = ClaustrophobiaWeight.getWeight(node, this);
                }
            }
        }

        public List<Node> getNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();

            //SDV characters do not path with diagonals, so the pathing can only be done in the cardinal directions
            if (node.x - 1 >= 0)
                neighbors.Add(getNode(node.x - 1, node.y));
            if (node.x + 1 < width)
                neighbors.Add(getNode(node.x + 1, node.y));
            if (node.y - 1 >= 0)
                neighbors.Add(getNode(node.x, node.y - 1));
            if (node.y + 1 < width)
                neighbors.Add(getNode(node.x, node.y + 1));
            return neighbors;
        }

        public Node getNode(Vector2 position)
        {
            return getNode((int)position.X, (int)position.Y);
        }

        public Node getNode(int x, int y)
        {
            //Logger.Log("Getting node (" + Math.Min(x, width) + ", " + Math.Min(y, height) + ")...");
            return grid[Math.Min(x, width),Math.Min(y, height)];
        }
    }
}
