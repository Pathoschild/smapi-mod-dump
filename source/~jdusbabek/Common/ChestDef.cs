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
            this.X = -1;
            this.Y = -1;
        }

        public ChestDef(int x, int y)
        {
            this.X = x;
            this.Y = y;

            this.Tile = new Vector2(x, y);

            this.Location = "Farm";
        }

        public ChestDef(int x, int y, string location)
        {
            this.X = x;
            this.Y = y;

            this.Tile = new Vector2(x, y);

            this.Location = location;
        }

        public ChestDef(int x, int y, string location, int count)
        {
            this.X = x;
            this.Y = y;
            this.Tile = new Vector2(x, y);
            this.Location = location;
            this.Count = count;
        }

        public ChestDef(int x, int y, string location, int count, Chest chest)
        {
            this.X = x;
            this.Y = y;
            this.Tile = new Vector2(x, y);
            this.Location = location;
            this.Count = count;
            this.Chest = chest;
        }

        public override string ToString()
        {
            return $"{this.Location} {this.Tile} #{this.Count}";
        }
    }
}
