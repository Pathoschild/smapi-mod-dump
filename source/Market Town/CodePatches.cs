/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/congha22/foodstore
**
*************************************************/

using HarmonyLib;
using MarketTown.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.LocationContexts;
using StardewValley.GameData.Shops;
using StardewValley.GameData.SpecialOrders;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace MarketTown
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry
    {
        private static void NPC_dayUpdate_Postfix(NPC __instance)
        {
            if (!__instance.IsVillager) return;

            __instance.modData["hapyke.FoodStore/timeVisitShed"] = "0";
            __instance.modData["hapyke.FoodStore/shedEntry"] = "-1,-1";
            __instance.modData["hapyke.FoodStore/gettingFood"] = "false";
            __instance.modData["hapyke.FoodStore/LastFood"] = "0";
            __instance.modData["hapyke.FoodStore/LastCheck"] = "0";
            __instance.modData["hapyke.FoodStore/LocationControl"] = ",";
            __instance.modData["hapyke.FoodStore/LastFoodTaste"] = "-1";
            __instance.modData["hapyke.FoodStore/LastFoodDecor"] = "-1";
            __instance.modData["hapyke.FoodStore/LastSay"] = "0";
            __instance.modData["hapyke.FoodStore/TotalCustomerResponse"] = "0";
            __instance.modData["hapyke.FoodStore/inviteTried"] = "false";
            __instance.modData["hapyke.FoodStore/stuckCounter"] = "0";
            __instance.modData["hapyke.FoodStore/festivalLastPurchase"] = "600";
            __instance.modData["hapyke.FoodStore/specialOrder"] = "-1,-1";
            __instance.modData["hapyke.FoodStore/shopOwnerToday"] = "-1,-1";

        }

        private static void NPC_performTenMinuteUpdate_Postfix(NPC __instance)
        {
            if (!Game1.hasLoadedGame || __instance == null || !__instance.IsVillager || __instance.currentLocation == null || __instance.getMasterScheduleRawData() == null )
                return;
            Random random = new Random();

            Farmer farmerInstance = Game1.player;
            NetStringDictionary<Friendship, NetRef<Friendship>> friendshipData = farmerInstance.friendshipData;
            GameLocation __instanceLocation = __instance.currentLocation;


            // Every island visitor will be faster
            if ( __instanceLocation.Name != null && ( __instanceLocation.Name.Contains("Custom_MT_Island") && random.NextDouble() < 0.5
                || __instanceLocation.GetParentLocation() != null && __instanceLocation.GetParentLocation().Name == "Custom_MT_Island" ) )
            {
                if (__instance.Age == 0) __instance.addedSpeed = random.Next(0, 2);
                else if (__instance.Age == 1) __instance.addedSpeed = random.Next(1, 3);
                else if (__instance.Age == 2) __instance.addedSpeed = random.Next(1, 4);
            }

            // This will generate a random schedule for island visitor in the next Game time change 
            if (__instanceLocation.Name != null && random.NextDouble() < Config.IslandWalkAround && Game1.timeOfDay > 620 && __instance.timerSinceLastMovement > 10000 && Game1.timeOfDay % 20 == 0
                && ( __instance.temporaryController == null && __instance.controller == null && !__instance.isMoving() &&  __instance.TilePoint == __instance.previousEndPoint || __instance.timerSinceLastMovement > 20000 )
                && ( __instanceLocation.Name.Contains("Custom_MT_Island") || __instanceLocation.GetParentLocation() != null && __instanceLocation.GetParentLocation().Name == "Custom_MT_Island")
                && ( !IsFestivalToday || Game1.timeOfDay > Config.FestivalTimeEnd + 100 || Game1.timeOfDay < Config.FestivalTimeStart - 130)
                 )
            {
                // Reset stuck NPC
                if (__instance.temporaryController != null || __instance.controller != null || __instance.isMoving() 
                    || __instance.TilePoint != __instance.previousEndPoint || __instance.queuedSchedulePaths != null || __instance.queuedSchedulePaths.Count > 0)  
                    ResetErrorNpc(__instance);
                
                double nextVisitChance = random.NextDouble();

                float islandChance = 1 - Config.VisitChanceIslandHouse - Config.VisitChanceIslandBuilding;
                float islandHouseChance = 1 - Config.VisitChanceIslandBuilding;

                // if npc is on the island and not shop owner
                if (__instanceLocation.Name == "Custom_MT_Island" && __instance.modData["hapyke.FoodStore/shopOwnerToday"] == "-1,-1")
                {
                    if (nextVisitChance < islandChance) // walk around on the island
                    {
                        var randomTile = FarmOutside.getRandomOpenPointInFarm(__instance, __instanceLocation, false).ToVector2();
                        if (randomTile != Vector2.Zero)
                        {
                            FarmOutside.AddRandomSchedulePoint(__instance, $"{ConvertToHour(Game1.timeOfDay + 10)}", $"{__instanceLocation.NameOrUniqueName}",
                                $"{randomTile.X}", $"{randomTile.Y}", $"{random.Next(0, 4)}");
                        }
                    }
                    else if (nextVisitChance < islandHouseChance && Game1.timeOfDay >= 700) // visit island house
                    {
                        var randomTile = FarmOutside.getRandomOpenPointInFarm(__instance, Game1.getLocationFromName("Custom_MT_Island_House"), false).ToVector2();
                        if (randomTile != Vector2.Zero)
                        {
                            FarmOutside.AddRandomSchedulePoint(__instance, $"{ConvertToHour(Game1.timeOfDay + 10)}", $"{Game1.getLocationFromName("Custom_MT_Island_House").NameOrUniqueName}",
                                $"{randomTile.X}", $"{randomTile.Y}", $"{random.Next(0, 4)}");
                        }
                    }
                    else if (Game1.timeOfDay >= 700) // visit a selected building
                    {
                        var selectedBuilding = IslandValidBuilding[random.Next(IslandValidBuilding.Count)].buildingLocation;
                        if (selectedBuilding != null && selectedBuilding != "Custom_MT_Island_House")
                        {
                            var randomTile = FarmOutside.getRandomOpenPointInFarm(__instance, Game1.getLocationFromName(selectedBuilding), false).ToVector2();
                            if (randomTile != Vector2.Zero)
                            {
                                FarmOutside.AddRandomSchedulePoint(__instance, $"{ConvertToHour(Game1.timeOfDay + 10)}", $"{Game1.getLocationFromName(selectedBuilding).NameOrUniqueName}",
                                    $"{randomTile.X}", $"{randomTile.Y}", $"{random.Next(0, 4)}");
                            }
                        }
                    }
                }
                // if npc is inside island house
                else if (__instanceLocation.Name == "Custom_MT_Island_House")
                {
                    if (nextVisitChance < 0.2)
                    {
                        var randomTile = FarmOutside.getRandomOpenPointInFarm(__instance, Game1.getLocationFromName("Custom_MT_Island"), false).ToVector2();
                        if (randomTile != Vector2.Zero)
                        {
                            FarmOutside.AddRandomSchedulePoint(__instance, $"{ConvertToHour(Game1.timeOfDay + 10)}", $"{Game1.getLocationFromName("Custom_MT_Island").NameOrUniqueName}",
                                $"{randomTile.X}", $"{randomTile.Y}", $"{random.Next(0, 4)}");
                        }
                    }
                    else
                    {
                        var randomTile = FarmOutside.getRandomOpenPointInFarm(__instance, __instanceLocation, false).ToVector2();
                        if (randomTile != Vector2.Zero)
                        {
                            FarmOutside.AddRandomSchedulePoint(__instance, $"{ConvertToHour(Game1.timeOfDay + 10)}", $"{__instanceLocation.NameOrUniqueName}",
                                $"{randomTile.X}", $"{randomTile.Y}", $"{random.Next(0, 4)}");
                        }
                    }
                }
                // if npc is in a building
                else if (__instanceLocation.parentLocationName.Value == "Custom_MT_Island")
                {
                    if (Game1.player.currentLocation != __instanceLocation && nextVisitChance < 0.2)
                    {
                        IslandBuildingProperties matchingBuilding = IslandValidBuilding.FirstOrDefault(building => building.buildingLocation == __instanceLocation.NameOrUniqueName);
                        if (matchingBuilding != null)
                        {
                            Game1.warpCharacter(__instance, Game1.getLocationFromName("Custom_MT_Island"), matchingBuilding.outdoorDoor);
                            ResetErrorNpc(__instance);
                            var randomTile = FarmOutside.getRandomOpenPointInFarm(__instance, Game1.getLocationFromName("Custom_MT_Island"), false).ToVector2();
                            if (randomTile != Vector2.Zero)
                            {
                                FarmOutside.AddRandomSchedulePoint(__instance, $"{ConvertToHour(Game1.timeOfDay + 10)}", $"{Game1.getLocationFromName("Custom_MT_Island").NameOrUniqueName}",
                                    $"{randomTile.X}", $"{randomTile.Y}", $"{random.Next(0, 4)}");
                            }
                        }
                    }
                    else if ( Game1.player.currentLocation == __instanceLocation)
                    {
                        var randomTile = FarmOutside.getRandomOpenPointInFarm(__instance, __instanceLocation, false).ToVector2();
                        if (randomTile != Vector2.Zero)
                        {
                            FarmOutside.AddRandomSchedulePoint(__instance, $"{ConvertToHour(Game1.timeOfDay + 10)}", $"{__instanceLocation.NameOrUniqueName}",
                                $"{randomTile.X}", $"{randomTile.Y}", $"{random.Next(0, 4)}");
                        }
                    }
                }
            }

            // for invited visitor
            if ((__instanceLocation.Name == "Farm" || __instanceLocation.Name == "FarmHouse") && __instance.modData["hapyke.FoodStore/invited"] == "true" && __instance.modData["hapyke.FoodStore/inviteDate"] == (Game1.stats.DaysPlayed - 1).ToString()
                && !__instance.isMoving() && __instance.controller == null && __instance.temporaryController == null && __instance.timerSinceLastMovement >= 3000)
            {
                var randomTile = FarmOutside.getRandomOpenPointInFarm(__instance, __instanceLocation, true).ToVector2();
                if (randomTile != Vector2.Zero)
                {
                    FarmOutside.AddRandomSchedulePoint(__instance, $"{ConvertToHour(Game1.timeOfDay + 10)}", $"{__instanceLocation.NameOrUniqueName}",
                        $"{randomTile.X}", $"{randomTile.Y}", $"{random.Next(0, 4)}");
                }
            }
            
            // add extra dialogue
            if (random.NextDouble() < 0.5 && Game1.hasLoadedGame && __instanceLocation == Game1.player.currentLocation
                && ((friendshipData.TryGetValue(__instance.Name, out var friendship) && !__instance.Name.Contains("MT.Guest_") && friendshipData[__instance.Name].TalkedToToday) || __instance.Name.Contains("MT.Guest_"))
                && __instance.CurrentDialogue.Count == 0 && __instance.Name != "Krobus" && __instance.Name != "Dwarf"
                && !(Game1.currentLocation == null
                    || Game1.eventUp
                    || Game1.isFestival()
                    || Game1.IsFading()
                    || Game1.activeClickableMenu != null))
            {
                try
                {
                    int randomIndex = random.Next(1, 8);

                    string __instanceAge, __instanceManner, __instanceSocial, __instanceHeartLevel;

                    int age = __instance.Age;
                    int manner = __instance.Manners;
                    int social = __instance.SocialAnxiety;
                    int heartLevel = 0;
                    if (Game1.player.friendshipData.ContainsKey(__instance.Name)) heartLevel = (int)Game1.player.friendshipData[__instance.Name].Points / 250;

                    switch (age)
                    {
                        case 0:
                            __instanceAge = "adult.";
                            break;
                        case 1:
                            __instanceAge = "teens.";
                            break;
                        case 2:
                            __instanceAge = "child.";
                            break;
                        default:
                            __instanceAge = "adult.";
                            break;
                    }
                    switch (manner)
                    {
                        case 0:
                            __instanceManner = "neutral.";
                            break;
                        case 1:
                            __instanceManner = "polite.";
                            break;
                        case 2:
                            __instanceManner = "rude.";
                            break;
                        default:
                            __instanceManner = "neutral.";
                            break;
                    }
                    switch (social)
                    {
                        case 0:
                            __instanceSocial = "outgoing.";
                            break;
                        case 1:
                            __instanceSocial = "shy.";
                            break;
                        case 2:
                            __instanceSocial = "neutral.";
                            break;
                        default:
                            __instanceSocial = "neutral.";
                            break;
                    }
                    switch (heartLevel)
                    {
                        case 0:
                        case 1:
                        case 2:
                            __instanceHeartLevel = ".0";
                            break;
                        case 3:
                        case 4:
                        case 5:
                            __instanceHeartLevel = ".3";
                            break;
                        default:
                            __instanceHeartLevel = ".6";
                            break;
                    }

                    if (__instance.Name.Contains("MT.Guest_") || !Game1.player.friendshipData[__instance.Name].IsMarried() && !Config.DisableChatAll && Int32.Parse(__instance.modData["hapyke.FoodStore/TotalCustomerResponse"]) < 2
                       )
                    {
                        __instance.CurrentDialogue.Push(new Dialogue(__instance, "key", SHelper.Translation.Get("foodstore.general." + __instanceAge + __instanceManner + __instanceSocial + randomIndex.ToString() + __instanceHeartLevel)));
                        __instance.modData["hapyke.FoodStore/TotalCustomerResponse"] = (Int32.Parse(__instance.modData["hapyke.FoodStore/TotalCustomerResponse"]) + 1).ToString();
                    }
                }
                catch (NullReferenceException) { }
            }

            //======================================================================================================================================
            // Farm building store's visitors moving logic
            try
            {
                // leave the NPC when time up
                if (__instance.currentLocation.GetParentLocation() != null && __instance.currentLocation.GetParentLocation().IsFarm 
                    && !bool.Parse(__instance.modData["hapyke.FoodStore/gettingFood"]) && __instance.modData["hapyke.FoodStore/timeVisitShed"] != "0"
                    && (Int32.Parse(__instance.modData["hapyke.FoodStore/timeVisitShed"]) <= (Game1.timeOfDay - Config.TimeStay) || Game1.timeOfDay > Config.CloseHour || Game1.timeOfDay >= 2500))
                {
                    ResetErrorNpc(__instance);
                    __instance.TryLoadSchedule();

                    var schedule = __instance.Schedule;
                    var lastLocation = __instance.DefaultMap;
                    var lastPoint = __instance.DefaultPosition;
                    var lastDirection = __instance.DefaultFacingDirection;

                    // Get the tile location where NPC should be at the current time
                    if (schedule != null && schedule.Count > 0)
                    {
                        foreach (var piece in schedule)
                        {
                            if (piece.Key > Game1.timeOfDay)
                            {
                                break;
                            }

                            SchedulePathDescription description = piece.Value;
                            lastLocation = description.targetLocationName;
                            lastPoint = description.targetTile.ToVector2();
                            lastDirection = description.facingDirection;
                        }
                    }
                    else Game1.warpCharacter(__instance, lastLocation, lastPoint);

                    if (lastLocation != Game1.player.currentLocation.Name)
                    {
                        if (Int32.Parse(__instance.modData["hapyke.FoodStore/timeVisitShed"]) <= (Game1.timeOfDay - Config.TimeStay * 2) || Game1.timeOfDay - 100 >= Config.CloseHour)                 // Force Remove
                        {
                            Game1.warpCharacter(__instance, lastLocation, lastPoint);
                            __instance.faceDirection(lastDirection);
                            ResetErrorNpc(__instance);
                            __instance.TryLoadSchedule();
                        }
                        else if (__instance.modData["hapyke.FoodStore/shedEntry"] != "-1,-1" && __instance.modData["hapyke.FoodStore/shedEntry"] != null)        // Walk to Remove
                        {
                            ResetErrorNpc(__instance);
                            string[] coordinates = __instance.modData["hapyke.FoodStore/shedEntry"].Split(',');

                            var shedEntryPoint = Point.Zero;
                            if (coordinates.Length == 2 && int.TryParse(coordinates[0], out int x) && int.TryParse(coordinates[1], out int y))
                            {
                                shedEntryPoint = new Point(x, y);
                            }
                            __instance.temporaryController = new PathFindController(__instance, __instanceLocation, shedEntryPoint, 0,
                                (character, location) =>
                                {
                                    Game1.warpCharacter(__instance, lastLocation, lastPoint);
                                    __instance.faceDirection(lastDirection);
                                    ResetErrorNpc(__instance);
                                    __instance.TryLoadSchedule();
                                } );
                        }
                        else
                        {
                            Game1.warpCharacter(__instance, lastLocation, lastPoint);

                            __instance.faceDirection(lastDirection);
                            ResetErrorNpc(__instance);
                            __instance.TryLoadSchedule();
                        }
                    }
                    else ResetErrorNpc(__instance);

                    // remove special order tile
                    string[] specialOrderCoor = __instance.modData["hapyke.FoodStore/specialOrder"].Split(',');
                    Vector2 specialOrderTile = new Vector2(int.Parse(specialOrderCoor[0]), int.Parse(specialOrderCoor[1]));

                    if (RestaurantSpot.ContainsKey(__instanceLocation))
                    {
                        var tileList = RestaurantSpot[__instanceLocation];
                        if (tileList.Contains(specialOrderTile)) tileList.Remove(specialOrderTile); 
                    }

                    __instance.modData["hapyke.FoodStore/specialOrder"] = "-1,-1";
                }
                // else random move around
                else if (__instanceLocation.GetParentLocation() != null && __instanceLocation.GetParentLocation().IsFarm 
                    && __instance.modData["hapyke.FoodStore/specialOrder"] == "-1,-1" && __instance.modData["hapyke.FoodStore/invited"] == "false"
                    && !__instance.isMoving() && __instance.controller == null && __instance.temporaryController == null && __instance.timerSinceLastMovement >= 3000
                    && (Game1.player.friendshipData.ContainsKey(__instance.Name) && !Game1.player.friendshipData[__instance.Name].IsMarried() && !Game1.player.friendshipData[__instance.Name].IsRoommate() || !Game1.player.friendshipData.ContainsKey(__instance.Name)))
                {
                    var randomTile = FarmOutside.getRandomOpenPointInFarm(__instance, __instanceLocation, true, true).ToVector2();
                    if (randomTile != Vector2.Zero)
                    {
                        //SMonitor.Log("Trying to add schedule. SpaceCore might show warning message but it should not cause issues", LogLevel.Debug);
                        FarmOutside.AddRandomSchedulePoint(__instance, $"{ConvertToHour(Game1.timeOfDay + 10)}", $"{__instanceLocation.NameOrUniqueName}",
                            $"{randomTile.X}", $"{randomTile.Y}", $"{random.Next(0, 4)}");
                    }
                }
            }
            catch { }

            try             //Warp invited guest __instance to and away
            {
                if (!Utility.isFestivalDay() && __instance.modData["hapyke.FoodStore/inviteDate"] == (Game1.stats.DaysPlayed - 1).ToString())
                {
                    Random rand = new Random();
                    int index = rand.Next(7);
                    if (__instance.modData["hapyke.FoodStore/invited"] == "true" && Game1.timeOfDay == Config.InviteComeTime && __instanceLocation.Name != "Farm" && __instanceLocation.Name != "FarmHouse")
                    {
                        Game1.DrawDialogue(new Dialogue(__instance, "key", SHelper.Translation.Get("foodstore.visitcome." + index)));
                        Game1.globalFadeToBlack();

                        FarmOutside.UpdateRandomLocationOpenTile(Game1.getFarm());
                        FarmOutside.UpdateRandomLocationOpenTile(Game1.getLocationFromName("FarmHouse"));

                        var door = Game1.getFarm().GetMainFarmHouseEntry();
                        door.X += 3 - index;
                        door.Y += 2;
                        var name = "Farm";

                        Game1.warpCharacter(__instance, name, door);
                        ResetErrorNpc(__instance);

                        __instance.faceDirection(2);

                        door.X--;
                        __instance.controller = new PathFindController(__instance, Game1.getFarm(), door, 2);

                    }

                    if (__instance.modData["hapyke.FoodStore/invited"] == "true" && (__instanceLocation.Name == "Farm" || __instanceLocation.Name == "FarmHouse")
                        && (Game1.timeOfDay == Config.InviteLeaveTime || Game1.timeOfDay == Config.InviteLeaveTime + 30 || Game1.timeOfDay == Config.InviteLeaveTime + 100 || Game1.timeOfDay == Config.InviteLeaveTime + 130))
                    {
                        Game1.DrawDialogue(new Dialogue(__instance, "key", SHelper.Translation.Get("foodstore.visitleave." + index)));
                        Game1.globalFadeToBlack();

                        __instance.modData["hapyke.FoodStore/invited"] = "false";
                        ResetErrorNpc(__instance);
                        Game1.warpCharacter(__instance, __instance.DefaultMap, __instance.DefaultPosition / 64);
                        ResetErrorNpc(__instance);
                    }
                }
            }
            catch { }

            //Get taste and decoration score, call to SaySomething for __instance to send bubble text
            if (random.NextDouble() < 0.033 && !Game1.eventUp && __instanceLocation is not null && !WantsToEat(__instance)
                && Microsoft.Xna.Framework.Vector2.Distance(__instance.Tile, Game1.player.Tile) < 15
                && __instance.modData["hapyke.FoodStore/LastFoodTaste"] != "-1" && Config.EnableDecor && !Config.DisableChatAll)
            {
                int randomIndex = random.Next(8);
                double shareIdea = random.NextDouble();

                //Get Taste score, Decoration score
                int lastTaste;
                if (__instance.modData.ContainsKey("hapyke.FoodStore/LastFoodTaste")) lastTaste = Int32.Parse(__instance.modData["hapyke.FoodStore/LastFoodTaste"]);
                else lastTaste = 8;

                double lastDecor;
                if (__instance.modData.ContainsKey("hapyke.FoodStore/LastFoodDecor")) lastDecor = Convert.ToDouble(__instance.modData["hapyke.FoodStore/LastFoodDecor"]);
                else lastDecor = 0;

                double lastTasteRate; // Variable to store the result
                switch (lastTaste)
                {
                    case 0:
                        lastTasteRate = 0.4;
                        break;

                    case 2:
                        lastTasteRate = 0.35;
                        break;

                    case 4:
                        lastTasteRate = 0.25;
                        break;

                    case 6:
                        lastTasteRate = 0.2;
                        break;

                    default:
                        lastTasteRate = 0.3;
                        break;
                }

                double lastDecorRate; // Variable to store the result
                switch (lastDecor)
                {
                    case double n when n >= -0.2 && n < 0:
                        lastDecorRate = -0.2;
                        break;

                    case double n when n >= 0 && n < 0.2:
                        lastDecorRate = 0;
                        break;

                    default:
                        lastDecorRate = 0.2;
                        break;
                }

                if (lastTaste == 0) //love
                {
                    NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.randomchat.love." + randomIndex));
                    if (shareIdea < 0.3 + (lastDecor / 2)) SaySomething(__instance, __instanceLocation, lastTasteRate, lastDecorRate);
                }
                else if (lastTaste == 2) //like
                {
                    NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.randomchat.like." + randomIndex));
                    if (shareIdea < 0.15 + (lastDecor / 2)) SaySomething(__instance, __instanceLocation, lastTasteRate, lastDecorRate);
                }
                else if (lastTaste == 4) //dislike
                {
                    NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.randomchat.dislike." + randomIndex));
                    if (shareIdea < Math.Abs(-0.15 + (lastDecor / 2.5))) SaySomething(__instance, __instanceLocation, lastTasteRate, lastDecorRate);
                }
                else if (lastTaste == 6) //hate
                {
                    NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.randomchat.hate." + randomIndex));
                    if (shareIdea < Math.Abs(-0.3 + (lastDecor / 2.5))) SaySomething(__instance, __instanceLocation, lastTasteRate, lastDecorRate);
                }
                else if (lastTaste == 8) //neutral
                {
                    NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.randomchat.neutral." + randomIndex));
                    if (shareIdea < Math.Abs(lastDecor / 2.5)) SaySomething(__instance, __instanceLocation, lastTasteRate, lastDecorRate);
                }
                else { }
            }


            //Fix position, do eating food
            if (Game1.eventUp || __instance is null || __instanceLocation is null || !__instance.IsVillager || !WantsToEat(__instance))
                return;

            // ****************************************************************************************************************************
            if ( __instance.currentLocation.Name != __instance.DefaultMap
                && (__instance.Tile.X > __instanceLocation.Map.Layers[0].LayerWidth
                || __instance.Tile.Y > __instanceLocation.Map.Layers[0].LayerHeight
                || __instance.Tile.X < 0
                || __instance.Tile.Y < 0)
                )
            {
                SMonitor.Log("NPC OFF MAP " + __instance.Name + __instance.Tile + __instanceLocation.Name, LogLevel.Warn);
                //if (__instance.Tile.X > 150) Game1.currentLocation.ShowAnimalShopMenu();

            }

            foreach (var pair in validBuildingObjectPairs)
            {
                Building building = pair.Building;
                string buildingType = pair.buildingType;

                var museumCheck = Game1.getLocationFromName(building.GetIndoorsName());

                if (museumCheck == __instanceLocation && buildingType == "museum") return;
            }

            DataPlacedFood food = GetClosestFood(__instance, __instanceLocation);
            TryToEatFood(__instance, food);
        }

        private static void FarmHouse_updateEvenIfFarmerIsntHere_Postfix(GameLocation __instance)
        {
            if (!Game1.hasLoadedGame) return;

            Random random = new Random();
            foreach (NPC npc in __instance.characters)
            {
                double talkChance = 0.00002;

                //Send bubble about decoration, dish of the week
                if (npc.IsVillager && npc.getMasterScheduleRawData() != null && __instance == Game1.player.currentLocation
                    && random.NextDouble() < talkChance
                    && WantsToSay(npc, 360)
                    && Utility.isThereAFarmerWithinDistance(new Microsoft.Xna.Framework.Vector2(npc.Tile.X, npc.Tile.Y), 20, npc.currentLocation) != null
                    && Config.EnableDecor
                    && !Config.DisableChatAll)
                {
                    DataPlacedFood tempFood = GetClosestFood(npc, npc.currentLocation);

                    int localNpcCount = 2;
                    if (Utility.isThereAFarmerOrCharacterWithinDistance(new Microsoft.Xna.Framework.Vector2(npc.Tile.X, npc.Tile.Y), 10, npc.currentLocation) != null) localNpcCount += 1;

                    int randomIndex = random.Next(5);
                    if (tempFood != null)       //If have item for sale
                    {
                        var decorPointComment = GetDecorPoint(tempFood.foodTile, npc.currentLocation);


                        //Send decorPoint message

                        if (decorPointComment >= 0.2)
                        {
                            NPCShowTextAboveHead(npc, SHelper.Translation.Get("foodstore.gooddecor." + randomIndex.ToString()));
                            npc.modData["hapyke.FoodStore/LastSay"] = Game1.timeOfDay.ToString();
                            continue;
                        }
                        else if (decorPointComment <= 0)
                        {
                            NPCShowTextAboveHead(npc, SHelper.Translation.Get("foodstore.baddecor." + randomIndex.ToString()));
                            npc.modData["hapyke.FoodStore/LastSay"] = Game1.timeOfDay.ToString();
                            continue;
                        }
                    }
                    else if (tempFood == null && npc.currentLocation is FarmHouse)      //if in FarmHouse and have no item for sale
                    {
                        var decorPointComment = GetDecorPoint(npc.Tile, npc.currentLocation);


                        //Send decorPoint message

                        if (decorPointComment >= 0.2)
                        {
                            NPCShowTextAboveHead(npc, SHelper.Translation.Get("foodstore.gooddecor." + randomIndex.ToString()));
                            npc.modData["hapyke.FoodStore/LastSay"] = Game1.timeOfDay.ToString();
                            continue;
                        }
                        else if (decorPointComment <= 0)
                        {
                            NPCShowTextAboveHead(npc, SHelper.Translation.Get("foodstore.baddecor." + randomIndex.ToString()));
                            npc.modData["hapyke.FoodStore/LastSay"] = Game1.timeOfDay.ToString();
                            continue;
                        }
                    }

                    if (random.NextDouble() < (talkChance / localNpcCount / 2))            //Send Dish of Week message
                    {
                        NPCShowTextAboveHead(npc, SHelper.Translation.Get("foodstore.dishweek." + randomIndex.ToString(), new { dishWeek = DishPrefer.dishWeek }));
                        npc.modData["hapyke.FoodStore/LastSay"] = Game1.timeOfDay.ToString();
                    }
                }


                //Control NPC walking to the food
                string text = "";
                if ( npc.IsVillager && npc.getMasterScheduleRawData() != null && npc.queuedSchedulePaths.Count == 0
                    && ( !npc.modData.ContainsKey("hapyke.FoodStore/shopOwnerToday") || npc.modData["hapyke.FoodStore/shopOwnerToday"] == "-1,-1" ) )
                {
                    double moveToFoodChance = Config.MoveToFoodChance;
                    try
                    {
                        if (npc.currentLocation.Name == "Custom_MT_Island" || npc.currentLocation.GetParentLocation() != null && npc.currentLocation.GetParentLocation().Name == "Custom_MT_Island")
                            moveToFoodChance = Config.MoveToFoodChance * 2;
                        else if (npc.currentLocation.Name == "Custom_MT_Island_House") moveToFoodChance = Config.MoveToFoodChance * 3;
                        else if (npc.currentLocation.GetParentLocation() is Farm) moveToFoodChance = Config.ShedMoveToFoodChance;
                    }
                    catch { }

                    if (Config.RushHour && ((800 < Game1.timeOfDay && Game1.timeOfDay < 930) || (1200 < Game1.timeOfDay && Game1.timeOfDay < 1300) || (1800 < Game1.timeOfDay && Game1.timeOfDay < 2000)))
                    {
                        moveToFoodChance = moveToFoodChance * 1.5;
                    }

                    try
                    {
                        if (npc != null && WantsToEat(npc) && Game1.random.NextDouble() < moveToFoodChance / 100f )
                        {
                            DataPlacedFood food = GetClosestFood(npc, __instance);

                            foreach (var pair in validBuildingObjectPairs)
                            {
                                Building building = pair.Building;
                                string buildingType = pair.buildingType;

                                var museumCheck = Game1.getLocationFromName(building.GetIndoorsName());

                                if (museumCheck == npc.currentLocation && buildingType == "museum") return;
                            }

                            if (food == null || (!Config.AllowRemoveNonFood && food.foodObject.Edibility <= 0 && (npc.currentLocation is Farm || npc.currentLocation is FarmHouse)))
                                return;
                            if (TryToEatFood(npc, food))
                            {
                                return;
                            }

                            Microsoft.Xna.Framework.Vector2 possibleLocation;
                            possibleLocation = food.foodTile;
                            int tries = 0;
                            int facingDirection = -3;


                            while (tries < 3)
                            {
                                int xMove = Game1.random.Next(-1, 2);
                                int yMove = Game1.random.Next(-1, 2);

                                possibleLocation.X += xMove;
                                if (xMove == 0)
                                {
                                    possibleLocation.Y += yMove;
                                }
                                if (xMove == -1)
                                {
                                    facingDirection = 1;
                                }
                                else if (xMove == 1)
                                {
                                    facingDirection = 3;
                                }
                                else if (yMove == -1)
                                {
                                    facingDirection = 2;
                                }
                                else if (yMove == 1)
                                {
                                    facingDirection = 0;
                                }
                                if (!__instance.IsTileBlockedBy(possibleLocation))
                                {
                                    break;
                                }
                                tries++;
                            }
                            if (tries < 3 && TimeDelayCheck(npc))
                            {
                                //Send message
                                if (npc.currentLocation.Name != "Farm" && npc.currentLocation.Name != "FarmHouse" && !Config.DisableChat && !Config.DisableChatAll)
                                {
                                    int randomIndex = random.Next(15);
                                    text = SHelper.Translation.Get("foodstore.coming." + randomIndex.ToString(), new { vName = npc.displayName });

                                    Game1.chatBox.addInfoMessage(text);
                                    MyMessage messageToSend = new MyMessage(text);
                                    SHelper.Multiplayer.SendMessage(messageToSend, "ExampleMessageType");
                                }

                                npc.modData["hapyke.FoodStore/LastCheck"] = Game1.timeOfDay.ToString();
                                npc.modData["hapyke.FoodStore/gettingFood"] = "true";

                                //Villager control

                                var npcWalk = FarmOutside.AddRandomSchedule(npc, ConvertToHour(Game1.timeOfDay + 10).ToString(), __instance.NameOrUniqueName, 
                                    possibleLocation.X.ToString(), possibleLocation.Y.ToString(), facingDirection.ToString());

                                npc.addedSpeed = 2;
                            }
                        }
                    }
                    catch { }
                }
            }
        }


        // NPC order part 

        [HarmonyPatch(typeof(NPC), nameof(NPC.draw))]
        public class NPC_draw_Patch
        {
            private static int emoteBaseIndex = 424242;

            public static void Prefix(NPC __instance, ref bool __state)
            {
                if (!__instance.IsEmoting || __instance.CurrentEmote != emoteBaseIndex)
                    return;
                __state = true;
                __instance.IsEmoting = false;
            }

            public static void Postfix(NPC __instance, SpriteBatch b, float alpha, ref bool __state)
            {
                if (!__state)
                    return;
                __instance.IsEmoting = true;
                if (!__instance.modData.TryGetValue(orderKey, out string data))
                    return;
                if (!Config.RestaurantLocations.Contains(__instance.currentLocation.Name))
                {
                    __instance.modData.Remove(orderKey);
                    return;
                }
                DataOrder orderData = JsonConvert.DeserializeObject<DataOrder>(data);
                int emoteIndex = __instance.CurrentEmoteIndex >= emoteBaseIndex ? __instance.CurrentEmoteIndex - emoteBaseIndex : __instance.CurrentEmoteIndex;
                if (__instance.CurrentEmoteIndex >= emoteBaseIndex + 3)
                {
                    AccessTools.Field(typeof(Character), "currentEmoteFrame").SetValue(__instance, emoteBaseIndex);
                }
                Microsoft.Xna.Framework.Vector2 emotePosition = __instance.getLocalPosition(Game1.viewport);
                emotePosition.Y -= 32 + __instance.Sprite.SpriteHeight * 4;
                if (SHelper.Input.IsDown(Config.ModKey))
                {
                    SpriteText.drawStringWithScrollCenteredAt(b, orderData.dishName, (int)emotePosition.X + 32, (int)emotePosition.Y, "", 1f, default, 1);
                }
                else
                {
                    b.Draw(emoteSprite, emotePosition, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(emoteIndex * 16 % Game1.emoteSpriteSheet.Width, emoteIndex * 16 / emoteSprite.Width * 16, 16, 16)), Color.White, 0f, Microsoft.Xna.Framework.Vector2.Zero, 4f, SpriteEffects.None, __instance.StandingPixel.Y / 10000f);
                    b.Draw(Game1.objectSpriteSheet, emotePosition + new Microsoft.Xna.Framework.Vector2(16, 8), GameLocation.getSourceRectForObject(orderData.dish), Color.White, 0f, Microsoft.Xna.Framework.Vector2.Zero, 2f, SpriteEffects.None, (__instance.StandingPixel.Y + 1) / 10000f);
                }

            }
        }

        [HarmonyPatch(typeof(Utility), nameof(Utility.checkForCharacterInteractionAtTile))]
        public class Utility_checkForCharacterInteractionAtTile_Patch
        {
            public static bool Prefix(Microsoft.Xna.Framework.Vector2 tileLocation, Farmer who)
            {
                NPC npc = Game1.currentLocation.isCharacterAtTile(tileLocation);
                if (npc is null || !npc.modData.TryGetValue(orderKey, out string data))
                    return true;
                if (!Config.RestaurantLocations.Contains(Game1.currentLocation.Name))
                {
                    npc.modData.Remove(orderKey);
                    return true;
                }
                DataOrder orderData = JsonConvert.DeserializeObject<DataOrder>(data);
                if (who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift() && who.ActiveObject.Name == orderData.dishName)
                {
                    Game1.mouseCursor = 6;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(NPC), nameof(NPC.tryToReceiveActiveObject))]
        public class NPC_tryToReceiveActiveObject_Patch
        {
            public static bool Prefix(NPC __instance, Farmer who)
            {
                if (!Config.RestaurantLocations.Contains(__instance.currentLocation.Name) || !__instance.modData.TryGetValue(orderKey, out string data))
                    return true;
                DataOrder orderData = JsonConvert.DeserializeObject<DataOrder>(data);
                if (who.ActiveObject?.ParentSheetIndex == orderData.dish)
                {
                    if (!npcOrderNumbers.Value.ContainsKey(__instance.Name))
                    {
                        npcOrderNumbers.Value[__instance.Name] = 1;
                    }
                    else
                    {
                        npcOrderNumbers.Value[__instance.Name]++;
                    }
                    List<string> possibleReactions = new();
                    if (orderData.loved == "love")
                    {
                        possibleReactions.Add(SHelper.Translation.Get("loved-order-reaction-1"));
                        possibleReactions.Add(SHelper.Translation.Get("loved-order-reaction-2"));
                        possibleReactions.Add(SHelper.Translation.Get("loved-order-reaction-3"));
                    }
                    else if (orderData.loved == "like")
                    {
                        possibleReactions.Add(SHelper.Translation.Get("liked-order-reaction-1"));
                        possibleReactions.Add(SHelper.Translation.Get("liked-order-reaction-2"));
                        possibleReactions.Add(SHelper.Translation.Get("liked-order-reaction-3"));
                    }
                    else
                    {
                        possibleReactions.Add(SHelper.Translation.Get("neutral-order-reaction-1"));
                        possibleReactions.Add(SHelper.Translation.Get("neutral-order-reaction-2"));
                        possibleReactions.Add(SHelper.Translation.Get("neutral-order-reaction-3"));
                    }
                    string reaction = possibleReactions[Game1.random.Next(possibleReactions.Count)];

                    switch (who.FacingDirection)
                    {
                        case 0:
                            ((FarmerSprite)who.Sprite).animateBackwardsOnce(80, 50f);
                            break;
                        case 1:
                            ((FarmerSprite)who.Sprite).animateBackwardsOnce(72, 50f);
                            break;
                        case 2:
                            ((FarmerSprite)who.Sprite).animateBackwardsOnce(64, 50f);
                            break;
                        case 3:
                            ((FarmerSprite)who.Sprite).animateBackwardsOnce(88, 50f);
                            break;
                    }

                    if (Config.PriceMarkup > 0)
                    {
                        int price = (int)Math.Round(who.ActiveObject.Price * Config.PriceMarkup);
                        AddToPlayerFunds(price);
                    }

                    who.reduceActiveItemByOne();
                    who.completelyStopAnimatingOrDoingAction();
                    Game1.DrawDialogue(new Dialogue(__instance, "key", reaction));
                    __instance.faceTowardFarmerForPeriod(2000, 3, false, who);
                    __instance.modData.Remove(orderKey);
                    return false;
                }
                return true;
            }
            public static void Postfix(ref bool __result, NPC __instance, Farmer who, bool probe)
            {
                try
                {
                    if (who.ActiveItem != null && (who.ActiveItem.Name == "Invite Letter" || who.ActiveItem.Name == "Customer Note"))
                    {
                        if (!probe)
                        {
                            var pc = new PlayerChat();

                            if (who.ActiveItem.Name == "Invite Letter")
                            {
                                pc.OnPlayerSend(__instance, "invite");
                                __instance.receiveGift(who.ActiveObject, who, false, 0f, false);
                                who.reduceActiveItemByOne();
                                who.completelyStopAnimatingOrDoingAction();
                                __instance.faceTowardFarmerForPeriod(4000, 3, faceAway: false, who);

                                __result = true;
                            }
                            if (who.ActiveItem.Name == "Customer Note")
                            {
                                if (UpdateCustomerNote(__instance, who))
                                {
                                    __instance.receiveGift(who.ActiveObject, who, false, 0f, false);
                                    who.reduceActiveItemByOne();
                                    who.completelyStopAnimatingOrDoingAction();
                                    __instance.faceTowardFarmerForPeriod(4000, 3, faceAway: false, who);
                                }
                                __result = true;
                            }

                        }
                    }
                }
                catch { }
            }

            public static bool UpdateCustomerNote(NPC __instance, Farmer who)
            {
                Random random = new Random();

                //Get Taste score, Decoration score
                int lastTaste = -1;
                if (__instance.modData.ContainsKey("hapyke.FoodStore/LastFoodTaste")) lastTaste = Int32.Parse(__instance.modData["hapyke.FoodStore/LastFoodTaste"]);


                double lastDecor = -1;
                if (__instance.modData.ContainsKey("hapyke.FoodStore/LastFoodDecor")) lastDecor = Convert.ToDouble(__instance.modData["hapyke.FoodStore/LastFoodDecor"]);

                if (lastTaste == -1 || lastDecor == -1) // have not buy anything yet
                {
                    NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.asktaste.empty." + random.Next(3)));
                    return true;
                }
                double lastTasteRate; // get the taste point
                switch (lastTaste)
                {
                    case 0:
                        lastTasteRate = 0.4;
                        break;

                    case 2:
                        lastTasteRate = 0.35;
                        break;

                    case 4:
                        lastTasteRate = 0.25;
                        break;

                    case 6:
                        lastTasteRate = 0.2;
                        break;

                    default:
                        lastTasteRate = 0.3;
                        break;
                }

                double lastDecorRate; // get the decor point
                switch (lastDecor)
                {
                    case double n when n >= -0.2 && n < 0:
                        lastDecorRate = -0.2;
                        break;

                    case double n when n >= 0 && n < 0.2:
                        lastDecorRate = 0;
                        break;

                    default:
                        lastDecorRate = 0.2;
                        break;
                }

                if (!TodayCustomerNoteName.Contains(__instance.Name) && (lastTasteRate == 0.3 && lastDecorRate > 0 || lastTasteRate > 0.3 && lastDecorRate >= 0))          // Normal food, good decor or like food, normal decor
                {
                    NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.customernote.yes." + random.Next(7).ToString()));

                    TodayCustomerNoteName.Add(__instance.Name);
                    TodayCustomerNoteYes++;
                    TodayCustomerNote++;
                    return true;
                }
                else if (!TodayCustomerNoteName.Contains(__instance.Name) && (lastTasteRate <= 0.25 || lastTasteRate == 0.3 && lastDecorRate < 0))     // Dishlike food, or neutral food and bad decor
                {
                    NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.customernote.no." + random.Next(7).ToString()));
                    TodayCustomerNoteName.Add(__instance.Name);
                    TodayCustomerNoteNo++;
                    TodayCustomerNote++;
                    return true;
                }
                else if (!TodayCustomerNoteName.Contains(__instance.Name))          // Other case
                {
                    NPCShowTextAboveHead(__instance, SHelper.Translation.Get("foodstore.randomchat.neutral." + random.Next(8)));
                    TodayCustomerNoteName.Add(__instance.Name);
                    TodayCustomerNoteNone++;
                    TodayCustomerNote++;
                    return true;

                }
                return false;
            }
        }

        [HarmonyPatch(typeof(FruitTree), nameof(FruitTree.dayUpdate))]      // Set Fruit tree grow
        public class FruitTree_Patch
        {
            static void Postfix(FruitTree __instance)
            {
                if (__instance != null && __instance.Location != null && __instance.Location.Name == "Custom_MT_Island"
                    && Config.IslandPlantBoost && Game1.random.NextDouble() < Config.IslandPlantBoostChance && __instance.growthStage.Value < 4)
                {
                    __instance.daysUntilMature.Value -= __instance.growthRate.Value;
                }
            }
        }

        [HarmonyPatch(typeof(FruitTree), nameof(FruitTree.TryAddFruit))]    // Set Fruit tree fruit
        public class FruitTree_TryAddFruit_Patch
        {
            static bool Prefix(FruitTree __instance, ref bool __result)
            {
                if (__instance != null && __instance.Location != null && __instance.Location.Name.Contains("Custom_MT_Island") && Config.IslandPlantBoost)
                {
                    if (!__instance.stump.Value && __instance.growthStage.Value >= 4 && __instance.fruit.Count < 8
                        && (__instance.IsInSeasonHere() || __instance.Location.GetSeason().ToString() == "Summer"
                        || (__instance.struckByLightningCountdown.Value > 0 && !__instance.IsWinterTreeHere())))
                    {
                        Random random = new Random();
                        FruitTreeData data = __instance.GetData();
                        if (data?.Fruit != null && (__instance.IsInSeasonHere() || !__instance.IsInSeasonHere() && random.NextDouble() < Config.IslandPlantBoostChance))
                        {
                            foreach (FruitTreeFruitData item2 in data.Fruit)
                            {
                                Item item = InvokeTryCreateFruit(__instance, item2);
                                if (item != null)
                                {
                                    __instance.fruit.Add(item);
                                    // Add a chance to add another fruit
                                    if (__instance.IsInSeasonHere() && random.NextDouble() < Config.IslandPlantBoostChance)
                                    {
                                        Item extraItem = InvokeTryCreateFruit(__instance, item2);
                                        if (extraItem != null)
                                            __instance.fruit.Add(extraItem);
                                    }

                                    __result = true;
                                    return false;
                                }
                            }
                        }
                    }
                    __result = false;
                    return false;
                }
                return true;
            }

            static Item InvokeTryCreateFruit(FruitTree tree, FruitTreeFruitData drop)
            {
                MethodInfo method = typeof(FruitTree).GetMethod("TryCreateFruit", BindingFlags.NonPublic | BindingFlags.Instance);
                return (Item)method.Invoke(tree, new object[] { drop });
            }
        }

        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.GetSeason))]    // Set 3 season
        public class GameLocation_GetSeason_Patch
        {
            static void Postfix(GameLocation __instance, ref Season __result)
            {
                if (__instance != null && __instance.Name != null && __instance.Name.Contains("Custom_MT_Island"))
                {
                    int customDay = (int)Game1.stats.DaysPlayed % 84;

                    if (1 <= customDay && customDay <= 28) __result = Season.Spring;
                    else if (29 <= customDay && customDay <= 56) __result = Season.Summer;
                    else __result = Season.Fall;
                }
            }
        }


        // Draw Festival item
        [HarmonyPatch(typeof(Chest))]
        [HarmonyPatch("draw")]
        [HarmonyPatch(new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) })]
        public class Postfix_draw
        {
            public static void Postfix(Chest __instance, SpriteBatch spriteBatch, int x, int y)
            {
                if (TodayShopInventory.Count == 0) return;
                if (__instance.Location.Name != "Custom_MT_Island" || __instance.TileLocation != new Vector2(63, 22)) return;
                foreach (var shop in TodayShopInventory)
                {
                    var tileLocation = shop.Tile;

                    var drawLayer = Math.Max(0f, (tileLocation.Y * Game1.tileSize - 24) / 10000f) + tileLocation.X * 1E-05f;
                    drawGrangeItems(tileLocation, spriteBatch, drawLayer, shop.ItemIds);
                }

            }
        }

        public static void drawGrangeItems(Vector2 tileLocation, SpriteBatch spriteBatch, float layerDepth, List<string> shopItem)
        {
            var start = Game1.GlobalToLocal(Game1.viewport, tileLocation * 64);

            start.X += 13f;
            start.Y -= 170f;

            float xStep = 0;
            float yStep = 0f;

            foreach (var itemId in shopItem)
            {
                if (itemId is null)
                {
                    xStep += 54f;
                    if (xStep > 108f)
                    {
                        xStep = 0f;
                        yStep += 55f;
                    }
                    continue;
                }
                // Draw shadow
                spriteBatch.Draw(Game1.shadowTexture, new Vector2(start.X + xStep + 10f, start.Y + yStep + 40f),
                    Game1.shadowTexture.Bounds, Color.Red, 0f,
                    Vector2.Zero, 3.2f, SpriteEffects.None, layerDepth + 0.01f);

                var item = ItemRegistry.GetDataOrErrorItem(itemId);

                float xModify = 0f;
                float yModify = 0f;

                int originalHeight = item.GetSourceRect().Height;
                int originalWidth = item.GetSourceRect().Width;

                int maxRectSize = 14;

                float scale = Math.Min((float)maxRectSize / originalHeight * 3.7f, (float)maxRectSize / originalWidth * 3.7f);


                if (itemId.Contains("(S)"))
                {
                    scale *= 0.75f;
                    xModify = 5f;
                    yModify = 10f;
                }
                spriteBatch.Draw(item.GetTexture(), new Vector2(start.X + xStep + xModify, start.Y + yStep + yModify),
                    item.GetSourceRect(), Color.White, 0f,
                    Vector2.Zero, scale, SpriteEffects.None, layerDepth + 0.02f);


                xStep += 54f;

                if (xStep > 108f)
                {
                    xStep = 0f;
                    yStep += 55f;
                }
            }
        }


        [HarmonyPatch(typeof(Building))]
        [HarmonyPatch("getPointForHumanDoor")]
        public class Building_GetPointForHumanDoor_Patch
        {
            static void Postfix(Building __instance, ref Point __result)
            {
                if (__instance.parentLocationName.Value == "Custom_MT_Island")
                {
                    // Modify the Y coordinate of the result
                    __result = new Point((int)__instance.tileX.Value + __instance.humanDoor.Value.X, (int)__instance.tileY.Value + __instance.humanDoor.Value.Y + 1);
                    __instance.GetParentLocation().setTileProperty(__result.X - 1, __result.Y, "Back", "NoPath", "T");
                    __instance.GetParentLocation().setTileProperty(__result.X + 1, __result.Y, "Back", "NoPath", "T");

                }
            }
        }
        public static void DrawAtNonTileSpot_Prefix(Furniture __instance, ref Vector2 location, float layerDepth, float alpha)
        {
            if (__instance != null && __instance.QualifiedItemId == "(F)MT.Objects.RestaurantDecor")
            {
                location.X -= 32;
                location.Y += 12;
            }
        }
        public static void LoadDescription_Postfix(Furniture __instance, ref string __result)
        {
            if (__instance != null && __instance.QualifiedItemId == "(F)MT.Objects.RestaurantDecor")
            {
                __result = "Customers would love to sit at the table that has this lovely decoration!";
            }
        }

        public static void GetSellPrice_Postfix(ref int __result, FarmAnimal __instance)
        {
            if (__instance.modData != null &&
                __instance.modData.TryGetValue("hapyke.FoodStore/isFakeAnimal", out string isFakeAnimal) &&
                isFakeAnimal != null &&
                isFakeAnimal == "true")
            {
                __result = 0;
            }
        }

        public static void GetCursorPetBoundingBox_Postfix(ref Microsoft.Xna.Framework.Rectangle __result, FarmAnimal __instance)
        {
            if (__instance.modData != null &&
                __instance.modData.TryGetValue("hapyke.FoodStore/isFakeAnimal", out string isFakeAnimal) &&
                isFakeAnimal != null &&
                isFakeAnimal == "true")
            {
                __result = Microsoft.Xna.Framework.Rectangle.Empty;
            }
        }

        public static void SetUpForReturnToIslandAfterLivestockPurchase(PurchaseAnimalsMenu __instance)
        {
            if (Game1.player.currentLocation.Name != "Custom_MT_Island") return;

            LocationRequest locationRequest = Game1.getLocationRequest("Custom_MT_Island");
            locationRequest.OnWarp += delegate
            {
                __instance.onFarm = false;
                Game1.player.viewingLocation.Value = null;
                Game1.displayHUD = true;
                Game1.viewportFreeze = false;
                __instance.namingAnimal = false;
                __instance.textBox.OnEnterPressed -= __instance.textBoxEvent;
                __instance.textBox.Selected = false;
                Game1.exitActiveMenu();
            };
            Game1.warpFarmer(locationRequest, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, Game1.player.FacingDirection);
        }
    }
}   