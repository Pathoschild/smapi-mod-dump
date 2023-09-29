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
using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Locations.CodeInjections.Initializers;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Quests;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship;
using StardewArchipelago.Locations.Festival;
using StardewArchipelago.Locations.GingerIsland;
using StardewArchipelago.Locations.GingerIsland.Boat;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.Patcher
{
    public class VanillaLocationPatcher : ILocationPatcher
    {
        private readonly ArchipelagoClient _archipelago;
        private readonly Harmony _harmony;
        private readonly IModHelper _modHelper;
        private readonly GingerIslandPatcher _gingerIslandPatcher;

        public VanillaLocationPatcher(IMonitor monitor, IModHelper modHelper, Harmony harmony, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _archipelago = archipelago;
            _harmony = harmony;
            _modHelper = modHelper;
            _gingerIslandPatcher = new GingerIslandPatcher(monitor, _modHelper, _harmony, _archipelago, locationChecker);
        }

        public void ReplaceAllLocationsRewardsWithChecks()
        {
            ReplaceCommunityCenterBundlesWithChecks();
            ReplaceCommunityCenterAreasWithChecks();
            ReplaceBackPackUpgradesWithChecks();
            ReplaceMineshaftChestsWithChecks();
            ReplaceElevatorsWithChecks();
            ReplaceToolUpgradesWithChecks();
            ReplaceFishingRodsWithChecks();
            ReplaceSkillsWithChecks();
            ReplaceQuestsWithChecks();
            ReplaceCarpenterBuildingsWithChecks();
            ReplaceWizardBuildingsWithChecks();
            ReplaceIsolatedEventsWithChecks();
            PatchAdventurerGuildShop();
            ReplaceArcadeMachinesWithChecks();
            PatchTravelingMerchant();
            AddFishsanityLocations();
            AddMuseumsanityLocations();
            AddFestivalLocations();
            AddCropSanityLocations();
            ReplaceFriendshipsWithChecks();
            ReplaceSpecialOrdersWithChecks();
            ReplaceChildrenWithChecks();
            _gingerIslandPatcher.PatchGingerIslandLocations();
            AddShipsanityLocations();
            PatchMonstersanity();
            AddCooksanityLocations();
            AddChefsanityLocations();
            AddCraftsanityLocations();
        }

        private void ReplaceCommunityCenterBundlesWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(JunimoNoteMenu), nameof(JunimoNoteMenu.checkForRewards)),
                postfix: new HarmonyMethod(typeof(CommunityCenterInjections), nameof(CommunityCenterInjections.CheckForRewards_PostFix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(CommunityCenter), nameof(CommunityCenter.shouldNoteAppearInArea)),
                prefix: new HarmonyMethod(typeof(CommunityCenterInjections), nameof(CommunityCenterInjections.ShouldNoteAppearInArea_AllowAccessEverything_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(CommunityCenter), nameof(CommunityCenter.checkAction)),
                prefix: new HarmonyMethod(typeof(CommunityCenterInjections), nameof(CommunityCenterInjections.CheckAction_BulletinBoardNoRequirements_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(JunimoNoteMenu), nameof(JunimoNoteMenu.getRewardNameForArea)),
                prefix: new HarmonyMethod(typeof(CommunityCenterInjections), nameof(CommunityCenterInjections.GetRewardNameForArea_ScoutRoomRewards_Prefix))
            );
        }

        private void ReplaceCommunityCenterAreasWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(CommunityCenter), "doAreaCompleteReward"),
                prefix: new HarmonyMethod(typeof(CommunityCenterInjections), nameof(CommunityCenterInjections.DoAreaCompleteReward_AreaLocations_Prefix))
            );
        }

        private void ReplaceBackPackUpgradesWithChecks()
        {
            if (_archipelago.SlotData.BackpackProgression == BackpackProgression.Vanilla)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                prefix: new HarmonyMethod(typeof(BackpackInjections), nameof(BackpackInjections.PerformAction_BuyBackpack_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(BackpackInjections), nameof(BackpackInjections.AnswerDialogueAction_BackPackPurchase_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(SeedShop), nameof(SeedShop.draw)),
                prefix: new HarmonyMethod(typeof(BackpackInjections), nameof(BackpackInjections.Draw_SeedShopBackpack_Prefix)),
                postfix: new HarmonyMethod(typeof(BackpackInjections), nameof(BackpackInjections.Draw_SeedShopBackpack_Postfix))
            );
        }

        private void ReplaceMineshaftChestsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.checkForAction)),
                prefix: new HarmonyMethod(typeof(MineshaftInjections), nameof(MineshaftInjections.CheckForAction_MineshaftChest_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(MineShaft), "addLevelChests"),
                prefix: new HarmonyMethod(typeof(MineshaftInjections), nameof(MineshaftInjections.AddLevelChests_Level120_Prefix))
            );
        }

        private void ReplaceToolUpgradesWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                prefix: new HarmonyMethod(typeof(ScytheInjections), nameof(ScytheInjections.PerformAction_GoldenScythe_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getBlacksmithUpgradeStock)),
                postfix: new HarmonyMethod(typeof(ToolInjections), nameof(ToolInjections.GetBlacksmithUpgradeStock_PriceReductionFromAp_Postfix))
            );

            if (!_archipelago.SlotData.ToolProgression.HasFlag(ToolProgression.Progressive))
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(ToolInjections), nameof(ToolInjections.AnswerDialogueAction_ToolUpgrade_Prefix))
            );
        }

        private void ReplaceFishingRodsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getFishShopStock)),
                prefix: new HarmonyMethod(typeof(FishingRodInjections), nameof(FishingRodInjections.GetFishShopStock_Prefix))
            );

            if (!_archipelago.SlotData.ToolProgression.HasFlag(ToolProgression.Progressive))
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.skipEvent)),
                prefix: new HarmonyMethod(typeof(FishingRodInjections), nameof(FishingRodInjections.SkipEvent_BambooPole_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.command_awardFestivalPrize)),
                prefix: new HarmonyMethod(typeof(FishingRodInjections), nameof(FishingRodInjections.AwardFestivalPrize_BambooPole_Prefix))
            );
        }

        private void ReplaceElevatorsWithChecks()
        {
            if (_archipelago.SlotData.ElevatorProgression == ElevatorProgression.Vanilla)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.enterMine)),
                postfix: new HarmonyMethod(typeof(MineshaftInjections), nameof(MineshaftInjections.EnterMine_SendElevatorCheck_PostFix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                prefix: new HarmonyMethod(typeof(MineshaftInjections), nameof(MineshaftInjections.PerformAction_LoadElevatorMenu_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(MineShaft), nameof(MineShaft.checkAction)),
                prefix: new HarmonyMethod(typeof(MineshaftInjections), nameof(MineshaftInjections.CheckAction_LoadElevatorMenu_Prefix))
            );
        }

        private void ReplaceSkillsWithChecks()
        {
            if (_archipelago.SlotData.SkillProgression == SkillsProgression.Vanilla)
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
                    prefix: new HarmonyMethod(typeof(SkillInjections), nameof(SkillInjections.GainExperience_NormalExperience_Prefix))
                );
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
                prefix: new HarmonyMethod(typeof(SkillInjections), nameof(SkillInjections.GainExperience_ArchipelagoExperience_Prefix))
            );
        }

        private void ReplaceQuestsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Quest), nameof(Quest.questComplete)),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.QuestComplete_LocationInsteadOfReward_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.command_removeQuest)),
                postfix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.Command_RemoveQuest_CheckLocation_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getQuestOfTheDay)),
                prefix:new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.GetQuestOfTheDay_BalanceQuests_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Mountain), nameof(Mountain.checkAction)),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.CheckAction_AdventurerGuild_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.PerformAction_MysteriousQiLumberPile_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Bush), "shake"),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.Shake_WinterMysteryBush_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Town), "mgThief_afterSpeech"),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.MgThief_AfterSpeech_WinterMysteryFinished_Prefix))
            );

            PatchSkillsPage();

            _harmony.Patch(
                original: AccessTools.Method(typeof(Object), "getPriceAfterMultipliers"),
                postfix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.GetPriceAfterMultipliers_BearKnowledge_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.command_awardFestivalPrize)),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.Command_AwardFestivalPrize_QiMilk_Prefix))
            );

            ReplaceDarkTalismanQuestsWithChecks();
        }

        private void ReplaceDarkTalismanQuestsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new HarmonyMethod(typeof(DarkTalismanInjections), nameof(DarkTalismanInjections.CheckAction_ShowWizardMagicInk_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.checkForAction)),
                prefix: new HarmonyMethod(typeof(DarkTalismanInjections), nameof(DarkTalismanInjections.CheckForAction_BuglandChest_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "performRemoveHenchman"),
                prefix: new HarmonyMethod(typeof(DarkTalismanInjections), nameof(DarkTalismanInjections.PerformRemoveHenchman_CheckGoblinProblemLocation_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Railroad), "resetLocalState"),
                prefix: new HarmonyMethod(typeof(DarkTalismanInjections), nameof(DarkTalismanInjections.ResetLocalState_PlayCutsceneIfConditionsAreMet_Postfix))
            ); 
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.setUpLocationSpecificFlair)),
                prefix: new HarmonyMethod(typeof(DarkTalismanInjections), nameof(DarkTalismanInjections.SetUpLocationSpecificFlair_BuglandChest_Prefix))
            ); 
        }

        private void PatchSkillsPage()
        {
            var desiredSkillsPageCtorParameters = new[] { typeof(int), typeof(int), typeof(int), typeof(int) };
            _harmony.Patch(
                original: AccessTools.Constructor(typeof(SkillsPage), desiredSkillsPageCtorParameters),
                postfix: new HarmonyMethod(typeof(QuestInjections),
                    nameof(QuestInjections.SkillsPageCtor_BearKnowledge_Postfix))
            );
        }

        private void ReplaceSpecialOrdersWithChecks()
        {
            if (_archipelago.SlotData.SpecialOrderLocations == SpecialOrderLocations.Disabled)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.IsSpecialOrdersBoardUnlocked)),
                prefix: new HarmonyMethod(typeof(SpecialOrderInjections), nameof(SpecialOrderInjections.IsSpecialOrdersBoardUnlocked_UnlockBasedOnApItem_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.GetSpecialOrder)),
                postfix: new HarmonyMethod(typeof(SpecialOrderInjections), nameof(SpecialOrderInjections.GetSpecialOrder_ArchipelagoReward_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.CheckCompletion)),
                postfix: new HarmonyMethod(typeof(SpecialOrderInjections), nameof(SpecialOrderInjections.CheckCompletion_ArchipelagoReward_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.SetDuration)),
                prefix: new HarmonyMethod(typeof(SpecialOrderInjections), nameof(SpecialOrderInjections.SetDuration_UseCorrectDateWithSeasonRandomizer_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.UpdateAvailableSpecialOrders)),
                prefix: new HarmonyMethod(typeof(SpecialOrderInjections), nameof(SpecialOrderInjections.UpdateAvailableSpecialOrders_ChangeFrequencyToBeLessRng_Prefix))
            );
        }

        private void ReplaceChildrenWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.canGetPregnant)),
                prefix: new HarmonyMethod(typeof(PregnancyInjections), nameof(PregnancyInjections.CanGetPregnant_ShuffledPregnancies_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(QuestionEvent), nameof(QuestionEvent.setUp)),
                prefix: new HarmonyMethod(typeof(PregnancyInjections), nameof(PregnancyInjections.Setup_PregnancyQuestionEvent_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(QuestionEvent), "answerPregnancyQuestion"),
                prefix: new HarmonyMethod(typeof(PregnancyInjections), nameof(PregnancyInjections.AnswerPregnancyQuestion_CorrectDate_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(BirthingEvent), nameof(BirthingEvent.tickUpdate)),
                prefix: new HarmonyMethod(typeof(PregnancyInjections), nameof(PregnancyInjections.TickUpdate_BirthingEvent_Prefix))
            );
        }

        private void ReplaceCarpenterBuildingsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Constructor(typeof(BluePrint), new[]{typeof(string)}),
                postfix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.BluePrintConstructor_CheaperInAP_Postfix))
            );

            if (!_archipelago.SlotData.BuildingProgression.HasFlag(BuildingProgression.Progressive))
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), "houseUpgradeOffer"),
                    prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.HouseUpgradeOffer_OfferCheaperUpgrade_Prefix))
                );

                _harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), "houseUpgradeAccept"),
                    prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.HouseUpgradeAccept_CheaperInAP_Prefix))
                );

                return;
            }

            var desiredOverloadParameters = new[] { typeof(string), typeof(Response[]), typeof(string) };
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.createQuestionDialogue), desiredOverloadParameters),
                prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.CreateQuestionDialogue_CarpenterDialogOptions_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.AnswerDialogueAction_CarpenterConstruct_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "houseUpgradeOffer"),
                prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.HouseUpgradeOffer_OfferFreeUpgrade_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "houseUpgradeAccept"),
                prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.HouseUpgradeAccept_FreeFromAP_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getCarpenterStock)),
                postfix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.GetCarpenterStock_PurchasableChecks_Postfix))
            );
        }

        private void ReplaceWizardBuildingsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                prefix: new HarmonyMethod(typeof(WizardInjections), nameof(WizardInjections.PerformAction_WizardBook_Prefix))
            );
        }

        private void ReplaceIsolatedEventsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.skipEvent)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.SkipEvent_RustySword_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.command_awardFestivalPrize)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.AwardFestivalPrize_RustySword_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Woods), nameof(Woods.checkAction)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.CheckAction_OldMasterCanolli_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Beach), nameof(Beach.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.AnswerDialogueAction_BeachBridge_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Beach), nameof(Beach.checkAction)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.CheckAction_BeachBridge_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Beach), nameof(Beach.fixBridge)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.FixBridge_DontFixDuringDraw_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Beach), nameof(Beach.draw)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.Draw_BeachBridgeQuestionMark_Prefix)),
                postfix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.Draw_BeachBridgeQuestionMark_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performTouchAction)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.PerformTouchAction_GalaxySwordShrine_Prefix))
            );
        }

        private void PatchAdventurerGuildShop()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(AdventureGuild), "resetLocalState"),
                postfix: new HarmonyMethod(typeof(AdventurerGuildInjections), nameof(AdventurerGuildInjections.ResetLocalState_GuildMemberOnlyIfReceived_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(AdventurerGuildInjections), nameof(AdventurerGuildInjections.TelephoneAdventureGuild_AddReceivedEquipments_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getAdventureShopStock)),
                prefix: new HarmonyMethod(typeof(AdventurerGuildInjections), nameof(AdventurerGuildInjections.GetAdventureShopStock_ShopBasedOnReceivedItems_Prefix))
            );
        }

        private void ReplaceArcadeMachinesWithChecks()
        {
            if (_archipelago.SlotData.ArcadeMachineLocations == ArcadeLocations.Disabled)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.usePowerup)),
                prefix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.UsePowerup_PrairieKingBossBeaten_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(MineCart), nameof(MineCart.EndCutscene)),
                prefix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.EndCutscene_JunimoKartLevelComplete_Prefix))
            );

            if (_archipelago.SlotData.ArcadeMachineLocations == ArcadeLocations.Victories)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(AbigailGame.CowboyMonster), nameof(AbigailGame.CowboyMonster.getLootDrop)),
                prefix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.GetLootDrop_ExtraLoot_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(MineCart), "restartLevel"),
                postfix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.RestartLevel_NewGame_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(MineCart), nameof(MineCart.UpdateFruitsSummary)),
                postfix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.UpdateFruitsSummary_ExtraLives_Postfix))
            );

            var desiredAbigailGameCtorParameters = new[] { typeof(bool) };
            _harmony.Patch(
                original: AccessTools.Constructor(typeof(AbigailGame), desiredAbigailGameCtorParameters),
                postfix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.AbigailGameCtor_Equipments_Postfix))
            );

            if (_archipelago.SlotData.ArcadeMachineLocations == ArcadeLocations.FullShuffling)
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.startShoppingLevel)),
                    postfix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.StartShoppingLevel_ShopBasedOnSentChecks_PostFix))
                );

                _harmony.Patch(
                    original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.tick)),
                    postfix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.Tick_Shopping_PostFix))
                );
            }
        }

        private void PatchTravelingMerchant()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Forest), nameof(Forest.DayUpdate)),
                postfix: new HarmonyMethod(typeof(TravelingMerchantInjections), nameof(TravelingMerchantInjections.DayUpdate_IsTravelingMerchantDay_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(BeachNightMarket), nameof(BeachNightMarket.checkAction)),
                prefix: new HarmonyMethod(typeof(TravelingMerchantInjections), nameof(TravelingMerchantInjections.NightMarketCheckAction_IsTravelingMerchantDay_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.setUpShopOwner)),
                postfix: new HarmonyMethod(typeof(TravelingMerchantInjections), nameof(TravelingMerchantInjections.SetUpShopOwner_TravelingMerchantApFlair_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), "generateLocalTravelingMerchantStock"),
                postfix: new HarmonyMethod(typeof(TravelingMerchantInjections), nameof(TravelingMerchantInjections.GenerateLocalTravelingMerchantStock_APStock_Postfix))
            );
        }

        private void AddFishsanityLocations()
        {
            if (_archipelago.SlotData.Goal == Goal.MasterAngler)
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.caughtFish)),
                    postfix: new HarmonyMethod(typeof(FishingInjections), nameof(FishingInjections.CaughtFish_CheckGoalCompletion_Postfix))
                );
            }

            if (_archipelago.SlotData.Fishsanity == Fishsanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.caughtFish)),
                postfix: new HarmonyMethod(typeof(FishingInjections), nameof(FishingInjections.CaughtFish_Fishsanity_Postfix))
            );
        }

        private void AddMuseumsanityLocations()
        {
            if (_archipelago.SlotData.Goal == Goal.MasterAngler)
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(LibraryMuseum), nameof(LibraryMuseum.getRewardsForPlayer)),
                    postfix: new HarmonyMethod(typeof(MuseumInjections), nameof(MuseumInjections.GetRewardsForPlayer_CheckGoalCompletion_Postfix))
                );
            }

            if (_archipelago.SlotData.Museumsanity == Museumsanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(LibraryMuseum), nameof(LibraryMuseum.getRewardsForPlayer)),
                prefix: new HarmonyMethod(typeof(MuseumInjections), nameof(MuseumInjections.GetRewardsForPlayer_Museumsanity_Prefix))
            );
        }

        private void ReplaceFriendshipsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Friendship), nameof(Friendship.Points)),
                prefix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.GetPoints_ArchipelagoHearts_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Constructor(typeof(SocialPage), new[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.SocialPageCtor_CheckHints_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(SocialPage), "drawNPCSlot"),
                postfix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.DrawNPCSlot_DrawEarnedHearts_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Pet), nameof(Pet.dayUpdate)),
                prefix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.DayUpdate_ArchipelagoPoints_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeFriendship)),
                prefix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.ChangeFriendship_ArchipelagoPoints_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.resetFriendshipsForNewDay)),
                prefix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.ResetFriendshipsForNewDay_AutopetHumans_Prefix))
            );
        }

        private void AddFestivalLocations()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(BeachNightMarket), nameof(BeachNightMarket.geMagicShopStock)),
                postfix: new HarmonyMethod(typeof(BeachNightMarketInjections), nameof(BeachNightMarketInjections.GetMagicShopStock_UniqueItemsAndSeeds_Postfix))
            );

            if (_archipelago.SlotData.FestivalLocations == FestivalLocations.Vanilla)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.command_awardFestivalPrize)),
                prefix: new HarmonyMethod(typeof(EggFestivalInjections), nameof(EggFestivalInjections.AwardFestivalPrize_Strawhat_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.setUpFestivalMainEvent)),
                postfix: new HarmonyMethod(typeof(FlowerDanceInjections), nameof(FlowerDanceInjections.SetUpFestivalMainEvent_FlowerDance_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.update)),
                postfix: new HarmonyMethod(typeof(FlowerDanceInjections), nameof(FlowerDanceInjections.Update_HandleFlowerDanceShopFirstTimeOnly_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.command_switchEvent)),
                postfix: new HarmonyMethod(typeof(LuauInjections), nameof(LuauInjections.SwitchEvent_GovernorReactionToSoup_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.setUpFestivalMainEvent)),
                postfix: new HarmonyMethod(typeof(MoonlightJelliesInjections), nameof(MoonlightJelliesInjections.SetUpFestivalMainEvent_MoonlightJellies_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(StrengthGame), nameof(StrengthGame.update)),
                prefix: new HarmonyMethod(typeof(FairInjections), nameof(FairInjections.StrengthGameUpdate_StrongEnough_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.interpretGrangeResults)),
                postfix: new HarmonyMethod(typeof(FairInjections), nameof(FairInjections.InterpretGrangeResults_Success_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.update)),
                postfix: new HarmonyMethod(typeof(FairInjections), nameof(FairInjections.Update_HandleFairItemsFirstTimeOnly_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.checkForAction)),
                prefix: new HarmonyMethod(typeof(SpiritEveInjections), nameof(SpiritEveInjections.CheckForAction_SpiritEveChest_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.update)),
                postfix: new HarmonyMethod(typeof(SpiritEveInjections),
                    nameof(SpiritEveInjections.Update_HandleSpiritEveShopFirstTimeOnly_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.command_awardFestivalPrize)),
                prefix: new HarmonyMethod(typeof(IceFestivalInjections), nameof(IceFestivalInjections.AwardFestivalPrize_FishingCompetition_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.update)),
                postfix: new HarmonyMethod(typeof(IceFestivalInjections),
                    nameof(IceFestivalInjections.Update_HandleRarecrow4FirstTimeOnly_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(MermaidHouse), nameof(MermaidHouse.playClamTone), new Type[] { typeof(int), typeof(Farmer) }),
                prefix: new HarmonyMethod(typeof(MermaidHouseInjections), nameof(MermaidHouseInjections.PlayClamTone_SongFinished_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(BeachNightMarket), nameof(BeachNightMarket.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(BeachNightMarketInjections), nameof(BeachNightMarketInjections.AnswerDialogueAction_LupiniPainting_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Dialogue), nameof(Dialogue.chooseResponse)),
                postfix: new HarmonyMethod(typeof(WinterStarInjections), nameof(WinterStarInjections.ChooseResponse_LegendOfTheWinterStar_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.chooseSecretSantaGift)),
                prefix: new HarmonyMethod(typeof(WinterStarInjections), nameof(WinterStarInjections.ChooseSecretSantaGift_SuccessfulGift_Prefix))
            );
        }

        private void AddCropSanityLocations()
        {
            if (_archipelago.SlotData.Cropsanity == Cropsanity.Disabled)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Crop), nameof(Crop.harvest)),
                postfix: new HarmonyMethod(typeof(CropsanityInjections), nameof(CropsanityInjections.Harvest_CheckCropsanityLocation_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.shake)),
                prefix: new HarmonyMethod(typeof(CropsanityInjections), nameof(CropsanityInjections.Shake_CheckCropsanityFruitTreeLocation_Prefix))
            );
        }

        private void AddShipsanityLocations()
        {
            if (_archipelago.SlotData.Goal == Goal.FullShipment)
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), "_newDayAfterFade"),
                    prefix: new HarmonyMethod(typeof(ShippingInjections), nameof(ShippingInjections.NewDayAfterFade_CheckGoalCompletion_Postfix))
                );
            }

            if (_archipelago.SlotData.Shipsanity == Shipsanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "_newDayAfterFade"),
                prefix: new HarmonyMethod(typeof(ShippingInjections), nameof(ShippingInjections.NewDayAfterFade_CheckShipsanityLocations_Prefix))
            );
        }

        private void PatchMonstersanity()
        {
            if (_archipelago.SlotData.Goal == Goal.ProtectorOfTheValley)
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(Stats), nameof(Stats.monstersKilled)),
                    postfix: new HarmonyMethod(typeof(MonsterSlayerInjections), nameof(MonsterSlayerInjections.MonsterKilled_CheckGoalCompletion_Postfix))
                );
            }

            if (_archipelago.SlotData.Monstersanity == Monstersanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(AdventureGuild), nameof(AdventureGuild.showMonsterKillList)),
                prefix: new HarmonyMethod(typeof(MonsterSlayerInjections), nameof(MonsterSlayerInjections.ShowMonsterKillList_CustomListFromAP_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Stats), nameof(Stats.monsterKilled)),
                postfix: new HarmonyMethod(typeof(MonsterSlayerInjections), nameof(MonsterSlayerInjections.MonsterKilled_SendMonstersanityCheck_Postfix))
            );
        }

        private void AddCooksanityLocations()
        {
            if (_archipelago.SlotData.Cooksanity == Cooksanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.cookedRecipe)),
                postfix: new HarmonyMethod(typeof(CookingInjections), nameof(CookingInjections.CookedRecipe_CheckCooksanityLocation_Postfix))
            );
        }

        private void AddChefsanityLocations()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(TV), "getWeeklyRecipe"),
                prefix: new HarmonyMethod(typeof(QueenOfSauceInjections), nameof(QueenOfSauceInjections.GetWeeklyRecipe_UseArchipelagoSchedule_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(VolcanoDungeon), nameof(VolcanoDungeon.checkAction)),
                prefix: new HarmonyMethod(typeof(RecipePurchaseInjections), nameof(RecipePurchaseInjections.CheckAction_ReplaceVolcanoDwarfRecipesWithChecks_Prefix))
            );

            if (_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Purchases))
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), nameof(Utility.getSaloonStock)),
                    prefix: new HarmonyMethod(typeof(RecipePurchaseInjections), nameof(RecipePurchaseInjections.GetSaloonStock_ReplaceRecipesWithChefsanityChecks_Prefix))
                );

                _harmony.Patch(
                    original: AccessTools.Method(typeof(IslandSouth), nameof(IslandSouth.checkAction)),
                    prefix: new HarmonyMethod(typeof(RecipePurchaseInjections), nameof(RecipePurchaseInjections.CheckAction_ReplaceTropicalCurryWithChefsanityCheck_Prefix))
                );

                _harmony.Patch(
                    original: AccessTools.Method(typeof(IslandNorth), nameof(IslandNorth.getIslandMerchantTradeStock)),
                    prefix: new HarmonyMethod(typeof(RecipePurchaseInjections), nameof(RecipePurchaseInjections.GetIslandMerchantTradeStock_ReplaceBananaPuddingWithChefsanityCheck_Prefix))
                );
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.InitShared)),
                postfix: new HarmonyMethod(typeof(RecipeDataInjections), nameof(RecipeDataInjections.InitShared_RemoveSkillAndFriendshipLearnConditions_Postfix))
            );
            
            _harmony.Patch(
                original: AccessTools.Constructor(typeof(LevelUpMenu), new []{typeof(int), typeof(int)}),
                prefix: new HarmonyMethod(typeof(RecipeLevelUpInjections), nameof(RecipeLevelUpInjections.LevelUpMenuConstructor_SendSkillRecipeChecks_Postfix))
            );
        
            _harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.grantConversationFriendship)),
                prefix: new HarmonyMethod(typeof(RecipeFriendshipInjections), nameof(RecipeFriendshipInjections.GrantConversationFriendship_SendFriendshipRecipeChecks_Postfix))
            );
        }

        private void AddCraftsanityLocations()
        {
            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Stats), nameof(Stats.checkForCraftingAchievements)),
                postfix: new HarmonyMethod(typeof(CraftingInjections), nameof(CraftingInjections.CheckForCraftingAchievements_CheckCraftsanityLocation_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getCarpenterStock)),
                postfix: new HarmonyMethod(typeof(CraftingInjections), nameof(CraftingInjections.GetCarpenterStock_PurchasableRecipeChecks_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getDwarfShopStock)),
                postfix: new HarmonyMethod(typeof(CraftingInjections), nameof(CraftingInjections.GetDwarfShopStock_PurchasableRecipeChecks_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Sewer), nameof(Sewer.getShadowShopStock)),
                postfix: new HarmonyMethod(typeof(CraftingInjections), nameof(CraftingInjections.GetShadowShopStock_PurchasableRecipeChecks_Postfix))
            );
        }
    }
}
