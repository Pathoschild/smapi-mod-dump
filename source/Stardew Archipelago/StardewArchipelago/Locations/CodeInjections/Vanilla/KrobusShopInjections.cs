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
using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    internal class KrobusShopInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private const int STARDROP = 434;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public Dictionary<ISalable, int[]> getShadowShopStock()
        public static void GetShadowShopStock_StardropCheck_Postfix(ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                RemoveStardropFromFromStock(__result);
                AddStardropCheckToStock(__result);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetShadowShopStock_StardropCheck_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void RemoveStardropFromFromStock(Dictionary<ISalable, int[]> stock)
        {
            foreach (var salable in stock.Keys.ToArray())
            {
                if (salable is Object { ParentSheetIndex: STARDROP })
                {
                    stock.Remove(salable);
                }
            }
        }

        private static void AddStardropCheckToStock(Dictionary<ISalable, int[]> stock)
        {
            const string apLocation = "Krobus Stardrop";
            if (!_locationChecker.IsLocationMissing(apLocation))
            {
                return;
            }

            var activeHints = _archipelago.GetMyActiveHints();
            var purchasableCheck = new PurchaseableArchipelagoLocation("Stardrop", apLocation, _modHelper, _locationChecker, _archipelago, activeHints);
            stock.Add(purchasableCheck, new[] { 20000, 1 });
        }
    }
}
