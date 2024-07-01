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
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Items.Unlocks.Vanilla;
using StardewArchipelago.Locations.Festival;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Quests
{
    public static class QuestInjections
    {
        private const string DESERT_FESTIVAL_FISHING_QUEST_ID = "98765";
        private static List<string> _ignoredQuests = new()
        {
            "To The Beach", "Explore The Mine", "Deeper In The Mine", "To The Bottom?", "The Mysterious Qi",
            "A Winter Mystery", "Cryptic Note", "Dark Talisman", "Goblin Problem",
        };

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static LocalizedContentManager _englishContentManager;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _englishContentManager = new LocalizedContentManager(Game1.game1.Content.ServiceProvider, Game1.game1.Content.RootDirectory, new CultureInfo("en-US"));
            UpdateIgnoredQuestList();
        }

        public static bool QuestComplete_LocationInsteadOfReward_Prefix(Quest __instance)
        {
            try
            {
                var questName = __instance.GetName();
                var englishQuestName = GetQuestEnglishName(__instance.id.Value, questName);
                if (__instance.completed.Value || _ignoredQuests.Contains(englishQuestName))
                {
                    return true; // run original logic
                }

                if (__instance.id.Value != null && __instance.id.Value.Equals(DESERT_FESTIVAL_FISHING_QUEST_ID, StringComparison.InvariantCultureIgnoreCase))
                {
                    // This one is a quest for some reason
                    _locationChecker.AddCheckedLocation(FestivalLocationNames.WILLYS_CHALLENGE);
                    OriginalQuestCompleteCode(__instance);
                    return false; // don't run original logic
                }

                if (!_archipelago.SlotData.QuestLocations.StoryQuestsEnabled)
                {
                    return true; // run original logic
                }

                // Item Delivery: __instance.dailyQuest == true and questType == 3 [Chance: 40 / 65]
                // Copper Ores: Daily True, Type 1
                // Slay Monsters: Daily True, Type 4
                // Catch fish: Daily True, Type 7
                if (__instance.dailyQuest.Value)
                {
                    var isArchipelago = true;
                    var numberOfSteps = GetNumberOfHelpWantedGroups();
                    switch (__instance.questType.Value)
                    {
                        case (int)QuestType.ItemDelivery:
                            isArchipelago = CheckDailyQuestLocationOfType(DailyQuest.ITEM_DELIVERY, numberOfSteps * 4);
                            break;
                        case (int)QuestType.SlayMonsters:
                            isArchipelago = CheckDailyQuestLocationOfType(DailyQuest.SLAY_MONSTERS, numberOfSteps);
                            break;
                        case (int)QuestType.Fishing:
                            isArchipelago = CheckDailyQuestLocationOfType(DailyQuest.FISHING, numberOfSteps);
                            break;
                        case (int)QuestType.ResourceCollection:
                            isArchipelago = CheckDailyQuestLocationOfType(DailyQuest.GATHERING, numberOfSteps);
                            break;
                    }

                    if (!isArchipelago)
                    {
                        return true; // run original logic
                    }

                    ++Game1.stats.QuestsCompleted;
                }
                else
                {
                    _locationChecker.AddCheckedLocation(englishQuestName);
                }

                OriginalQuestCompleteCode(__instance);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(QuestComplete_LocationInsteadOfReward_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static int GetNumberOfHelpWantedGroups()
        {
            var numberOfSteps = _archipelago.SlotData.QuestLocations.HelpWantedNumber / 7;
            if (_archipelago.SlotData.QuestLocations.HelpWantedNumber % 7 > 0)
            {
                numberOfSteps++;
            }

            return numberOfSteps;
        }

        private static string GetQuestEnglishName(string questId, string defaultName)
        {
            var englishQuests = DataLoader.Quests(_englishContentManager);

            if (string.IsNullOrWhiteSpace(questId) || !englishQuests.ContainsKey(questId))
            {
                return defaultName;
            }

            var equivalentEnglishQuestString = englishQuests[questId];
            var englishTitle = equivalentEnglishQuestString.Split('/')[1];
            return englishTitle;
        }

        private static void OriginalQuestCompleteCode(Quest __instance)
        {
            __instance.completed.Value = true;
            Game1.player.currentLocation?.customQuestCompleteBehavior(__instance.id.Value);
            if (__instance.nextQuests.Count > 0)
            {
                foreach (var nextQuest in __instance.nextQuests.Where(x => !string.IsNullOrEmpty(x) && x != "-1"))
                {
                    Game1.player.addQuest(nextQuest);
                }

                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest.cs.13636"), 2));
            }

            Game1.player.questLog.Remove(__instance);
            Game1.playSound("questcomplete");
            if (__instance.id.Value == "126")
            {
                Game1.player.mailReceived.Add("emilyFiber");
                Game1.player.activeDialogueEvents.Add("emilyFiber", 2);
            }

            Game1.dayTimeMoneyBox.questsDirty = true;
            Game1.player.autoGenerateActiveDialogueEvent("questComplete_" + __instance.id.Value);
        }

        private static bool CheckDailyQuestLocationOfType(string typeApName, int max)
        {
            var locationName = string.Format(DailyQuest.HELP_WANTED, typeApName);
            return CheckDailyQuestLocation(locationName, max);
        }

        public static bool CheckDailyQuestLocation(string locationName, int max)
        {
            if (GetNextDailyQuestLocation(locationName, max, out var nextQuestLocationName))
            {
                _locationChecker.AddCheckedLocation(nextQuestLocationName);
                return true;
            }

            return false;
        }

        private static bool GetNextDailyQuestLocation(string locationName, int max, out string nextLocationName)
        {
            nextLocationName = string.Empty;
            var nextLocationNumber = 1;
            while (nextLocationNumber <= max)
            {
                var fullName = $"{locationName} {nextLocationNumber}";
                var id = _archipelago.GetLocationId(fullName);
                if (id < 1)
                {
                    return false;
                }

                if (_locationChecker.IsLocationChecked(fullName))
                {
                    nextLocationNumber++;
                    continue;
                }

                nextLocationName = fullName;
                return true;
            }

            return false;
        }

        // public static void RemoveQuest(Event @event, string[] args, EventContext context)
        public static void RemoveQuest_CheckLocation_Postfix(Event @event, string[] args, EventContext context)
        {
            try
            {
                var questId = args[1];
                var quest = Quest.getQuestFromId(questId);
                var questName = quest.GetName();
                var englishQuestName = GetQuestEnglishName(questId, questName);
                if (_ignoredQuests.Contains(englishQuestName))
                {
                    return;
                }
                _locationChecker.AddCheckedLocation(englishQuestName);

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(RemoveQuest_CheckLocation_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public static Quest getQuestOfTheDay()
        public static bool GetQuestOfTheDay_BalanceQuests_Prefix(ref Quest __result)
        {
            try
            {
                if (Game1.stats.DaysPlayed <= 1U)
                {
                    __result = null;
                    return false; // don't run original logic
                }

                var todayRandom = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);
                var weightedLocations = CreateWeightedMissingLocations();
                if (!weightedLocations.Any())
                {
                    __result = null;
                    return true; // run original logic
                }

                var chosenIndex = todayRandom.Next(0, weightedLocations.Count);
                var chosenQuestType = weightedLocations[chosenIndex];
                switch (chosenQuestType)
                {
                    case DailyQuest.ITEM_DELIVERY:
                        __result = new ItemDeliveryQuest();
                        return false; // don't run original logic
                    case DailyQuest.FISHING:
                        __result = new FishingQuest();
                        return false; // don't run original logic
                    case DailyQuest.GATHERING:
                        __result = new ResourceCollectionQuest();
                        return false; // don't run original logic
                    case DailyQuest.SLAY_MONSTERS:
                        __result = new SlayMonsterQuest();
                        return false; // don't run original logic
                    default:
                        __result = null;
                        return true; // run original logic
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetQuestOfTheDay_BalanceQuests_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static List<string> CreateWeightedMissingLocations()
        {
            var hints = _archipelago.GetHints()
                .Where(x => !x.Found && _archipelago.GetPlayerName(x.FindingPlayer) == _archipelago.SlotData.SlotName)
                .ToArray();
            var numberOfSteps = GetNumberOfHelpWantedGroups();
            var remainingHelpWantedQuests = new List<string>();
            for (var groupNumber = 1; groupNumber <= numberOfSteps; groupNumber++)
            {
                AddWeightedItemDeliveries(groupNumber, hints, remainingHelpWantedQuests);
                AddWeightedFishing(groupNumber, hints, remainingHelpWantedQuests);
                AddWeightedHelpWanted(groupNumber, DailyQuest.GATHERING, hints, remainingHelpWantedQuests);
                AddWeightedSlaying(groupNumber, hints, remainingHelpWantedQuests);
            }

            return remainingHelpWantedQuests;
        }

        private static void AddWeightedItemDeliveries(int groupNumber, Hint[] hints, List<string> remainingHelpWantedQuests)
        {
            const int itemDeliveryMultiplier = 4;
            var offset = ((groupNumber - 1) * itemDeliveryMultiplier) + 1;
            for (var delivery = 0; delivery < 4; delivery++)
            {
                AddWeightedHelpWanted(offset + delivery, DailyQuest.ITEM_DELIVERY, hints, remainingHelpWantedQuests);
            }
        }

        private static void AddWeightedFishing(int groupNumber, Hint[] hints, List<string> remainingHelpWantedQuests)
        {
            if (!_archipelago.HasReceivedItem(ToolUnlockManager.PROGRESSIVE_FISHING_ROD))
            {
                return;
            }

            AddWeightedHelpWanted(groupNumber, DailyQuest.FISHING, hints, remainingHelpWantedQuests);
        }

        private static void AddWeightedSlaying(int groupNumber, Hint[] hints, List<string> remainingHelpWantedQuests)
        {
            if (Game1.stats.MonstersKilled < 10)
            {
                return;
            }

            AddWeightedHelpWanted(groupNumber, DailyQuest.SLAY_MONSTERS, hints, remainingHelpWantedQuests);
        }

        private static void AddWeightedHelpWanted(int questNumber, string questType, Hint[] hints, List<string> remainingHelpWantedQuests)
        {
            var location = GetHelpWantedLocationName(questType, questNumber);
            if (!_locationChecker.IsLocationMissing(location))
            {
                return;
            }

            var weight = hints.Any(hint => _archipelago.GetLocationName(hint.LocationId) == location) ? 10 : 1;
            remainingHelpWantedQuests.AddRange(Enumerable.Repeat(questType, weight));
        }

        private static string GetHelpWantedLocationName(string type, int number)
        {
            return $"{string.Format(DailyQuest.HELP_WANTED, type)} {number}";
        }

        // public virtual bool performAction(string[] action, Farmer who, Location tileLocation)
        public static bool PerformAction_MysteriousQiLumberPile_Prefix(GameLocation __instance, string[] action, Farmer who, Location tileLocation, ref bool __result)
        {
            try
            {
                if (action == null || !who.IsLocalPlayer)
                {
                    return true; // run original logic
                }

                var actionFirstWord = action[0];

                if (actionFirstWord != "LumberPile" || who.hasOrWillReceiveMail("TH_LumberPile") || !who.hasOrWillReceiveMail("TH_SandDragon"))
                {
                    return true; // run original logic
                }

                Game1.player.mailReceived.Add("TH_LumberPile");
                Game1.player.removeQuest("5");
                _locationChecker.AddCheckedLocation("The Mysterious Qi");

                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformAction_MysteriousQiLumberPile_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool Shake_WinterMysteryBush_Prefix(Bush __instance, Vector2 tileLocation,
            bool doEvenIfStillShaking)
        {
            try
            {
                var maxShakeField = _helper.Reflection.GetField<float>(__instance, "maxShake");
                if (!(maxShakeField.GetValue() == 0.0 || doEvenIfStillShaking))
                {
                    return false; // don't run original logic
                }

                var shakeLeftField = _helper.Reflection.GetField<bool>(__instance, "shakeLeft");

                var playerIsOnTheRight = Game1.player.Tile.X > tileLocation.X;
                var playerIsAlignedVertically = Game1.player.Tile.X == tileLocation.X;

                shakeLeftField.SetValue(playerIsOnTheRight || playerIsAlignedVertically && Game1.random.NextDouble() < 0.5);
                maxShakeField.SetValue((float)Math.PI / 128f);
                var isTownBush = __instance.townBush.Value;
                var isInBloom = __instance.inBloom();
                if (!isTownBush && __instance.tileSheetOffset.Value == 1 && isInBloom)
                {
                    ShakeForBushItem(__instance, tileLocation);
                    return false; // run original logic;
                }

                if (tileLocation.X == 20.0 && tileLocation.Y == 8.0 && Game1.dayOfMonth == 28 && Game1.timeOfDay == 1200 && !Game1.player.mailReceived.Contains("junimoPlush"))
                {
                    GetJunimoPlush(__instance);
                }
                else if (tileLocation.X == 28.0 && tileLocation.Y == 14.0 && Game1.player.eventsSeen.Contains("520702") && Game1.player.hasQuest("31") && Game1.currentLocation is Town currentTown)
                {
                    currentTown.initiateMagnifyingGlassGet();
                }
                else
                {
                    if (tileLocation.X != 47.0 || tileLocation.Y != 100.0 ||
                        !Game1.player.secretNotesSeen.Contains(21) || Game1.timeOfDay != 2440 ||
                        !(Game1.currentLocation is Town) || Game1.player.mailReceived.Contains("secretNote21_done"))
                    {
                        return false; // don't run original logic
                    }
                    Game1.player.mailReceived.Add("secretNote21_done");
                    ((Town)Game1.currentLocation).initiateMarnieLewisBush();
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Shake_WinterMysteryBush_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void ShakeForBushItem(Bush bush, Vector2 tileLocation)
        {

            var shakeOffItem = bush.GetShakeOffItem();
            if (string.IsNullOrWhiteSpace(shakeOffItem))
            {
                return;
            }

            bush.tileSheetOffset.Value = 0;
            bush.setUpSourceRect();
            switch (bush.size.Value)
            {
                case 3:
                    Game1.createObjectDebris(shakeOffItem, (int)tileLocation.X, (int)tileLocation.Y);
                    break;
                case 4:
                    bush.uniqueSpawnMutex.RequestLock((Action)(() =>
                    {
                        Game1.player.team.MarkCollectedNut($"Bush_{bush.Location.Name}_{tileLocation.X}_{tileLocation.Y}");
                        var obj = ItemRegistry.Create(shakeOffItem);
                        var boundingBox = bush.getBoundingBox();
                        var x = (double)boundingBox.Center.X;
                        boundingBox = bush.getBoundingBox();
                        var y = (double)(boundingBox.Bottom - 2);
                        var pixelOrigin = new Vector2((float)x, (float)y);
                        var location = bush.Location;
                        boundingBox = bush.getBoundingBox();
                        var bottom = boundingBox.Bottom;
                        Game1.createItemDebris(obj, pixelOrigin, 0, location, bottom);
                    }));
                    break;
                default:
                    var random = Utility.CreateRandom((double)tileLocation.X, (double)tileLocation.Y * 5000.0, (double)Game1.uniqueIDForThisGame, (double)Game1.stats.DaysPlayed);
                    var howMuch = random.Next(1, 2) + Game1.player.ForagingLevel / 4;
                    for (var index = 0; index < howMuch; ++index)
                    {
                        var obj = ItemRegistry.Create(shakeOffItem);
                        if (Game1.player.professions.Contains(16))
                        {
                            obj.Quality = 4;
                        }
                        Game1.createItemDebris(obj, Utility.PointToVector2(bush.getBoundingBox().Center), Game1.random.Next(1, 4));
                    }
                    Game1.player.gainExperience(2, howMuch);
                    break;
            }
            if (bush.size.Value == 3)
            {
                return;
            }
            DelayedAction.playSoundAfterDelay("leafrustle", 100);
        }

        private static void GetJunimoPlush(Bush __instance)
        {
            Game1.player.addItemByMenuIfNecessaryElseHoldUp(new Furniture("1733", Vector2.Zero), __instance.junimoPlushCallback);
        }

        public static bool MgThief_AfterSpeech_WinterMysteryFinished_Prefix(Town __instance)
        {
            try
            {
                var afterGlassMethod = _helper.Reflection.GetMethod(__instance, "mgThief_afterGlass");
                Game1.player.removeQuest("31");
                _locationChecker.AddCheckedLocation("A Winter Mystery");
                afterGlassMethod.Invoke();
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MgThief_AfterSpeech_WinterMysteryFinished_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public override void populateClickableComponentList()
        public static void PopulateClickableComponentList_BearKnowledge_Postfix(PowersTab __instance)
        {
            try
            {
                var hasBearKnowledge = _archipelago.HasReceivedItem(APItem.BEARS_KNOWLEDGE);
                foreach (var powersLine in __instance.powers)
                {
                    foreach (var powerComponent in powersLine)
                    {
                        // I couldn't really find a better way to identify this icon uniquely.
                        // Ideally, in the long term, the condition should be changed in the Data itself, instead of this jank patching.
                        if (powerComponent.sourceRect.X != 192 || powerComponent.sourceRect.Y != 336)
                        {
                            continue;
                        }

                        // drawShadow is poorly named. If drawShadow, then it's "real", otherwise it's a black outline.
                        powerComponent.drawShadow = hasBearKnowledge;
                        return;
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PopulateClickableComponentList_BearKnowledge_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static void GetPriceAfterMultipliers_BearKnowledge_Postfix(Object __instance, float startPrice, long specificPlayerID, ref float __result)
        {
            try
            {
                if (__instance.QualifiedItemId != QualifiedItemIds.SALMONBERRY && __instance.QualifiedItemId != QualifiedItemIds.BLACKBERRY)
                {
                    return;
                }

                var hasSeenBearEvent = Game1.player.eventsSeen.Contains(EventIds.BEAR_KNOWLEDGE_EVENT);
                var hasReceivedBearKnowledge = _archipelago.HasReceivedItem(APItem.BEARS_KNOWLEDGE);
                if (hasSeenBearEvent == hasReceivedBearKnowledge)
                {
                    return;
                }

                if (hasReceivedBearKnowledge)
                {
                    __result *= 3f;
                    return;
                }

                __result /= 3f;
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetPriceAfterMultipliers_BearKnowledge_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public static void AwardFestivalPrize(Event @event, string[] args, EventContext context)
        public static bool AwardFestivalPrize_QiMilk_Prefix(Event @event, string[] args, EventContext context)
        {
            try
            {
                if (args.Length < 2)
                {
                    return true; // run original logic
                }
                var prize = args[1];
                if (!prize.Equals("qimilk", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true; // run original logic
                }

                _locationChecker.AddCheckedLocation("Cryptic Note");

                if (!Game1.player.mailReceived.Contains("qiCave"))
                {
                    Game1.player.mailReceived.Add("qiCave");
                }

                if (_archipelago.SlotData.Goal == Goal.CrypticNote)
                {
                    _archipelago.ReportGoalCompletion();
                }

                ++@event.CurrentCommand;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AwardFestivalPrize_QiMilk_Prefix)}:\n{ex}",
                    LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void UpdateIgnoredQuestList()
        {
            _ignoredQuests.AddRange(IgnoredModdedStrings.Quests);
        }
    }
}
