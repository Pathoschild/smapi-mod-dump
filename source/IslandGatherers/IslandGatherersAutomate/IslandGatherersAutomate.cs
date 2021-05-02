/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/IslandGatherers
**
*************************************************/

using Pathoschild.Stardew.Automate;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandGatherersAutomate.Automate
{
    public class IslandGatherersAutomate : Mod
    {
        internal static IMonitor monitor;
        internal static IModHelper modHelper;

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor and helper
            monitor = Monitor;
            modHelper = helper;

            // Check if Pathoschild's Automate is in the current mod list
            if (!Helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            {
                return;
            }

            // Hook into the game launch
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Monitor.Log("Attempting to hook into Pathoschild.Automate.", LogLevel.Debug);
            try
            {
                IAutomateAPI automateApi = Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");

                // Add the AutomationFactory for Parrot Pot
                automateApi.AddFactory(new ParrotStorageFactory());
            }
            catch (Exception ex)
            {
                Monitor.Log($"There was an issue with hooking into Pathoschild.Automate: {ex}", LogLevel.Error);
            }
        }
    }
}
