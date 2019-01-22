using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using MTN2.Locations;
using MTN2.MapData;
using MTN2.Menus;
using MTN2.Messages;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MTN2
{
    /// <summary>The mod entry point.</summary>
    public class MTN : Mod {
        protected ICustomManager CustomManager;
        protected HarmonyInstance Harmony;
        protected PatchManager PatchManager;
        protected SpawnManager SpawnManager;
        protected Templates Template;

        /// <summary>
        /// Constructor
        /// </summary>
        public MTN() {
            CustomManager = new CustomManager();
            PatchManager = new PatchManager(CustomManager);
            SpawnManager = new SpawnManager(CustomManager);
            Template = new Templates();
        }

        /// <summary>
        /// Main function / Entry point of MTN. Executed by SMAPI.
        /// </summary>
        /// <param name="helper">Interface of ModHelper. Provides access to various SMAPI tools/methods.</param>
        public override void Entry(IModHelper helper) {
            Monitor.Log("Begin: Harmony Patching", LogLevel.Trace);
            Harmony = HarmonyInstance.Create("MTN.SgtPickles");
            PatchManager.Initialize(this, Monitor);
            PatchManager.Apply(Harmony);
            
            Helper.Events.GameLoop.UpdateTicked += NewGameMenu;
            Helper.Events.GameLoop.GameLaunched += Populate;
            Helper.Events.GameLoop.ReturnedToTitle += ClearData;
            Helper.Events.GameLoop.SaveLoaded += OverrideWarps;
            Helper.Events.GameLoop.SaveLoaded += InitialScienceLab;
            Helper.Events.GameLoop.SaveLoaded += SpawnManager.InitalizeResources;
            Helper.Events.GameLoop.Saving += BeforeSaveScienceLab;
            Helper.Events.GameLoop.Saving += SpawnManager.ManageAll;
            Helper.Events.GameLoop.Saved += AfterSaveScienceLab;
            Helper.Events.Multiplayer.PeerContextReceived += BeforeServerIntroduction;
            Helper.Events.Multiplayer.ModMessageReceived += MessageRecieved;

            Helper.ConsoleCommands.Add("LocationEntry", "Lists (all) the location loaded in the game.\n" +
                                                        "Usage: LocationEntry <number>\n" +
                                                        "- number: An integer value.\n" +
                                                        "  If omitted, all locations will be listed.", 
                                                        ListLocation);
            Helper.ConsoleCommands.Add("CreateTemplate", "Generates a JSON template in the MTN2 folder.\n" +
                                                         "Useful for map makers and content creators.\n" +
                                                         "Usage: CreateTemplate <string>\n" +
                                                         "- string: Can be the following:\n" +
                                                         "  + Farm: Creates a farmType.json Template" + 
                                                         "  + GreenHouse: Creates a greenHouseType.json Template",
                                                         CreateTemplate);
            return;
        }

        /// <summary>
        /// Resets various managers after the user quits the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearData(object sender, EventArgs e) {
            CustomManager.Reset();
        }

        /// <summary>
        /// Populates the <see cref="CustomFarmManager"/> with the installed Content Packs registered for MTN.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Populate(object sender, EventArgs e) {
            CustomManager.Populate(Helper, Monitor);
        }

        /// <summary>
        /// Used to replace the vanilla Character Customization Menu with MTN's version. This allows the user to
        /// be able to select custom farms and other options available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGameMenu(object sender, EventArgs e) {
            if (Game1.activeClickableMenu is TitleMenu) {
                if (TitleMenu.subMenu is CharacterCustomization) {
                    CharacterCustomization oldMenu = (CharacterCustomization)TitleMenu.subMenu;
                    CharacterCustomizationMTN menu = new CharacterCustomizationMTN(CustomManager, Monitor, oldMenu.source);
                    TitleMenu.subMenu = menu;
                }
            }
        }

        /// <summary>
        /// Replaces the canon ScienceHouse map with AdvancedScienceHouse. Enables the players to access additional
        /// options from Robin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitialScienceLab(object sender, EventArgs e) {
            GameLocation scienceHouse = Game1.getLocationFromName("ScienceHouse"); ;
            int index = Game1.locations.IndexOf(scienceHouse);

            Game1.locations[index] = new AdvancedScienceHouse(Path.Combine("Maps", "ScienceHouse"), "ScienceHouse", scienceHouse);
            CustomManager.SetScienceIndex(index);
        }

        /// <summary>
        /// Routine call prior to save game being called. Swaps the AdvancedScienceHouse with ScienceHouse. This is done
        /// to bypass Serialization concerns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeSaveScienceLab(object sender, EventArgs e) {
            AdvancedScienceHouse scienceHouse = (AdvancedScienceHouse)Game1.locations[CustomManager.ScienceHouseIndex];
            Game1.locations[CustomManager.ScienceHouseIndex] = scienceHouse.Export();
        }

        /// <summary>
        /// Routine call made after save game was finished. Swaps the ScienceHouse with AdvancedScienceHouse. Re-enables the
        /// player to access additional options from Robin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AfterSaveScienceLab(object sender, EventArgs e) {
            AdvancedScienceHouse reloadedHouse = new AdvancedScienceHouse(Path.Combine("Maps", "ScienceHouse"), "ScienceHouse", Game1.locations[CustomManager.ScienceHouseIndex]);
            Game1.locations[CustomManager.ScienceHouseIndex] = reloadedHouse;
        }

        /// <summary>
        /// Will list the location(s) that are currently loaded in memory. Can list all the locations or a specific location
        /// according to its index.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        private void ListLocation(string command, string[] args) {
            int index;

            if (args.Length < 1) {
                PrintAllLocations();
                return;
            }

            index = int.Parse(args[0]);
            if (index >= Game1.locations.Count) {
                Monitor.Log($"Error: Value must be lower than the number of locations (Current have {Game1.locations.Count} locations).");
            } else {
                Monitor.Log($"Location {index}: {Game1.locations[index].Name} - Type: {Game1.locations[index].ToString()}");
                if (Game1.locations[index].Root == null) {
                    Monitor.Log($"Location Root is null. (This map is disposable)", LogLevel.Error);
                } else {
                    Monitor.Log($"NetRef: {Game1.locations[index].Root} (This map is always active)");
                }
            }
        }

        public void CreateTemplate(string command, string[] args) {
            if (args.Length < 1) {
                Monitor.Log($"Invalid command.");
                return;
            }
            Template.CreateTemplate(args[0], Helper, Monitor);
        }

        /// <summary>
        /// Prints out all the locations loaded in memory.
        /// </summary>
        private void PrintAllLocations() {
            for (int i = 0; i < Game1.locations.Count; i++) {
                Monitor.Log($"Location {i}: {Game1.locations[i].Name} - Type: {Game1.locations[i].ToString()}");
            }
            return;
        }

        /// <summary>
        /// Sends the nessecary information needed for connecting players.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeServerIntroduction(object sender, EventArgs e) {
            if (Game1.multiplayerMode != 2) return;
            ServerIntro message = new ServerIntro();
            message.Mode = Game1.whichFarm;
            Helper.Multiplayer.SendMessage(message, "MTNBeforeServerIntro", new[] { this.ModManifest.UniqueID });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageRecieved(object sender, ModMessageReceivedEventArgs e) {
            if (e.FromModID == "SgtPickles.MTN") {
                if (e.Type == "MTNBeforeServerIntro") {
                    ServerIntro newMsg = e.ReadAs<ServerIntro>();
                    Game1.whichFarm = newMsg.Mode;
                    CustomManager.LoadCustomFarm(newMsg.Mode);
                }
            }
        }

        private void OverrideWarps(object sender, EventArgs e) {
            if (CustomManager.Canon) return;
            if (CustomManager.LoadedFarm == null) return;

            if (CustomManager.LoadedFarm.Neighbors != null) {
                foreach (Neighbor neighbor in CustomManager.LoadedFarm.Neighbors) {
                    UpdateMapsWarps(neighbor);
                }
            }

            if (CustomManager.LoadedFarm.FarmCave != null) {
                GameLocation farmCave = Game1.getLocationFromName("FarmCave");
                Point farmCaveOpening = CustomManager.FarmCaveOpening;
                farmCave.warps.Clear();
                farmCave.warps.Add(new StardewValley.Warp(8, 12, "Farm", farmCaveOpening.X, farmCaveOpening.Y + 1, false));
            }

            if (CustomManager.LoadedFarm.GreenHouse != null) {
                GameLocation greenHouse = Game1.getLocationFromName("Greenhouse");
                Point greenHouseDoor = CustomManager.GreenHouseDoor;
                greenHouse.warps.Clear();
                greenHouse.warps.Add(new StardewValley.Warp(10, 24, "Farm", greenHouseDoor.X, greenHouseDoor.Y + 1, false));
            }
        }

        private void UpdateMapsWarps(Neighbor neighbor) {
            GameLocation location = Game1.getLocationFromName(neighbor.MapName);

            foreach (MapData.Warp warp in neighbor.WarpPoints) {
                for (int i = 0; i < location.warps.Count; i++) {
                    if (CheckWarp(warp, location.warps[i])) {
                        location.warps[i].TargetX = warp.ToX;
                        location.warps[i].TargetY = warp.ToY;
                    }
                }
            }
        }

        private bool CheckWarp(MapData.Warp mtnWarp, StardewValley.Warp sdvWarp) {
            return (mtnWarp.TargetMap == sdvWarp.TargetName && mtnWarp.FromX == sdvWarp.X && mtnWarp.FromY == sdvWarp.Y);
        }
    }
}
