/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Objects;

namespace StardewLib
{
    internal class ChestDef
    {
        /*********
        ** Accessors
        *********/
        public int X { get; set; }
        public int Y { get; set; }
        public Vector2 Tile { get; set; }
        public string Location { get; set; }
        public int Count { get; set; }
        public Chest Chest { get; set; }


        /*********
        ** Public methods
        *********/
        public ChestDef()
        {
            X = -1;
            Y = -1;
        }

        public ChestDef(int x, int y)
        {
            X = x;
            Y = y;

            Tile = new Vector2(x, y);

            Location = "Farm";
        }

        public ChestDef(int x, int y, string location)
        {
            X = x;
            Y = y;

            Tile = new Vector2(x, y);

            Location = location;
        }

        public ChestDef(int x, int y, string location, int count)
        {
            X = x;
            Y = y;
            Tile = new Vector2(x, y);
            Location = location;
            Count = count;
        }

        public ChestDef(int x, int y, string location, int count, Chest chest)
        {
            X = x;
            Y = y;
            Tile = new Vector2(x, y);
            Location = location;
            Count = count;
            Chest = chest;
        }

        public override string ToString()
        {
            return $"{Location} {Tile} #{Count}";
        }
    }
}