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
using StardewArchipelago.Goals;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections
{
    public static class FishingInjections
    {
        private const int GREEN_ALGAE = 153;
        private const int WHITE_ALGAE = 157;
        private const int SEAWEED = 152;
        private const int GOLDEN_WALNUT = 73;
        private const int JOURNAL_SCRAP = 842;
        private const string FISHSANITY_PREFIX = "Fishsanity: ";

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static StardewItemManager _itemManager;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, StardewItemManager itemManager)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _itemManager = itemManager;
        }
        
        public static void CaughtFish_Fishsanity_Postfix(Farmer __instance, int index, int size,
            bool from_fish_pond, int numberCaught, ref bool __result)
        {
            try
            {
                if (from_fish_pond || (index >= 167 && index < 173) || !_itemManager.ObjectExists(index))
                {
                    return;
                }

                var fish = _itemManager.GetObjectById(index);
                var fishName = fish.Name;
                var apLocation = $"{FISHSANITY_PREFIX}{fishName}";
                if (_archipelago.GetLocationId(apLocation) > -1)
                {
                    _locationChecker.AddCheckedLocation(apLocation);
                }
                else if (index != GREEN_ALGAE && index != WHITE_ALGAE && index != SEAWEED && index != GOLDEN_WALNUT && index != JOURNAL_SCRAP)
                {
                    _monitor.Log($"Unrecognized Fishsanity Location: {fishName} [{index}]", LogLevel.Error);
                }

                GoalCodeInjection.CheckMasterAnglerGoalCompletion();
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CaughtFish_Fishsanity_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
