using System;
using System.IO;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Harmony;
using System.Collections.Generic;
using Newtonsoft.Json;
using MTN.FarmInfo;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using Netcode;
using MTN.Menus;
using MTN.MapTypes;

namespace MTN {
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {
        HarmonyInstance harmony;
        List<Patch> patches;

        /// <summary>
        /// Main function / Entry point of MTN. Executed by SMAPI.
        /// </summary>
        /// <param name="helper">Interface of ModHelper. Provides access to various SMAPI tools/methods.</param>
        public override void Entry(IModHelper helper) {
            Memory.instance = this;

            //Create Harmony Instance and patch needed classes.
            Monitor.Log("Begin: Harmony Patching", LogLevel.Trace);
            harmony = HarmonyInstance.Create("MTN.SgtPickles");
            initalizePatches();
            
            //Store Event triggered routines into SMAPI.
            MenuEvents.MenuChanged += switchOutMenu;
            GameEvents.FirstUpdateTick += generateCustomFarmTypeList;
            //SaveEvents.AfterLoad += addHouseDesigns;
            GameEvents.UpdateTick += newGameMenu;
            SaveEvents.BeforeSave += executeSpawns;
            SaveEvents.BeforeSave += swapScienceLab_beforeSave;
            SaveEvents.AfterSave += swapScienceLab_afterSave;
            SaveEvents.AfterLoad += overRideWarps;
            SaveEvents.AfterLoad += printStatus;
            SaveEvents.AfterLoad += swapScienceLab_init;
            SaveEvents.AfterReturnToTitle += clearMemory;
            GameEvents.OneSecondTick += countMPUp;

            //Get Multiplayer and replace with MTNMultiplayer.
            //This will be removed and implemented into SMAPI at a later date.
            Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").SetValue(new MTNMultiplayer());

            //Register console commands into SMAPI.
            //Helper.ConsoleCommands.Add("kick", "Remove the user from the game. Must be host.\n\nUsage: kick <username>\n- username: Ingame screename of the user.", kickUser);
            //Helper.ConsoleCommands.Add("unbanmod", "Removes a mod from the Blacklist, unbanning it.\n\nUsage: unbanmod <uniqueId>\n - uniqueId: The mod's uniqueId", manageBlacklist);
            //Helper.ConsoleCommands.Add("banmod", "Blacklists/Bans a mod. If a player has the mod installed, they will be removed.\nAny newly connected players will be disconnected if the mod is detected.\nMust be host\n\nUsage: banmod <uniqueId>\n - uniqueId: The mod's uniqueId", manageBlacklist);
            //Helper.ConsoleCommands.Add("listbannedmods", "List all the banned mods and their UniqueIds.\nUsage: listbannedmods", listBannedMods);
            Helper.ConsoleCommands.Add("showLocationEntry", "Lists all the location entries within the game.\nUsage: showLocationEntry <number>\n- number: An integer value.\nIf omitted, all locations will be listed.", listLocation);
        }

        /// <summary>
        /// Lists the GameLocations loaded within memory. Used for debugging purposes.
        /// </summary>
        /// <param name="command">The command that triggered the method routine.</param>
        /// <param name="args">Arguments passed into the function via console.</param>
        private void listLocation(string command, string[] args) {
            bool showAll = false;

            if (args.Length < 1) {
                showAll = true;
            } else if (args[0] == "all") {
                showAll = true;
            }

            if (showAll) {
                for (int i = 0; i < Game1.locations.Count; i++) {
                    Monitor.Log($"Location {i}: {Game1.locations[i].Name} - Type: {Game1.locations[i].ToString()}");
                }
            } else if (int.Parse(args[0]) > Game1.locations.Count) {
                Monitor.Log($"Error: Value must be lower than the number of locations (Current have {Game1.locations.Count} locations).");
            } else {
                int i = int.Parse(args[0]);
                Monitor.Log($"Location {i}: {Game1.locations[i].Name} - Type: {Game1.locations[i].ToString()}");
                if (Game1.locations[i].Root == null) {
                    Monitor.Log($"Location Root is null. (This map is disposable)", LogLevel.Error);
                } else {
                    Monitor.Log($"NetRef: {Game1.locations[i].Root} (This map is always active)");
                }
            }
        }

        private void swapScienceLab_afterSave(object sender, EventArgs e) {
            AdvancedScienceHouse reloadedHouse = new AdvancedScienceHouse(Path.Combine("Maps", "ScienceHouse"), "ScienceHouse", Game1.locations[Memory.scienceHouseLocInList]);
            Game1.locations[Memory.scienceHouseLocInList] = reloadedHouse;
        }

        private void swapScienceLab_beforeSave(object sender, EventArgs e) {
            AdvancedScienceHouse scienceHouse = (AdvancedScienceHouse)Game1.locations[Memory.scienceHouseLocInList];
            Game1.locations[Memory.scienceHouseLocInList] = scienceHouse.export();
        }

        private void swapScienceLab_init(object sender, EventArgs e) {
            int i;
            GameLocation scienceHouse = null;
            for (i = 0; i < Game1.locations.Count; i++) {
                if (Game1.locations[i].Name == "ScienceHouse") {
                    scienceHouse = Game1.getLocationFromName("ScienceHouse");
                    break;
                }
            }
            
            Game1.locations[i] = new AdvancedScienceHouse(Path.Combine("Maps", "ScienceHouse"), "ScienceHouse", scienceHouse);
            Memory.scienceHouseLocInList = i;
            return;
        }

        private void executeSpawns(object sender, EventArgs e) {
            if (Game1.multiplayerMode != 1 && Game1.whichFarm > 4) {
                if (Memory.spawnIntegrityChecked == false) {
                    checkSpawns();
                    Memory.spawnIntegrityChecked = true;
                }
                Memory.loadedFarm.executeResourceSpawns();
                Memory.loadedFarm.executeForageSpawns();
            }
        }

        private void checkSpawns() {
            if (Game1.multiplayerMode != 1 && Game1.whichFarm > 4) {
                Memory.loadedFarm.checkSpawnIntegrity();
            }
        }


        private void countMPUp(object sender, EventArgs e) {
            if (Game1.multiplayerMode == 2 || Game1.multiplayerMode == 1) {
                Memory.multiplayer.countTick();
            }
        }

        private void printStatus(object sender, EventArgs e) {
            Monitor.Log("Game loaded.");
            Monitor.Log("MultiplayerMode = " + Game1.multiplayerMode + " (" + ((Game1.multiplayerMode == 2) ? "You are the host" : (Game1.multiplayerMode == 1) ? "You are a client" : "Offline/Single Player") + ")");
            Monitor.Log("whichFarm = " + Game1.whichFarm + " (Farm Type ID)");
            foreach (GameLocation g in Game1.locations) {
                if (g is Farm)
                    Monitor.Log("Map by name '" + g.Name + "' is a farm map");
            }
        }

        /// <summary>
        /// Generates and applies the Harmony Patches.
        /// </summary>
        private void initalizePatches() {
            //The list of harmony patches. Disable one by commenting out the line.
            patches = new List<Patch>
            {
                new Patch("Event", "setExitLocation", true, false, false, typeof(Patches.EventPatch.setExitLocationPatch)),
                new Patch("Locations.FarmHouse", "", false, true, false, typeof(Patches.FarmHousePatch.FarmHouseConstructorPatch), 1, new Type[] { typeof(string), typeof(string) }),
                //new Patch("Locations.FarmHouse", "getPorchStandingSpot", true, true, false, typeof(Patches.FarmHousePatch.getPorchStandingSpotPatch)),
                new Patch("Locations.FarmHouse", "updateMap", false, true, false, typeof(Patches.FarmHousePatch.updateMapPatch)),
                new Patch("Farm", "checkAction", false, true, true, typeof(Patches.FarmPatch.checkActionPatch)),
                new Patch("Farm", "", false, true, false, typeof(Patches.FarmPatch.constructor), 1, new Type[] { typeof(string), typeof(string) }),
                new Patch("Farm", "draw", false, true, true, typeof(Patches.FarmPatch.drawPatch)),
                new Patch("Farm", "getFrontDoorPositionForFarmer", false, true, false, typeof(Patches.FarmPatch.getFrontDoorPositionForFarmerPatch)),
                new Patch("Farm", "leftClick", true, true, false, typeof(Patches.FarmPatch.leftClickPatch)),
                new Patch("Farm", "resetLocalState", false, true, false, typeof(Patches.FarmPatch.resetLocalStatePatch)),
                new Patch("Farm", "UpdateWhenCurrentLocation", false, true, true, typeof(Patches.FarmPatch.UpdateWhenCurrentLocationPatch)),
                new Patch("Game1", "loadForNewGame", false, true, false, typeof(Patches.Game1Patch.loadForNewGamePatch)),
                new Patch("GameLocation", "loadObjects", true, true, false, typeof(Patches.GameLocationPatch.LoadObjectPatch)),
                new Patch("GameLocation", "startEvent", true, false, false, typeof(Patches.GameLocationPatch.startEventPatch)),
                // new Patch("Network.NetBuildingRef", "get_Value", false, false, true, typeof(Patches.NetBuildingRefPatch.ValueGetter)),
                new Patch("NPC", "updateConstructionAnimation", false, true, true, typeof(Patches.NPCPatch.updateConstructionAnimationPatch)),
                new Patch("Object", "totemWarpForReal", true, true, false, typeof(Patches.ObjectsPatch.totemWarpForRealPatch)),
                new Patch("Characters.Pet", "setAtFarmPosition", true, true, false, typeof(Patches.PetPatch.setAtFarmPositionPatch)),
                new Patch("Characters.Pet", "dayUpdate", true, true, false, typeof(Patches.PetPatch.dayUpdatePatch)),
                new Patch("SaveGame", "loadDataToLocations", false, false, true, typeof(Patches.SaveGamePatch.loadDataToLocationsPatch)),
                new Patch("Menus.TitleMenu", "setUpIcons", true, false, false, typeof(Patches.TitleMenuPatch.setUpIconsPatch)),
                new Patch("Tools.Wand", "wandWarpForReal", true, true, false, typeof(Patches.WandPatch.wandWarpForRealPatch)),
                new Patch("Events.WorldChangeEvent", "setUp", true, false, false, typeof(Patches.WorldChangeEventPatch.setUpPatch))
            };

            //Apply patches.
            for (int i = 0; i < patches.Count; i++) {
                patches[i].Apply(harmony);
            }
        }

        /// <summary>
        /// Called to switch out the canon CharacterCustomization Menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newGameMenu(object sender, EventArgs e) {
            if (Game1.activeClickableMenu is TitleMenu) {
                if (TitleMenu.subMenu is CharacterCustomization) {
                    CharacterCustomization oldMenu = (CharacterCustomization)TitleMenu.subMenu;
                    CharacterCustomizationWithCustom menu = new CharacterCustomizationWithCustom(oldMenu.source);
                    TitleMenu.subMenu = menu;
                }
            }
        }

