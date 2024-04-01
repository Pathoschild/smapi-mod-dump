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
        /// <summary>A SMAPI GameLaunched event that enables Content Patcher support.</summary>
        public void EnableContentPatcher(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                var api = this.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher"); //attempt to get an instance of CP's API

                if (api == null) //if the API is NOT available
                {
                    Monitor.Log($"API not found: Content Patcher (CP).", LogLevel.Trace);
                    return;
                }

                Monitor.Log($"API found: Content Patcher (CP).", LogLevel.Trace);
                Utility.ContentPatcherAPI = api; //pass the API to this mod's static utility property
                Utility.ContentPatcherVersion = Helper.ModRegistry.Get("Pathoschild.ContentPatcher")?.Manifest.Version; //pass CP's current semantic version
            }
            catch (Exception ex)
            {
                Utility.Monitor.Log($"An error happened while loading FTM's Content Patcher (CP) interface. Any spawn areas with \"CPConditions\" settings will be disabled. The auto-generated error message has been added to the log.", LogLevel.Warn);
                Utility.Monitor.Log($"----------", LogLevel.Trace);
                Utility.Monitor.Log($"{ex}", LogLevel.Trace);
            }
        }
    }
}
