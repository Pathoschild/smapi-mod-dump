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
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Constants;
using StardewValley.Menus;

namespace StardewArchipelago.Locations.Festival
{
    internal class SquidFestInjections
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

        // public virtual bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
        public static bool AnswerDialogueAction_SquidFestRewards_Prefix(GameLocation __instance, string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                if (!questionAndAnswer.Equals("SquidFestBooth_Rewards", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true; // run original logic
                }

                var monthNumber = (int)(Game1.stats.DaysPlayed / 28);
                var lastRewardMailToday = $"GotSquidFestReward_{monthNumber}_{Game1.dayOfMonth}_3";
                if (Game1.player.mailReceived.Contains(lastRewardMailToday))
                {
                    return true; // run original logic
                }
                __result = true;

                var rewardsToGive = new List<string>();
                var requiredSquidsToday = Game1.dayOfMonth != 12 ? new[] { 2, 5, 7, 10 } : new[] { 1, 3, 5, 8 };
                var currentSquidScore = (int)Game1.stats.Get(StatKeys.SquidFestScore(Game1.dayOfMonth, monthNumber));
                var alreadyGotSomeRewards = false;
                var hasCrabBook = Game1.player.mailReceived.Contains("GotCrabbingBook");
                if (!hasCrabBook)
                {
                    // Always set this flag, the book is an AP reward
                    Game1.player.mailReceived.Add("GotCrabbingBook");
                }
                for (var rewardIndex = 0; rewardIndex < requiredSquidsToday.Length; ++rewardIndex)
                {
                    if (currentSquidScore < requiredSquidsToday[rewardIndex])
                    {
                        break;
                    }
                    var rewardMail = $"GotSquidFestReward_{monthNumber}_{Game1.dayOfMonth}_{rewardIndex}";
                    if (!Game1.player.mailReceived.Contains(rewardMail))
                    {
                        Game1.player.mailReceived.Add(rewardMail);
                        var apLocation = FestivalLocationNames.SQUIDFEST_REWARDS[Game1.dayOfMonth - 12, rewardIndex];
                        if (_locationChecker.IsLocationMissing(apLocation))
                        {
                            _locationChecker.AddCheckedLocation(apLocation);
                            continue;
                        }
                        rewardsToGive.Add($"{Game1.dayOfMonth}_{rewardIndex}");
                        continue;
                    }
                    alreadyGotSomeRewards = true;
                }

                if (rewardsToGive.Count > 0)
                {
                    var inventory = new List<Item>();
                    var daySaveRandom = Utility.CreateDaySaveRandom(Game1.stats.DaysPlayed * 2000, Game1.dayOfMonth * 10);
                    foreach (var rewardKey in rewardsToGive)
                    {
                        switch (rewardKey)
                        {
                            case "12_0":
                                inventory.Add(ItemRegistry.Create("(O)DeluxeBait", 20));
                                continue;
                            case "12_1":
                                inventory.Add(daySaveRandom.NextDouble() < 0.5 ? ItemRegistry.Create("(O)498", 10) : ItemRegistry.Create("(O)MysteryBox", 2));
                                inventory.Add(ItemRegistry.Create("(O)242"));
                                continue;
                            case "12_2":
                                inventory.Add(ItemRegistry.Create("(O)797"));
                                inventory.Add(ItemRegistry.Create("(O)395", 3));
                                continue;
                            case "12_3":
                                // inventory.Add(new Furniture("SquidKid_Painting", Vector2.Zero));
                                //if (!hasCrabBook)
                                //{
                                //    inventory.Add(ItemRegistry.Create("(O)Book_Crabbing"));
                                //    continue;
                                //}
                                inventory.Add(ItemRegistry.Create("(O)MysteryBox", 3));
                                inventory.Add(ItemRegistry.Create("(O)265"));
                                continue;
                            case "13_0":
                                inventory.Add(ItemRegistry.Create("(O)694"));
                                continue;
                            case "13_1":
                                inventory.Add(daySaveRandom.NextDouble() < 0.5 ? ItemRegistry.Create("(O)498", 15) : ItemRegistry.Create("(O)MysteryBox", 3));
                                inventory.Add(ItemRegistry.Create("(O)242"));
                                continue;
                            case "13_2":
                                inventory.Add(ItemRegistry.Create("(O)166"));
                                inventory.Add(ItemRegistry.Create("(O)253", 3));
                                continue;
                            case "13_3":
                                //inventory.Add(new StardewValley.Objects.Hat("SquidHat"));
                                //if (!hasCrabBook)
                                //{
                                //    inventory.Add(ItemRegistry.Create("(O)Book_Crabbing"));
                                //    continue;
                                //}
                                inventory.Add(ItemRegistry.Create("(O)MysteryBox", 3));
                                inventory.Add(ItemRegistry.Create("(O)265"));
                                continue;
                            default:
                                continue;
                        }
                    }

                    if (inventory.Count <= 0)
                    {
                        return false; // run original logic
                    }
                    var itemGrabMenu = new ItemGrabMenu(inventory).setEssential(true, true);
                    itemGrabMenu.inventory.showGrayedOutSlots = true;
                    itemGrabMenu.source = 2;
                    Game1.activeClickableMenu = itemGrabMenu;

                    return false; // run original logic
                }

                Game1.drawObjectDialogue(Game1.content.LoadString(alreadyGotSomeRewards ? "Strings\\1_6_Strings:SquidFest_AlreadyGotAvailableRewards" : "Strings\\1_6_Strings:SquidFestBooth_NoRewards"));
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_SquidFestRewards_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public static string SquidFestScore(int dayOfMonth, int year)
        public static bool SquidFestScore_UseMonthInsteadOfYear_Prefix(int dayOfMonth, ref int year, ref string __result)
        {
            try
            {
                year = (int)(Game1.stats.DaysPlayed / 28);
                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SquidFestScore_UseMonthInsteadOfYear_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