        /// <summary>
        /// Called when menu has been changed. Specifically looking for CarpenterMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switchOutMenu(object sender, EventArgsClickableMenuChanged e) {
            if (e.NewMenu is CarpenterMenu && !(e.NewMenu is CarpenterMenuWithCustoms)) {
                bool magicBuildings = Helper.Reflection.GetField<bool>(e.NewMenu, "magicalConstruction", true).GetValue();
                CarpenterMenuWithCustoms newMenu = new CarpenterMenuWithCustoms(magicBuildings);
                Game1.activeClickableMenu = newMenu;
            } else if (e.NewMenu is PurchaseAnimalsMenu && !(e.NewMenu is PurchaseAnimalsMenuWithCustoms)) {
                PurchaseAnimalsMenuWithCustoms newMenu = new PurchaseAnimalsMenuWithCustoms(Utilities.getPurchaseAnimalStock("Farm"));
                Game1.activeClickableMenu = newMenu;
            }
        }

        /// <summary>
        /// Called when player returns to the title screen. Resets memory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearMemory(object sender, EventArgs e) {
            //!! Known SMAPI bug, is not called on disconnection from Multiplayer !!
            Memory.isCustomFarmLoaded = false;
            Memory.isFarmHouseRelocated = false;
            Memory.isGreenHouseRelocated = false;
            Memory.isMailBoxRelocated = false;
            Memory.isShippBinRelocated = false;
            Memory.isShrineRelocated = false;
            Memory.isRabbitRelocated = false;
            Memory.isWaterBowlRelocated = false;
            Memory.loadedFarm = null;
            Memory.noDebrisRequested = false;
            Memory.spawnIntegrityChecked = false;

            Memory.farmMaps.Clear();

            Memory.allowCustomizableFarmHouse = true;
            Memory.designList.Clear();

            Memory.mapLoadSignal = 0;
        }

        /// <summary>
        /// Reads the content packs for MTN, and stores the needed data accordingly.
        /// This is where each custom farm map is read.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generateCustomFarmTypeList(object sender, EventArgs e) {
            CustomFarmEntry readInFarm;
            JsonSerializer serializer = new JsonSerializer();
            string jsonFile;
            string iconFile;

            Memory.customFarms.Clear();

            //Before we begin... get the canon farms.
            generateCanonFarmTypeList();

            //Read each content pack for MTN.
            foreach (IContentPack contentPacks in Helper.GetContentPacks()) {
                Monitor.Log($"Reading content pack: {contentPacks.Manifest.Name} {contentPacks.Manifest.Version}.");
                jsonFile = Path.Combine(contentPacks.DirectoryPath, "farmType.json");
                //If jsonFile exists. Read it. Otherwise, skip the custom map from registering with MTN.
                if (File.Exists(jsonFile)) {
                    using (StreamReader sr = new StreamReader(jsonFile)) {
                        using (JsonReader reader = new JsonTextReader(sr)) {
                            readInFarm = (CustomFarmEntry)serializer.Deserialize(reader, typeof(CustomFarmEntry));
                            readInFarm.Name = "MTN_" + readInFarm.Name;
                            iconFile = Path.Combine(contentPacks.DirectoryPath, "icon.png");
                            if (File.Exists(iconFile)) {
                                readInFarm.IconSource = contentPacks.LoadAsset<Texture2D>("icon.png");
                            } else {
                                readInFarm.IconSource = Helper.Content.Load<Texture2D>(Path.Combine("res", "missingIcon.png"), ContentSource.ModFolder);
                            }
                            readInFarm.contentpack = contentPacks;
                            Memory.customFarms.Add(readInFarm);
                        }
                    }
                } else {
                    Monitor.Log($"Could not find farmType.json for {contentPacks.Manifest.Name}! This content pack will be skipped.", LogLevel.Warn);
                }
            }

            return;
        }

        /// <summary>
        /// Unused experimental Method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addHouseDesigns(object sender, EventArgs e) {
            if (Memory.allowCustomizableFarmHouse) {
                HouseExteriorDesign readInDesign;
                JsonSerializer serializer = new JsonSerializer();
                Memory.designList = new List<HouseExteriorDesign>();

                string pathToDirectory = Path.Combine(new string[] {
                    Path.Combine(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Mods"), "MTN"), "Building")
                });

                if (Directory.Exists(pathToDirectory)) {
                    foreach (string dir in Directory.GetDirectories(pathToDirectory)) {
                        string jsonFile = Path.Combine(pathToDirectory, dir, "houseDesign.json");
                        if (File.Exists(jsonFile)) {
                            using (StreamReader sr = new StreamReader(@jsonFile)) {
                                using (JsonReader reader = new JsonTextReader(sr)) {
                                    readInDesign = (HouseExteriorDesign)serializer.Deserialize(reader, typeof(HouseExteriorDesign));
                                    Memory.designList.Add(readInDesign);
                                }
                            }
                        } else {
                            Monitor.Log(String.Format("Unable to find houseDesign.json for Building Directory: {0}", dir), LogLevel.Warn);
                            Monitor.Log("This design will be skipped.", LogLevel.Warn);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates placeholders for canon farms, as to not get confused/lost.
        /// </summary>
        private void generateCanonFarmTypeList() {
            Memory.customFarms.Add(new CustomFarmEntry(0, "Standard", null, null, 3, true, true));
            Memory.customFarms.Add(new CustomFarmEntry(1, "Riverland", null, null, 3, true, true));
            Memory.customFarms.Add(new CustomFarmEntry(2, "Forest", null, null, 3, true, true));
            Memory.customFarms.Add(new CustomFarmEntry(3, "Hills", null, null, 3, true, true));
            Memory.customFarms.Add(new CustomFarmEntry(4, "Wilderness", null, null, 3, true, true));
            Memory.selectedFarm = Memory.customFarms[0];
        }

        /// <summary>
        /// Called after load. Performs the needed warp overrides when a custom farm is loaded in.
        /// If a canon farm is loaded in, no overrides will occur. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void overRideWarps(object sender, EventArgs e) {
            //Check if Canon or Custom
            if (Game1.whichFarm > 4) {
                int i;

                //This should never be true, but just in case (Due to specific tests)
                if (Memory.loadedFarm == null) return;

                //Take care of Neighboring map's warps, if entry/exit has moved.
                if (Memory.loadedFarm.neighboringMaps != null) {
                    foreach (neighboringMap map in Memory.loadedFarm.neighboringMaps) {
                        string mapName = map.MapName;
                        GameLocation workingMap = Game1.getLocationFromName(mapName);
                        foreach (neighboringMap.Warp warp in map.warpPoints) {
                            for (i = 0; i < workingMap.warps.Count; i++) {
                                if (workingMap.warps[i].TargetName == warp.TargetMap && warp.fromX == workingMap.warps[i].X && warp.fromY == workingMap.warps[i].Y) {
                                    workingMap.warps[i].TargetX = warp.toX;
                                    workingMap.warps[i].TargetY = warp.toY;
                                }
                            }

                        }
                    }
                }

                //Cave (Skipped if it was not specified in farmType.Json. It will be assumed it did not move from canon position).
                if (Memory.loadedFarm.farmCave != null && Memory.loadedFarm.farmCave.useCustomMapPoints == false)
                {
                    Game1.locations[2].warps.Clear();
                    Game1.locations[2].warps.Add(new Warp(8, 12, "Farm", Memory.loadedFarm.farmCavePointX(), Memory.loadedFarm.farmCavePointY(), false));
                }

                //Greenhouse (Skipped if it was not specified in farmType.Json. It will be assumed it did not move from canon position).
                GameLocation greenhouse = Game1.getLocationFromName("Greenhouse");
                if (greenhouse != null && Memory.loadedFarm.greenHouse != null && Memory.loadedFarm.greenHouse.useCustomMapPoints == false)
                {
                    greenhouse.warps.Clear();
                    greenhouse.warps.Add(new Warp(10, 24, "Farm", Memory.loadedFarm.greenHousePorchX(), Memory.loadedFarm.greenHousePorchY(), false));
                }

                //Buildings for fucks.
                if (Memory.loadedFarm.additionalMaps != null) {
                    foreach (additionalMap<GameLocation> m in Memory.loadedFarm.additionalMaps) {
                        //if (Game1.multiplayerMode == 1) return;
                        if (m.mapType == "Farm") {
                            Farm workingFarm = (Farm)Game1.getLocationFromName(m.Location);
                            if (workingFarm == null) return;
                            foreach (Building b in workingFarm.buildings) {
                                if (b.indoors.Value == null) continue;
                                foreach (Warp w in b.indoors.Value.warps) {
                                    NetString warp = Helper.Reflection.GetField<NetString>(w, "targetName").GetValue();
                                    warp.Value = m.Location;
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}