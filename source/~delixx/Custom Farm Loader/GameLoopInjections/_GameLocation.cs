/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/custom-farm-loader
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using System.Xml;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using System.Reflection;
using Custom_Farm_Loader.Lib;
using StardewValley.Buildings;
using StardewValley.Tools;

namespace Custom_Farm_Loader.GameLoopInjections
{
    public class _GameLocation
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.loadMap)),
               postfix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.loadMap_Postfix))
            );

            harmony.Patch(
                original: AccessTools.PropertySetter(typeof(GameLocation), nameof(GameLocation.Map)),
                postfix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.setMap_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
               prefix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.getFish_Prefix))
            );

            harmony.Patch(
               original: AccessTools.DeclaredMethod(typeof(GameLocation), nameof(GameLocation.GetFishFromLocationData), new[] { typeof(string), typeof(Vector2), typeof(int), typeof(Farmer), typeof(bool), typeof(bool), typeof(GameLocation) }),
               prefix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.GetFishFromLocationData_Prefix))
            );

            Helper.Events.GameLoop.SaveCreating += SaveCreating;
            Helper.Events.GameLoop.DayStarted += DayStarted;
        }

        private static void SaveCreating(object sender, SaveCreatingEventArgs e)
        {
            if (!CustomFarm.IsCFLMapSelected())
                return;

            //Thinking about it, I am not entirely sure if SaveCreating is the right place for this.
            //Any locations that get added afterwards don't get processed
            //But it's fine for now, I guess and there's more important things to do :)
            placeFurniture();
            placeFences();
            placeBuildings();
        }

        private static void placeBuildings()
        {
            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();

            foreach (var sb in customFarm.StartBuildings) {
                var loc = Game1.getLocationFromName(sb.Area.LocationName);

                if(!loc.isAlwaysActive.Value) {
                    Monitor.Log($"Tried placing a StartBuilding {sb.Type} at {sb.Area.LocationName}, but that location is not set as AlwaysActive.");
                    continue;
                }

                foreach(var pos in sb.Area.Include) {
                    var tile = new Vector2(pos.X, pos.Y);
                    if (!loc.buildStructure(sb.Type, tile, Game1.player, out var building, false, true))
                        continue;

                    building.FinishConstruction(true);

                    if (building.GetIndoors() is not AnimalHouse animalHouse)
                        continue;

                    foreach(var a in sb.Animals) {
                        if (animalHouse.isFull())
                            break;

                        var animal = new FarmAnimal(a.Value, Game1.random.Next(int.MaxValue), Game1.player.UniqueMultiplayerID);
                        animal.Name = a.Key;
                        animalHouse.adoptAnimal(animal);
                    }
                }
            }
        }

        private static void placeFences()
        {
            foreach (var loc in Game1.locations) {
                if (!loc.HasMapPropertyWithValue("cfl_PlaceFences"))
                    return;

                for (int x = 0; x < loc.Map.Layers[0].LayerWidth; x++)
                    for (int y = 0; y < loc.Map.Layers[0].LayerHeight; y++) {
                        var fenceProperty = loc.doesTileHavePropertyNoNull(x, y, "cfl_Fence", "Back");
                        if (fenceProperty == "")
                            continue;

                        _GameLocation.clearTileOfLitter(loc, new Vector2(x, y));
                        loc.objects.TryAdd(new Vector2(x, y), new Fence(new Vector2(x, y), fenceProperty, fenceProperty == "325")); //The game is hardcoded like this as well :P
                    }
            }
        }

        private static void placeFurniture()
        {
            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();

            foreach (Furniture furniture in customFarm.StartFurniture.Where(el => el.LocationName != "FarmHouse"))
                furniture.tryPlacingFurniture(Game1.getLocationFromName(furniture.LocationName));
        }

        public static void clearTileOfLitter(GameLocation location, Vector2 tile)
        {
            var obj = location.getObjectAtTile((int)tile.X, (int)tile.Y);

            if(obj is not null && obj.Category == StardewValley.Object.litterCategory)
                location.Objects.Remove(tile);

        }

        public static void setMap_Postfix(GameLocation __instance)
        {
            ReplaceMapProperties(__instance);
        }

        public static void loadMap_Postfix(GameLocation __instance, string mapPath)
        {
            ReplaceMapProperties(__instance);
        }

        private static void DayStarted(object sender, DayStartedEventArgs e)
        {
            ReplaceMapProperties(Game1.getFarm());
        }

        private static void ReplaceMapProperties(GameLocation __instance)
        {
            if (__instance is not Farm || !CustomFarm.IsCFLMapSelected())
                return;

            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();


            //Warps
            foreach (var warp in customFarm.Warps) {
                string coordinates = warp.Value.X + " " + warp.Value.Y;
                string propertyName = warp.Key.ToLower().Replace(" ", "") switch {
                    "busstop" => "BusStopEntry",
                    "forest" => "ForestEntry",
                    "backwoods" => "BackwoodsEntry",
                    "farmcave" => "FarmCaveEntry",
                    "warptotem" => "WarpTotemEntry",
                    _ => ""
                };

                if (__instance.map.Properties.ContainsKey(propertyName))
                    __instance.map.Properties.Remove(propertyName);

                __instance.map.Properties.Add(propertyName, coordinates);
            }


            //Locations
            foreach (var location in customFarm.Locations) {
                var coordinates = location.Value.X + " " + location.Value.Y;
                string propertyName = location.Key.ToLower().Replace(" ", "") switch {
                    "farmhouse" => "FarmHouseEntry",
                    "greenhouse" => "GreenhouseLocation",
                    //The SpouseAreaLocation isn't updated in reloadMap, so we prefix GetSpouseOutdoorAreaCorner in _Farm
                    "spousearea" => "SpouseAreaLocation",
                    "petbowl" => "PetBowlLocation",
                    "farmcave" => "FarmCaveEntry",
                    //"mailbox" => "MailboxLocation", //Deprecated
                    "shippingbin" => "ShippingBinLocation",
                    "grandpashrine" => "GrandpaShrineLocation",
                    _ => ""
                };

                if (__instance.map.Properties.ContainsKey(propertyName))
                    __instance.map.Properties.Remove(propertyName);

                __instance.map.Properties.Add(propertyName, coordinates);
            }
        }

        public static bool GetFishFromLocationData_Prefix(ref Item __result, string locationName, Vector2 bobberTile, int waterDepth, Farmer player, bool isTutorialCatch, bool isInherited, GameLocation location = null)
        {
            try {
                var loc = location is null ? Game1.getLocationFromName(locationName) : location;
                if (loc is null)
                    return true;

                return getFish_Prefix(loc, ref __result, 0f, "", waterDepth, player, 0f, bobberTile);
            } catch (Exception err) {
                Monitor.LogOnce("Ran into an error at GetFishFromLocationData_Prefix", LogLevel.Error);
                Monitor.LogOnce(err.Message, LogLevel.Trace);

            }
            return true;
        }

        public static bool getFish_Prefix(GameLocation __instance, ref Item __result, float millisecondsAfterNibble, string bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName = null)
        {
            if (!CustomFarm.IsCFLMapSelected())
                return true;

            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();

            if (!customFarm.FishingRules.Any(e => e.LocationName == __instance.Name))
                return true;

            foreach (Building b in __instance.buildings)
                if (b is FishPond && b.isTileFishable(bobberTile)) {
                    __result = (b as FishPond).CatchFish();
                    return false;
                }

            bool isUsingMagicBait = false;
            bool hasCuriosityLure = false;
            if (who?.CurrentTool is FishingRod rod) {
                isUsingMagicBait = rod.HasMagicBait();
                hasCuriosityLure = rod.HasCuriosityLure();
            }

            var validFishingRules = customFarm.FishingRules.FindAll(el => el.Area.LocationName == __instance.Name
                                                                       && el.Area.isTileIncluded(bobberTile)
                                                                       && el.Filter.isValid(excludeSeason: isUsingMagicBait, excludeTime: isUsingMagicBait, excludeWeather: isUsingMagicBait, who: who)
                                                                       && el.Fish.Count > 0);

            if (validFishingRules.Count == 0)
                return true;

            FishingRule chosenFishingRule = UtilityMisc.PickSomeInRandomOrder(validFishingRules, 1).First();

            __result = chosenFishingRule.getFish(isUsingMagicBait, millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile);
            return false;
        }
    }
}