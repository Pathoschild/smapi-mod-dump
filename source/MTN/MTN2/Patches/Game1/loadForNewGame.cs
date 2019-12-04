using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using StardewValley;
using StardewValley.Locations;
using StardewModdingAPI;
using MTN2.MapData;
using Harmony;

namespace MTN2.Patches.Game1Patches {
    /// <summary>
    /// REASON FOR PATCHING: Load the correct farm map when the user wants to play
    /// on a custom farm.
    /// 
    /// Patches the method Game1.loadForNewGame to allow the implementation
    /// of custom farms, overriding existing maps, and additional maps pertaining to
    /// the content pack of said custom farm.
    /// </summary>
    public class loadForNewGamePatch {

        private static ICustomManager customManager;
        private static IMonitor Monitor;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="farmManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        /// <param name="Monitor">SMAPI's IMonitor, to print out useful information to user.</param>
        public loadForNewGamePatch(ICustomManager customManager, IMonitor Monitor) {
            loadForNewGamePatch.customManager = customManager;
            loadForNewGamePatch.Monitor = Monitor;
        }

        /// <summary>
        /// Postfix method. Occurs after the original method of Game1.loadForNewGame is executed.
        /// 
        /// Loads the custom farm in, if it hasn't already (saved game is being loaded) into farmManager.
        /// Sets the Farm map with the correct map (the custom map that was requested).
        /// Loads additional maps that are apart of custom farm's content pack.
        /// Loads map overrides that are apart of custom farm's content pack.
        /// </summary>
        public static void Postfix() {
            int farmIndex;
            int greenhouseIndex;
            Map map;
            string mapAssetKey;

            if (customManager.LoadedFarm == null) {
                if (Game1.whichFarm < 6)
                    customManager.LoadCustomFarm(Game1.whichFarm);
                
            }

            if (!customManager.Canon) {
                for (farmIndex = 0; farmIndex < Game1.locations.Count; farmIndex++) {
                    if (Game1.locations[farmIndex].Name == "Farm") break;
                }

                mapAssetKey = customManager.GetAssetKey(out map, "Farm");
                Game1.locations[farmIndex] = new Farm(mapAssetKey, "Farm");
                //if (customManager.LoadedFarm.FarmMap.FileType != FileType.xnb && Game1.multiplayerMode == 1) {
                //    customManager.LoadedFarm.ContentPack.LoadAsset<Map>(mapAssetKey);
                //}

                if (customManager.LoadedFarm.StartingGreenHouse != null) {
                    for (greenhouseIndex = 0; greenhouseIndex < Game1.locations.Count; greenhouseIndex++) {
                        if (Game1.locations[greenhouseIndex].Name == "Greenhouse") break;
                    }

                    mapAssetKey = customManager.GetAssetKey(out map, "Greenhouse");
                    Game1.locations[greenhouseIndex] = new GameLocation(mapAssetKey, "Greenhouse");
                }
            }

            if (!customManager.Canon && customManager.LoadedFarm.AdditionalMaps != null) {
                foreach (MapFile mf in customManager.LoadedFarm.AdditionalMaps) {
                    object newMap;

                    if (mf.FileType == FileType.raw) {
                        map = customManager.LoadMap(mf.FileName + ".tbin");
                    }

                    mapAssetKey = customManager.GetAssetKey(mf.FileName, mf.FileType);

                    switch (mf.MapType) {
                        case "Farm":
                        case "FarmExpansion":
                        case "MTNFarmExtension":
                            newMap = new Farm(mapAssetKey, mf.Name);
                            Game1.locations.Add((Farm)newMap);
                            //Game1.locations.Add(new FarmExtension(mapAssetKey, m.Location, newMap as Farm));
                            //Memory.farmMaps.Add(new additionalMap<Farm>(m, Game1.locations.Last() as Farm));
                            break;
                        case "FarmCave":
                            newMap = new FarmCave(mapAssetKey, mf.Name);
                            Game1.locations.Add((FarmCave)newMap);
                            break;
                        case "GameLocation":
                            newMap = new GameLocation(mapAssetKey, mf.Name);
                            Game1.locations.Add((GameLocation)newMap);
                            break;
                        case "BuildableGameLocation":
                            newMap = new BuildableGameLocation(mapAssetKey, mf.Name);
                            Game1.locations.Add((BuildableGameLocation)newMap);
                            break;
                        default:
                            newMap = new GameLocation(mapAssetKey, mf.Name);
                            Game1.locations.Add((GameLocation)newMap);
                            break;
                    }
                    Monitor.Log("Custom map loaded. Name: " + (newMap as GameLocation).Name + " Type: " + newMap.ToString());
                }
            }

            if (!customManager.Canon && customManager.LoadedFarm.Overrides != null) {
                int i;
                foreach (MapFile mf in customManager.LoadedFarm.Overrides) {
                    if (mf.FileType == FileType.raw) {
                        map = customManager.LoadMap(mf.FileName + ".tbin");
                    }
                    mapAssetKey = customManager.GetAssetKey(mf.FileName, mf.FileType);

                    for (i = 0; i < Game1.locations.Count; i++) {
                        if (Game1.locations[i].Name == mf.Name) break;
                    }

                    if (i >= Game1.locations.Count) {
                        Monitor.Log(String.Format("Unable to replace {0}, map was not found. Skipping", mf.Name), LogLevel.Warn);
                    } else {
                        switch (mf.Name) {
                            case "AdventureGuild":
                                Game1.locations[i] = new AdventureGuild(mapAssetKey, mf.Name);
                                break;
                            case "BathHousePool":
                                Game1.locations[i] = new BathHousePool(mapAssetKey, mf.Name);
                                break;
                            case "Beach":
                                Game1.locations[i] = new Beach(mapAssetKey, mf.Name);
                                break;
                            case "BusStop":
                                Game1.locations[i] = new BusStop(mapAssetKey, mf.Name);
                                break;
                            case "Club":
                                Game1.locations[i] = new Club(mapAssetKey, mf.Name);
                                break;
                            case "Desert":
                                Game1.locations[i] = new Desert(mapAssetKey, mf.Name);
                                break;
                            case "Forest":
                                Game1.locations[i] = new Forest(mapAssetKey, mf.Name);
                                break;
                            case "FarmCave":
                                Game1.locations[i] = new FarmCave(mapAssetKey, mf.Name);
                                break;
                            case "Mountain":
                                Game1.locations[i] = new Mountain(mapAssetKey, mf.Name);
                                break;
                            case "Railroad":
                                Game1.locations[i] = new Railroad(mapAssetKey, mf.Name);
                                break;
                            case "SeedShop":
                                Game1.locations[i] = new SeedShop(mapAssetKey, mf.Name);
                                break;
                            case "Sewer":
                                Game1.locations[i] = new Sewer(mapAssetKey, mf.Name);
                                break;
                            case "Town":
                                Game1.locations[i] = new Town(mapAssetKey, mf.Name);
                                break;
                            case "WizardHouse":
                                Game1.locations[i] = new WizardHouse(mapAssetKey, mf.Name);
                                break;
                            case "Woods":
                                Game1.locations[i] = new Woods(mapAssetKey, mf.Name);
                                break;
                            default:
                                Game1.locations[i] = new GameLocation(mapAssetKey, mf.Name);
                                break;
                        }
                        Monitor.Log("Map has been overridden with a custom map: " + mf.Name);
                    }
                }
            }
        }
    }
}
