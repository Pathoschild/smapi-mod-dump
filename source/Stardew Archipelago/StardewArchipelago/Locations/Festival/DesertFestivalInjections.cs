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
using System.Globalization;
using Microsoft.Xna.Framework;
using Netcode;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace StardewArchipelago.Locations.Festival
{
    internal class DesertFestivalInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static LocalizedContentManager _englishContentManager;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _englishContentManager = new LocalizedContentManager(Game1.game1.Content.ServiceProvider, Game1.game1.Content.RootDirectory, new CultureInfo("en-US"));
        }

        // public virtual void CollectRacePrizes()
        public static bool CollectRacePrizes_RaceWinner_Prefix(DesertFestival __instance)
        {
            try
            {
                var rewards = new List<Item>();
                if (__instance.specialRewardsCollected.ContainsKey(Game1.player.UniqueMultiplayerID) && !__instance.specialRewardsCollected[Game1.player.UniqueMultiplayerID])
                {
                    __instance.specialRewardsCollected[Game1.player.UniqueMultiplayerID] = true;
                    // No 100 eggs reward
                    // rewards.Add(ItemRegistry.Create("CalicoEgg", 100));
                }

                _locationChecker.AddCheckedLocation(FestivalLocationNames.CALICO_RACE);

                for (var index = 0; index < __instance.rewardsToCollect[Game1.player.UniqueMultiplayerID]; ++index)
                {
                    rewards.Add(ItemRegistry.Create("CalicoEgg", 20));
                }
                __instance.rewardsToCollect[Game1.player.UniqueMultiplayerID] = 0;
                Game1.activeClickableMenu = new ItemGrabMenu(rewards, false, true, null, null, "Rewards", canBeExitedWithKey: true, playRightClickSound: false, allowRightClick: false, context: __instance);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CollectRacePrizes_RaceWinner_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public virtual void ReceiveMakeOver(int randomSeedOverride = -1)
        public static void ReceiveMakeOver_EmilyServices_Postfix(DesertFestival __instance, int randomSeedOverride)
        {
            try
            {
                _locationChecker.AddCheckedLocation(FestivalLocationNames.EMILYS_OUTFIT_SERVICES);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ReceiveMakeOver_EmilyServices_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public override bool answerDialogueAction(string question_and_answer, string[] question_params)
        public static bool AnswerDialogueAction_CactusAndGil_Prefix(DesertFestival __instance, string question_and_answer, string[] question_params, ref bool __result)
        {
            try
            {
                if (HandleCactusMan(__instance, question_and_answer, ref __result))
                {
                    return false; // don't run original logic
                }

                if (HandleGilRewards(question_and_answer, ref __result))
                {
                    return false; // don't run original logic
                }

                if (HandleScholar(__instance, question_and_answer, ref __result))
                {
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_CactusAndGil_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        private static bool HandleCactusMan(DesertFestival desertFestival, string questionAndAnswer, ref bool __result)
        {
            if (!questionAndAnswer.Equals("CactusMan_Yes", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_Yes_Intro"));
            Game1.afterDialogues += () => TryGetFreeCactus(desertFestival);

            __result = true;
            return true; // don't run original logic
        }

        private static void TryGetFreeCactus(DesertFestival desertFestival)
        {
            if (Game1.player.isInventoryFull())
            {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_Yes_Full"));
                return;
            }

            var seed = (int)(Game1.player.UniqueMultiplayerID + Game1.year);
            Game1.player.freezePause = 4000;

            // protected NetEvent1Field<int, NetInt> _revealCactusEvent = new NetEvent1Field<int, NetInt>();
            var revealCactusEventField = _modHelper.Reflection.GetField<NetEvent1Field<int, NetInt>>(desertFestival, "_revealCactusEvent");
            var revealCactusEvent = revealCactusEventField.GetValue();

            DelayedAction.functionAfterDelay(() => revealCactusEvent.Fire(seed), 1000);
            DelayedAction.functionAfterDelay(() => GetFreeCactus(desertFestival, seed), 3000);
        }

        private static void GetFreeCactus(DesertFestival desertFestival, int seed)
        {
            var random = Utility.CreateRandom(seed);
            random.Next();
            random.Next();
            random.Next();
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_Yes_" + random.Next(1, 6).ToString()));
            Game1.afterDialogues += () =>
            {
                _locationChecker.AddCheckedLocation(FestivalLocationNames.FREE_CACTIS);
                Game1.playSound("coin");
                Game1.player.mailReceived.Add(desertFestival.GetCactusMail());

                // protected NetEvent1Field<int, NetInt> _hideCactusEvent = new NetEvent1Field<int, NetInt>();
                var hideCactusEventField = _modHelper.Reflection.GetField<NetEvent1Field<int, NetInt>>(desertFestival, "_hideCactusEvent");
                var hideCactusEvent = hideCactusEventField.GetValue();
                hideCactusEvent.Fire(seed);

                Game1.player.freezePause = 100;
                if (_archipelago.HasReceivedItem(FestivalLocationNames.FREE_CACTIS))
                {
                    Game1.player.addItemToInventoryBool(new RandomizedPlantFurniture("FreeCactus", Vector2.Zero, seed));
                    return;
                }
            };
        }

        private static bool HandleGilRewards(string questionAndAnswer, ref bool __result)
        {
            if (!questionAndAnswer.Equals("Gil_EggRating_Yes", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            Game1.player.lastGotPrizeFromGil.Value = Game1.Date;
            Game1.player.freezePause = 1400;
            DelayedAction.playSoundAfterDelay("coin", 500);
            DelayedAction.functionAfterDelay(GetGilReward, 1000);

            __result = true;
            return true;
        }

        private static void GetGilReward()
        {
            var calicoEggRating = Game1.player.team.highestCalicoEggRatingToday.Value + 1;
            var eggPrize = 0;
            Item extraPrize = null;
            var adventureGuild = (AdventureGuild)Game1.getLocationFromName("AdventureGuild");
            var gil = adventureGuild.Gil;
            var isMissing = _locationChecker.IsLocationMissing(FestivalLocationNames.REAL_CALICO_EGG_HUNTER);
            var earnedCheck = false;
            switch (calicoEggRating)
            {
                case >= 1000:
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Gil_Rating_1000"));
                    break;
                case >= 55:
                    Game1.DrawDialogue(gil, "Strings\\1_6_Strings:Gil_Rating_50", calicoEggRating);
                    eggPrize = 500;
                    extraPrize = new StardewValley.Object("279", 1);
                    earnedCheck = true;
                    break;
                case >= 25:
                {
                    Game1.DrawDialogue(gil, "Strings\\1_6_Strings:Gil_Rating_25", calicoEggRating);
                    eggPrize = 200;
                    extraPrize = new StardewValley.Object("253", 5);
                    earnedCheck = true;
                    break;
                }
                case >= 20:
                    Game1.DrawDialogue(gil, "Strings\\1_6_Strings:Gil_Rating_20to24", calicoEggRating);
                    eggPrize = 100;
                    extraPrize = new StardewValley.Object("253", 5);
                    if (_archipelago.SlotData.FestivalLocations == FestivalLocations.Easy)
                    {
                        earnedCheck = true;
                    }
                    break;
                case >= 15:
                    Game1.DrawDialogue(gil, "Strings\\1_6_Strings:Gil_Rating_15to19", calicoEggRating);
                    eggPrize = 50;
                    extraPrize = new StardewValley.Object("253", 3);
                    if (_archipelago.SlotData.FestivalLocations == FestivalLocations.Easy)
                    {
                        earnedCheck = true;
                    }
                    break;
                case >= 10:
                    Game1.DrawDialogue(gil, "Strings\\1_6_Strings:Gil_Rating_10to14", calicoEggRating);
                    eggPrize = 25;
                    extraPrize = new StardewValley.Object("253", 1);
                    if (_archipelago.SlotData.FestivalLocations == FestivalLocations.Easy)
                    {
                        earnedCheck = true;
                    }
                    break;
                case >= 5:
                    Game1.DrawDialogue(gil, "Strings\\1_6_Strings:Gil_Rating_5to9", calicoEggRating);
                    eggPrize = 10;
                    extraPrize = new StardewValley.Object("395", 1);
                    break;
                default:
                    Game1.DrawDialogue(gil, "Strings\\1_6_Strings:Gil_Rating_1to4", calicoEggRating);
                    eggPrize = 1;
                    extraPrize = new StardewValley.Object("243", 1);
                    break;
            }
            Game1.afterDialogues = () =>
            {
                if (isMissing && earnedCheck)
                {
                    _locationChecker.AddCheckedLocation(FestivalLocationNames.REAL_CALICO_EGG_HUNTER);
                }
                else
                {
                    Game1.player.addItemByMenuIfNecessaryElseHoldUp(new StardewValley.Object("CalicoEgg", eggPrize));
                    if (extraPrize == null)
                    {
                        return;
                    }
                    Game1.afterDialogues = () => Game1.player.addItemByMenuIfNecessary(extraPrize);
                }
            };
        }

        private static bool HandleScholar(DesertFestival desertFestival, string questionAndAnswer, ref bool __result)
        {
            if (!questionAndAnswer.Equals("DesertScholar_Answer__Correct", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            // protected int _currentScholarQuestion = -1;
            var currentScholarQuestionField = _modHelper.Reflection.GetField<int>(desertFestival, "_currentScholarQuestion");
            var currentScholarQuestion = currentScholarQuestionField.GetValue();
            if (currentScholarQuestion != 4)
            {
                return false;
            }

            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_Correct"));
            Game1.playSound("give_gift");
            Game1.player.mailReceived.Add(desertFestival.GetScholarMail());
            Game1.afterDialogues += () =>
            {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_Win"));
                Game1.afterDialogues += () =>
                {
                    if (_locationChecker.IsLocationMissing(FestivalLocationNames.DESERT_SCHOLAR))
                    {
                        _locationChecker.AddCheckedLocation(FestivalLocationNames.DESERT_SCHOLAR);
                    }
                    else
                    {
                        Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("CalicoEgg", 50));
                    }
                    Game1.playSound("coin");
                };
            };

            __result = true;
            return true; // don't run original logic
        }

        // public override bool answerDialogueAction(string question_and_answer, string[] question_params)
        public static void AnswerDialogueAction_DesertChef_Postfix(DesertFestival __instance, string question_and_answer, string[] question_params, ref bool __result)
        {
            try
            {
                CheckDesertChefLocation(__instance, question_and_answer);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_DesertChef_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void CheckDesertChefLocation(DesertFestival desertFestival, string question_and_answer)
        {
            if (!question_and_answer.StartsWith("Cook_ChoseSauce", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            // protected int _cookIngredient = -1;
            // protected int _cookSauce = -1;
            var cookIngredientField = _modHelper.Reflection.GetField<int>(desertFestival, "_cookIngredient");
            var cookSauceField = _modHelper.Reflection.GetField<int>(desertFestival, "_cookSauce");
            var cookIngredient = cookIngredientField.GetValue();
            var cookSauce = cookSauceField.GetValue();

            var foodPath = $"Strings\\1_6_Strings:Cook_DishNames_{cookIngredient}_{cookSauce}";
            var foodName = _englishContentManager.LoadString(foodPath);
            _locationChecker.AddCheckedLocation(foodName);
        }

        // private void signalCalicoStatueActivation(int whichEffect)
        public static void SignalCalicoStatueActivation_DesertChef_Postfix(MineShaft __instance, int whichEffect)
        {
            try
            {
                _locationChecker.AddCheckedLocation(FestivalLocationNames.TOUCH_A_CALICO_STATUE);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SignalCalicoStatueActivation_DesertChef_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
