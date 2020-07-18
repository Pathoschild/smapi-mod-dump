using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Harmony;

namespace WaterproofItems
{
    /// <summary>A static class containing any extensions shared by separate parts of this mod.</summary>
    public static class Extensions
    {
        /// <summary>Indicates whether a <see cref="Debris"/> instance contains or represents an item.</summary>
        /// <param name="debris">The debris instance.</param>
        /// <returns>True if the debris contains or represents an item; otherwise false.</returns>
        public static bool IsAnItem(this Debris debris)
        {
            if
            (
                debris.debrisType == Debris.DebrisType.OBJECT
                || debris.debrisType == Debris.DebrisType.ARCHAEOLOGY
                || debris.debrisType == Debris.DebrisType.RESOURCE
                || debris.item != null
            )
                return true; //this is an item
            else
                return false; //this is NOT an item
        }
    }
}
