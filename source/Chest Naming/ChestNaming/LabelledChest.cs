using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestNaming
{
    public class LabelledChest
    {
        public int TileX
        {
            get;
            set;
        }
        public int TileY
        {
            get;
            set;
        }
        public string ChestName
        {
            get;
            set;
        }
        public string GameLocation
        {
            get;
            set;
        }

        public LabelledChest(int tileX, int tileY, string chestName, string gameLocation)
        {
            TileX = tileX;
            TileY = tileY;
            ChestName = chestName;
            GameLocation = gameLocation;
        }

        public bool EqualsChest(Chest chest)
        {
            return chest != null && chest.boundingBox.X == TileX && chest.boundingBox.Y == TileY && GameLocation == Game1.player.currentLocation.Name;
        }
    }
}
