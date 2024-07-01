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
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Extensions;
using StardewArchipelago.Items.Unlocks.Vanilla;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.SpecialOrders;
using StardewValley.SpecialOrders;
using StardewValley.SpecialOrders.Rewards;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public class SpecialOrderInjections
    {
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
            _englishContentManager = new ContentManager(Game1.game1.Content.ServiceProvider, Game1.game1.Content.RootDirectory);
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
                RemoveObsoleteRewards(__result);
                AdjustRequirements(__result);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetSpecialOrder_ArchipelagoReward_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        private static void RemoveObsoleteRewards(SpecialOrder specialOrder)
        {
            var specialOrderName = GetEnglishQuestName(specialOrder.questName.Value);
            if (!_archipelago.LocationExists(specialOrderName))
            {
                return;
            }

            // Remove vanilla rewards if the player has not received the check.
            // We will keep vanilla rewards for repeated orders
            var checkMissing = _locationChecker.IsLocationMissing(specialOrderName);
            var shouldHaveVanillaRewards = IgnoredModdedStrings.SpecialOrders.Contains(specialOrderName);
            if (shouldHaveVanillaRewards)
            {
                return;
            }

            if (checkMissing)
            {
                specialOrder.rewards.Clear();
                Game1.player.team.specialOrders.Remove(specialOrder); // Might as well, and it cleans up SVE special orders.
                return;
            }

            // If the order has already been completed once, we can allow some non-unique rewards only
            for (var i = specialOrder.rewards.Count - 1; i >= 0; i--)
            {
                var reward = specialOrder.rewards[i];
                if (reward is MoneyReward or GemsReward or FriendshipReward)
                {
                    continue;
                }
                if (reward is ObjectReward objectReward)
                {
                    if (objectReward.itemKey.Value == "CalicoEgg")
                    {
                        continue;
                    }
                }
                specialOrder.rewards.RemoveAt(i);
            }
            return;
        }

        private static void AdjustRequirements(SpecialOrder specialOrder)
        {
            var requirementMultiplier = 1.0;
            if (_archipelago.SlotData.SpecialOrderLocations.HasFlag(SpecialOrderLocations.VeryShort))
            {
                requirementMultiplier = 0.2;
            }
            else if (_archipelago.SlotData.SpecialOrderLocations.HasFlag(SpecialOrderLocations.Short))
            {
                requirementMultiplier = 0.6;
            }
            else
            {
                return;
            }

            foreach (var objective in specialOrder.objectives)
            {
                if (objective.maxCount.Value <= 1)
                {
                    continue;
                }

                objective.maxCount.Value = Math.Max(1, (int)Math.Round(objective.maxCount.Value * requirementMultiplier));
            }
        }

        // public void CheckCompletion()
        public static void CheckCompletion_ArchipelagoReward_Postfix(SpecialOrder __instance)
        {
            try
            {
                if (__instance.questState.Value != SpecialOrderStatus.Complete)
                {
                    return;
                }

                var specialOrderName = GetEnglishQuestName(__instance.questName.Value);
                if (!_archipelago.LocationExists(specialOrderName))
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

        // public virtual void SetDuration(QuestDuration duration)
        public static bool SetDuration_UseCorrectDateWithSeasonRandomizer_Prefix(SpecialOrder __instance, QuestDuration duration)
        {
            try
            {
                __instance.questDuration.Value = duration;
                var today = Game1.Date.TotalDays;
                switch (duration)
                {
                    case QuestDuration.Week:
                        // worldDate = new WorldDate(Game1.year, Game1.currentSeason, (Game1.dayOfMonth - 1) / 7 * 7);
                        __instance.dueDate.Value = today + (7 - Game1.dayOfMonth % 7) + 1;
                        break;
                    case QuestDuration.Month:
                        __instance.dueDate.Value = today + (28 - Game1.dayOfMonth) + 1;
                        break;
                    case QuestDuration.TwoWeeks:
                        // worldDate = new WorldDate(Game1.year, Game1.currentSeason, (Game1.dayOfMonth - 1) / 7 * 7);
                        __instance.dueDate.Value = today + (14 - Game1.dayOfMonth % 7) + 1;
                        break;
                    case QuestDuration.OneDay:
                        __instance.dueDate.Value = today + 1;
                        break;
                    case QuestDuration.TwoDays:
                        __instance.dueDate.Value = today + 2;
                        break;
                    case QuestDuration.ThreeDays:
                        __instance.dueDate.Value = today + 3;
                        break;
                }

                return false; // don't run original logic;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SetDuration_UseCorrectDateWithSeasonRandomizer_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic;
            }
        }

        // public static void UpdateAvailableSpecialOrders(string orderType, bool forceRefresh)
        public static bool UpdateAvailableSpecialOrders_ChangeFrequencyToBeLessRng_Prefix(string orderType, bool forceRefresh)
        {
            try
            {
                if (Game1.player.team.availableSpecialOrders­ is null)
                {
                    return true; // run original logic
                }

                SetDurationOfSpecialOrders(Game1.player.team.availableSpecialOrders);

                // Let the game pick festival orders, they aren't checks anyway, right?
                if (orderType.Equals("DesertFestivalMarlon", StringComparison.InvariantCultureIgnoreCase) || (!_archipelago.SlotData.SpecialOrderLocations.HasFlag(SpecialOrderLocations.Board) && !_archipelago.SlotData.SpecialOrderLocations.HasFlag(SpecialOrderLocations.Qi)))
                {
                    return true; // run original logic
                }

                UpdateAvailableSpecialOrdersBasedOnApState(orderType, forceRefresh);
                return false; // don't run original logic;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(UpdateAvailableSpecialOrders_ChangeFrequencyToBeLessRng_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic;
            }
        }

        private static void UpdateAvailableSpecialOrdersBasedOnApState(string orderType, bool forceRefresh)
        {
            if (!forceRefresh)
            {
                if (Game1.player.team.availableSpecialOrders.Any(availableSpecialOrder => availableSpecialOrder.orderType.Value == orderType))
                {
                    return;
                }
            }

            SpecialOrder.RemoveAllSpecialOrders(orderType);

            var random = Utility.CreateRandom((double)Game1.uniqueIDForThisGame, (double)Game1.stats.DaysPlayed * 1.3);
            var allSpecialOrdersData = DataLoader.SpecialOrders(Game1.content);
            var specialOrdersThatCanBeStartedToday = FilterToSpecialOrdersThatCanBeStartedToday(allSpecialOrdersData, orderType);

            var specialOrderInstances = CreateSpecialOrderInstancesForType(specialOrdersThatCanBeStartedToday, orderType, random);

            var hints = _archipelago.GetHints().Where(x => !x.Found && _archipelago.GetPlayerName(x.FindingPlayer) == _archipelago.SlotData.SlotName).ToArray();

            ChooseTwoOrders(specialOrderInstances, hints, random);
        }

        private static void SetDurationOfSpecialOrders(IEnumerable<SpecialOrder> specialOrders)
        {
            foreach (var availableSpecialOrder in specialOrders)
            {
                if (availableSpecialOrder.questDuration.Value is QuestDuration.OneDay or QuestDuration.TwoDays or QuestDuration.ThreeDays &&
                    !Game1.player.team.acceptedSpecialOrderTypes.Contains(availableSpecialOrder.orderType.Value))
                {
                    availableSpecialOrder.SetDuration(availableSpecialOrder.questDuration.Value);
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, SpecialOrderData>> FilterToSpecialOrdersThatCanBeStartedToday(
            Dictionary<string, SpecialOrderData> allSpecialOrdersData, string specialOrderType)
        {
            // A lot of this code is duplicated from SpecialOrder.CanStartOrderNow(orderId, order)
            // But I need to do something special with CheckTags so I had to split it and run it on my own
            var specialOrdersThatCanBeStartedToday = allSpecialOrdersData
                .Where(order => order.Value.OrderType == specialOrderType)
                .Where(order => order.Value.Repeatable || !Game1.MasterPlayer.team.completedSpecialOrders.Contains(order.Key))
                .Where(order => Game1.dayOfMonth < 16 || order.Value.Duration != QuestDuration.Month)
                .Where(order => CheckTags(order.Value.RequiredTags))
                .Where(order => GameStateQuery.CheckConditions(order.Value.Condition))
                .Where(order => Game1.player.team.specialOrders.All(x => x.questKey.Value != order.Key))
                .Where(order => !_archipelago.SlotData.ToolProgression.HasFlag(ToolProgression.Progressive) || !order.Key.StartsWith("Demetrius") ||
                                _archipelago.HasReceivedItem("Progressive Fishing Rod"));
            return specialOrdersThatCanBeStartedToday;
        }

        private static bool CheckTags(string requiredTags)
        {
            if (requiredTags == null)
            {
                return true;
            }

            var splitTags = requiredTags.Split(",").Select(x => x.Trim()).Where(x => x.Length > 0);
            if (splitTags.Any(tag => !CheckIslandTagArchipelago(tag)))
            {
                return false;
            }

            return SpecialOrder.CheckTags(requiredTags);
        }

        private static bool CheckIslandTagArchipelago(string requiredTag)
        {
            if (requiredTag.Equals("island", StringComparison.InvariantCultureIgnoreCase))
            {
                return _archipelago.HasReceivedItem("Island Obelisk") || _archipelago.HasReceivedItem("Boat Repair");
            }

            return true;
        }

        private static Dictionary<string, SpecialOrder> CreateSpecialOrderInstancesForType(
            IEnumerable<KeyValuePair<string, SpecialOrderData>> specialOrdersThatCanBeStartedToday, string orderType, Random random)
        {
            var specialOrders = specialOrdersThatCanBeStartedToday
                .Where(order => order.Value.OrderType == orderType)
                .Select(x => SpecialOrder.GetSpecialOrder(x.Key, random.Next()))
                .ToDictionary(x => x.GetName(), x => x);
            return specialOrders;
        }

        private static void ChooseTwoOrders(Dictionary<string, SpecialOrder> specialOrders,
            Hint[] hints, Random random)
        {
            var allSpecialOrders = specialOrders.Select(x => x.Key).ToList();

            var specialOrdersNeverCompletedBefore = allSpecialOrders.Where(key =>
                _locationChecker.IsLocationMissing(specialOrders[key].GetName())).ToList();

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

        public static string GetEnglishQuestName(string questNameKey)
        {
            var specialOrderStrings = _englishContentManager.Load<Dictionary<string, string>>("Strings\\SpecialOrderStrings");
            questNameKey = questNameKey.Trim();
            int startIndex;
            do
            {
                startIndex = questNameKey.LastIndexOf('[');
                if (startIndex >= 0)
                {
                    var num = questNameKey.IndexOf(']', startIndex);
                    if (num == -1)
                    {
                        return questNameKey;
                    }

                    var str1 = questNameKey.Substring(startIndex + 1, num - startIndex - 1);
                    var thisString = specialOrderStrings.ContainsKey(str1) ? specialOrderStrings[str1] : SpecialOrderNames.Mods[str1];
                    questNameKey = questNameKey.Remove(startIndex, num - startIndex + 1);
                    questNameKey = questNameKey.Insert(startIndex, thisString);
                }
            } while (startIndex >= 0);

            return questNameKey;
        }
    }
}
