/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using System;
using System.Reflection;
using GreenhouseGatherersAutomate.GreenhouseGatherersAutomate.Automate;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace GreenhouseGatherersAutomate.GreenhouseGatherersAutomate
{
    /// <summary>The mod entry point.</summary>
    public class AutomateModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            // Check if Pathoschild's Automate is in the current mod list
            if (!Helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            {
                return;
            }

            // Load the monitor
            AutomateModResources.LoadMonitor(this.Monitor);

            // Hook into the game launch
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Monitor.Log("Attempting to hook into Pathoschild.Automate.", LogLevel.Debug);
            try
            {
                IAutomateAPI automateApi = Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");

                // Add the AutomationFactory for Harvest Statue
                automateApi.AddFactory(new HarvestStatueFactory());
            }
            catch (Exception ex)
            {
                Monitor.Log($"There was an issue with hooking into Pathoschild.Automate: {ex}", LogLevel.Error);
            }
        }
    }
}