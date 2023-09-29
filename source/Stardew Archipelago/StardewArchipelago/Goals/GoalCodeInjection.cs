/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Locations;
using StardewValley.Locations;

namespace StardewArchipelago.Goals
{
    internal class GoalCodeInjection
    {
        public const string MASTER_ANGLER_LETTER = "CF_Fish";

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

        public static void CheckCommunityCenterGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.CommunityCenter)
            {
                return;
            }

            var communityCenter = Game1.locations.OfType<CommunityCenter>().First();
            if (!communityCenter.areAllAreasComplete())
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckGrandpaEvaluationGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.GrandpaEvaluation)
            {
                return;
            }

            var farm = Game1.getFarm();
            int candlesFromScore = Utility.getGrandpaCandlesFromScore(Utility.getGrandpaScore());
            farm.grandpaScore.Value = candlesFromScore;
            for (int index = 0; index < candlesFromScore; ++index)
            {
                DelayedAction.playSoundAfterDelay("fireball", 100 * index);
            }
            farm.addGrandpaCandles();

            if (farm.grandpaScore.Value < 4)
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckBottomOfTheMinesGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.BottomOfMines)
            {
                return;
            }

            var lowestMineLevel = Game1.netWorldState.Value.LowestMineLevel;

            if (lowestMineLevel < 120)
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckCrypticNoteGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.CrypticNote)
            {
                return;
            }

            if (!Game1.player.mailReceived.Contains("qiCave"))
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckMasterAnglerGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.MasterAngler)
            {
                return;
            }

            CheckMasterAnglerWithoutIslandFish();
            if (!Game1.player.hasOrWillReceiveMail(MASTER_ANGLER_LETTER))
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        private static void CheckMasterAnglerWithoutIslandFish()
        {
            if (!_archipelago.SlotData.ExcludeGingerIsland || Game1.player.hasOrWillReceiveMail(MASTER_ANGLER_LETTER))
            {
                return;
            }

            var uniqueFishCaught = 0;
            var totalFishExist = 0;
            foreach (var (id, information) in Game1.objectInformation)
            {
                var isFish = information.Split('/')[3].Contains("Fish");
                var isTrash = (id >= 167 && id <= 172);
                var isLegendaryFamily = (id >= 898 && id <= 902);
                var isIslandFish = (id >= 836 && id <= 838);
                if (!isFish || isTrash || isLegendaryFamily || isIslandFish)
                {
                    continue;
                }

                ++totalFishExist;
                if (Game1.player.fishCaught.ContainsKey(id))
                {
                    ++uniqueFishCaught;
                }
            }

            if (uniqueFishCaught < totalFishExist)
            {
                return;
            }

            if (!Game1.player.hasOrWillReceiveMail(MASTER_ANGLER_LETTER))
            {
                Game1.addMailForTomorrow(MASTER_ANGLER_LETTER);
            }
        }

        public static void CheckCompleteCollectionGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.CompleteCollection)
            {
                return;
            }

            if (!Game1.player.hasOrWillReceiveMail("museumComplete"))
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckFullHouseGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.FullHouse)
            {
                return;
            }

            if (Game1.player.getChildrenCount() < 2 || !Game1.player.isMarried() || Game1.player.HouseUpgradeLevel < 2)
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckWalnutHunterGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.GreatestWalnutHunter)
            {
                return;
            }

            if (Game1.netWorldState.Value.GoldenWalnutsFound.Value < 130)
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckProtectorOfTheValleyGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.ProtectorOfTheValley)
            {
                return;
            }

            if (!AdventureGuild.areAllMonsterSlayerQuestsComplete())
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckFullShipmentGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.FullShipment)
            {
                return;
            }

            if (!Utility.hasFarmerShippedAllItems())
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckPerfectionGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.Perfection)
            {
                return;
            }

            if (Utility.percentGameComplete() < 1.0)
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void DoAreaCompleteReward_CommunityCenterGoal_PostFix(CommunityCenter __instance, int whichArea)
        {
            try
            {
                CheckCommunityCenterGoalCompletion();
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(DoAreaCompleteReward_CommunityCenterGoal_PostFix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static void EnterMine_Level120Goal_PostFix(int whatLevel)
        {
            try
            {
                if (whatLevel != 120)
                {
                    return;
                }

                _archipelago.ReportGoalCompletion();
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(EnterMine_Level120Goal_PostFix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public void foundWalnut(int stack = 1)
        public static void FounddWalnut_WalnutHunterGoal_Postfix(Farmer __instance, int stack)
        {
            try
            {
                CheckWalnutHunterGoalCompletion();
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(FounddWalnut_WalnutHunterGoal_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public static float percentGameComplete()
        public static void PercentGameComplete_PerfectionGoal_Postfix(ref float __result)
        {
            try
            {
                if (__result < 1.0)
                {
                    return;
                }

                _archipelago.ReportGoalCompletion();
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PercentGameComplete_PerfectionGoal_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static string GetGoalString()
        {
            var goal = _archipelago.SlotData.Goal switch
            {
                Goal.GrandpaEvaluation => "Complete Grandpa's Evaluation with a score of at least 12 (4 candles)",
                Goal.BottomOfMines => "Reach Floor 120 in the Pelican Town Mineshaft",
                Goal.CommunityCenter => "Complete the Community Center",
                Goal.CrypticNote => "Find Secret Note #10 and complete the \"Cryptic Note\" Quest",
                Goal.MasterAngler => "Catch every single one of the 55 fish available in the game",
                Goal.CompleteCollection => "Complete the Museum Collection by donating all 95 items",
                Goal.FullHouse => "Get married and have two children",
                Goal.GreatestWalnutHunter => "Find all 130 Golden Walnuts",
                Goal.ProtectorOfTheValley => "Complete all the monster slaying goals",
                Goal.FullShipment => "Ship every item",
                Goal.Perfection => "Achieve Perfection",
                _ => throw new NotImplementedException(),
            };
            return goal;
        }

        public static string GetGoalStringGrandpa()
        {
            var goal = _archipelago.SlotData.Goal switch
            {
                Goal.GrandpaEvaluation => "Make the most of this farm, and make me proud",
                Goal.BottomOfMines => "Finish exploring the mineshaft in this town for me",
                Goal.CommunityCenter => "Restore the old Community Center for the sake of all the villagers",
                Goal.CrypticNote => "Meet an old friend of mine on floor 100 of the Skull Cavern",
                Goal.MasterAngler => "Catch and document every specie of fish in the Ferngill Republic",
                Goal.CompleteCollection => "Restore our beautiful museum with a full collection of various artifacts and minerals",
                Goal.FullHouse => "I wish for my bloodline to thrive. Please find a partner and live happily ever after",
                Goal.GreatestWalnutHunter => "Prove your worth to an old friend of mine, and become the greatest walnut hunter",
                Goal.ProtectorOfTheValley => "Make sure the valley is safe for generations to come, by slaying all the monsters",
                Goal.FullShipment => "Contribute to the local economy and market, by shipping as many things as you can",
                Goal.Perfection => "For a fulfilling life, you need to do a lot of everything. Leave no loose ends",
                _ => throw new NotImplementedException(),
            };
            return goal;
        }
    }
}
