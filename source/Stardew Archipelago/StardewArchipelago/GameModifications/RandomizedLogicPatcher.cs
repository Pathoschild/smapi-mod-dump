/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework.Content;
using StardewArchipelago.Archipelago;
using StardewArchipelago.GameModifications.CodeInjections;
using StardewArchipelago.GameModifications.EntranceRandomizer;
using StardewArchipelago.GameModifications.Seasons;
using StardewArchipelago.Locations;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewArchipelago.GameModifications
{
    public class RandomizedLogicPatcher
    {
        private readonly Harmony _harmony;
        private readonly ArchipelagoClient _archipelago;
        private readonly StardewItemManager _stardewItemManager;
        private readonly StartingResources _startingResources;

        public RandomizedLogicPatcher(IMonitor monitor, IModHelper helper, Harmony harmony, ArchipelagoClient archipelago, LocationChecker locationChecker, StardewItemManager stardewItemManager, EntranceManager entranceManager)
        {
            _harmony = harmony;
            _archipelago = archipelago;
            _stardewItemManager = stardewItemManager;
            _startingResources = new StartingResources(_archipelago, _stardewItemManager);
            MineshaftLogicInjections.Initialize(monitor);
            CommunityCenterLogicInjections.Initialize(monitor, locationChecker);
            FarmInjections.Initialize(monitor, _archipelago);
            AchievementInjections.Initialize(monitor, _archipelago);
            EntranceInjections.Initialize(monitor, _archipelago, entranceManager);
            ForestInjections.Initialize(monitor, _archipelago);
            MountainInjections.Initialize(monitor, _archipelago);
            SeedShopsInjections.Initialize(monitor, helper, archipelago, locationChecker);
            LostAndFoundInjections.Initialize(monitor, archipelago);
            TVInjections.Initialize(monitor, archipelago);
            LivinOffTheLandInjections.Initialize(monitor, archipelago);
            ProfitInjections.Initialize(monitor, archipelago);
            QuestLogInjections.Initialize(monitor, archipelago, locationChecker);
            WorldChangeEventInjections.Initialize(monitor);
            CropInjections.Initialize(monitor, archipelago, stardewItemManager);
            VoidMayoInjections.Initialize(monitor);
            KentInjections.Initialize(monitor, archipelago);
            GoldenEggInjections.Initialize(monitor, archipelago);
            GoldenClockInjections.Initialize(monitor, archipelago);

            DebugPatchInjections.Initialize(monitor, archipelago);
        }

        public void PatchAllGameLogic()
        {
            PatchAchievements();
            PatchMineMaxFloorReached();
            PatchDefinitionOfCommunityCenterComplete();
            PatchGrandpaNote();
            PatchDebris();
            PatchForest();
            PatchMountain();
            PatchEntrances();
            PatchSeasons();
            PatchSeedShops();
            PatchJodiFishQuest();
            PatchQuestLog();
            PatchWorldChangedEvent();
            PatchLostAndFoundBox();
            PatchMixedSeeds();            
            PatchTvChannels();
            PatchCleanupBeforeSave();
            PatchProfitMargin();
            PatchVoidMayo();
            PatchKent();
            PatchGoldenEgg();
            PatchGoldenClock();
            _startingResources.GivePlayerStartingResources();

            PatchDebugMethods();
        }

        private void PatchAchievements()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.getSteamAchievement)),
                prefix: new HarmonyMethod(typeof(AchievementInjections), nameof(AchievementInjections.GetSteamAchievement_DisableUndeservedAchievements_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Stats), nameof(Stats.checkForMoneyAchievements)),
                prefix: new HarmonyMethod(typeof(AchievementInjections), nameof(AchievementInjections.CheckForMoneyAchievements_GrantMoneyAchievementsFairly_Prefix))
            );
        }

        private void PatchMineMaxFloorReached()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(NetWorldState), typeof(NetWorldState).GetProperty(nameof(NetWorldState.LowestMineLevel)).GetSetMethod().Name),
                prefix: new HarmonyMethod(typeof(MineshaftLogicInjections), nameof(MineshaftLogicInjections.SetLowestMineLevel_SkipToSkullCavern_Prefix))
            );
        }

        private void PatchDefinitionOfCommunityCenterComplete()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.hasCompletedCommunityCenter)),
                prefix: new HarmonyMethod(typeof(CommunityCenterLogicInjections), nameof(CommunityCenterLogicInjections.HasCompletedCommunityCenter_CheckGameStateInsteadOfLetters_Prefix))
            );

            var townEvents = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\Town");
            var communityCenterCeremonyEventKey = "";
            var communityCenterCeremonyEventValue = "";

            foreach (var (key, value) in townEvents)
            {
                if (!key.StartsWith("191393"))
                {
                    continue;
                }

                communityCenterCeremonyEventKey = key;
                communityCenterCeremonyEventValue = value;
            }
            townEvents.Remove(communityCenterCeremonyEventKey);
            communityCenterCeremonyEventKey = communityCenterCeremonyEventKey.Replace(" cc", " apcc");
            townEvents.Add(communityCenterCeremonyEventKey, communityCenterCeremonyEventValue);

            SendMissedAPCCMails();
        }

        private static void SendMissedAPCCMails()
        {
            var communityCenter = Game1.locations.OfType<CommunityCenter>().First();
            if (!communityCenter.areAllAreasComplete())
            {
                return;
            }
            string[] apccMails =
                { "apccPantry", "apccCraftsRoom", "apccFishTank", "apccBoilerRoom", "apccVault", "apccBulletin" };
            foreach (var apccMail in apccMails)
            {
                if (!Game1.player.mailReceived.Contains(apccMail))
                {
                    Game1.player.mailForTomorrow.Add(apccMail + "%&NL&%");
                }
            }
        }

        private void PatchGrandpaNote()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.checkAction)),
                prefix: new HarmonyMethod(typeof(FarmInjections), nameof(FarmInjections.CheckAction_GrandpaNote_PreFix))
            );
        }

        private void PatchDebris()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.spawnWeedsAndStones)),
                prefix: new HarmonyMethod(typeof(FarmInjections), nameof(FarmInjections.SpawnWeedsAndStones_ConsiderUserPreference_PreFix))
            );
        }

        private void PatchForest()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Forest), "isWizardHouseUnlocked"),
                prefix: new HarmonyMethod(typeof(ForestInjections), nameof(ForestInjections.IsWizardHouseUnlocked_UnlockAtRatProblem_Prefix))
            );
        }

        private void PatchMountain()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Mountain), nameof(Mountain.ApplyTreehouseIfNecessary)),
                prefix: new HarmonyMethod(typeof(MountainInjections), nameof(MountainInjections.ApplyTreehouseIfNecessary_ApplyTreeHouseIfReceivedApItem_Prefix))
            );
        }

        private void PatchEntrances()
        {
            if (_archipelago.SlotData.EntranceRandomization == EntranceRandomization.Disabled)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "performWarpFarmer"),
                prefix: new HarmonyMethod(typeof(EntranceInjections), nameof(EntranceInjections.PerformWarpFarmer_EntranceRandomization_Prefix))
            );
        }

        private void PatchSeasons()
        {
            // Game1: public static void loadForNewGame(bool loadedGame = false)
            // Game1: private static void newSeason()
            // Game1: public static void NewDay(float timeToPause)

            var original = AccessTools.Method(typeof(Game1), nameof(Game1.NewDay));
            var prefix = new HarmonyMethod(typeof(SeasonsRandomizer), nameof(SeasonsRandomizer.NewDay_SeasonChoice_Prefix));

            _harmony.Patch(
                original: original,
                prefix: prefix
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "newSeason"),
                prefix: new HarmonyMethod(typeof(SeasonsRandomizer), nameof(SeasonsRandomizer.NewSeason_UsePredefinedChoice_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(SeasonsRandomizer), nameof(SeasonsRandomizer.AnswerDialogueAction_SeasonChoice_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.Date)),
                prefix: new HarmonyMethod(typeof(SeasonsRandomizer), nameof(SeasonsRandomizer.Date_UseTotalDaysStats_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Friendship), nameof(Friendship.CountdownToWedding)),
                prefix: new HarmonyMethod(typeof(SeasonsRandomizer), nameof(SeasonsRandomizer.CountdownToWedding_Add1_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.getWeatherModificationsForDate)),
                prefix: new HarmonyMethod(typeof(SeasonsRandomizer), nameof(SeasonsRandomizer.GetWeatherModificationsForDate_UseCorrectDates_Prefix))
            );

            SeasonsRandomizer.ChangeMailKeysBasedOnSeasonsToDaysElapsed();
        }

        private void PatchSeedShops()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.openShopMenu)),
                prefix: new HarmonyMethod(typeof(SeedShopsInjections), nameof(SeedShopsInjections.OpenShopMenu_PierreAndSandyPersistentEvent_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getJojaStock)),
                prefix: new HarmonyMethod(typeof(SeedShopsInjections), nameof(SeedShopsInjections.GetJojaStock_FullCostco_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.update)),
                postfix: new HarmonyMethod(typeof(SeedShopsInjections), nameof(SeedShopsInjections.Update_SeedShuffleFirstTimeOnly_Postfix))
            );
        }

        private const int FISH_CASSEROLE_QUEST_ID = 22;

        private void PatchJodiFishQuest()
        {
            if (_archipelago.SlotData.EntranceRandomization == EntranceRandomization.Disabled)
            {
                return;
            }

            ChangeFishCasseroleEventTriggerTimes();
            ChangeFishCasseroleQuestText();
            RefreshFishCasseroleQuestIfAlreadyHasIt();
        }

        private static void ChangeFishCasseroleEventTriggerTimes()
        {
            var jodiHouseEvents = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\SamHouse");
            const string originalTimeRequired = "1800 1950";
            const string newTimeRequired = "1200 2250";
            var eventsToFix = new HashSet<string>();

            foreach (var (key, value) in jodiHouseEvents)
            {
                if (!key.Contains(originalTimeRequired))
                {
                    continue;
                }

                eventsToFix.Add(key);
            }

            foreach (var eventToFix in eventsToFix)
            {
                var eventValue = jodiHouseEvents[eventToFix];
                jodiHouseEvents.Remove(eventToFix);
                var fixedKey = eventToFix.Replace(originalTimeRequired, newTimeRequired);
                jodiHouseEvents.Add(fixedKey, eventValue);
            }
        }

        private static void ChangeFishCasseroleQuestText()
        {
            ChangeFishCasseroleQuestText(Game1.content);
            ChangeFishCasseroleQuestText(Game1.temporaryContent);
        }

        private static void ChangeFishCasseroleQuestText(ContentManager contentManager)
        {
            var quests = contentManager.Load<Dictionary<int, string>>("Data\\Quests");
            var fishQuest = quests[FISH_CASSEROLE_QUEST_ID];
            var modifiedFishQuest = fishQuest.Replace("dinner at 7:00 PM.", "dinner.")
                .Replace("bass at 7:00 PM.", "bass in the afternoon.");
            quests[FISH_CASSEROLE_QUEST_ID] = modifiedFishQuest;
        }

        private static void RefreshFishCasseroleQuestIfAlreadyHasIt()
        {
            foreach (var quest in Game1.player.questLog.ToArray())
            {
                if (quest.id.Value != FISH_CASSEROLE_QUEST_ID || quest.completed.Value ||
                    !quest.currentObjective.Contains("7:00 PM"))
                {
                    continue;
                }

                Game1.player.removeQuest(FISH_CASSEROLE_QUEST_ID);
                Game1.player.addQuest(FISH_CASSEROLE_QUEST_ID);
            }
        }

        private void PatchLostAndFoundBox()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(FarmerTeam), nameof(FarmerTeam.CheckReturnedDonations)),
                prefix: new HarmonyMethod(typeof(LostAndFoundInjections), nameof(LostAndFoundInjections.CheckReturnedDonations_UpgradeToolsProperly_Prefix))
            );
        }

        private void PatchQuestLog()
        {
            _harmony.Patch(
                original: AccessTools.Constructor(typeof(QuestLog)),
                postfix: new HarmonyMethod(typeof(QuestLogInjections), nameof(QuestLogInjections.Constructor_MakeQuestsNonCancellable_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.foundArtifact)),
                postfix: new HarmonyMethod(typeof(QuestLogInjections), nameof(QuestLogInjections.FoundArtifact_StartArchaeologyIfMissed_Postfix))
            );
        }

        private void PatchWorldChangedEvent()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(WorldChangeEvent), nameof(WorldChangeEvent.setUp)),
                prefix: new HarmonyMethod(typeof(WorldChangeEventInjections), nameof(WorldChangeEventInjections.SetUp_MakeSureEventsAreNotDuplicated_Prefix))
            );
        }

        private void PatchMixedSeeds()
        {
            if (_archipelago.SlotData.Cropsanity == Cropsanity.Disabled)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Constructor(typeof(Crop), new[] { typeof(int), typeof(int), typeof(int)}),
                prefix: new HarmonyMethod(typeof(CropInjections), nameof(CropInjections.CropConstructor_WildSeedsBecomesUnlockedCrop_Prefix))
            );
        }

        private void PatchTvChannels()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(TV), nameof(TV.checkForAction)),
                prefix: new HarmonyMethod(typeof(TVInjections), nameof(TVInjections.CheckForAction_TVChannels_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(TV), "getTodaysTip"),
                prefix: new HarmonyMethod(typeof(LivinOffTheLandInjections), nameof(LivinOffTheLandInjections.GetTodaysTip_CustomLivinOffTheLand_Prefix))
            );
        }

        private void PatchCleanupBeforeSave()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.cleanupBeforeSave)),
                postfix: new HarmonyMethod(typeof(CleanupBeforeSaveInjections), nameof(CleanupBeforeSaveInjections.CleanupBeforeSave_RemoveIllegalMonsters_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(CommunityCenter), nameof(CommunityCenter.cleanupBeforeSave)),
                postfix: new HarmonyMethod(typeof(CleanupBeforeSaveInjections), nameof(CleanupBeforeSaveInjections.CleanupBeforeSave_RemoveIllegalMonsters_Postfix))
            );
        }

        private void PatchProfitMargin()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.sellToStorePrice)),
                postfix: new HarmonyMethod(typeof(ProfitInjections), nameof(ProfitInjections.SellToStorePrice_ApplyProfitMargin_Postfix))
            );
        }

        private void PatchVoidMayo()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
                prefix: new HarmonyMethod(typeof(VoidMayoInjections), nameof(VoidMayoInjections.GetFish_FishVoidMayo_PreFix))
            );
        }

        private void PatchKent()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.addKentIfNecessary)),
                prefix: new HarmonyMethod(typeof(KentInjections), nameof(KentInjections.AddKentIfNecessary_ConsiderSeasonsRandomizer_Prefix))
            );
        }

        private void PatchGoldenEgg()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getAnimalShopStock)),
                prefix: new HarmonyMethod(typeof(GoldenEggInjections), nameof(GoldenEggInjections.GetAnimalShopStock_GoldenEggIfReceived_Postfix))
            );
        }

        private void PatchGoldenClock()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Building), nameof(Building.doAction)),
                postfix: new HarmonyMethod(typeof(GoldenClockInjections), nameof(GoldenClockInjections.DoAction_GoldenClockIncreaseTime_Postfix))
            );
        }

        private void PatchDebugMethods()
        {
        }
    }
}
