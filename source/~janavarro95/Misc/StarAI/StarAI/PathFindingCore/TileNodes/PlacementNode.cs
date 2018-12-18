using StardewValley;
using StardustCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.PathFindingCore
{
    public class PlacementNode
    {
        public TileNode cObj;
        public GameLocation location;
        public int x;
        public int y;

        public PlacementNode(TileNode C, GameLocation Location, int X, int Y)
        {
            cObj = C;
            location = Location;
            x = X;
            y = Y;
            ModCore.CoreMonitor.Log(location.name);
        }

    }
}
