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
using Microsoft.Xna.Framework.Content;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using xTile.ObjectModel;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Quests
{
    public static class QuestInjections
    {
        private static readonly string[] _ignoredQuests = {
            "To The Beach", "Explore The Mine", "Deeper In The Mine", "To The Bottom?", "The Mysterious Qi",
            "A Winter Mystery", "Cryptic Note", "Dark Talisman", "Goblin Problem"
        };

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static ContentManager _englishContentManager;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _englishContentManager =
                new ContentManager(Game1.game1.Content.ServiceProvider, Game1.game1.Content.RootDirectory);
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

                // Item Delivery: __instance.dailyQuest == true and questType == 3 [Chance: 40 / 65]
                // Copper Ores: Daily True, Type 10 [Chance: 8 / 65]
                // Slay Monsters: Daily True, Type 4 [Chance: 10 / 65]
                // Catch fish: Daily Trye, Type 7 [Chance: 7 / 65]
                if (__instance.dailyQuest.Value)
                {
                    var isArchipelago = true;
                    var numberOfSteps = _archipelago.SlotData.HelpWantedLocationNumber / 7;
                    if (_archipelago.SlotData.HelpWantedLocationNumber % 7 > 0)
                    {
                        numberOfSteps++;
                    }
                    switch (__instance.questType.Value)
                    {
                        case (int)QuestType.ItemDelivery:
                            isArchipelago = CheckDailyQuestLocationOfType("Item Delivery", numberOfSteps * 4);
                            break;
                        case (int)QuestType.SlayMonsters:
                            isArchipelago = CheckDailyQuestLocationOfType("Slay Monsters", numberOfSteps);
                            break;
                        case (int)QuestType.Fishing:
                            isArchipelago = CheckDailyQuestLocationOfType("Fishing", numberOfSteps);
                            break;
                        case (int)QuestType.ResourceCollection:
                            isArchipelago = CheckDailyQuestLocationOfType("Gathering", numberOfSteps);
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

        private static string GetQuestEnglishName(int questId, string defaultName)
        {
            var englishQuests = _englishContentManager.Load<Dictionary<int, string>>("Data\\Quests");

            if (!englishQuests.ContainsKey(questId))
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
            if (__instance.nextQuests.Count > 0)
            {
                foreach (var nextQuest in __instance.nextQuests.Where(nextQuest => nextQuest > 0))
                {
                    Game1.player.questLog.Add(Quest.getQuestFromId(nextQuest));
                }

                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest.cs.13636"), 2));
            }

            Game1.player.questLog.Remove(__instance);
            Game1.playSound("questcomplete");
            if (__instance.id.Value == 126)
            {
                Game1.player.mailReceived.Add("emilyFiber");
                Game1.player.activeDialogueEvents.Add("emilyFiber", 2);
            }

            Game1.dayTimeMoneyBox.questsDirty = true;
        }

        private static bool CheckDailyQuestLocationOfType(string typeApName, int max)
        {
            var locationName = $"Help Wanted: {typeApName}";
            return CheckDailyQuestLocation(locationName, max);
        }

        public static bool CheckDailyQuestLocation(string locationName, int max)
        {
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

                _locationChecker.AddCheckedLocation(fullName);
                return true;
            }

            return false;
        }

        public static void Command_RemoveQuest_CheckLocation_Postfix(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            try
            {
                var questId = Convert.ToInt32(split[1]);
                var quest = Quest.getQuestFromId(questId);
                var questName = quest.GetName();
                if (_ignoredQuests.Contains(questName))
                {
                    return;
                }
                _locationChecker.AddCheckedLocation(questName);

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Command_RemoveQuest_CheckLocation_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static bool CheckAction_AdventurerGuild_Prefix(Mountain __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                var tile = __instance.map.GetLayer("Buildings").Tiles[tileLocation];
                if (tile == null || tile.TileIndex != 1136)
                {
                    return true; // run original logic
                }

                if (!who.mailReceived.Contains("guildMember"))
                {
                    Game1.drawLetterMessage(Game1.content.LoadString("Strings\\Locations:Mountain_AdventurersGuildNote").Replace('\n', '^'));
                    __result = true;
                    return false; // don't run original logic
                }

                string action = null;
                //var tile = __instance.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
                PropertyValue propertyValue;
                tile.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null)
                    action = propertyValue.ToString();
                if (action == null)
                {
                    action = __instance.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
                }
                if (action != null)
                {
                    __result = __instance.performAction(action, who, tileLocation);
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckAction_AdventurerGuild_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool PerformAction_MysteriousQiLumberPile_Prefix(GameLocation __instance, string action, Farmer who, Location tileLocation, ref bool __result)
        {
            try
            {
                if (action == null || !who.IsLocalPlayer)
                {
                    return true; // run original logic
                }

                var actionWords = action.Split(' ');
                var actionFirstWord = actionWords[0];

                if (actionFirstWord != "LumberPile" || who.hasOrWillReceiveMail("TH_LumberPile") || !who.hasOrWillReceiveMail("TH_SandDragon"))
                {
                    return true; // run original logic
                }

                Game1.player.mailReceived.Add("TH_LumberPile");
                Game1.player.removeQuest(5);
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

                var playerIsOnTheRight = Game1.player.getTileLocation().X > (double)tileLocation.X;
                var playerIsAlignedVertically = Game1.player.getTileLocation().X == (double)tileLocation.X;

                shakeLeftField.SetValue(playerIsOnTheRight || playerIsAlignedVertically && Game1.random.NextDouble() < 0.5);
                maxShakeField.SetValue((float)Math.PI / 128f);
                var isTownBush = __instance.townBush.Value;
                var seasonForLocation = Game1.GetSeasonForLocation(__instance.currentLocation);
                var isInBloom = __instance.inBloom(seasonForLocation, Game1.dayOfMonth);
                if (!isTownBush && __instance.tileSheetOffset.Value == 1 && isInBloom)
                {
                    ShakeForBushItem(__instance, tileLocation);
                    return false; // run original logic;
                }

                if (tileLocation.X == 20.0 && tileLocation.Y == 8.0 && Game1.dayOfMonth == 28 && Game1.timeOfDay == 1200 && !Game1.player.mailReceived.Contains("junimoPlush"))
                {
                    GetJunimoPlush(__instance);
                }
                else if (tileLocation.X == 28.0 && tileLocation.Y == 14.0 && Game1.player.eventsSeen.Contains(520702) && Game1.player.hasQuest(31) && Game1.currentLocation is Town currentTown)
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
            var str = bush.overrideSeason.Value == -1 ? Game1.GetSeasonForLocation(bush.currentLocation) : Utility.getSeasonNameFromNumber(bush.overrideSeason.Value);
            var shakeOff = -1;
            if (str != "spring")
            {
                if (str == "fall")
                {
                    shakeOff = 410;
                }
            }
            else
            {
                shakeOff = 296;
            }

            if (bush.size.Value == 3)
            {
                shakeOff = 815;
            }

            if (bush.size.Value == 4)
            {
                shakeOff = 73;
            }

            if (shakeOff == -1)
            {
                return;
            }

            bush.tileSheetOffset.Value = 0;
            bush.setUpSourceRect();
            var random = new Random((int)tileLocation.X + (int)tileLocation.Y * 5000 + (int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);
            if (bush.size.Value == 3 || bush.size.Value == 4)
            {
                var num = 1;
                for (var index = 0; index < num; ++index)
                {
                    if (bush.size.Value == 4)
                    {
                        bush.uniqueSpawnMutex.RequestLock(() =>
                        {
                            Game1.player.team.MarkCollectedNut("Bush_" + bush.currentLocation.Name + "_" + tileLocation.X + "_" + tileLocation.Y);
                            var @object = new Object(shakeOff, 1);
                            var boundingBox = bush.getBoundingBox();
                            var x = (double)boundingBox.Center.X;
                            boundingBox = bush.getBoundingBox();
                            var y = (double)(boundingBox.Bottom - 2);
                            var origin = new Vector2((float)x, (float)y);
                            var currentLocation = bush.currentLocation;
                            boundingBox = bush.getBoundingBox();
                            var bottom = boundingBox.Bottom;
                            Game1.createItemDebris(@object, origin, 0, currentLocation, bottom);
                        });
                    }
                    else
                    {
                        Game1.createObjectDebris(shakeOff, (int)tileLocation.X, (int)tileLocation.Y);
                    }
                }
            }
            else
            {
                var num = random.Next(1, 2) + Game1.player.ForagingLevel / 4;
                for (var index = 0; index < num; ++index)
                {
                    Game1.createItemDebris(new Object(shakeOff, 1, quality: (Game1.player.professions.Contains(16) ? 4 : 0)), Utility.PointToVector2(bush.getBoundingBox().Center), Game1.random.Next(1, 4));
                }
            }
            if (bush.size.Value == 3)
            {
                return;
            }

            DelayedAction.playSoundAfterDelay("leafrustle", 100);
        }

        private static void GetJunimoPlush(Bush __instance)
        {
            Game1.player.addItemByMenuIfNecessaryElseHoldUp(new Furniture(1733, Vector2.Zero), __instance.junimoPlushCallback);
        }

        public static bool MgThief_AfterSpeech_WinterMysteryFinished_Prefix(Town __instance)
        {
            try
            {
                var afterGlassMethod = _helper.Reflection.GetMethod(__instance, "mgThief_afterGlass");
                Game1.player.removeQuest(31);
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

        public static void SkillsPageCtor_BearKnowledge_Postfix(SkillsPage __instance, int x, int y, int width, int height)
        {
            try
            {
                const int bearKnowledgeIndex = 8;
                var x1 = __instance.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 80;
                var y1 = __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (int)(height / 2.0) + 80;
                if (_archipelago.HasReceivedItem("Bear's Knowledge"))
                {
                    var textureComponent = new ClickableTextureComponent("", new Rectangle(x1 + 544, y1, 64, 64), null, Game1.content.LoadString("Strings\\Objects:BearPaw"), Game1.mouseCursors, new Rectangle(192, 336, 16, 16), 4f, true);
                    textureComponent.myID = 10208;
                    textureComponent.rightNeighborID = -99998;
                    textureComponent.leftNeighborID = -99998;
                    textureComponent.upNeighborID = 4;
                    __instance.specialItems[bearKnowledgeIndex] = textureComponent;
                }
                else
                {
                    __instance.specialItems[bearKnowledgeIndex] = null;
                }


                var num1 = 680 / __instance.specialItems.Count;
                for (var index = 0; index < __instance.specialItems.Count; ++index)
                {
                    if (__instance.specialItems[index] != null)
                        __instance.specialItems[index].bounds.X = x1 + index * num1;
                }
                ClickableComponent.SetUpNeighbors(__instance.specialItems, 4);
                ClickableComponent.ChainNeighborsLeftRight(__instance.specialItems);

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SkillsPageCtor_BearKnowledge_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static void GetPriceAfterMultipliers_BearKnowledge_Postfix(Object __instance, float startPrice, long specificPlayerID, ref float __result)
        {
            try
            {
                if (__instance.ParentSheetIndex != 296 && __instance.ParentSheetIndex != 410)
                {
                    return;
                }

                var hasSeenBearEvent = Game1.player.eventsSeen.Contains(2120303);
                var hasReceivedBearKnowledge = _archipelago.HasReceivedItem("Bear's Knowledge");
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

        public static bool Command_AwardFestivalPrize_QiMilk_Prefix(Event __instance, GameLocation location,
            GameTime time, string[] split)
        {
            try
            {
                if (split.Length < 2)
                {
                    return true; // run original logic
                }
                var lower = split[1].ToLower();
                if (lower != "qimilk")
                {
                    return true; // run original logic
                }

                if (!Game1.player.mailReceived.Contains("qiCave"))
                {
                    _locationChecker.AddCheckedLocation("Cryptic Note");
                    Game1.player.mailReceived.Add("qiCave");

                    if (_archipelago.SlotData.Goal == Goal.CrypticNote)
                    {
                        _archipelago.ReportGoalCompletion();
                    }
                }
                ++__instance.CurrentCommand;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Command_AwardFestivalPrize_QiMilk_Prefix)}:\n{ex}",
                    LogLevel.Error);
                return true; // run original logic
            }
        }
    }

    public enum QuestType
    {
        Basic = 1,
        Crafting = 2,
        ItemDelivery = 3,
        Monster = 4,
        SlayMonsters = 4,
        Socialize = 5,
        Location = 6,
        Fishing = 7,
        Building = 8,
        ItemHarvest = 9,
        LostItem = 9,
        SecretLostItem = 9,
        ResourceCollection = 10,
    }
}
