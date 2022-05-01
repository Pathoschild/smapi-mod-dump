/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley;

namespace BetterReturnScepter
{
    public class PreviousPoint
    {
        private GameLocation location;
        private Vector2 tile = new Vector2();
     
        /// <summary>
        /// The GameLocation the player used the return sceptre's vanilla function in.
        /// </summary>
        public GameLocation Location
        {
            get { return location; }
            set { location = value; }
        }
        
        /// <summary>
        /// The tile (as a Vector2) the player was on when using the return sceptre.
        /// </summary>
        public Vector2 Tile
        {
            get { return tile; }
            set { tile = value; }
        }
    }
}