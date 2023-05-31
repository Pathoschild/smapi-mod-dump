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
using Archipelago.MultiClient.Net.Models;
using Microsoft.Xna.Framework.Content;
using Netcode;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Extensions;
using StardewArchipelago.Items.Unlocks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public class SpecialOrderInjections
    {
        private static readonly string[] _ignoredSpecialOrders =
        {
            // Add ignored orders here
        };

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static ContentManager _englishContentManager;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _englishContentManager =
                new ContentManager(Game1.game1.Content.ServiceProvider, Game1.game1.Content.RootDirectory);
        }

        // public static bool IsSpecialOrdersBoardUnlocked()
        public static bool IsSpecialOrdersBoardUnlocked_UnlockBasedOnApItem_Prefix(ref bool __result)
        {
            try
            {
                __result = _archipelago.HasReceivedItem(VanillaUnlockManager.SPECIAL_ORDER_BOARD_AP_NAME);
                return false; // don't run original logic;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(IsSpecialOrdersBoardUnlocked_UnlockBasedOnApItem_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic;
            }
        }

        // public static SpecialOrder GetSpecialOrder(string key, int? generation_seed)
        public static void GetSpecialOrder_ArchipelagoReward_Postfix(string key, int? generation_seed, ref SpecialOrder __result)
        {
            try
            {
                var specialOrderName = __result.GetName();
                if (_ignoredSpecialOrders.Contains(specialOrderName))
                {
                    return;
                }

                // Remove vanilla rewards if the player has not received the check.
                // We will keep vanilla rewards for repeated orders
                if (_locationChecker.IsLocationMissingAndExists(specialOrderName))
                {
                    __result.rewards.Clear();
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetSpecialOrder_ArchipelagoReward_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        // public void CheckCompletion()
        public static void CheckCompletion_ArchipelagoReward_Postfix(SpecialOrder __instance)
        {
            try
            {
                if (__instance.questState.Value != SpecialOrder.QuestState.Complete)
                {
                    return;
                }

                var specialOrderName = __instance.GetName();
                if (_ignoredSpecialOrders.Contains(specialOrderName))
                {
                    return;
                }

                _locationChecker.AddCheckedLocation(specialOrderName);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckCompletion_ArchipelagoReward_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        // public virtual void SetDuration(SpecialOrder.QuestDuration duration)
        public static bool SetDuration_UseCorrectDateWithSeasonRandomizer_Prefix(SpecialOrder __instance, SpecialOrder.QuestDuration duration)
        {
            try
            {
                __instance.questDuration.Value = duration;
                var today = Game1.Date.TotalDays;
                switch (duration)
                {
                    case SpecialOrder.QuestDuration.Week:
                        // worldDate = new WorldDate(Game1.year, Game1.currentSeason, (Game1.dayOfMonth - 1) / 7 * 7);
                        __instance.dueDate.Value = today + (7 - Game1.dayOfMonth % 7);
                        break;
                    case SpecialOrder.QuestDuration.Month:
                        __instance.dueDate.Value = today + (28 - Game1.dayOfMonth);
                        break;
                    case SpecialOrder.QuestDuration.TwoWeeks:
                        // worldDate = new WorldDate(Game1.year, Game1.currentSeason, (Game1.dayOfMonth - 1) / 7 * 7);
                        __instance.dueDate.Value = today + (14 - Game1.dayOfMonth % 7);
                        break;
                    case SpecialOrder.QuestDuration.TwoDays:
                        __instance.dueDate.Value = today + 2;
                        break;
                    case SpecialOrder.QuestDuration.ThreeDays:
                        __instance.dueDate.Value = today + 3;
                        break;
                }

                return false; // don't run original logic;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(IsSpecialOrdersBoardUnlocked_UnlockBasedOnApItem_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic;
            }
        }

        // public static void UpdateAvailableSpecialOrders(bool force_refresh)
        public static bool UpdateAvailableSpecialOrders_ChangeFrequencyToBeLessRng_Prefix(bool force_refresh)
        {
            try
            {
                UpdateAvailableSpecialOrdersBasedOnApState(force_refresh);
                return false; // don't run original logic;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(IsSpecialOrdersBoardUnlocked_UnlockBasedOnApItem_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic;
            }
        }

        private static void UpdateAvailableSpecialOrdersBasedOnApState(bool force_refresh)
        {
            if (Game1.player.team.availableSpecialOrders != null)
            {
                foreach (var availableSpecialOrder in Game1.player.team.availableSpecialOrders)
                {
                    if ((availableSpecialOrder.questDuration.Value == SpecialOrder.QuestDuration.TwoDays ||
                         availableSpecialOrder.questDuration.Value == SpecialOrder.QuestDuration.ThreeDays) &&
                        !Game1.player.team.acceptedSpecialOrderTypes.Contains(availableSpecialOrder.orderType.Value))
                    {
                        availableSpecialOrder.SetDuration((SpecialOrder.QuestDuration)availableSpecialOrder.questDuration);
                    }
                }
            }

            if (Game1.player.team.availableSpecialOrders.Count > 0 && !force_refresh)
            {
                return;
            }


            Game1.player.team.availableSpecialOrders.Clear();
            Game1.player.team.acceptedSpecialOrderTypes.Clear();
            var random = new Random((int)Game1.uniqueIDForThisGame + (int)(Game1.stats.DaysPlayed * 1.2999999523162842));
            var allSpecialOrdersData = Game1.content.Load<Dictionary<string, SpecialOrderData>>("Data\\SpecialOrders");
            var specialOrdersThatCanBeStartedToday = FilterToSpecialOrdersThatCanBeStartedToday(allSpecialOrdersData);
            var specialOrdersForBoard = CreateSpecialOrderInstancesForType(specialOrdersThatCanBeStartedToday, "", random);
            var specialOrdersForQi = CreateSpecialOrderInstancesForType(specialOrdersThatCanBeStartedToday, "Qi", random);

            var hints = _archipelago.GetHints().Where(x => !x.Found && _archipelago.GetPlayerName(x.FindingPlayer) == _archipelago.SlotData.SlotName).ToArray();

            AddTwoOrdersToBoard(specialOrdersForBoard, hints, random);
            AddTwoOrdersToBoard(specialOrdersForQi, hints, random);
        }

        private static IEnumerable<KeyValuePair<string, SpecialOrderData>> FilterToSpecialOrdersThatCanBeStartedToday(Dictionary<string, SpecialOrderData> allSpecialOrdersData)
        {
            var specialOrdersThatCanBeStartedToday = allSpecialOrdersData
                .Where(order => !Game1.player.team.completedSpecialOrders.ContainsKey(order.Key) ||
                                order.Value.Repeatable == "True")
                .Where(order => order.Value.Duration != "Month" || Game1.dayOfMonth <= 16)
                .Where(order => SpecialOrder.CheckTags(order.Value.RequiredTags))
                .Where(order => Game1.player.team.specialOrders.All(x => x.questKey.Value != order.Key));
            return specialOrdersThatCanBeStartedToday;
        }

        private static Dictionary<string, SpecialOrder> CreateSpecialOrderInstancesForType(IEnumerable<KeyValuePair<string, SpecialOrderData>> specialOrdersThatCanBeStartedToday, string orderType, Random random)
        {
            var specialOrders = specialOrdersThatCanBeStartedToday
                .Where(order => order.Value.OrderType == orderType)
                .Select(x => SpecialOrder.GetSpecialOrder(x.Key, random.Next()))
                .ToDictionary(x => x.GetName(), x => x);
            return specialOrders;
        }

        private static void AddTwoOrdersToBoard(Dictionary<string, SpecialOrder> specialOrders,
            Hint[] hints, Random random)
        {
            var allSpecialOrders = specialOrders.Select(x => x.Key).ToList();

            var specialOrdersNeverCompletedBefore = allSpecialOrders.Where(key =>
                _locationChecker.IsLocationMissingAndExists(specialOrders[key].GetName())).ToList();

            var hintedSpecialOrders = specialOrdersNeverCompletedBefore.Where(key =>
                hints.Any(hint => _archipelago.GetLocationName(hint.LocationId) == specialOrders[key].GetName())).ToList();

            allSpecialOrders.RemoveAll(x => specialOrdersNeverCompletedBefore.Contains(x));
            specialOrdersNeverCompletedBefore.RemoveAll(x => hintedSpecialOrders.Contains(x));

            hintedSpecialOrders = hintedSpecialOrders.Shuffle(random);
            specialOrdersNeverCompletedBefore = specialOrdersNeverCompletedBefore.Shuffle(random);
            allSpecialOrders = allSpecialOrders.Shuffle(random);

            var allOrdersOrdered = new List<string>(hintedSpecialOrders);
            if (allOrdersOrdered.Count < 2)
            {
                allOrdersOrdered.AddRange(specialOrdersNeverCompletedBefore);
            }
            if (allOrdersOrdered.Count < 2)
            {
                allOrdersOrdered.AddRange(allSpecialOrders);
            }

            for (var i = 0; i < 2; ++i)
            {
                var order = allOrdersOrdered[i];
                Game1.player.team.availableSpecialOrders.Add(specialOrders[order]);
            }
        }
    }
}