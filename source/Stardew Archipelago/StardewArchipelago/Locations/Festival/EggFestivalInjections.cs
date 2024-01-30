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
    public static class EggFestivalInjections
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

        // public virtual void command_awardFestivalPrize(GameLocation location, GameTime time, string[] split)
        public static bool AwardFestivalPrize_Strawhat_Prefix(Event __instance, GameLocation location, GameTime time, string[] split)
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
                var isEggFestivalDay = festivalData["file"] == "spring13";

                if (!playerWonFestival || !isEggFestivalDay)
                {
                    return true; // run original logic
                }

                _locationChecker.AddCheckedLocation(FestivalLocationNames.EGG_HUNT);
                if (Game1.player.mailReceived.Contains("Egg Festival"))
                {
                    return true; // run original logic
                }

                Game1.player.mailReceived.Add("Egg Festival");
                __instance.CurrentCommand += 2;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AwardFestivalPrize_Strawhat_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static ShopMenu _lastShopMenuUpdated = null;
        // public override void update(GameTime time)
        public static void Update_AddStrawberrySeedsCheck_Postfix(ShopMenu __instance, GameTime time)
        {
            try
            {
                // We only run this once for each menu
                if (_lastShopMenuUpdated == __instance || Game1.CurrentEvent == null ||!Game1.CurrentEvent.isSpecificFestival("spring13"))
                {
                    return;
                }

                _lastShopMenuUpdated = __instance;
                if (!_locationChecker.IsLocationMissing(FestivalLocationNames.STRAWBERRY_SEEDS))
                {
                    return;
                }

                if (__instance.itemPriceAndStock.Any(x =>
                        x.Key is PurchaseableArchipelagoLocation { LocationName: FestivalLocationNames.STRAWBERRY_SEEDS }))
                {
                    return;
                }


                var myActiveHints = _archipelago.GetMyActiveHints();
                var strawberrySeedsApItem = new PurchaseableArchipelagoLocation("Strawberry Seeds", FestivalLocationNames.STRAWBERRY_SEEDS, _modHelper, _locationChecker, _archipelago, myActiveHints);
                __instance.itemPriceAndStock.Add(strawberrySeedsApItem, new[] { 1000, 1 });
                __instance.forSale.Add(strawberrySeedsApItem);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Update_AddStrawberrySeedsCheck_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
