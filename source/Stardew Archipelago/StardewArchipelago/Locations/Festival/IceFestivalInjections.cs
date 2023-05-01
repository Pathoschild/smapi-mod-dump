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
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.Locations.Festival
{
    internal class IceFestivalInjections
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

        // public virtual void command_awardFestivalPrize(GameLocation location, GameTime time, string[] split)
        public static bool AwardFestivalPrize_FishingCompetition_Prefix(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            try
            {
                var festivalWinnersField = _modHelper.Reflection.GetField<HashSet<long>>(__instance, "festivalWinners");
                var festivalWinners = festivalWinnersField.GetValue();
                var festivalDataField = _modHelper.Reflection.GetField<Dictionary<string, string>>(__instance, "festivalData");
                var festivalData = festivalDataField.GetValue();

                if (festivalWinners == null || festivalData == null)
                {
                    return true; // run original logic
                }

                var playerWonFestival = festivalWinners.Contains(Game1.player.UniqueMultiplayerID);
                var isIceFestivalDay = festivalData["file"] == "winter8";

                if (!playerWonFestival || !isIceFestivalDay)
                {
                    return true; // run original logic
                }

                _locationChecker.AddCheckedLocation(FestivalLocationNames.FISHING_COMPETITION);
                if (Game1.player.mailReceived.Contains("Ice Festival"))
                {
                    return true; // run original logic
                }

                Game1.player.mailReceived.Add("Ice Festival");
                __instance.CurrentCommand += 2;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AwardFestivalPrize_FishingCompetition_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public ShopMenu(Dictionary<ISalable, int[]> itemPriceAndStock, int currency = 0, string who = null, Func<ISalable, Farmer, int, bool> on_purchase = null, Func<ISalable, bool> on_sell = null, string context = null)
        public static bool ShopMenu_HandleRarecrow4_Prefix(ShopMenu __instance, ref Dictionary<ISalable, int[]> itemPriceAndStock, int currency = 0, string who = null, Func<ISalable, Farmer, int, bool> on_purchase = null, Func<ISalable, bool> on_sell = null, string context = null)
        {
            try
            {
                foreach (var salableItem in itemPriceAndStock.Keys.ToArray())
                {
                    _shopReplacer.ReplaceShopItem(itemPriceAndStock, salableItem, FestivalLocationNames.RARECROW_4, item => _shopReplacer.IsRarecrow(item, 4));
                }
                return true; //  run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ShopMenu_HandleRarecrow4_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
