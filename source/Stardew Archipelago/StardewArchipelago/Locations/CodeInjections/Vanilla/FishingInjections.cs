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
using StardewArchipelago.Goals;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class FishingInjections
    {
        private const int GREEN_ALGAE = 153;
        private const int WHITE_ALGAE = 157;
        private const int SEAWEED = 152;
        private const int ORNATE_NECKLACE = 191;
        private const int GOLDEN_WALNUT = 73;
        private const int SECRET_NOTE = 79;
        private const int FOSSILIZED_SPINE = 821;
        private const int SNAKE_SKULL = 825;
        private const int JOURNAL_SCRAP = 842;
        private const int QI_BEAN = 890;

        private static readonly int[] _fishsanityExceptions = new[]
        {
            GREEN_ALGAE, WHITE_ALGAE, SEAWEED, ORNATE_NECKLACE, GOLDEN_WALNUT, SECRET_NOTE, FOSSILIZED_SPINE, SNAKE_SKULL, JOURNAL_SCRAP,
            QI_BEAN,
        };
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

        public static void CaughtFish_Fishsanity_Postfix(Farmer __instance, int index, int size, bool from_fish_pond, int numberCaught, ref bool __result)
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
                else if (!_fishsanityExceptions.Contains(index))
                {
                    _monitor.Log($"Unrecognized Fishsanity Location: {fishName} [{index}]", LogLevel.Error);
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CaughtFish_Fishsanity_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static void CaughtFish_CheckGoalCompletion_Postfix(Farmer __instance, int index, int size,
            bool from_fish_pond, int numberCaught, ref bool __result)
        {
            try
            {
                GoalCodeInjection.CheckMasterAnglerGoalCompletion();
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CaughtFish_CheckGoalCompletion_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
