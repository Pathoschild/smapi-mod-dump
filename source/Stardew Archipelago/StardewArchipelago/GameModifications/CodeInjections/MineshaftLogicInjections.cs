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
using StardewModdingAPI;
using StardewValley.Network;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public static class MineshaftLogicInjections
    {
        private static IMonitor _monitor;

        public static void Initialize(IMonitor monitor)
        {
            _monitor = monitor;
        }

        public static bool SetLowestMineLevel_SkipToSkullCavern_Prefix(NetWorldState __instance, int value)
        {
            try
            {
                if (__instance.lowestMineLevel.Value < 120 && value > 120)
                {
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SetLowestMineLevel_SkipToSkullCavern_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
