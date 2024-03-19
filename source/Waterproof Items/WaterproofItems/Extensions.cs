/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/WaterproofItems
**
*************************************************/

using StardewValley;

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
                debris.debrisType.Value == Debris.DebrisType.OBJECT
                || debris.debrisType.Value == Debris.DebrisType.ARCHAEOLOGY
                || debris.debrisType.Value == Debris.DebrisType.RESOURCE
                || debris.item != null
            )
                return true; //this is an item
            else
                return false; //this is NOT an item
        }
    }
}
