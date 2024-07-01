/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;
using System.Linq;
using AnimalsNeedWater.Types;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Pathfinding;
using StardewValley.Tools;
using xTile.Layers;
using xTile.Tiles;
// ReSharper disable InconsistentNaming

namespace AnimalsNeedWater.Patching
{
    public static class HarmonyPatchExecutors
    {
        public static void AnimalDayUpdateExecutor(ref FarmAnimal __instance, ref GameLocation environment)
        {
            if (__instance.home != null &&
                !((AnimalHouse) __instance.home.indoors.Value).animals.ContainsKey(__instance.myID.Value) &&
                environment is not AnimalHouse && !__instance.home.animalDoorOpen.Value) return;
            
            if (__instance.home != null && __instance.home.buildingType.Value.ToLower().Contains("coop"))
            {
                // check whether CoopsWithWateredTrough contains the coop the animal lives in and whether it was able to drink outside or not
                if (ModData.CoopsWithWateredTrough.Contains(__instance.home.indoors.Value.NameOrUniqueName.ToLower()) || ModData.FullAnimals.Contains(__instance))
                {
                    // increase friendship points if any of the conditions above is met
                    __instance.friendshipTowardFarmer.Value += Math.Abs(ModEntry.Instance.Config.FriendshipPointsForWateredTrough);
                }
            }
            else if (__instance.home != null && __instance.home.buildingType.Value.ToLower().Contains("barn"))
            {
                // check whether BarnsWithWateredTrough contains the coop the animal lives in and whether it was able to drink outside or not
                if (ModData.BarnsWithWateredTrough.Contains(__instance.home.indoors.Value.NameOrUniqueName.ToLower()) || ModData.FullAnimals.Contains(__instance))
                {
                    // increase friendship points if any of the conditions above is met
                    __instance.friendshipTowardFarmer.Value += Math.Abs(ModEntry.Instance.Config.FriendshipPointsForWateredTrough);
                }
            }
        }
        
