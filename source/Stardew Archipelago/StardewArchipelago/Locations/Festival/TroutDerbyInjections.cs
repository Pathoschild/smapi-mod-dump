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
    internal class TroutDerbyInjections
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

        // public virtual bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
        public static bool AnswerDialogueAction_TroutDerbyRewards_Prefix(GameLocation __instance, string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                if (!questionAndAnswer.Equals("TroutDerbyBooth_Rewards", StringComparison.InvariantCultureIgnoreCase) || Game1.player.Items.CountId("TroutDerbyTag") <= 0)
                {
                    return true; // run original logic
                }

                __result = true;
                var numberOfThisTag = 1;
                var locationName = string.Format(FestivalLocationNames.TROUT_DERBY_REWARD_PATTERN, numberOfThisTag);
                while (_archipelago.LocationExists(locationName) && _locationChecker.IsLocationChecked(locationName))
                {
                    numberOfThisTag++;
                    locationName = string.Format(FestivalLocationNames.TROUT_DERBY_REWARD_PATTERN, numberOfThisTag);
                }

                if (_locationChecker.IsLocationMissing(locationName))
                {
                    Game1.stats.Increment("GoldenTagsTurnedIn");
                    Game1.player.Items.ReduceId("TroutDerbyTag", 1);
                    _locationChecker.AddCheckedLocation(locationName);
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_TroutDerbyRewards_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
