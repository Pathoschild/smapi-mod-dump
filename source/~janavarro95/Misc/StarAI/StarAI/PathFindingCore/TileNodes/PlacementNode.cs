/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

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
