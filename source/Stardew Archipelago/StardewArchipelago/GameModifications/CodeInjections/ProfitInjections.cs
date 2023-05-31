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
using Object = StardewValley.Object;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    internal class ProfitInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        // public virtual int sellToStorePrice(long specificPlayerID = -1)
        public static void SellToStorePrice_ApplyProfitMargin_Postfix(Object __instance, long specificPlayerID, ref int __result)
        {
            try
            {
                __result = (int)Math.Round(__result * _archipelago.SlotData.ProfitMargin);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SellToStorePrice_ApplyProfitMargin_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
