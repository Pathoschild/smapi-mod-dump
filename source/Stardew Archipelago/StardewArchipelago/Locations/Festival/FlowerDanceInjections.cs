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
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.Festival
{
    public static class FlowerDanceInjections
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

        // public void setUpFestivalMainEvent()
        public static void SetUpFestivalMainEvent_FlowerDance_Postfix(Event __instance)
        {
            try
            {
                if (!__instance.isSpecificFestival("spring24"))
                {
                    return;
                }

                if (Game1.player.dancePartner.Value == null)
                {
                    return;
                }

                _locationChecker.AddCheckedLocation(FestivalLocationNames.DANCE_WITH_SOMEONE);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SetUpFestivalMainEvent_FlowerDance_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
