/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A SMAPI GameLaunched event that enables DGA support if that mod is available.</summary>
        public void EnableDGA(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                IDynamicGameAssetsApi api = Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets"); //attempt to get an instance of DGA's API

                if (api == null) //if the API is NOT available
                {
                    Monitor.Log($"Optional API not found: Dynamic Game Assets (DGA).", LogLevel.Trace);
                    return;
                }
                else //if the API is available
                {
                    Monitor.Log($"Optional API found: Dynamic Game Assets (DGA).", LogLevel.Trace);
                }

                Utility.DGAItemAPI = api; //pass the API to this mod's static utility property
            }
            catch (Exception ex)
            {
                Utility.Monitor.Log($"An error happened while loading FTM's Dynamic Game Assets (DGA) interface. FTM will be unable to spawn custom items added by DGA. The auto-generated error message has been added to the log.", LogLevel.Warn);
                Utility.Monitor.Log($"----------", LogLevel.Trace);
                Utility.Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
            }
        }
    }

    /// <summary>Dynamic Game Assets' API interface. Used to recognize & interact with the mod's API when available.</summary>
    /// <remarks>Comments within this API were provided within DGA's original code.</remarks>
    public interface IDynamicGameAssetsApi
    {
        /// <summary>
        /// Get the DGA item ID of this item, if it has one.
        /// </summary>
        /// <param name="item">The item to get the DGA item ID of.</param>
        /// <returns>The DGA item ID if it has one, otherwise null.</returns>
        string GetDGAItemId(object item);

        /// <summary>
        /// Spawn a DGA item, referenced with its full ID ("mod.id/ItemId").
        /// Some items, such as crafting recipes or crops, don't have an item representation.
        /// </summary>
        /// <param name="fullId">The full ID of the item to spawn.</param>
        /// <returns></returns>
        object SpawnDGAItem(string fullId);
    }
}
