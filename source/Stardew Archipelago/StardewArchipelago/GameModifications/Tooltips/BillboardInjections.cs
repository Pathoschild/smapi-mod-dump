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
using StardewArchipelago.Constants;
using StardewArchipelago.Extensions;
using StardewArchipelago.Locations;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship;
using StardewArchipelago.Locations.Festival;
using StardewArchipelago.Textures;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace StardewArchipelago.GameModifications.Tooltips
{
    public class BillboardInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static Friends _friends;
        private static Texture2D _bigArchipelagoIcon;
        private static Texture2D _miniArchipelagoIcon;
        private static Texture2D _travelingMerchantIcon;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, Friends friends)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _friends = friends;

            var desiredTextureName = ArchipelagoTextures.COLOR;
            _bigArchipelagoIcon = ArchipelagoTextures.GetColoredLogo(modHelper, 48, desiredTextureName);
            _miniArchipelagoIcon = ArchipelagoTextures.GetColoredLogo(modHelper, 24, desiredTextureName);
            _travelingMerchantIcon = TexturesLoader.GetTexture(modHelper, "traveling_merchant.png");
        }

        // public override void draw(SpriteBatch spriteBatch)
        public static void Draw_AddArchipelagoIndicators_Postfix(Billboard __instance, SpriteBatch b)
        {
            try
            {
                // private bool dailyQuestBoard;
                var dailyQuestBoard = _modHelper.Reflection.GetField<bool>(__instance, "dailyQuestBoard").GetValue();
                if (dailyQuestBoard)
                {
                    DrawDailyQuestIndicator(__instance, b);
                }
                else
                {
                    DrawCalendarIndicators(__instance, b);
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Draw_AddArchipelagoIndicators_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void DrawDailyQuestIndicator(Billboard billBoard, SpriteBatch spriteBatch)
        {
            var quest = Game1.questOfTheDay;
            if (quest?.currentObjective == null || quest.currentObjective.Length == 0)
            {
                return;
            }

            var dailyQuestCheckName = GetDailyQuestCheckName(quest);

            if (string.IsNullOrWhiteSpace(dailyQuestCheckName) || !_locationChecker.GetAllLocationsNotCheckedContainingWord(dailyQuestCheckName).Any())
            {
                return;
            }

            var size = 48;
            var position1 = new Vector2(billBoard.acceptQuestButton.bounds.X - size - 12, billBoard.acceptQuestButton.bounds.Y + 12);
            var position2 = new Vector2(billBoard.acceptQuestButton.bounds.X + billBoard.acceptQuestButton.bounds.Width + 12, billBoard.acceptQuestButton.bounds.Y + 12);
            var sourceRectangle = new Rectangle(0, 0, size, size);
            var color = Color.White;
            spriteBatch.Draw(_bigArchipelagoIcon, position1, sourceRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            spriteBatch.Draw(_bigArchipelagoIcon, position2, sourceRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        private static string GetDailyQuestCheckName(Quest quest)
        {
            return quest.questType.Value switch
            {
                (int)QuestType.ItemDelivery => string.Format(DailyQuest.HELP_WANTED, DailyQuest.ITEM_DELIVERY),
                (int)QuestType.SlayMonsters => string.Format(DailyQuest.HELP_WANTED, DailyQuest.SLAY_MONSTERS),
                (int)QuestType.Fishing => string.Format(DailyQuest.HELP_WANTED, DailyQuest.FISHING),
                (int)QuestType.ResourceCollection => string.Format(DailyQuest.HELP_WANTED, DailyQuest.GATHERING),
                _ => "",
            };
        }

        private static void DrawCalendarIndicators(Billboard billBoard, SpriteBatch spriteBatch)
        {
            var calendarDays = billBoard.calendarDays;
            for (int i = 0; i < calendarDays.Count; i++)
            {
                DrawAPIconIfNeeded(spriteBatch, calendarDays, i);
                DrawTravelingMerchantIconIfNeeded(spriteBatch, calendarDays, i);
            }
        }

        private static void DrawAPIconIfNeeded(SpriteBatch b, List<ClickableTextureComponent> calendarDays, int i)
        {
            var festivalName = calendarDays[i].name;
            var birthdayName = calendarDays[i].hoverText;

            if (!GetMissingFestivalChecks(festivalName, i).Any() && !GetMissingNpcChecks(birthdayName).Any() && !GetMissingTravelingCartChecks(i).Any())
            {
                return;
            }

            var calendarDayPosition = new Vector2(calendarDays[i].bounds.X, calendarDays[i].bounds.Y);
            var logoPosition = calendarDayPosition + new Vector2(calendarDays[i].bounds.Width - 24 - 4, 4f);
            var sourceRectangle = new Rectangle(0, 0, 24, 24);
            b.Draw(_miniArchipelagoIcon, logoPosition, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        private static void DrawTravelingMerchantIconIfNeeded(SpriteBatch b, List<ClickableTextureComponent> calendarDays, int i)
        {
            var day = i + 1;
            var dayOfWeek = Days.GetDayOfWeekName(day);
            var merchantDayItem = string.Format(TravelingMerchantInjections.AP_MERCHANT_DAYS, dayOfWeek);
            if (!_archipelago.HasReceivedItem(merchantDayItem))
            {
                return;
            }

            var calendarDayPosition = new Vector2(calendarDays[i].bounds.X, calendarDays[i].bounds.Y);
            var logoPosition = calendarDayPosition + new Vector2(4, calendarDays[i].bounds.Height - 24 - 4);
            var sourceRectangle = new Rectangle(0, 0, 12, 12);
            b.Draw(_travelingMerchantIcon, logoPosition, sourceRectangle, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
        }

        // public override void performHoverAction(int x, int y)
        public static void PerformHoverAction_AddArchipelagoChecksToTooltips_Postfix(Billboard __instance, int x, int y)
        {
            try
            {
                // private bool dailyQuestBoard;
                if (_modHelper.Reflection.GetField<bool>(__instance, "dailyQuestBoard").GetValue())
                {
                    return;
                }

                // private string hoverText = "";
                var hoverTextField = _modHelper.Reflection.GetField<string>(__instance, "hoverText");
                var hoverText = hoverTextField.GetValue();

                for (int i = 0; i < __instance.calendarDays.Count; i++)
                {
                    if (!__instance.calendarDays[i].bounds.Contains(x, y))
                    {
                        continue;
                    }

                    var festivalName = __instance.calendarDays[i].name;
                    var birthdayName = __instance.calendarDays[i].hoverText;

                    var missingFestivalChecks = GetMissingFestivalChecks(festivalName, i);
                    var missingNpcChecks = GetMissingNpcChecks(birthdayName);
                    var missingCartChecks = GetMissingTravelingCartChecks(i);

                    foreach (var location in missingFestivalChecks)
                    {
                        hoverText += $"{Environment.NewLine}{location}";
                    }

                    foreach (var location in missingNpcChecks)
                    {
                        hoverText += $"{Environment.NewLine}{location.TurnHeartsIntoStardewHearts()}";
                    }

                    foreach (var location in missingCartChecks)
                    {
                        hoverText += $"{Environment.NewLine}{location}";
                    }
                }

                hoverTextField.SetValue(hoverText.Trim());
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformHoverAction_AddArchipelagoChecksToTooltips_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static IEnumerable<string> GetMissingFestivalChecks(string festivalName, int day)
        {
            if (Game1.currentSeason.Equals("winter") && day >= 14 && day <= 16)
            {
                var festivalDay = FestivalLocationNames.NIGHT_MARKET_ALL;
                foreach (var location in FestivalLocationNames.LocationsByFestival[festivalDay])
                {
                    if (_locationChecker.IsLocationMissing(location))
                    {
                        yield return location;
                    }
                }

                if (day == 14)
                {
                    festivalDay = FestivalLocationNames.NIGHT_MARKET_15;
                }
                else if (day == 15)
                {
                    festivalDay = FestivalLocationNames.NIGHT_MARKET_16;
                }
                else if (day == 16)
                {
                    festivalDay = FestivalLocationNames.NIGHT_MARKET_17;
                }

                foreach (var location in FestivalLocationNames.LocationsByFestival[festivalDay])
                {
                    if (_locationChecker.IsLocationMissing(location))
                    {
                        yield return location;
                    }
                }

                yield break;
            }

            if (!FestivalLocationNames.LocationsByFestival.ContainsKey(festivalName))
            {
                yield break;
            }

            var festivalLocations = FestivalLocationNames.LocationsByFestival[festivalName];
            foreach (var location in festivalLocations)
            {
                if (_locationChecker.IsLocationMissing(location))
                {
                    yield return location;
                }
            }
        }

        private static IEnumerable<string> GetMissingNpcChecks(string npcName)
        {
            var friend = _friends.GetFriend(npcName);
            if (friend == null)
            {
                return new string[0];
            }
            return _locationChecker.GetAllLocationsNotCheckedContainingWord($"Friendsanity: {friend.ArchipelagoName}");
        }

        private static IEnumerable<string> GetMissingTravelingCartChecks(int i)
        {
            var day = i + 1;
            var dayOfWeek = Days.GetDayOfWeekName(day);
            var merchantDayItem = string.Format(TravelingMerchantInjections.AP_MERCHANT_DAYS, dayOfWeek);
            if (!_archipelago.HasReceivedItem(merchantDayItem))
            {
                return Enumerable.Empty<string>();
            }

            var merchantItemPatern = $"Traveling Merchant {dayOfWeek} Item";
            return _locationChecker.GetAllLocationsNotCheckedContainingWord(merchantItemPatern);
        }
    }
}
