/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.GingerIsland
{
    public class FieldOfficeInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public virtual void command_addCraftingRecipe(GameLocation location, GameTime time, string[] split)
        public static bool CommandAddCraftingRecipe_OstrichIncubator_Prefix(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            try
            {
                var currentCommand = __instance.CurrentCommand;
                var currentCommandText = __instance.eventCommands[currentCommand];
                var recipe = currentCommandText.Substring(currentCommandText.IndexOf(' ') + 1);

                if (!recipe.Equals("Ostrich Incubator", StringComparison.OrdinalIgnoreCase))
                {
                    return true; // run original logic
                }
                
                _locationChecker.AddCheckedLocation("Complete Island Field Office");
                __instance.CurrentCommand++;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CommandAddCraftingRecipe_OstrichIncubator_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
