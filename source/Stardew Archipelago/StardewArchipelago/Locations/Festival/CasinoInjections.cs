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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace StardewArchipelago.Locations.Festival
{
    public class CasinoInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static ShopReplacer _shopReplacer;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, ShopReplacer shopReplacer)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _shopReplacer = shopReplacer;
        }

        // public static Dictionary<ISalable, int[]> getQiShopStock()
        public static void GetQiShopStock_AlienRarecrowCheck_Postfix(ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                var myActiveHints = _archipelago.GetMyActiveHints();
                foreach (var salableItem in __result.Keys.ToArray())
                {
                    if (salableItem is not Item)
                    {
                        continue;
                    }

                    _shopReplacer.ReplaceShopItem(__result, salableItem, FestivalLocationNames.RARECROW_3, item => _shopReplacer.IsRarecrow(item, 3), myActiveHints);
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetQiShopStock_AlienRarecrowCheck_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
