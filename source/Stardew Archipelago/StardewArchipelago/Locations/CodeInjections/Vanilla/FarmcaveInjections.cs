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

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class FarmCaveInjections
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

        // public void answerDialogue(string questionKey, int answerChoice)
        public static bool AnswerDialogue_SendFarmCaveCheck_Prefix(Event __instance, string questionKey, int answerChoice)
        {
            try
            {
                if (questionKey != "cave")
                {
                    return true; // run original logic
                }

                _locationChecker.AddCheckedLocation("Demetrius's Breakthrough");
                return false; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogue_SendFarmCaveCheck_Prefix)}:\n{ex}",
                    LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
