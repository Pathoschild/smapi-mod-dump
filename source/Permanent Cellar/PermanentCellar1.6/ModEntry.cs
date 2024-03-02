/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CyanFireUK/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using static System.StringComparer;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using xTile;
using xTile.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;



namespace PermanentCellar
{
    public interface ISaveAnywhereApi
    {
        event EventHandler AfterLoad;
    }


    public class FlooringData
    {
        public Rectangle area;
        public string id;
        public bool ignore;
    }


    public static class ModHelperExtensions
    {
        public static JObject ReadContentPackConfig(IModHelper helper)
        {
            var modInfo = helper.ModRegistry.Get("aedenthorn.DynamicFlooring");
            if (modInfo is null)
            {
                return null;
            }

            var modPath = (string)modInfo.GetType().GetProperty("DirectoryPath")!.GetValue(modInfo)!;
            try
            {
                var config = JObject.Parse(File.ReadAllText(Path.Combine(modPath, "config.json")));
                return config;
            }
            catch (FileNotFoundException)
            {

                return null;
            }
        }
    }



    public class ModEntry : Mod
    {
        private ModConfig config_;
        private string saveGameName_;
        private bool isDFLoaded;
        private List<FlooringData> list = new();
        private PropertyValue Cellar0ExitFH;
        private PropertyValue Cellar1ExitFH;
        private PropertyValue Cellar0ExitCB;
        private PropertyValue Cellar1ExitCB;
        private float CE0XPositionFH1;
        private float CE0YPositionFH1;
        private float CE0XPositionFH2;
        private float CE0YPositionFH2;
        private float CE1XPositionFH1;
        private float CE1YPositionFH1;
        private float CE1XPositionFH2;
        private float CE1YPositionFH2;
        private float CE0XPositionCB1;
        private float CE0YPositionCB1;
        private float CE0XPositionCB2;
        private float CE0YPositionCB2;
        private float CE1XPositionCB1;
        private float CE1YPositionCB1;
        private float CE1XPositionCB2;
        private float CE1YPositionCB2;


        internal class ModConfig
        {
            public IDictionary<string, ConfigEntry> SaveGame { get; set; }
                = new Dictionary<string, ConfigEntry>();
        }

        internal class ConfigEntry
        {
            public bool ShowCommunityUpgrade { get; set; } = false;
            public bool RemoveFarmHouseCasks { get; set; } = false;
            public bool RemoveCabinCasks { get; set; } = false;
        }

        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            Helper.Events.Player.Warped += OnWarped;
            Helper.Events.Content.AssetRequested += OnAssetRequested;
            Helper.Events.Display.MenuChanged += OnMenuChanged;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;


            isDFLoaded = Helper.ModRegistry.IsLoaded("aedenthorn.DynamicFlooring");

        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var saveAnywhereApi = Helper.ModRegistry.GetApi<ISaveAnywhereApi>("Omegasis.SaveAnywhere");


            if (saveAnywhereApi != null)
            {
                saveAnywhereApi.AfterLoad += OnAfterLoad;
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            config_ = Helper.ReadConfig<ModConfig>();

            saveGameName_ = $"{Game1.GetSaveGameName()}_{Game1.uniqueIDForThisGame}";
            if (!config_.SaveGame.ContainsKey(saveGameName_) && Game1.IsMasterGame)
            {
                config_.SaveGame.Add(saveGameName_, new ConfigEntry());
                Helper.WriteConfig(config_);
            }

            if (config_.SaveGame[saveGameName_].RemoveFarmHouseCasks && Game1.IsMasterGame)
            {
                FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.player);
                GameLocation cellar = Game1.getLocationFromName(farmHouse.GetCellarName());

                cellar.Objects
                      .Pairs
                      .Where(item => item.Value is Cask)
                      .Select(item => item.Key)
                      .ToList()
                      .ForEach(key => cellar.Objects.Remove(key));
            }
            if (config_.SaveGame[saveGameName_].RemoveCabinCasks && Game1.IsMasterGame)
            {
                foreach (Cabin cabin in GetLocations().OfType<Cabin>())
                {
                    GameLocation cellar = Game1.getLocationFromName(cabin.GetCellarName());

                    cellar.Objects
                          .Pairs
                          .Where(item => item.Value is Cask)
                          .Select(item => item.Key)
                          .ToList()
                          .ForEach(key => cellar.Objects.Remove(key));
                }
            }
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (config_.SaveGame[saveGameName_].RemoveFarmHouseCasks && Game1.IsMasterGame)
            {
                config_.SaveGame[saveGameName_].RemoveFarmHouseCasks = false;
                Helper.WriteConfig(config_);
            }
            if (config_.SaveGame[saveGameName_].RemoveCabinCasks && Game1.IsMasterGame)
            {
                config_.SaveGame[saveGameName_].RemoveCabinCasks = false;
                Helper.WriteConfig(config_);
            }
        }


