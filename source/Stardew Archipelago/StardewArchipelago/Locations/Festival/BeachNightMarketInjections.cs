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
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.Festival
{
    public class BeachNightMarketInjections
    {
        private const int CATEGORY_SEEDS = -74;

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

        // public Dictionary<ISalable, int[]> geMagicShopStock()
        public static void GetMagicShopStock_UniqueItemsAndSeeds_Postfix(ShopMenu __instance, ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                foreach (var salableItem in __result.Keys.ToArray())
                {
                    if (salableItem is not Item)
                    {
                        continue;
                    }

                    if (_archipelago.SlotData.FestivalLocations == FestivalLocations.Vanilla)
                    {
                        continue;
                    }

                    _shopReplacer.ReplaceShopItem(__result, salableItem, FestivalLocationNames.RARECROW_7, item => _shopReplacer.IsRarecrow(item, 7));
                    _shopReplacer.ReplaceShopItem(__result, salableItem, FestivalLocationNames.RARECROW_8, item => _shopReplacer.IsRarecrow(item, 8));

                    if (_archipelago.SlotData.FestivalLocations != FestivalLocations.Hard)
                    {
                        continue;
                    }

                    _shopReplacer.ReplaceShopItem(__result, salableItem, FestivalLocationNames.CONE_HAT,
                        item => item.which.Value == 39);
                    _shopReplacer.ReplaceShopItem(__result, salableItem, FestivalLocationNames.IRIDIUM_FIREPLACE,
                        (Furniture item) => item.ParentSheetIndex == 1796);
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetMagicShopStock_UniqueItemsAndSeeds_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
        public static bool AnswerDialogueAction_LupiniPainting_Prefix(BeachNightMarket __instance, string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                if (_archipelago.SlotData.FestivalLocations == FestivalLocations.Vanilla || questionAndAnswer != "PainterQuestion_Yes")
                {
                    return true; // run original logic
                }

                __result = true;
                var paintingLocations = GetPaintingLocations();
                var month = (int)((Game1.stats.daysPlayed / 28) % 3) + 1;
                var day = __instance.getDayOfNightMarket();
                var paintingLocationSoldToday = paintingLocations[month][day];
                // var paintingMailKey = $"NightMarketYear{Game1.year}Day{__instance.getDayOfNightMarket()}_paintingSold";
                if (_locationChecker.IsLocationChecked(paintingLocationSoldToday))
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterSold"));
                    return false; // don't run original logic
                }
                if (Game1.player.Money < 1200)
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
                    return false; // don't run original logic
                }

                Game1.player.Money -= 1200;
                Game1.activeClickableMenu = (IClickableMenu)null;
                _locationChecker.AddCheckedLocation(paintingLocationSoldToday);
                Game1.player.CanMove = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_LupiniPainting_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static Dictionary<int, Dictionary<int, string>> GetPaintingLocations()
        {
            var year1Locations = new Dictionary<int, string>
            {
                { 1, FestivalLocationNames.LUPINI_YEAR_1_PAINTING_1 },
                { 2, FestivalLocationNames.LUPINI_YEAR_1_PAINTING_2 },
                { 3, FestivalLocationNames.LUPINI_YEAR_1_PAINTING_3 },
            };
            var year2Locations = new Dictionary<int, string>
            {
                { 1, FestivalLocationNames.LUPINI_YEAR_2_PAINTING_1 },
                { 2, FestivalLocationNames.LUPINI_YEAR_2_PAINTING_2 },
                { 3, FestivalLocationNames.LUPINI_YEAR_2_PAINTING_3 },
            };
            var year3Locations = new Dictionary<int, string>
            {
                { 1, FestivalLocationNames.LUPINI_YEAR_3_PAINTING_1 },
                { 2, FestivalLocationNames.LUPINI_YEAR_3_PAINTING_2 },
                { 3, FestivalLocationNames.LUPINI_YEAR_3_PAINTING_3 },
            };
            var paintingLocations = new Dictionary<int, Dictionary<int, string>>();
            paintingLocations.Add(1, year1Locations);
            if (_archipelago.SlotData.FestivalLocations == FestivalLocations.Hard)
            {
                paintingLocations.Add(2, year2Locations);
                paintingLocations.Add(3, year3Locations);
            }
            else
            {
                paintingLocations.Add(2, year1Locations);
                paintingLocations.Add(3, year1Locations);
            }

            return paintingLocations;
        }
    }
}
