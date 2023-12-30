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
                var myActiveHints = _archipelago.GetMyActiveHints();
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

                    _shopReplacer.ReplaceShopItem(__result, salableItem, FestivalLocationNames.RARECROW_7, item => _shopReplacer.IsRarecrow(item, 7), myActiveHints);
                    _shopReplacer.ReplaceShopItem(__result, salableItem, FestivalLocationNames.RARECROW_8, item => _shopReplacer.IsRarecrow(item, 8), myActiveHints);

                    if (_archipelago.SlotData.FestivalLocations != FestivalLocations.Hard)
                    {
                        continue;
                    }

                    _shopReplacer.ReplaceShopItem(__result, salableItem, FestivalLocationNames.CONE_HAT, item => item.which.Value == 39, myActiveHints);
                    _shopReplacer.ReplaceShopItem(__result, salableItem, FestivalLocationNames.IRIDIUM_FIREPLACE, (Furniture item) => item.ParentSheetIndex == 1796, myActiveHints);
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetMagicShopStock_UniqueItemsAndSeeds_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public override void draw(SpriteBatch b)
        public static bool Draw_DontDrawOriginalPainting_Prefix(BeachNightMarket __instance, SpriteBatch b)
        {
            try
            {
                var paintingMailKey = $"NightMarketYear{Game1.year}Day{GetDayOfNightMarket()}_paintingSold";
                if (!Game1.player.mailReceived.Contains(paintingMailKey))
                {
                    Game1.player.mailReceived.Add(paintingMailKey);
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Draw_DontDrawOriginalPainting_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public override void draw(SpriteBatch b)
        public static void Draw_DrawCorrectPainting_Postfix(BeachNightMarket __instance, SpriteBatch b)
        {
            try
            {
                var shopClosedTextureField = _modHelper.Reflection.GetField<Texture2D>(__instance, "shopClosedTexture");
                var shopClosedTexture = shopClosedTextureField.GetValue();
                var position = Game1.GlobalToLocal(new Vector2(41f, 33f) * 64f + new Vector2(2f, 2f) * 4f);
                var day = GetDayOfNightMarket();
                var month = GetNightMarketMonth();
                var sourceRectangle = new Rectangle?(new Rectangle(144 + (day - 1 + (month - 1) % 3 * 3) * 28, 201, 28, 13));
                var paintingLocationSoldToday = GetPaintingLocationToday(__instance);
                // var paintingMailKey = $"NightMarketYear{Game1.year}Day{nightMarket.getDayOfNightMarket()}_paintingSold";
                if (_locationChecker.IsLocationMissingAndExists(paintingLocationSoldToday))
                {
                    b.Draw(shopClosedTexture, position, sourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.225000009f);
                }
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Draw_DrawCorrectPainting_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        public static bool CheckAction_LupiniPainting_Prefix(BeachNightMarket __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                var buildings = __instance.map.GetLayer("Buildings");
                if (buildings.Tiles[tileLocation] == null || buildings.Tiles[tileLocation].TileIndex != 68 || Game1.timeOfDay < 1700)
                {
                    return true; // run original logic
                }

                var paintingLocationSoldToday = GetPaintingLocationToday(__instance);
                if (_locationChecker.IsLocationChecked(paintingLocationSoldToday))
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterSold"));
                    return false; // don't run original logic
                }

                __instance.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterQuestion"), __instance.createYesNoResponses(), "PainterQuestion");

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckAction_LupiniPainting_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
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
                var paintingLocationSoldToday = GetPaintingLocationToday(__instance);
                // var paintingMailKey = $"NightMarketYear{Game1.year}Day{nightMarket.getDayOfNightMarket()}_paintingSold";
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

        private static string GetPaintingLocationToday(BeachNightMarket nightMarket)
        {
            var paintingLocations = GetPaintingLocations();
            var month = GetNightMarketMonth();
            var day = nightMarket.getDayOfNightMarket();
            var paintingLocationSoldToday = paintingLocations[month][day];
            return paintingLocationSoldToday;
        }

        private static int GetNightMarketMonth()
        {
            if (_archipelago.SlotData.FestivalLocations == FestivalLocations.Easy)
            {
                return 1;
            }

            return (int)((Game1.stats.daysPlayed / 28) % 3) + 1;
        }
        public static int GetDayOfNightMarket()
        {
            return (Game1.dayOfMonth % 3) + 1;
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
