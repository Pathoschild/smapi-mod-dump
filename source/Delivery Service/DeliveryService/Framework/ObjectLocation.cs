/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AxesOfEvil/SV_DeliveryService
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace DeliveryService.Framework
{
    public class ObjectLocation
    {
        public GameLocation Location;
        public Vector2 TileLocation;
        public ObjectLocation(GameLocation location, Vector2 pos)
        {
            Location = location;
            TileLocation = pos;
        }
        public override string ToString()
        {
            string name = "<Unknown>";
            if (Location != null)
                name = Location.NameOrUniqueName;
            return name + "@(" + ((int)TileLocation.X).ToString() + "," + ((int)TileLocation.Y).ToString() + ")";
        }
    }
}