        public static bool AnimalBehaviorsExecutor(ref bool __result, ref FarmAnimal __instance, ref GameTime time, ref GameLocation location)
        {
            if (__instance.home == null)
            {
                __result = false;
                return false;
            }
            if (!Game1.IsMasterGame) // do not run if not host
            {
                __result = false;
                return false;
            }
            if (__instance.controller != null) // do not run if has other pathfinding in progress
            {
                __result = true;
                return true;
            }
            
            if (ModEntry.Instance.Config.AnimalsCanDrinkOutside 
                && !__instance.isSwimming.Value 
                && location.IsOutdoors 
                && !ModData.FullAnimals.Contains(__instance) 
                && Game1.random.NextDouble() < 0.002 // set a random chance of 0.2% each frame to pathfind
                && FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
            {
                // pathfind to the closest water tile
                ++FarmAnimal.NumPathfindingThisTick;
                __instance.controller = new PathFindController(__instance, location, WaterEndPointFunction, -1, BehaviorAfterFindingWater, 200, Point.Zero);
            }

            __result = true;
            return true;
        }
        
        /// <summary> Search for water tiles. </summary>
        private static bool WaterEndPointFunction(
            PathNode currentPoint,
            Point endPoint,
            GameLocation location,
            Character c) 
        {
            if (!ModEntry.Instance.Config.AnimalsCanOnlyDrinkFromWaterBodies)
            {
                // check four adjacent tiles for wells, fish ponds, etc.
                return location.CanRefillWateringCanOnTile(currentPoint.x - 1, currentPoint.y) 
                       || location.CanRefillWateringCanOnTile(currentPoint.x, currentPoint.y - 1) 
                       || location.CanRefillWateringCanOnTile(currentPoint.x, currentPoint.y + 1) 
                       || location.CanRefillWateringCanOnTile(currentPoint.x + 1, currentPoint.y);
            }
            
            // check four adjacent tiles for open water (no wells, fish ponds, etc.)
            return location.isOpenWater(currentPoint.x - 1, currentPoint.y) 
                   || location.isOpenWater(currentPoint.x, currentPoint.y - 1) 
                   || location.isOpenWater(currentPoint.x, currentPoint.y + 1) 
                   || location.isOpenWater(currentPoint.x + 1, currentPoint.y);
        }

        /// <summary> Animal behavior after finding a water tile and pathfinding to it. </summary>
        private static void BehaviorAfterFindingWater(Character c, GameLocation environment)
        {
            // return if the animal is already on the list
            if (ModData.FullAnimals.Contains(c as FarmAnimal))
                return;
            
            // do the 'happy' emote and add the animal to the Full Animals list
            c.doEmote(32);
            ((FarmAnimal)c).isEating.Value = true; // do the eating animation
            ModData.FullAnimals.Add(c as FarmAnimal);
        }
        
        public static bool GameLocationToolActionExecutor(ref GameLocation __instance, ref Tool t, ref int tileX, ref int tileY)
        {
            GameLocation gameLocation = Game1.currentLocation;
            
            // execute original method if not in coop or barn
            if (!gameLocation.Name.ToLower().Contains("coop") && !gameLocation.Name.ToLower().Contains("barn"))
                return true;
            // execute original method if this is not a watering can
            if (!(t is WateringCan) || ((WateringCan) t).WaterLeft <= 0) 
                return true;

            if (Game1.currentLocation.Name.ToLower().Contains("coop") && !ModData.CoopsWithWateredTrough.Contains(__instance.NameOrUniqueName.ToLower()))
            {
                string buildingType = "coop";
                
                if (__instance.GetContainingBuilding().buildingType.Value.ToLower() == "coop")
                {
                    foreach (TroughTile troughTile in ModEntry.Instance.CurrentTroughPlacementProfile.coopTroughTiles)
                    {
                        if (troughTile.TileX != tileX || troughTile.TileY != tileY) continue;
                        
                        ModEntry.Instance.SendTroughWateredMessage(buildingType, __instance.NameOrUniqueName.ToLower());
                        
                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coopTroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coopTroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                        }

                        ModData.CoopsWithWateredTrough.Add(__instance.NameOrUniqueName.ToLower());
                        ModEntry.Instance.ChangeCoopTexture(__instance.GetContainingBuilding(), false);

                        foreach (FarmAnimal animal in __instance.animals.Values)
                        {
                            if (ModEntry.Instance.Config.ShowLoveBubblesOverAnimalsWhenWateredTrough)
                            {
                                animal.doEmote(ModData.LoveEmote);
                            }
                            animal.friendshipTowardFarmer.Value += Math.Abs(ModEntry.Instance.Config.AdditionalFriendshipPointsForWateredTroughWithAnimalsInsideBuilding);
                        }
                    }
                }
                else if (__instance.GetContainingBuilding().buildingType.Value.ToLower() == "big coop")
                {
                    foreach (TroughTile troughTile in ModEntry.Instance.CurrentTroughPlacementProfile.coop2TroughTiles)
                    {
                        if (troughTile.TileX != tileX || troughTile.TileY != tileY) continue;
                        
                        ModEntry.Instance.SendTroughWateredMessage(buildingType, __instance.NameOrUniqueName.ToLower());

                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop2TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop2TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                        }

                        ModData.CoopsWithWateredTrough.Add(__instance.NameOrUniqueName.ToLower());
                        ModEntry.Instance.ChangeBigCoopTexture(__instance.GetContainingBuilding(), false);

                        foreach (FarmAnimal animal in __instance.animals.Values)
                        {
                            if (ModEntry.Instance.Config.ShowLoveBubblesOverAnimalsWhenWateredTrough)
                            {
                                animal.doEmote(ModData.LoveEmote);
                            }
                            animal.friendshipTowardFarmer.Value += Math.Abs(ModEntry.Instance.Config.AdditionalFriendshipPointsForWateredTroughWithAnimalsInsideBuilding);
                        }
                    }
                }
                else if (__instance.GetContainingBuilding().buildingType.Value.ToLower() == "deluxe coop")
                {
                    foreach (TroughTile troughTile in ModEntry.Instance.CurrentTroughPlacementProfile.coop3TroughTiles)
                    {
                        if (troughTile.TileX != tileX || troughTile.TileY != tileY) continue;
                        
                        ModEntry.Instance.SendTroughWateredMessage(buildingType, __instance.NameOrUniqueName.ToLower());

                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop3TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop3TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                        }

                        ModData.CoopsWithWateredTrough.Add(__instance.NameOrUniqueName.ToLower());

                        foreach (FarmAnimal animal in __instance.animals.Values)
                        {
                            if (ModEntry.Instance.Config.ShowLoveBubblesOverAnimalsWhenWateredTrough)
                            {
                                animal.doEmote(ModData.LoveEmote);
                            }
                            animal.friendshipTowardFarmer.Value += Math.Abs(ModEntry.Instance.Config.AdditionalFriendshipPointsForWateredTroughWithAnimalsInsideBuilding);
                        }
                    }
                }
            }
            else if (Game1.currentLocation.Name.ToLower().Contains("barn") && !ModData.BarnsWithWateredTrough.Contains(__instance.NameOrUniqueName.ToLower()))
            {
                string buildingType = "barn";

                if (__instance.GetContainingBuilding().buildingType.Value.ToLower() == "barn")
                {
                    foreach (TroughTile troughTile in ModEntry.Instance.CurrentTroughPlacementProfile.barnTroughTiles)
                    {
                        if (troughTile.TileX != tileX || troughTile.TileY != tileY) continue;
                        
                        ModEntry.Instance.SendTroughWateredMessage(buildingType, __instance.NameOrUniqueName.ToLower());

                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barnTroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barnTroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                        }

                        ModData.BarnsWithWateredTrough.Add(__instance.NameOrUniqueName.ToLower());

                        foreach (FarmAnimal animal in __instance.animals.Values)
                        {
                            if (ModEntry.Instance.Config.ShowLoveBubblesOverAnimalsWhenWateredTrough)
                            {
                                animal.doEmote(ModData.LoveEmote);
                            }
                            animal.friendshipTowardFarmer.Value += Math.Abs(ModEntry.Instance.Config.AdditionalFriendshipPointsForWateredTroughWithAnimalsInsideBuilding);
                        }
                    }
                }
                else if (__instance.GetContainingBuilding().buildingType.Value.ToLower() == "big barn")
                {
                    foreach (TroughTile troughTile in ModEntry.Instance.CurrentTroughPlacementProfile.barn2TroughTiles)
                    {
                        if (troughTile.TileX != tileX || troughTile.TileY != tileY) continue;
                        
                        ModEntry.Instance.SendTroughWateredMessage(buildingType, __instance.NameOrUniqueName.ToLower());

                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn2TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn2TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                        }

                        ModData.BarnsWithWateredTrough.Add(__instance.NameOrUniqueName.ToLower());

                        foreach (FarmAnimal animal in __instance.animals.Values)
                        {
                            if (ModEntry.Instance.Config.ShowLoveBubblesOverAnimalsWhenWateredTrough)
                            {
                                animal.doEmote(ModData.LoveEmote);
                            }
                            animal.friendshipTowardFarmer.Value += Math.Abs(ModEntry.Instance.Config.AdditionalFriendshipPointsForWateredTroughWithAnimalsInsideBuilding);
                        }
                    }
                }
                else if (__instance.GetContainingBuilding().buildingType.Value.ToLower() == "deluxe barn")
                {
                    foreach (TroughTile troughTile in ModEntry.Instance.CurrentTroughPlacementProfile.barn3TroughTiles)
                    {
                        if (troughTile.TileX != tileX || troughTile.TileY != tileY) continue;
                        
                        ModEntry.Instance.SendTroughWateredMessage(buildingType, __instance.NameOrUniqueName.ToLower());

                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn3TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn3TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                        }

                        ModData.BarnsWithWateredTrough.Add(__instance.NameOrUniqueName.ToLower());

                        foreach (FarmAnimal animal in __instance.animals.Values)
                        {
                            if (ModEntry.Instance.Config.ShowLoveBubblesOverAnimalsWhenWateredTrough)
                            {
                                animal.doEmote(ModData.LoveEmote);
                            }
                            animal.friendshipTowardFarmer.Value += Math.Abs(ModEntry.Instance.Config.AdditionalFriendshipPointsForWateredTroughWithAnimalsInsideBuilding);
                        }
                    }
                }
            }

            return false;
        }
        
        public static void OnLocationChangedExecutor(GameLocation oldLocation, GameLocation newLocation)
        {
            var locationName = newLocation.NameOrUniqueName;
            if (!locationName.ToLower().Contains("coop") && !locationName.ToLower().Contains("barn")) return;
            
            string locationNameWithoutUnique = Game1.getLocationFromName(locationName).Name;
            Building building = ((AnimalHouse)Game1.getLocationFromName(locationName)).GetContainingBuilding();
            
            CheckForWateredTroughs(building, locationName, locationNameWithoutUnique);
        }

        public static void CheckForWateredTroughs(Building building, string locationName, string locationNameWithoutUnique)
        {
            if (ModData.BarnsWithWateredTrough.Contains(locationName.ToLower()) || ModData.CoopsWithWateredTrough.Contains(locationName.ToLower()))
            {
                if (locationNameWithoutUnique.Contains("Coop"))
                {
                    switch (building.buildingType.Value.ToLower())
                    {
                        case "coop":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coopTroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coopTroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            }

                            break;
                        }
                        case "big coop":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop2TroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop2TroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            }

                            break;
                        }
                        case "deluxe coop":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop3TroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop3TroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            }

                            break;
                        }
                    }
                }
                else if (locationNameWithoutUnique.Contains("Barn"))
                {
                    switch (building.buildingType.Value.ToLower())
                    {
                        case "barn":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barnTroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barnTroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            }

                            break;
                        }
                        case "big barn":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn2TroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn2TroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            }

                            break;
                        }
                        case "deluxe barn":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn3TroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn3TroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.FullIndex);
                            }

                            break;
                        }
                    }
                }
            }
            else if (!ModData.BarnsWithWateredTrough.Contains(locationName.ToLower()) ||
                      !ModData.CoopsWithWateredTrough.Contains(locationName.ToLower()))
            {
                if (locationNameWithoutUnique.Contains("Coop"))
                {
                    switch (building.buildingType.Value.ToLower())
                    {
                        case "coop":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coopTroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coopTroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                            }

                            break;
                        }
                        case "big coop":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop2TroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop2TroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                            }

                            break;
                        }
                        case "deluxe coop":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop3TroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.coop3TroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                            }

                            break;
                        }
                    }
                }
                else if (locationNameWithoutUnique.Contains("Barn"))
                {
                    switch (building.buildingType.Value.ToLower())
                    {
                        case "barn":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barnTroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barnTroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                            }

                            break;
                        }
                        case "big barn":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn2TroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn2TroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                            }

                            break;
                        }
                        case "deluxe barn":
                        {
                            GameLocation gameLocation = building.indoors.Value;

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn3TroughTiles)
                            {
                                gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                            }

                            Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                            Layer frontLayer = gameLocation.Map.GetLayer("Front");
                            TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                            foreach (TroughTile tile in ModEntry.Instance.CurrentTroughPlacementProfile.barn3TroughTiles)
                            {
                                if (tile.Layer.Equals("Buildings"))
                                    buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                                else if (tile.Layer.Equals("Front"))
                                    frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}
