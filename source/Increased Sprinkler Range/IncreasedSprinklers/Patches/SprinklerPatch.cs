/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ArjanSeijs/SprinklerMod
**
*************************************************/

using System;
using StardewModdingAPI;

// ReSharper disable InconsistentNaming

namespace IncreasedSprinklers.Patches
{
    public class SprinklerPatch
    {
        private static IMonitor Monitor;

        // call this method from your Entry class
        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }
        
        internal static void GetModifiedRadiusForSprinkler_Postfix(StardewValley.Object __instance, ref int __result)
        {
            try
            {
                if (__result >= 0)
                {
                    __result = Math.Clamp(__result + ModEntry.Instance.Config.RangeIncrease, 0, 256);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(GetModifiedRadiusForSprinkler_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}