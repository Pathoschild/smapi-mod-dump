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
using StardewValley.Locations;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public class ForestInjections
    {
        private const int RAT_PROBLEM_ID = 26;

        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }
        
        public static bool IsWizardHouseUnlocked_UnlockAtRatProblem_Prefix(Forest __instance, ref bool __result)
        {
            try
            {
                if (!Game1.player.hasQuest(RAT_PROBLEM_ID))
                {
                    return true; // run original logic
                }

                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(IsWizardHouseUnlocked_UnlockAtRatProblem_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
