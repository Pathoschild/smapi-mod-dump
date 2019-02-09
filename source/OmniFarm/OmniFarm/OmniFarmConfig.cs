using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OmniFarm
{
    internal class OmniFarmConfig
    {
        /*********
        ** Accessors
        *********/
        public bool useOptionalCave { get; set; } = false;
        
        public int GrassGrowth_1forsparse_4forFull = 4;

        public Tuple<Vector2, Vector2>[] mineAreas { get; set; } = {
            Tuple.Create(new Vector2(89, 3), new Vector2(96, 7)),
            Tuple.Create(new Vector2(97, 4), new Vector2(115, 10)),
            Tuple.Create(new Vector2(91, 8), new Vector2(96, 8)),
            Tuple.Create(new Vector2(92, 9), new Vector2(96, 9)),
            Tuple.Create(new Vector2(93, 10), new Vector2(96, 10))
        };

        public Tuple<Vector2, Vector2>[] grassAreas { get; set; } = {
            Tuple.Create(new Vector2(99, 73), new Vector2(115, 84)),
            Tuple.Create(new Vector2(99, 96), new Vector2(115, 108))
        };

        public Vector2[] stumpLocations { get; set; } = GetDefaultStumpLocations();

        public Vector2[] hollowLogLocations { get; set; } = GetDefaultLogLocations();

        public Vector2[] meteoriteLocations { get; set; } = new Vector2[0];

        public Vector2[] boulderLocations { get; set; } = new Vector2[0];

        public Vector2[] largeRockLocations { get; set; } = new Vector2[0];

        public double oreChance { get; set; } = 0.05;

        public double gemChance { get; set; } = 0.01;

        public Vector2 WarpFromForest { get; set; } = new Vector2(32, 117);

        public Vector2 WarpFromBackWood { get; set; } = new Vector2(-1, -1);

        public Vector2 WarpFromBusStop { get; set; } = new Vector2(-1, -1);


        /*********
        ** Public methods
        *********/
        public List<Vector2> getMineLocations()
        {
            var tiles = new List<Vector2>();
            foreach (Tuple<Vector2, Vector2> T in mineAreas)
                AddVector2Grid(T.Item1, T.Item2, ref tiles);
            return tiles;
        }

        public List<Vector2> getGrassLocations()
        {
            var tiles = new List<Vector2>();
            foreach (Tuple<Vector2, Vector2> T in grassAreas)
                AddVector2Grid(T.Item1, T.Item2, ref tiles);
            return tiles;
        }


        /*********
        ** Private methods
        *********/
        private static Vector2[] GetDefaultStumpLocations()
        {
            List<Vector2> tiles = new List<Vector2>();
            AddVector2Grid(new Vector2(7, 24), new Vector2(7, 24), ref tiles);
            AddVector2Grid(new Vector2(9, 26), new Vector2(9, 26), ref tiles);
            AddVector2Grid(new Vector2(13, 27), new Vector2(13, 27), ref tiles);
            return tiles.ToArray();
        }

        private static Vector2[] GetDefaultLogLocations()
        {
            List<Vector2> tiles = new List<Vector2>();
            AddVector2Grid(new Vector2(3, 23), new Vector2(3, 23), ref tiles);
            AddVector2Grid(new Vector2(4, 26), new Vector2(4, 26), ref tiles);
            AddVector2Grid(new Vector2(18, 28), new Vector2(18, 28), ref tiles);
            return tiles.ToArray();
        }

        private static void AddVector2Grid(Vector2 TopLeftTile, Vector2 BottomRightTile, ref List<Vector2> grid)
        {
            if (TopLeftTile == BottomRightTile)
            {
                grid.Add(TopLeftTile);
                return;
            }

            int i = (int)TopLeftTile.X;
            while (i <= (int)BottomRightTile.X)
            {
                int j = (int)TopLeftTile.Y;
                while (j <= (int)BottomRightTile.Y)
                {
                    grid.Add(new Vector2(i, j));
                    j++;
                }
                i++;
            }
        }
    }
}
