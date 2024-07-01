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
using StardewArchipelago.Constants.Vanilla;
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

namespace StardewArchipelago.GameModifications.Tooltips
{
    public class BillboardInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ModConfig _config;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static Friends _friends;
        private static Texture2D _bigArchipelagoIcon;
        private static Texture2D _miniArchipelagoIcon;
        private static Texture2D _travelingMerchantIcon;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ModConfig config, ArchipelagoClient archipelago, LocationChecker locationChecker, Friends friends)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _config = config;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _friends = friends;

            var desiredTextureName = ArchipelagoTextures.COLOR;
            _bigArchipelagoIcon = ArchipelagoTextures.GetArchipelagoLogo(monitor, modHelper, 48, desiredTextureName);
            _miniArchipelagoIcon = ArchipelagoTextures.GetArchipelagoLogo(monitor, modHelper, 24, desiredTextureName);
            _travelingMerchantIcon = TexturesLoader.GetTexture(monitor, modHelper, "traveling_merchant.png");
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
            var birthdays = billBoard.GetBirthdays();
            for (var i = 0; i < calendarDays.Count; i++)
            {
                var birthdaysToday = birthdays.GetValueOrDefault(i + 1) ?? new List<NPC>();
                DrawAPIconIfNeeded(birthdaysToday, spriteBatch, calendarDays, i);
                DrawTravelingMerchantIconIfNeeded(spriteBatch, calendarDays, i);
            }
        }

        private static void DrawAPIconIfNeeded(List<NPC> birthdaysToday, SpriteBatch b, List<ClickableTextureComponent> calendarDays, int index)
        {
            if (!_config.ShowCalendarIndicators)
            {
                return;
            }

            var day = index + 1;
            var dayComponent = calendarDays[index];

            if (!GetMissingFestivalChecks(day).Any() && !GetMissingNpcChecks(birthdaysToday).Any() && !GetMissingTravelingCartChecks(day).Any())
            {
                return;
            }

            var calendarDayPosition = new Vector2(dayComponent.bounds.X, dayComponent.bounds.Y);
            var logoPosition = calendarDayPosition + new Vector2(dayComponent.bounds.Width - 24 - 4, 4f);
            var sourceRectangle = new Rectangle(0, 0, 24, 24);
            b.Draw(_miniArchipelagoIcon, logoPosition, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        private static void DrawTravelingMerchantIconIfNeeded(SpriteBatch b, List<ClickableTextureComponent> calendarDays, int index)
        {
            var day = index + 1;
            var dayOfWeek = Days.GetDayOfWeekName(day);
            var merchantDayItem = string.Format(TravelingMerchantInjections.AP_MERCHANT_DAYS, dayOfWeek);
            if (!_archipelago.HasReceivedItem(merchantDayItem))
            {
                return;
            }

            var calendarDayPosition = new Vector2(calendarDays[index].bounds.X, calendarDays[index].bounds.Y);
            var logoPosition = calendarDayPosition + new Vector2(4, calendarDays[index].bounds.Height - 24 - 4);
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

                if (!_config.ShowCalendarIndicators)
                {
                    return;
                }

                // private string hoverText = "";
                var hoverTextField = _modHelper.Reflection.GetField<string>(__instance, "hoverText");
                var hoverText = hoverTextField.GetValue();

                var birthdays = __instance.GetBirthdays();

                for (var i = 0; i < __instance.calendarDays.Count; i++)
                {
                    var day = i + 1;
                    if (!__instance.calendarDays[i].bounds.Contains(x, y))
                    {
                        continue;
                    }

                    var birthdaysToday = birthdays.GetValueOrDefault(day) ?? new List<NPC>();

                    var missingFestivalChecks = GetMissingFestivalChecks(day);
                    var missingNpcChecks = GetMissingNpcChecks(birthdaysToday);
                    var missingCartChecks = GetMissingTravelingCartChecks(day);

                    foreach (var location in missingFestivalChecks)
                    {
                        hoverText += $"{Environment.NewLine}- {location}";
                    }

                    foreach (var location in missingNpcChecks)
                    {
                        hoverText += $"{Environment.NewLine}- {location.TurnHeartsIntoStardewHearts()}";
                    }

                    foreach (var location in missingCartChecks)
                    {
                        hoverText += $"{Environment.NewLine}- {location}";
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

        private static IEnumerable<string> GetMissingFestivalChecks(int day)
        {
            var season = Game1.currentSeason;
            var festivalIdentifier = FestivalLocationNames.FestivalIdentifier(Game1.season, day);

            if (!FestivalLocationNames.LocationsByFestival.ContainsKey(festivalIdentifier))
            {
                yield break;
            }

            var festivalLocations = FestivalLocationNames.LocationsByFestival[festivalIdentifier];
            foreach (var location in festivalLocations)
            {
                if (_locationChecker.IsLocationMissing(location))
                {
                    yield return location;
                }
            }
        }

        private static IEnumerable<string> GetMissingNpcChecks(List<NPC> birthdaysToday)
        {
            foreach (var npc in birthdaysToday)
            {
                var friend = _friends.GetFriend(npc.Name);
                if (friend == null)
                {
                    continue;
                }

                foreach (var location in _locationChecker.GetAllLocationsNotCheckedContainingWord($"Friendsanity: {friend.ArchipelagoName}"))
                {
                    yield return location;
                }
            }
        }

        private static IEnumerable<string> GetMissingTravelingCartChecks(int day)
        {
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
