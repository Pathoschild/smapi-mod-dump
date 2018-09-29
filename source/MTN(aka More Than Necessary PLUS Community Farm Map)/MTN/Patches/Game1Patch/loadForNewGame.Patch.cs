using MTN.FarmInfo;
using MTN.MapTypes;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;

namespace MTN.Patches.Game1Patch {
    class loadForNewGamePatch {
        public static void Postfix() {
            int farmIndex;
            Map map;
            string mapAssetKey;

            if (Game1.whichFarm > 4) {
                if (Memory.loadedFarm == null) {
                    Memory.loadCustomFarmType(Game1.whichFarm);
                }

                for (farmIndex = 0; farmIndex < Game1.locations.Count; farmIndex++) {
                    if (Game1.locations[farmIndex].Name == "Farm") break;
                }

                if (Memory.loadedFarm.farmMapType == fileType.raw) {
                    map = Memory.loadedFarm.contentpack.LoadAsset<Map>(Memory.loadedFarm.farmMapFile + ".tbin");
                }
                mapAssetKey = Memory.loadedFarm.contentpack.GetActualAssetKey(Memory.loadedFarm.farmMapFile + ((Memory.loadedFarm.farmMapType == fileType.raw) ? ".tbin" : ".xnb"));
                Game1.locations[farmIndex] = new Farm(mapAssetKey, "Farm");
            }

            Memory.farmMaps.Add(new additionalMap<Farm>("BaseFarm", "Farm", (Game1.whichFarm > 4) ? Memory.loadedFarm.farmMapType : fileType.xnb, "Farm", "Base Farm", Game1.getFarm()));

            if (Memory.isCustomFarmLoaded && Memory.loadedFarm.additionalMaps != null) {
                foreach (additionalMap<GameLocation> m in Memory.loadedFarm.additionalMaps) {
                    object newMap;

                    if (m.type == fileType.raw) {
                        map = Memory.loadedFarm.contentpack.LoadAsset<Map>(m.FileName + ".tbin");
                    }

                    mapAssetKey = Memory.loadedFarm.contentpack.GetActualAssetKey(m.FileName + ((m.type == fileType.raw) ? ".tbin" : ".xnb"));

                    switch (m.mapType) {
                        case "Farm":
                        case "FarmExpansion":
                        case "MTNFarmExtension":
                            newMap = new Farm(mapAssetKey, m.Location);
                            Game1.locations.Add((Farm)newMap);
                            //Game1.locations.Add(new FarmExtension(mapAssetKey, m.Location, newMap as Farm));
                            Memory.farmMaps.Add(new additionalMap<Farm>(m, Game1.locations.Last() as Farm));
                            break;
                        case "FarmCave":
                            newMap = new FarmCave(mapAssetKey, m.Location);
                            Game1.locations.Add((FarmCave)newMap);
                            break;
                        case "GameLocation":
                            newMap = new GameLocation(mapAssetKey, m.Location);
                            Game1.locations.Add((GameLocation)newMap);
                            break;
                        case "BuildableGameLocation":
                            newMap = new BuildableGameLocation(mapAssetKey, m.Location);
                            Game1.locations.Add((BuildableGameLocation)newMap);
                            break;
                        default:
                            newMap = new GameLocation(mapAssetKey, m.Location);
                            Game1.locations.Add((GameLocation)newMap);
                            break;
                    }
                    Memory.instance.Monitor.Log("Custom map loaded. Name: " + (newMap as GameLocation).Name + " Type: " + newMap.ToString());
                }
            }

            if (Memory.isCustomFarmLoaded && Memory.loadedFarm.overrideMaps != null) {
                int i;
                foreach (overrideMap m in Memory.loadedFarm.overrideMaps) {
                    if (m.type == fileType.raw) {
                        map = Memory.loadedFarm.contentpack.LoadAsset<Map>(m.FileName + ".tbin");
                    }
                    mapAssetKey = Memory.loadedFarm.contentpack.GetActualAssetKey(m.FileName + ((m.type == fileType.raw) ? ".tbin" : ".xnb"));

                    for (i = 0; i < Game1.locations.Count; i++) {
                        if (Game1.locations[i].Name == m.Location) break;
                    }
                    if (i >= Game1.locations.Count) {
                        Memory.instance.Monitor.Log(String.Format("Unable to replace {0}, map was not found. Skipping", m.Location), LogLevel.Warn);
                    } else {
                        switch (m.Location) {
                            case "AdventureGuild":
                                Game1.locations[i] = new AdventureGuild(mapAssetKey, m.Location);
                                break;
                            case "BathHousePool":
                                Game1.locations[i] = new BathHousePool(mapAssetKey, m.Location);
                                break;
                            case "Beach":
                                Game1.locations[i] = new Beach(mapAssetKey, m.Location);
                                break;
                            case "BusStop":
                                Game1.locations[i] = new BusStop(mapAssetKey, m.Location);
                                break;
                            case "Club":
                                Game1.locations[i] = new Club(mapAssetKey, m.Location);
                                break;
                            case "Desert":
                                Game1.locations[i] = new Desert(mapAssetKey, m.Location);
                                break;
                            case "Forest":
                                Game1.locations[i] = new Forest(mapAssetKey, m.Location);
                                break;
                            case "FarmCave":
                                Game1.locations[i] = new FarmCave(mapAssetKey, m.Location);
                                break;
                            case "Mountain":
                                Game1.locations[i] = new Mountain(mapAssetKey, m.Location);
                                break;
                            case "Railroad":
                                Game1.locations[i] = new Railroad(mapAssetKey, m.Location);
                                break;
                            case "SeedShop":
                                Game1.locations[i] = new SeedShop(mapAssetKey, m.Location);
                                break;
                            case "Sewer":
                                Game1.locations[i] = new Sewer(mapAssetKey, m.Location);
                                break;
                            case "WizardHouse":
                                Game1.locations[i] = new WizardHouse(mapAssetKey, m.Location);
                                break;
                            case "Woods":
                                Game1.locations[i] = new Woods(mapAssetKey, m.Location);
                                break;
                            default:
                                Game1.locations[i] = new GameLocation(mapAssetKey, m.Location);
                                break;
                        }
                        Memory.instance.Monitor.Log("Map has been overridden with a custom map: " + m.Location);
                    }
                }
            }
        }
    }
}
