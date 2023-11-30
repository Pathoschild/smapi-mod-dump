/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MysticalBuildings
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CaveOfMemories.Framework.GameLocations;
using CaveOfMemories.Framework.Managers;
using CaveOfMemories.Framework.Models;
using SolidFoundations.Framework.Interfaces.Internal;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CaveOfMemories
{
    public class MysticalBuildings : Mod
    {
        // Shared static helpers
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static ITranslationHelper i18n;
        internal static ModConfig modConfig;

        // Managers
        internal static ApiManager apiManager;

        // Functional variables
        internal static int shakeTimer = 0;
        internal static int cavernTimer = 0;

        private const string CRUMBLING_MINESHAFT_ID = "PeacefulEnd.SolidFoundations.MysticalBuildings_CrumblingMineshaft";
        private const string STATUE_OF_GREED_ID = "PeacefulEnd.SolidFoundations.MysticalBuildings_StatueofGreed";
        private const string QUIZZICAL_STATUE_ID = "PeacefulEnd.SolidFoundations.MysticalBuildings_QuizzicalStatue";
        private const string PHANTOM_CLOCK_ID = "PeacefulEnd.SolidFoundations.MysticalBuildings_PhantomClock";
        private const string ORB_OF_REFLECTION_ID = "PeacefulEnd.SolidFoundations.MysticalBuildings_OrbofReflection";
        private const string OBELISK_OF_WEATHER_ID = "PeacefulEnd.SolidFoundations.MysticalBuildings_ObeliskofWeather";
        private static List<string> _targetBuildingID = new List<string>()
        {
            CRUMBLING_MINESHAFT_ID,
            STATUE_OF_GREED_ID,
            QUIZZICAL_STATUE_ID,
            PHANTOM_CLOCK_ID,
            ORB_OF_REFLECTION_ID,
            OBELISK_OF_WEATHER_ID
        };

        private const string REFRESH_DAYS_REMAINING = "PeacefulEnd.MysticalBuildings.RefreshDaysRemaining";
        private const string WARN_OF_TIME_RESET_FLAG = "PeacefulEnd.MysticalBuildings.WarnOfTimeReset";
        private const string HAS_ENTERED_FLAG = "HasEntered";
        private const string ATTEMPTED_TEST_FLAG = "AttemptedTest";
        private const string IS_NOT_READY_FLAG = "IsNotReady";
        private const string IS_EATING_FLAG = "IsEating";
        private const string QUERY_COOLDOWN_MESSAGE = "QueryCooldown";
        private const string HAS_COG_FLAG = "HasCog";


        public override void Entry(IModHelper helper)
        {
            // Set up the monitor, helper and multiplayer
            monitor = Monitor;
            modHelper = helper;
            i18n = helper.Translation;

            // Set up the managers
            apiManager = new ApiManager(monitor);

            // Hook into required events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.Display.RenderingWorld += OnRenderingWorld;
            helper.Events.Display.RenderedHud += OnRenderedHud;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var solidFoundationsApi = apiManager.GetSolidFoundationsApi();
            foreach (BuildableGameLocation buildableGameLocation in Game1.locations.Where(b => b is BuildableGameLocation))
            {
                foreach (var building in buildableGameLocation.buildings.Where(b => _targetBuildingID.Contains(b.buildingType.Value)))
                {
                    int actualDaysRemaining = 0;
                    var rawDaysRemaining = building.modData.ContainsKey(REFRESH_DAYS_REMAINING) is true ? building.modData[REFRESH_DAYS_REMAINING] : null;
                    if (rawDaysRemaining is not null && int.TryParse(rawDaysRemaining, out actualDaysRemaining) is false)
                    {
                        actualDaysRemaining = 0;
                    }

                    if (String.IsNullOrEmpty(rawDaysRemaining))
                    {
                        actualDaysRemaining = GetActualDaysRemaining(solidFoundationsApi, building);
                    }

                    switch (building.buildingType.Value)
                    {
                        case CRUMBLING_MINESHAFT_ID:
                            if (solidFoundationsApi.DoesBuildingHaveFlag(building, HAS_ENTERED_FLAG) && actualDaysRemaining - 1 <= 0)
                            {
                                solidFoundationsApi.RemoveBuildingFlags(building, new List<string>() { HAS_ENTERED_FLAG });
                                building.modData[REFRESH_DAYS_REMAINING] = null;
                                continue;
                            }
                            break;
                        case STATUE_OF_GREED_ID:
                            if (solidFoundationsApi.DoesBuildingHaveFlag(building, IS_EATING_FLAG) && actualDaysRemaining - 1 <= 0)
                            {
                                solidFoundationsApi.RemoveBuildingFlags(building, new List<string>() { IS_EATING_FLAG });
                                building.modData[REFRESH_DAYS_REMAINING] = null;
                                continue;
                            }
                            break;
                        case QUIZZICAL_STATUE_ID:
                            if (solidFoundationsApi.DoesBuildingHaveFlag(building, ATTEMPTED_TEST_FLAG) && actualDaysRemaining - 1 <= 0)
                            {
                                solidFoundationsApi.RemoveBuildingFlags(building, new List<string>() { ATTEMPTED_TEST_FLAG });
                                building.modData[REFRESH_DAYS_REMAINING] = null;
                                continue;
                            }
                            break;
                        case ORB_OF_REFLECTION_ID:
                            if (solidFoundationsApi.DoesBuildingHaveFlag(building, IS_NOT_READY_FLAG) && actualDaysRemaining - 1 <= 0)
                            {
                                solidFoundationsApi.RemoveBuildingFlags(building, new List<string>() { IS_NOT_READY_FLAG });
                                building.modData[REFRESH_DAYS_REMAINING] = null;
                                continue;
                            }
                            break;
                        case OBELISK_OF_WEATHER_ID:
                            if (solidFoundationsApi.DoesBuildingHaveFlag(building, IS_NOT_READY_FLAG) && actualDaysRemaining - 1 <= 0)
                            {
                                solidFoundationsApi.RemoveBuildingFlags(building, new List<string>() { IS_NOT_READY_FLAG, "Sunny", "Rainy", "Stormy", "Random", "HasSettled" });
                                building.modData[REFRESH_DAYS_REMAINING] = null;
                                continue;
                            }
                            break;
                    }

                    if (actualDaysRemaining - 1 >= 0)
                    {
                        building.modData[REFRESH_DAYS_REMAINING] = (actualDaysRemaining - 1).ToString();
                    }
                }
            }

            if (Game1.player.modData.ContainsKey(WARN_OF_TIME_RESET_FLAG))
            {
                Game1.addHUDMessage(new HUDMessage(i18n.Get("Clock.Message.Warning"), null));
                Game1.player.currentLocation.playSound("crystal");
                Game1.player.modData.Remove(WARN_OF_TIME_RESET_FLAG);
            }
        }

        private void HandleBuildingsBeforeDayEnding()
        {
            var solidFoundationsApi = apiManager.GetSolidFoundationsApi();
            foreach (BuildableGameLocation buildableGameLocation in Game1.locations.Where(b => b is BuildableGameLocation))
            {
                foreach (var building in buildableGameLocation.buildings.Where(b => _targetBuildingID.Contains(b.buildingType.Value)))
                {
                    switch (building.buildingType.Value)
                    {
                        case PHANTOM_CLOCK_ID:
                            if (solidFoundationsApi.DoesBuildingHaveFlag(building, HAS_COG_FLAG) && SDate.Now().DaysSinceStart > modConfig.PhantomClockDaysToGoBack + 1)
                            {
                                var targetDate = SDate.Now().AddDays(-(modConfig.PhantomClockDaysToGoBack + 1));
                                Game1.dayOfMonth = targetDate.Day;
                                Game1.currentSeason = targetDate.Season;
                                Game1.setGraphicsForSeason();

                                solidFoundationsApi.RemoveBuildingFlags(building, new List<string>() { HAS_COG_FLAG });
                                Game1.player.modData[WARN_OF_TIME_RESET_FLAG] = true.ToString();
                                continue;
                            }
                            break;
                    }
                }
            }
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            // This line does nothing, as Solid Foundations serializes (and removes the buildings) from the game right before the day ends
            //HandleBuildingsBeforeDayEnding();
        }

        // TODO: Delete this after SDV v1.6
        private void OnBeforeBuildingSerialization(object sender, EventArgs e)
        {
            HandleBuildingsBeforeDayEnding();
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.OldLocation is UnstableCavern unstableCavern && unstableCavern is not null)
            {
                Game1.locations.Remove(e.OldLocation);
                shakeTimer = 0;
            }
        }

        internal static Random GenerateRandom(Farmer who = null)
        {
            if (who is not null)
            {
                return new Random((int)((long)Game1.uniqueIDForThisGame + who.DailyLuck + Game1.stats.DaysPlayed * 500 + Game1.ticks + DateTime.Now.Ticks));
            }
            return new Random((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed * 500 + Game1.ticks + DateTime.Now.Ticks));
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (Context.IsWorldReady is false || Game1.activeClickableMenu is not null)
            {
                return;
            }

            if (cavernTimer >= 0 && Game1.player.currentLocation is UnstableCavern)
            {
                int color = 2;
                if (cavernTimer >= 40)
                {
                    color = 7;
                }
                else if (cavernTimer >= 30)
                {
                    color = 6;
                }
                else if (cavernTimer >= 20)
                {
                    color = 4;
                }
                else if (cavernTimer >= 10)
                {
                    color = 3;
                }

                Rectangle tsarea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
                SpriteText.drawString(e.SpriteBatch, cavernTimer.ToString(), tsarea.Left + 16, tsarea.Top + 16, 999999, -1, 999999, 1f, 1f, junimoText: false, 2, "", color);
            }
        }

        private void OnRenderingWorld(object sender, RenderingWorldEventArgs e)
        {
            if (Context.IsWorldReady is false || Game1.activeClickableMenu is not null)
            {
                return;
            }

            if (shakeTimer > 0)
            {
                var offset = new Vector2(Game1.random.Next(-2, 2), Game1.random.Next(-2, 2));
                Game1.viewport.X += (int)offset.X;
                Game1.viewport.Y += (int)offset.Y;
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Set our default configuration file
            modConfig = Helper.ReadConfig<ModConfig>();

            // Hook into the APIs we utilize
            if (Helper.ModRegistry.IsLoaded("PeacefulEnd.SolidFoundations") && apiManager.HookIntoSolidFoundations(Helper))
            {
                var solidFoundationsApi = apiManager.GetSolidFoundationsApi();
                solidFoundationsApi.BroadcastSpecialActionTriggered += OnBroadcastSpecialActionTriggered;
                solidFoundationsApi.BeforeBuildingSerialization += OnBeforeBuildingSerialization;
            }
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && apiManager.HookIntoGMCM(Helper))
            {
                var configApi = apiManager.GetGMCMApi();
                configApi.Register(ModManifest, () => modConfig = new ModConfig(), () => Helper.WriteConfig(modConfig));
                configApi.AddNumberOption(this.ModManifest, () => modConfig.CrumblingMineshaftRefreshInDays, value => modConfig.CrumblingMineshaftRefreshInDays = value, () => "Crumbling Mineshaft Refresh (in days)", min: 1, max: 28, interval: 1);
                configApi.AddNumberOption(this.ModManifest, () => modConfig.StatueOfGreedRefreshInDays, value => modConfig.StatueOfGreedRefreshInDays = value, () => "Statue of Greed Refresh (in days)", min: 1, max: 28, interval: 1);
                configApi.AddNumberOption(this.ModManifest, () => modConfig.QuizzicalStatueRefreshInDays, value => modConfig.QuizzicalStatueRefreshInDays = value, () => "Quizzical Statue Refresh (in days)", min: 1, max: 28, interval: 1);
                configApi.AddNumberOption(this.ModManifest, () => modConfig.PhantomClockDaysToGoBack, value => modConfig.PhantomClockDaysToGoBack = value, () => "Phantom Clock Rewind (in days)", min: 1, max: 28, interval: 1);
                configApi.AddNumberOption(this.ModManifest, () => modConfig.OrbOfReflectionRefreshInDays, value => modConfig.OrbOfReflectionRefreshInDays = value, () => "Orb of Reflection Refresh (in days)", min: 1, max: 28, interval: 1);
                configApi.AddNumberOption(this.ModManifest, () => modConfig.ObeliskOfWeatherRefreshInDays, value => modConfig.ObeliskOfWeatherRefreshInDays = value, () => "Obelisk of Weather Refresh (in days)", min: 1, max: 28, interval: 1);
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            shakeTimer = 0;
            cavernTimer = 0;
        }

        private void OnBroadcastSpecialActionTriggered(object sender, IApi.BroadcastEventArgs e)
        {
            var solidFoundationsApi = apiManager.GetSolidFoundationsApi();
            if (e.Message.Equals(QUERY_COOLDOWN_MESSAGE, StringComparison.OrdinalIgnoreCase))
            {
                var rawDaysRemaining = e.Building.modData.ContainsKey(REFRESH_DAYS_REMAINING) is true ? e.Building.modData[REFRESH_DAYS_REMAINING] : null;
                if (rawDaysRemaining is null)
                {
                    rawDaysRemaining = GetActualDaysRemaining(solidFoundationsApi, e.Building).ToString();
                }

                switch (e.Building.buildingType.Value)
                {
                    case CRUMBLING_MINESHAFT_ID:
                        Game1.activeClickableMenu = new DialogueBox(rawDaysRemaining == "1" ? i18n.Get("Mine.Response.AlreadyEntered") : String.Format(i18n.Get("Mine.Response.AlreadyEntered.DaysLeft"), rawDaysRemaining));
                        break;
                    case STATUE_OF_GREED_ID:
                        Game1.activeClickableMenu = new DialogueBox(rawDaysRemaining == "1" ? i18n.Get("Greed.Dialogue.Full") : String.Format(i18n.Get("Greed.Dialogue.Full.DaysLeft"), rawDaysRemaining));
                        break;
                    case QUIZZICAL_STATUE_ID:
                        Game1.activeClickableMenu = new DialogueBox(rawDaysRemaining == "1" ? i18n.Get("Quiz.Response.AlreadyTested") : String.Format(i18n.Get("Quiz.Response.AlreadyTested.DaysLeft"), rawDaysRemaining));
                        break;
                    case ORB_OF_REFLECTION_ID:
                        Game1.activeClickableMenu = new DialogueBox(rawDaysRemaining == "1" ? i18n.Get("Orb.Response.NotReady") : String.Format(i18n.Get("Orb.Response.NotReady.DaysLeft"), rawDaysRemaining));
                        break;
                    case OBELISK_OF_WEATHER_ID:
                        Game1.activeClickableMenu = new DialogueBox(rawDaysRemaining == "1" ? i18n.Get("Obelisk.Response.NotReady") : String.Format(i18n.Get("Obelisk.Response.NotReady.DaysLeft"), rawDaysRemaining));
                        break;
                }
                return;
            }

            if (e.BuildingId == "PeacefulEnd.SolidFoundations.MysticalBuildings_StatueofGreed")
            {
                if (e.Farmer.ActiveObject is null)
                {
                    return;
                }

                var item = e.Farmer.ActiveObject;
                e.Farmer.currentLocation.createQuestionDialogue(String.Format(i18n.Get("Greed.Question.Confirmation"), item.Name), e.Farmer.currentLocation.createYesNoResponses(), new GameLocation.afterQuestionBehavior((who, whichAnswer) => HandleStatueOfGreed(e.Building, who, whichAnswer, item)));
            }
            else if (e.BuildingId == "PeacefulEnd.SolidFoundations.MysticalBuildings_CrumblingMineshaft")
            {
                HandleCrumblingMineshaft(e.Building, e.Farmer);
            }
            else if (e.BuildingId == "PeacefulEnd.SolidFoundations.MysticalBuildings_PhantomClock")
            {
                HandlePhantomClock(e.Building, e.Farmer, e.Message);
            }
            else if (e.BuildingId == "PeacefulEnd.SolidFoundations.MysticalBuildings_OrbofReflection")
            {
                HandleOrbOfReflection(e.Building, e.Farmer, e.Message);
            }
            else if (e.BuildingId == "PeacefulEnd.SolidFoundations.MysticalBuildings_ObeliskofWeather")
            {
                HandleObeliskOfWeather(e.Building, e.Farmer, e.Message);
            }
        }

        private void HandleStatueOfGreed(Building building, Farmer who, string whichAnswer, Item item)
        {
            if (whichAnswer == "No")
            {
                return;
            }
            var solidFoundationsApi = apiManager.GetSolidFoundationsApi();

            double baseChance = 0.5;
            double targetChance = GenerateRandom(who).NextDouble();
            double modifier = 1.0 + who.DailyLuck * 2.0 + who.LuckLevel * 0.08;
            Monitor.Log($"Attempting to double item via Statue of Greed: {targetChance} < {baseChance} * {modifier} ({baseChance * modifier})", LogLevel.Trace);

            who.removeItemFromInventory(item);
            if (targetChance < baseChance * modifier)
            {
                // Double the item
                int stackSize = item.Stack * 2;
                List<Item> itemsToAdd = new List<Item>() { item };
                if (stackSize > item.maximumStackSize())
                {
                    var secondaryItem = item.getOne();
                    secondaryItem.Stack = item.maximumStackSize() - stackSize;
                    itemsToAdd.Add(secondaryItem);

                    stackSize = item.maximumStackSize();
                }
                item.Stack = stackSize;

                who.addItemsByMenuIfNecessary(itemsToAdd);

                if (Game1.activeClickableMenu is null)
                {
                    Game1.activeClickableMenu = new DialogueBox(String.Format(i18n.Get("Greed.Response.Reward"), item.Name));
                }
            }
            else
            {
                solidFoundationsApi.AddBuildingFlags(building, new List<string>() { IS_EATING_FLAG }, isTemporary: false);
                Game1.activeClickableMenu = new DialogueBox(i18n.Get("Greed.Response.Hungry"));
            }
        }

        private void HandleCrumblingMineshaft(Building building, Farmer who)
        {
            var exitTile = new Point(building.tileX.Value + 2, building.tileY.Value + 3);
            UnstableCavern unstableCavern = new UnstableCavern(who.currentLocation, exitTile);
            Game1.locations.Add(unstableCavern);

            var warpTile = unstableCavern.tileBeneathLadder;
            Game1.warpFarmer(unstableCavern.NameOrUniqueName, (int)warpTile.X, (int)warpTile.Y, 2);
        }

        private void HandlePhantomClock(Building building, Farmer farmer, string message)
        {
            var solidFoundationsApi = apiManager.GetSolidFoundationsApi();
            if (message.ToLower() == "addcog")
            {
                solidFoundationsApi.AddBuildingFlags(building, new List<string>() { HAS_COG_FLAG, "IsTransitioning" }, isTemporary: false);
                if (farmer.ActiveObject.Stack == 1)
                {
                    farmer.removeItemFromInventory(farmer.ActiveObject);
                }
                else
                {
                    farmer.ActiveObject.Stack -= 1;
                }
            }
            if (message.ToLower() == "removecog")
            {
                farmer.addItemToInventory(new StardewValley.Object(112, 1));
            }
        }

        private void HandleOrbOfReflection(Building building, Farmer farmer, string message)
        {
            Game1.globalFadeToBlack(delegate { HandleReflectEvent(building); }, fadeSpeed: 0.01f);
        }

        private void HandleReflectEvent(Building building)
        {
            Game1.player.CanMove = false;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.X = -500 * 64;
            Game1.viewport.Y = -500 * 64;

            // Set the initial dialogue
            var dialogues = new List<string>()
            {
                i18n.Get("Orb.Dialogue.Intro"),
                i18n.Get("Orb.Dialogue.Intro.Second")
            };

            // Determine the buff to use
            var buffOptions = new List<string>()
            {
                "Farming",
                "Mining",
                "Fishing",
                "Foraging",
                "Luck",
                "Speed"
            };

            // Add the buff dialogue
            int index = GenerateRandom().Next(0, buffOptions.Count);
            dialogues.Add(i18n.Get($"Orb.Dialogue.Buff.{buffOptions[index]}"));
            dialogues.Add(i18n.Get("Orb.Dialogue.Outro"));

            // Add the actual buff
            int buffLevel = GenerateRandom().Next(1, 4);
            switch (buffOptions[index])
            {
                case "Farming":
                    Game1.player.addedFarmingLevel.Value += buffLevel;
                    break;
                case "Mining":
                    Game1.player.addedMiningLevel.Value += buffLevel;
                    break;
                case "Fishing":
                    Game1.player.addedFishingLevel.Value += buffLevel;
                    break;
                case "Foraging":
                    Game1.player.addedForagingLevel.Value += buffLevel;
                    break;
                case "Luck":
                    Game1.player.addedLuckLevel.Value += buffLevel;
                    break;
                case "Speed":
                    Game1.player.addedSpeed += buffLevel;
                    break;
            }

            // Show the dialogue
            var dialogueBox = new DialogueBox(dialogues);
            Game1.activeClickableMenu = new DialogueBox(dialogues);
            Game1.afterDialogues = delegate { Game1.globalFadeToClear(delegate { Game1.viewportFreeze = false; Game1.displayHUD = true; Game1.player.CanMove = true; }); };

            var solidFoundationsApi = apiManager.GetSolidFoundationsApi();
            solidFoundationsApi.AddBuildingFlags(building, new List<string>() { IS_NOT_READY_FLAG }, isTemporary: false);
        }

        private void HandleObeliskOfWeather(Building building, Farmer farmer, string message)
        {
            int randomWeatherIndex = GenerateRandom().Next(0, 3);
            switch (message.ToLower())
            {
                case "sunny":
                    Game1.weatherForTomorrow = 0;
                    return;
                case "rainy":
                    Game1.weatherForTomorrow = SDate.Now().AddDays(1).SeasonIndex == 3 ? 5 : 1;
                    return;
                case "stormy":
                    Game1.weatherForTomorrow = 3;
                    return;
                case "random":
                    Game1.weatherForTomorrow = randomWeatherIndex == 1 && SDate.Now().AddDays(1).SeasonIndex == 3 ? 5 : randomWeatherIndex;
                    return;
            }
        }

        private int GetActualDaysRemaining(IApi solidFoundationsApi, Building building)
        {
            switch (building.buildingType.Value)
            {
                case CRUMBLING_MINESHAFT_ID:
                    if (solidFoundationsApi.DoesBuildingHaveFlag(building, HAS_ENTERED_FLAG))
                    {
                        return modConfig.CrumblingMineshaftRefreshInDays;
                    }
                    break;
                case STATUE_OF_GREED_ID:
                    if (solidFoundationsApi.DoesBuildingHaveFlag(building, IS_EATING_FLAG))
                    {
                        return modConfig.StatueOfGreedRefreshInDays;
                    }
                    break;
                case QUIZZICAL_STATUE_ID:
                    if (solidFoundationsApi.DoesBuildingHaveFlag(building, ATTEMPTED_TEST_FLAG))
                    {
                        return modConfig.QuizzicalStatueRefreshInDays;
                    }
                    break;
                case ORB_OF_REFLECTION_ID:
                    if (solidFoundationsApi.DoesBuildingHaveFlag(building, IS_NOT_READY_FLAG))
                    {
                        return modConfig.OrbOfReflectionRefreshInDays;
                    }
                    break;
                case OBELISK_OF_WEATHER_ID:
                    if (solidFoundationsApi.DoesBuildingHaveFlag(building, IS_NOT_READY_FLAG))
                    {
                        return modConfig.ObeliskOfWeatherRefreshInDays;
                    }
                    break;
            }

            return 0;
        }
    }
}
