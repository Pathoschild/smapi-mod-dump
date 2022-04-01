/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Locations;

namespace MagicScepterRedux
{
    public enum WarpLocationChoice
    {
        Farm,
        Beach,
        Mountain,
        Desert,
        Island,
        IslandFarm,
        DeepWoods,
        MiniObelisk,
        None
    }
    public class WarpLocation
    {
        public string Name { get; private set; }

        public int CoordX { get; private set; }

        public int CoordY { get; private set; }

        public WarpLocation(string name, int x, int y)
        {
            Name = name;
            CoordX = x;
            CoordY = y;
        }
    }

    public static class WarpLocations
	{
		public static WarpLocation GetWarpLocation(WarpLocationChoice targetTocation)
		{
			switch (targetTocation)
			{
			case WarpLocationChoice.Farm:
			{
				FarmHouse home = Utility.getHomeOfFarmer(Game1.player);
				int x = home?.getFrontDoorSpot().X ?? 64;
				int y = home?.getFrontDoorSpot().Y ?? 15;
				return new WarpLocation("Farm", x, y);
			}
			case WarpLocationChoice.Beach:
				return new WarpLocation("Beach", 20, 4);
			case WarpLocationChoice.Mountain:
				return new WarpLocation("Mountain", 31, 20);
			case WarpLocationChoice.Desert:
				return new WarpLocation("Desert", 35, 43);
			case WarpLocationChoice.Island:
				return new WarpLocation("IslandSouth", 11, 11);
			case WarpLocationChoice.IslandFarm:
				return new WarpLocation("IslandWest", 77, 40);
			default:
				throw new Exception("Invalid WarpLocationChoice");
			}
		}
	}
}
