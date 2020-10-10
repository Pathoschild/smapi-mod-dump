/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/M3ales/ChestNaming
**
*************************************************/

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
