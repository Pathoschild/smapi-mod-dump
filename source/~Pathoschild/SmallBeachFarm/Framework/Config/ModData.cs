/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewValley.GameData.Locations;

namespace Pathoschild.Stardew.SmallBeachFarm.Framework.Config
{
    /// <summary>The mod's hardcoded data.</summary>
    internal class ModData
    {
        /// <summary>The data to add to <c>Data\Locations</c> for the farm type.</summary>
        public LocationData? LocationData { get; set; }
    }
}
