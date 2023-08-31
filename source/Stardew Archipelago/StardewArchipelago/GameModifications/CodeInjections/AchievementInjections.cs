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
using StardewArchipelago.Archipelago;
using StardewArchipelago.Extensions;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public static class AchievementInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        public static bool GetSteamAchievement_DisableUndeservedAchievements_Prefix(string which)
        {
            try
            {
                if (_archipelago.SlotData.ArcadeMachineLocations == ArcadeLocations.FullShuffling || _archipelago.SlotData.ArcadeMachineLocations == ArcadeLocations.VictoriesEasy)
                {
                    var jotpkVictory = "Achievement_PrairieKing";
                    var fector = "Achievement_FectorsChallenge";
                    if (which == jotpkVictory || which == fector)
                    {
                        return false; // don't run original logic
                    }
                }

                if (_archipelago.SlotData.ElevatorProgression != ElevatorProgression.Vanilla)
                {
                    var bottom_of_the_mine = "Achievement_TheBottom";
                    if (which == bottom_of_the_mine)
                    {
                        return false; // don't run original logic
                    }
                }

                return true; // run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetSteamAchievement_DisableUndeservedAchievements_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public void checkForMoneyAchievements()
        public static bool CheckForMoneyAchievements_GrantMoneyAchievementsFairly_Prefix(Stats __instance)
        {
            try
            {
                if (_archipelago.SlotData.StartingMoney.IsUnlimited())
                {
                    return false; // don't run original logic
                }

                var totalMoneyEarned = Game1.player.totalMoneyEarned;
                var moneyEarnedAfterStartingMoney = totalMoneyEarned - _archipelago.SlotData.StartingMoney;
                var moneyEarnedIfProfitMarginWas1 = moneyEarnedAfterStartingMoney / _archipelago.SlotData.ProfitMargin;

                const uint greenhornAmount = 15000U;
                const uint cowpokeAmount = 50000U;
                const uint homesteaderAmount = 250000U;
                const uint millionaireAmount = 1000000U;
                const uint legendAmount = 10000000U;

                if (moneyEarnedIfProfitMarginWas1 >= legendAmount)
                {
                    Game1.getAchievement((int)MoneyAchievement.Legend);
                }

                if (moneyEarnedIfProfitMarginWas1 >= millionaireAmount)
                {
                    Game1.getAchievement((int)MoneyAchievement.Millionaire);
                }

                if (moneyEarnedIfProfitMarginWas1 >= homesteaderAmount)
                {
                    Game1.getAchievement((int)MoneyAchievement.Homesteader);
                }

                if (moneyEarnedIfProfitMarginWas1 >= cowpokeAmount)
                {
                    Game1.getAchievement((int)MoneyAchievement.Cowpoke);
                }

                if (moneyEarnedIfProfitMarginWas1 >= greenhornAmount)
                {
                    Game1.getAchievement((int)MoneyAchievement.Greenhorn);
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForMoneyAchievements_GrantMoneyAchievementsFairly_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }

    public enum MoneyAchievement
    {
        Greenhorn = 0,
        Cowpoke = 1,
        Homesteader = 2,
        Millionaire = 3,
        Legend = 4,
    }
}