        [EventPriority((EventPriority)int.MinValue)]
        private void OnAfterLoad(object sender, EventArgs e)
        {
            FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.MasterPlayer);


            if (!Game1.newDay && Game1.player.currentLocation == farmHouse || Game1.player.currentLocation == Game1.getLocationFromName("Cellar") && farmHouse.upgradeLevel < 3)
            {
                CreateCellarEntranceFH(farmHouse);
                CreateCellarToFarmHouseWarps(farmHouse);
            }

            foreach (Cabin cabin in GetLocations().OfType<Cabin>())
                if (!Game1.newDay && Game1.player.currentLocation == cabin || Game1.player.currentLocation == Game1.getLocationFromName(cabin.GetCellarName()) && cabin.upgradeLevel < 3)
                {
                    CreateCellarEntranceCB(cabin);
                    CreateCellarToCabinWarps(cabin);
                }
        }

        [EventPriority((EventPriority)int.MinValue)]
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.MasterPlayer);

            if (Context.IsWorldReady && isDFLoaded == true && (Game1.player.currentLocation == farmHouse || Game1.player.currentLocation == Game1.getLocationFromName("Cabin")) && Game1.player.currentLocation.modData.TryGetValue("aedenthorn.DynamicFlooring/flooring", out string listString) && e.IsMultipleOf(30))
            {
                list = JsonConvert.DeserializeObject<List<FlooringData>>(listString);
            }

        }


        [EventPriority((EventPriority)int.MinValue)]
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.MasterPlayer);

            if (Game1.year == 1 && Game1.dayOfMonth == 1 && Game1.IsSpring)
            {
                Game1.updateCellarAssignments();
            }

            if (!Game1.player.craftingRecipes.ContainsKey("Cask"))
            {
                Game1.player.craftingRecipes.Add("Cask", 0);
            }

            if (Game1.player.currentLocation == farmHouse && farmHouse.upgradeLevel < 3)
            {
                CreateCellarEntranceFH(farmHouse);
                CreateCellarToFarmHouseWarps(farmHouse);
            }

            foreach (Cabin cabin in GetLocations().OfType<Cabin>())
            if (Game1.player.currentLocation == cabin && cabin.upgradeLevel < 3)
            {
                CreateCellarEntranceCB(cabin);
                CreateCellarToCabinWarps(cabin);
            }

        }


        [EventPriority((EventPriority)int.MinValue)]
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.MasterPlayer);

            if (Game1.player.currentLocation == farmHouse || Game1.player.currentLocation == Game1.getLocationFromName("Cellar") && Game1.timeOfDay != 600 && farmHouse.upgradeLevel < 3)
            {
                CreateCellarEntranceFH(farmHouse);
            }

            foreach (Cabin cabin in GetLocations().OfType<Cabin>())
            if (Game1.player.currentLocation == cabin || Game1.player.currentLocation == Game1.getLocationFromName(cabin.GetCellarName()) && Game1.timeOfDay != 600 && cabin.upgradeLevel < 3)
            {
                CreateCellarEntranceCB(cabin);
            }

        }

        [EventPriority((EventPriority)int.MinValue)]
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.MasterPlayer);

            if (e.NewLocation == farmHouse || e.NewLocation == Game1.getLocationFromName("Cellar") && farmHouse.upgradeLevel < 3)
            {
                CreateCellarEntranceFH(farmHouse);
                CreateCellarToFarmHouseWarps(farmHouse);
            }


            foreach (Cabin cabin in GetLocations().OfType<Cabin>())
                if (e.NewLocation == cabin || e.NewLocation == Game1.getLocationFromName(cabin.GetCellarName()) && cabin.upgradeLevel < 3)
                {
                    CreateCellarEntranceCB(cabin);
                    CreateCellarToCabinWarps(cabin);
                }
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (config_ == null || !config_.SaveGame[saveGameName_].ShowCommunityUpgrade)
            {
                return;
            }

            bool ccIsComplete = Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") ||
                                Game1.MasterPlayer.hasCompletedCommunityCenter();
            bool jojaMember = Game1.MasterPlayer.mailReceived.Contains("JojaMember");

            bool communityUpgradeInProgress = (Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade.Value > 0;
            bool pamHouseUpgrade = Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade");
            bool communityUpgradeShortcuts = Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts");

            if ((!ccIsComplete && !jojaMember) || communityUpgradeInProgress || (pamHouseUpgrade && communityUpgradeShortcuts))
            {
                return;
            }

            if (e.NewMenu is DialogueBox dialogue)
            {
                string text = dialogue.dialogues.FirstOrDefault();
                string menuText = Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu");
                if (text == menuText)
                {
                    Response upgrade = dialogue.responses.FirstOrDefault(r => r.responseKey == "Upgrade");
                    Response communityUpgrade = dialogue.responses.FirstOrDefault(r => r.responseKey == "CommunityUpgrade");
                    if (upgrade == null || communityUpgrade != null)
                    {
                        return;
                    }

                    upgrade.responseKey = "CommunityUpgrade";
                    upgrade.responseText = Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade");
                }
            }
        }

        [EventPriority((EventPriority)int.MinValue)]
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady && isDFLoaded == true && e.Button == ModHelperExtensions.ReadContentPackConfig(Helper).GetValue("RemoveButton").ToObject<SButton>() && Game1.player.currentLocation == Utility.getHomeOfFarmer(Game1.MasterPlayer) && Utility.getHomeOfFarmer(Game1.MasterPlayer).upgradeLevel < 3)
            {

                var point = Utility.Vector2ToPoint(Game1.currentCursorTile);

                if (list.Any())
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if (list[i].area.Contains(point))
                        {
                            CreateCellarEntranceFH(Utility.getHomeOfFarmer(Game1.MasterPlayer));
                        }
                    }

                }

            }
            foreach (Cabin cabin in GetLocations().OfType<Cabin>())
            if (Context.IsWorldReady && isDFLoaded == true && e.Button == ModHelperExtensions.ReadContentPackConfig(Helper).GetValue("RemoveButton").ToObject<SButton>() && Game1.player.currentLocation == cabin && cabin.upgradeLevel < 3)
            {
                var point = Utility.Vector2ToPoint(Game1.currentCursorTile);

                if (list.Any())
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if (list[i].area.Contains(point))
                        {
                           CreateCellarEntranceCB(cabin);
                        }
                    }

                }

            }
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Maps/FarmHouse_Cellar0"))
            {
                e.LoadFromModFile<Map>("assets/FarmHouse_Cellar0.tmx", AssetLoadPriority.Medium);

            }
            if (e.Name.IsEquivalentTo("Maps/FarmHouse_Cellar1"))
            {
                e.LoadFromModFile<Map>("assets/FarmHouse_Cellar1.tmx", AssetLoadPriority.Medium);
            }
            if (e.Name.IsEquivalentTo("Maps/FarmHouse"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();

                    editor.ExtendMap(minHeight: 13);
                }, AssetEditPriority.Early + -1000);

            }
            if (e.Name.IsEquivalentTo("Maps/FarmHouse1") || e.Name.IsEquivalentTo("Maps/FarmHouse1_marriage"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();

                    editor.ExtendMap(minHeight: 13);
                }, AssetEditPriority.Early + -1000);

            }

        }

        public static IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }


        [EventPriority((EventPriority)int.MinValue)]
        private void CreateCellarEntranceFH(FarmHouse farmHouse)
        {


            if (Cellar1ExitFH != null && Cellar1ExitFH != "")
            {
                string[] CE1xyVals = Cellar1ExitFH.ToString().Split();
                CE1XPositionFH1 = float.Parse(CE1xyVals[0]);
                CE1YPositionFH1 = float.Parse(CE1xyVals[1]);
                try
                {
                    CE1XPositionFH2 = float.Parse(CE1xyVals[2]);
                    CE1YPositionFH2 = float.Parse(CE1xyVals[3]);
                }
                catch { }

            }


            if (farmHouse.upgradeLevel >= 3)
            {
                return;
            }

            if (farmHouse.upgradeLevel == 0)
            {
                if (Helper.Reflection.GetField<HashSet<string>>(farmHouse, "_appliedMapOverrides").GetValue().Contains("cellar"))
                    Helper.Reflection.GetField<HashSet<string>>(farmHouse, "_appliedMapOverrides").GetValue().Remove("cellar");

                farmHouse.ApplyMapOverride("FarmHouse_Cellar0", "cellar");
                farmHouse.createCellarWarps();
                farmHouse.ReadWallpaperAndFloorTileData();
                farmHouse.setFloors();
            }
            else if (farmHouse.upgradeLevel == 1)
            {
                if (Helper.Reflection.GetField<HashSet<string>>(farmHouse, "_appliedMapOverrides").GetValue().Contains("cellar"))
                    Helper.Reflection.GetField<HashSet<string>>(farmHouse, "_appliedMapOverrides").GetValue().Remove("cellar");

                farmHouse.ApplyMapOverride("FarmHouse_Cellar1", "cellar");
                farmHouse.createCellarWarps();
                farmHouse.ReadWallpaperAndFloorTileData();
                farmHouse.setFloors();
            }
            else if (farmHouse.upgradeLevel == 2)
            {
                farmHouse.upgradeLevel = 3;
                farmHouse.updateFarmLayout();

            }
        }

        [EventPriority((EventPriority)int.MinValue)]
        private void CreateCellarEntranceCB(Cabin cabin)
        {
            if (cabin.upgradeLevel >= 3)
            {
                return;
            }

            if (cabin.upgradeLevel == 0)
            {
                if (Helper.Reflection.GetField<HashSet<string>>(cabin, "_appliedMapOverrides").GetValue().Contains("cellar"))
                    Helper.Reflection.GetField<HashSet<string>>(cabin, "_appliedMapOverrides").GetValue().Remove("cellar");

                cabin.ApplyMapOverride("FarmHouse_Cellar0", "cellar");
                cabin.createCellarWarps();
                cabin.ReadWallpaperAndFloorTileData();
                cabin.setFloors();
            }
            else if (cabin.upgradeLevel == 1)
            {
                if (Helper.Reflection.GetField<HashSet<string>>(cabin, "_appliedMapOverrides").GetValue().Contains("cellar"))
                    Helper.Reflection.GetField<HashSet<string>>(cabin, "_appliedMapOverrides").GetValue().Remove("cellar");

                cabin.ApplyMapOverride("FarmHouse_Cellar1", "cellar");
                cabin.createCellarWarps();
                cabin.ReadWallpaperAndFloorTileData();
                cabin.setFloors();
            }
            else if (cabin.upgradeLevel == 2)
            {
                cabin.upgradeLevel = 3;
                cabin.updateFarmLayout();
            }
        }

        [EventPriority((EventPriority)int.MinValue)]
        private void CreateCellarToFarmHouseWarps(FarmHouse farmHouse)
        {
            Tuple<Warp, Warp> warps = GetCellarToFarmHouseWarps(farmHouse);

            Map Cellar0Stairs = Game1.content.Load<Map>("Maps/FarmHouse_Cellar0");
            Map Cellar1Stairs = Game1.content.Load<Map>("Maps/FarmHouse_Cellar1");

            Helper.ModContent.GetPatchHelper(Cellar0Stairs).AsMap().Data.Properties.TryGetValue("CellarExit", out Cellar0ExitFH);
            Helper.ModContent.GetPatchHelper(Cellar1Stairs).AsMap().Data.Properties.TryGetValue("CellarExit", out Cellar1ExitFH);

            if (Cellar0ExitFH != null && Cellar0ExitFH != "")
            {
                string[] CE0xyVals = Cellar0ExitFH.ToString().Split();
                CE0XPositionFH1 = float.Parse(CE0xyVals[0]);
                CE0YPositionFH1 = float.Parse(CE0xyVals[1]);
                try
                {
                    CE0XPositionFH2 = float.Parse(CE0xyVals[2]);
                    CE0YPositionFH2 = float.Parse(CE0xyVals[3]);
                }
                catch { }
            }

            if (Cellar1ExitFH != null && Cellar1ExitFH != "")
            {
                string[] CE1xyVals = Cellar1ExitFH.ToString().Split();
                CE1XPositionFH1 = float.Parse(CE1xyVals[0]);
                CE1YPositionFH1 = float.Parse(CE1xyVals[1]);
                try
                {
                    CE1XPositionFH2 = float.Parse(CE1xyVals[2]);
                    CE1YPositionFH2 = float.Parse(CE1xyVals[3]);
                }
                catch { }

            }



            if (farmHouse.upgradeLevel == 0)
            {
                if (CE0XPositionFH1 != 0 && CE0YPositionFH1 != 0)
                {
                    warps.Item1.TargetX = (int)CE0XPositionFH1;
                    warps.Item1.TargetY = (int)CE0YPositionFH1;
                }

                if (CE0XPositionFH2 == 0 && CE0YPositionFH2 == 0 && CE0XPositionFH1 != 0 && CE0YPositionFH1 != 0)
                {
                    warps.Item2.TargetX = (int)CE0XPositionFH1;
                    warps.Item2.TargetY = (int)CE0YPositionFH1;
                }
                else if (CE0XPositionFH2 != 0 && CE0YPositionFH2 != 0)
                {
                    warps.Item2.TargetX = (int)CE0XPositionFH2;
                    warps.Item2.TargetY = (int)CE0YPositionFH2;
                }
            }
            else if (farmHouse.upgradeLevel == 1)
            {
                if (CE1XPositionFH1 != 0 && CE1YPositionFH1 != 0)
                {
                    warps.Item1.TargetX = (int)CE1XPositionFH1;
                    warps.Item1.TargetY = (int)CE1YPositionFH1;
                }

                if (CE1XPositionFH2 == 0 && CE1YPositionFH2 == 0 && CE1XPositionFH1 != 0 && CE1YPositionFH1 != 0)
                {
                    warps.Item2.TargetX = (int)CE1XPositionFH1;
                    warps.Item2.TargetY = (int)CE1YPositionFH1;
                }
                else if (CE1XPositionFH2 != 0 && CE1YPositionFH2 != 0)
                {
                    warps.Item2.TargetX = (int)CE1XPositionFH2;
                    warps.Item2.TargetY = (int)CE1YPositionFH2;
                }
            }
        }


        [EventPriority((EventPriority)int.MinValue)]
        private void CreateCellarToCabinWarps(Cabin cabin)
        {
            Tuple<Warp, Warp> warps = GetCellarToCabinWarps(cabin);

            Map Cellar0Stairs = Game1.content.Load<Map>("Maps/FarmHouse_Cellar0");
            Map Cellar1Stairs = Game1.content.Load<Map>("Maps/FarmHouse_Cellar1");

            Helper.ModContent.GetPatchHelper(Cellar0Stairs).AsMap().Data.Properties.TryGetValue("CellarExit", out Cellar0ExitCB);
            Helper.ModContent.GetPatchHelper(Cellar1Stairs).AsMap().Data.Properties.TryGetValue("CellarExit", out Cellar1ExitCB);

            if (Cellar0ExitCB != null && Cellar0ExitCB != "")
            {
                string[] CE0xyVals = Cellar0ExitCB.ToString().Split();
                CE0XPositionCB1 = float.Parse(CE0xyVals[0]);
                CE0YPositionCB1 = float.Parse(CE0xyVals[1]);
                try
                {
                    CE0XPositionCB2 = float.Parse(CE0xyVals[2]);
                    CE0YPositionCB2 = float.Parse(CE0xyVals[3]);
                }
                catch { }
            }

            if (Cellar1ExitCB != null && Cellar1ExitCB != "")
            {
                string[] CE1xyVals = Cellar1ExitCB.ToString().Split();
                CE1XPositionCB1 = float.Parse(CE1xyVals[0]);
                CE1YPositionCB1 = float.Parse(CE1xyVals[1]);
                try
                {
                    CE1XPositionCB2 = float.Parse(CE1xyVals[2]);
                    CE1YPositionCB2 = float.Parse(CE1xyVals[3]);
                }
                catch { }

            }



            if (cabin.upgradeLevel >= 2)
            {
                return;
            }

            if (cabin.upgradeLevel == 0)
            {
                if (CE0XPositionCB1 != 0 && CE0YPositionCB1 != 0)
                {
                    warps.Item1.TargetX = (int)CE0XPositionCB1;
                    warps.Item1.TargetY = (int)CE0YPositionCB1;
                }

                if (CE0XPositionCB2 == 0 && CE0YPositionCB2 == 0 && CE0XPositionCB1 != 0 && CE0YPositionCB1 != 0)
                {
                    warps.Item2.TargetX = (int)CE0XPositionCB1;
                    warps.Item2.TargetY = (int)CE0YPositionCB1;
                }
                else if (CE0XPositionCB2 != 0 && CE0YPositionCB2 != 0)
                {
                    warps.Item2.TargetX = (int)CE0XPositionCB2;
                    warps.Item2.TargetY = (int)CE0YPositionCB2;
                }
            }
            else if (cabin.upgradeLevel == 1)
            {
                if (CE1XPositionCB1 != 0 && CE1YPositionCB1 != 0)
                {
                    warps.Item1.TargetX = (int)CE1XPositionCB1;
                    warps.Item1.TargetY = (int)CE1YPositionCB1;
                }

                if (CE1XPositionCB2 == 0 && CE1YPositionCB2 == 0 && CE1XPositionCB1 != 0 && CE1YPositionCB1 != 0)
                {
                    warps.Item2.TargetX = (int)CE1XPositionCB1;
                    warps.Item2.TargetY = (int)CE1YPositionCB1;
                }
                else if (CE1XPositionCB2 != 0 && CE1YPositionCB2 != 0)
                {
                    warps.Item2.TargetX = (int)CE1XPositionCB2;
                    warps.Item2.TargetY = (int)CE1YPositionCB2;
                }
            }
        }


        private static Tuple<Warp, Warp> GetCellarToFarmHouseWarps(FarmHouse farmHouse)
        {
                GameLocation cellar = Game1.getLocationFromName(farmHouse.GetCellarName());

                try
                {
                    Warp warp1 = cellar.warps.First(warp =>
                    {
                        return OrdinalIgnoreCase.Equals(warp.TargetName, "FarmHouse");
                    });

                    Warp warp2 = cellar.warps.Skip(1).First(warp =>
                    {
                        return OrdinalIgnoreCase.Equals(warp.TargetName, "FarmHouse");
                    });

                    return Tuple.Create(warp1, warp2);
                }
                catch
                {
                    throw new Exception($"The farmhouse cellar map doesn't have the required warp points.");
                }
        }

        private static Tuple<Warp, Warp> GetCellarToCabinWarps(Cabin cabin)
        {
                GameLocation cellar = Game1.getLocationFromName(cabin.GetCellarName());

                try
                {
                    Warp warp1 = cellar.warps.First(warp =>
                    {
                        return OrdinalIgnoreCase.Equals(warp.TargetName, "FarmHouse") || OrdinalIgnoreCase.Equals(warp.TargetName, cabin.NameOrUniqueName);
                    });

                    Warp warp2 = cellar.warps.Skip(1).First(warp =>
                    {
                        return OrdinalIgnoreCase.Equals(warp.TargetName, "FarmHouse") || OrdinalIgnoreCase.Equals(warp.TargetName, cabin.NameOrUniqueName);
                    });

                    return Tuple.Create(warp1, warp2);
                }
                catch
                {
                    throw new Exception($"The cabin cellar map doesn't have the required warp points.");
                }
        }

    }

}
