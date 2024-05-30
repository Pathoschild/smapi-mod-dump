/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

//To use FTM's API:
//1. Copy this entire file into your C# mod. 
//2. After all mods are loaded, use this code to get your API instance: var api = this.Helper.ModRegistry.GetApi<FarmTypeManager.IFtmApi>("Esca.FarmTypeManager");
//3. Check that the "api" variable is not null before use.

using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace FarmTypeManager
{
    /// <summary>The public API interface for Farm Type Manager (FTM), provided through SMAPI's mod helper.</summary>
    public interface IFtmApi
    {
        /// <summary>Gets information about all the valid forage IDs in loaded FTM content packs. Keys are content pack IDs (or "" for other sources). Values are a list of each valid, qualified item ID.</summary>
        /// <remarks>
        /// This method will produce information about the current in-game day.
        /// It is available as soon as SMAPI has loaded all content packs, e.g. during GameLaunched events.
        /// Results may change after FTM's DayStarted events, during which FTM reloads all content pack data.
        /// Data from save-specific personal config files will only be included if "Context.IsWorldReady" is true.
        /// </remarks>
        /// <param name="includePlacedItems">If true, this list will include the IDs of forage items that are NOT normal <see cref="StardewValley.Object"/> instances. These spawn inside a custom <see cref="TerrainFeature"/> subclass, but imitate most normal forage behavior.</param>
        /// <param name="includeContainers">If true, this list will include the IDs of containers that are spawned as forage (chests, breakable barrels and crates, etc).</param>
        public IDictionary<string, IEnumerable<string>> GetForageIDsFromContentPacks(bool includePlacedItems = false, bool includeContainers = false);
    }
}
