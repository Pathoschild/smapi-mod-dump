/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

namespace BetterJunimosForestry
{
    public static class RoomTypes
    {
        public const int WALL = 0;
        public const int PATH = 1;
        public const int HUT = 2;
        public const int DOOR = 3;
        public const int FIXED_WALL = 4;
    }
    
    public class Maze
    {
        private readonly int[,] Rooms;

        private Maze(int radius, int[,] maze = null)
        {
            if (maze is null)
            {
                var size = 2 * radius + 1;
                maze = new int[size, size];
            }
            if (radius <= 0) throw new ArgumentOutOfRangeException(nameof(radius));
            Rooms = GetMaze(radius, maze);
        }

        private int Radius()
        {
            return Rooms.GetLength(0) / 2;
        }
        
        private static readonly List<Vector2> Directions = new() {new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1)};
        
        
        // utility methods

        /// <summary>Identify tiles that already contain something impassable</summary>
        /// so that they can be treated as non-removable walls during maze building
        private static int[,] FixedWallsForHut(JunimoHut hut)
        {
            var radius = ModEntry.BJApi.GetJunimoHutMaxRadius();
            var sx = hut.tileX.Value - radius + 1;
            var sy = hut.tileY.Value - radius + 1;
            var size = 2 * radius + 1;
            var maze = new int[size, size];
            var farm = Game1.getFarm();
            
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var pos = new Vector2(x+sx, y+sy);
                    
                    // trellis crop check
                    if (farm.terrainFeatures.ContainsKey(pos) && farm.terrainFeatures[pos] is HoeDirt {crop: { }} hd)
                    {
                        if (!hd.crop.dead.Value && hd.crop.raisedSeeds.Value) 
                        {
                            maze[x, y] = RoomTypes.FIXED_WALL;
                        }
                        
                        // skip the crop check in Util.IsOccupied()
                        continue;
                    }
                    
                    // general obstruction check
                    if (Util.IsOccupied(farm, pos))
                    {
                        maze[x, y] = RoomTypes.FIXED_WALL;
                    }
                }
            }
            
            // ModEntry.SMonitor.Log($"FixedWallsForHut: sx {sx} sy {sy} size {size} radius {radius}", LogLevel.Debug);
            // print(maze);
            return maze;
        }
        
        internal static void MakeMazeForHut(JunimoHut hut) {
            if (hut is null) {
                // ModEntry.SMonitor.Log($"SetMazeForHut: hut is null", LogLevel.Warn);
                return;
            }
            
            var m = new Maze(ModEntry.BJApi.GetJunimoHutMaxRadius(), FixedWallsForHut(hut));
            var ct = new string[m.Rooms.GetLength(0), m.Rooms.GetLength(1)];
            
            for (var r = 0; r < m.Rooms.GetLength(0); r++)
            {
                for (var c = 0; c < m.Rooms.GetLength(1); c++)
                {
                    ct[r, c] = CropTypeForRoomType(m.Rooms[r, c]);
                }
            }

            var ctm = new BetterJunimos.CropMap {Map = ct};
            ModEntry.BJApi.SetCropMapForHut(Util.GetHutIdFromHut(hut), ctm);
        }

        internal static void ClearMazeForHut(JunimoHut hut)
        {
            ModEntry.BJApi.ClearCropMapForHut(Util.GetHutIdFromHut(hut));
        }

        private static string CropTypeForRoomType(int roomType)
        {
            return roomType == RoomTypes.WALL ? BetterJunimos.CropTypes.Trellis : BetterJunimos.CropTypes.Ground;
        }
        
        public static int getRoomAtTile(JunimoHut hut, Vector2 pos)
        {
            var m = getMazeForHut(hut);

            var (x, y) = pos;
            var dx = (int)x - hut.tileX.Value;
            var dy = (int)y - hut.tileY.Value;

            var mx = m.Radius() - 1 + dx;
            var my = m.Radius() - 1 + dy;
            
            return m.Rooms[mx, my];
        }

        private static Maze getMazeForHut(JunimoHut hut)
        {
            var pos = new Vector2(hut.tileX.Value, hut.tileY.Value);
            if (!ModEntry.HutMazes.ContainsKey(pos))
            {
                ModEntry.HutMazes[pos] = new Maze(ModEntry.BJApi.GetJunimoHutMaxRadius());
            }

            return ModEntry.HutMazes[pos];
        }

        private static int[,] GetMaze(int radius, int[,] maze)
        {
            // ModEntry.SMonitor.Log($"GetMaze: radius {radius}", LogLevel.Debug);
            var size = 2 * radius + 1;
            
            placeHut(maze);
            
            // start at the hut door and visit all rooms in the maze
            var bx = radius + 1;
            var by = radius + 1;
            visit(maze, bx, by);
            
            // pick a random spot at the top of the grid as the maze entry
            var offset = Game1.random.Next(size);
            for (var x = 0; x < size; x++)
            {
                var checkX = (x + offset) % size;
                if (maze[checkX, 0] == RoomTypes.FIXED_WALL || maze[checkX, 1] != RoomTypes.PATH) continue;
                setPath(maze, checkX, 0);
                break;
            }

            // print(maze);
            return maze;
        }

        
        // internals, may not need to be static if we stop passing maze around
        private static void placeHut(int[,] maze)
        {
            var hx = maze.GetLength(0) / 2 - 1;
            var hy = maze.GetLength(1) / 2 - 1;
            for (var x = hx; x < hx + 3; x++)
            {
                for (var y = hy; y < hy + 2; y++)
                {
                    maze[x, y] = RoomTypes.HUT;
                }
            }

            maze[hx + 1, hy + 2] = RoomTypes.DOOR;
        }

        internal void print()
        {
            print(Rooms);
        }

        private static void print(int[,] maze)
        {
            for (var y = 0; y < maze.GetLength(1); y++)
            {
                var line = "";
                for (var x = 0; x < maze.GetLength(0); x++)
                {
                    var c = maze[x, y] switch
                    {
                        RoomTypes.PATH => "  ",
                        RoomTypes.WALL => "##",
                        RoomTypes.HUT => "HH",
                        RoomTypes.DOOR => "[]",
                        RoomTypes.FIXED_WALL => "FW",
                        _ => "??"
                    };
                    line += c;
                }
                ModEntry.SMonitor.Log($"{line}", LogLevel.Debug);
            }
        }

        private static void visit(int[,] maze, int cx, int cy)
        {
            setPath(maze, cx, cy);
            
            var directions = Directions.OrderBy(_ => Guid.NewGuid()).ToList();
            foreach (var (x, y) in directions)
            {
                var wx = cx + (int)x;
                var wy = cy + (int)y;
                var nx = cx + (int)x * 2;
                var ny = cy + (int)y * 2;


                if (!isUnvisitedRoom(maze, nx, ny) || maze[wx, wy] == RoomTypes.FIXED_WALL) continue;
                // success
                setPath(maze, wx, wy);
                visit(maze, nx, ny);
            }
        }

        private static bool isRoom(int[,] maze, int x, int y)
        {
            if (maze[x, y] == RoomTypes.DOOR) return true;
            return (x % 2 == 1 && y % 2 == 1);
        }

        private static bool isUnvisitedRoom(int[,] maze, int x, int y)
        {
            var wall = false;
            try
            {
                if (isRoom(maze, x, y) && maze[x, y] == RoomTypes.WALL ) wall = true;
                if (isRoom(maze, x, y) && maze[x, y] == RoomTypes.DOOR ) wall = true;
            }
            catch (IndexOutOfRangeException)
            {
                wall = false;
            }

            return wall;
        }

        private static void setPath(int[,] maze, int x, int y)
        {
            try
            {
                maze[x, y] = RoomTypes.PATH;
            }
            catch (IndexOutOfRangeException) { }
        }
    }
}