/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/MegaStorageMod
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley.Objects;

namespace MegaStorage
{
    public class ChestPatches
    {
        private static IMonitor Monitor;
        public static void init(IMonitor monitor)
        {
            Monitor = monitor;
        }
        public static bool GetActualCapacity_Prefix(Chest __instance, ref int __result)
        {
            try
            {
                if (!__instance.modData.TryGetValue("ImJustMatt.ExpandedStorage/actual-capacity",
                    out var actualCapacity))
                    return true;
                __result = Convert.ToInt32(actualCapacity);
                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(GetActualCapacity_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}