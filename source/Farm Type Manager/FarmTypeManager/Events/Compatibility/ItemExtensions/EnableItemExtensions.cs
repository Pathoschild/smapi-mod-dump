/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A SMAPI event that enables Item Extensions support if that mod is available.</summary>
        public void EnableItemExtensions(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                var api = Helper.ModRegistry.GetApi<ItemExtensions.IApi>("mistyspring.ItemExtensions"); //attempt to get the API instance

                if (api == null) //if the API is NOT available
                {
                    Monitor.Log($"Optional API not found: Item Extensions.", LogLevel.Trace);
                    return;
                }
                else //if the API is available
                {
                    Monitor.Log($"Optional API found: Item Extensions.", LogLevel.Trace);
                    Utility.ItemExtensionsAPI = api;
                }
            }
            catch (Exception ex)
            {
                Utility.Monitor.Log($"An error happened while loading the mod interface for Item Extensions. Some objects that require it might fail to spawn. The auto-generated error message has been added to the log.", LogLevel.Warn);
                Utility.Monitor.Log($"----------", LogLevel.Trace);
                Utility.Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
            }
        }
    }
}