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
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.GameModifications.Buildings;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class WizardInjections
    {
        public const string BUILDING_EARTH_OBELISK = "Earth Obelisk";
        public const string BUILDING_WATER_OBELISK = "Water Obelisk";
        public const string BUILDING_DESERT_OBELISK = "Desert Obelisk";
        public const string BUILDING_ISLAND_OBELISK = "Island Obelisk";
        public const string BUILDING_JUNIMO_HUT = "Junimo Hut";
        public const string BUILDING_GOLD_CLOCK = "Gold Clock";
        public const string BUILDING_DEEP_WOODS_OBELISK = "Deep Woods Obelisk";

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

        public static bool PerformAction_WizardBook_Prefix(GameLocation __instance, string action, Farmer who, Location tileLocation, ref bool __result)
        {
            try
            {
                if (action == null || !who.IsLocalPlayer)
                {
                    return true; // run original logic
                }
                var words = action.Split(' ');
                var firstWord = words[0];

                if (firstWord != "WizardBook")
                {
                    return true; // run original logic
                }

                var wizardMenu = new WizardMenuArchipelago(_modHelper, _archipelago);
                var wizardBlueprints = wizardMenu.GetAvailableBlueprints();

                if (wizardBlueprints.Any())
                {
                    Game1.activeClickableMenu = wizardMenu;
                }

                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformAction_WizardBook_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
