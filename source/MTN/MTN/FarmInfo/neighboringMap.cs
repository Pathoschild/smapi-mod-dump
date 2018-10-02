using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.FarmInfo
{
    /// <summary>
    /// A simple class that contains the information pertaining to warps to override for a custom farm.
    /// 
    /// Populated by JsonSerializer
    /// </summary>
    public class neighboringMap
    {
        public class Warp {
            public string TargetMap { get; set; } = "Farm";
            public int fromX { get; set; }
            public int fromY { get; set; }
            public int toX { get; set; }
            public int toY { get; set; }

            public Warp() { }
            public Warp(string map, int x, int y, int xx, int yy)
            {
                TargetMap = map;
                fromX = x;
                fromY = y;
                toX = xx;
                toY = yy;
            }
        }

        public string MapName { get; set; }
        public List<Warp> warpPoints { get; set; }

        public neighboringMap() { }

        public neighboringMap(string targetMap)
        {
            MapName = targetMap;
            warpPoints = new List<Warp>();
        }

        public neighboringMap(string targetMap, List<Warp> points)
        {
            MapName = targetMap;
            warpPoints = points;
        }

        public void Add(string map, int fromX, int fromY, int toX, int toY)
        {
            warpPoints.Add(new Warp(map, fromX, fromY, toX, toY));
        }
    }
}
