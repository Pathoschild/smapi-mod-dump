using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmHouseRedone.Pathing
{
    public static class ClaustrophobiaWeight
    {

        public static int distance = 4;

        public static float getWeight(Node node, PathingGrid grid)
        {
            float totalImpassableWeight = 0;
            if(node == null)
            {
                Logger.Log("Null node!");
                return -1;
            }
            for(int x = node.x - distance; x < node.x + distance + 1; x++)
            {
                for(int y = node.y - distance; y < node.y + distance + 1; y++)
                {
                    if (x < 0 || x >= grid.width || y < 0 || y >= grid.height || !grid.getNode(x, y).traversible)
                    {
                        totalImpassableWeight += (distance) - (float)Math.Min(Math.Sqrt(Math.Pow(x - node.x, 2) + Math.Pow(y - node.y, 2)), (float)distance);
                    }
                }
            }
            return totalImpassableWeight;
        }
    }
}
