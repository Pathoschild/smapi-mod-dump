/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using AnythingAnywhere.Framework.Patches.Locations;
using AnythingAnywhere.Framework.UI;
using Common.Helpers;
using Common.Managers;
using Common.Utilities;
using Common.Utilities.Options;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Locations;
using StardewValley.Locations;
using StardewValley.TokenizableStrings;
using System.Collections.Generic;
using System.Linq;

namespace AnythingAnywhere.Framework
{
    internal static class EventHandlers
    {
        private static bool _buildingConfigChanged;

        #region Event Subscriptions
        internal static void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            ResetBlacklist(true);
        }

        internal static void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            if (e.Added.Any())
            {
                ResetBlacklist(true); // For multiplayer
            }
        }

        internal static void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !ModEntry.Config.EnableBuilding)
                return;

            if (ModEntry.Config.BuildMenu!.JustPressed() && ModEntry.Config.EnableBuilding)
                HandleBuildButtonPress("Robin");

            if (ModEntry.Config.WizardBuildMenu!.JustPressed() && ModEntry.Config.EnableBuilding)
                HandleBuildButtonPress("Wizard");
        }

        internal static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Data/Buildings"))
            {
                e.Edit(
                    asset =>
                    {
                        var data = asset.AsDictionary<string, BuildingData>().Data;
                        foreach (var buildingDataKey in data.Keys.ToList())
                        {
                            data[buildingDataKey] = ModifyBuildingData(data[buildingDataKey], ModEntry.Config.EnableFreeBuild, ModEntry.Config.EnableInstantBuild, ModEntry.Config.RemoveBuildConditions, ModEntry.Config.EnableGreenhouse);
                        }
                    }, AssetEditPriority.Late);
                return;
            }

            if (e.Name.IsEquivalentTo("Data/Locations"))
            {
                e.Edit(
                    asset =>
                    {
                        var data = asset.AsDictionary<string, LocationData>().Data;
                        foreach (var locationDataKey in data.Keys.ToList())
                        {
                            data[locationDataKey] = ModifyLocationData(data[locationDataKey], ModEntry.Config.EnablePlanting);
                        }

                        if (!ModEntry.Config.EnablePlanting) return;
                        foreach (var location in Game1.locations)
                        {
                            location.Map.Properties.TryAdd("ForceAllowTreePlanting", "T");
                        }
                    }, AssetEditPriority.Late);
            }
        }

        internal static void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!_buildingConfigChanged) return;
            ModEntry.ModHelper.GameContent.InvalidateCache("Data/Buildings");
            _buildingConfigChanged = false;
        }

        internal static void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.OldLocation.Name.StartsWith("ScienceHouse") || e.OldLocation.Name.EndsWith("ScienceHouse") || e.OldLocation.IsOutdoors) return;
            if (e.OldLocation is Cellar or FarmHouse or Cabin || (e.NewLocation is not FarmHouse && e.OldLocation is not Cabin)) return;

            Game1.player.Position = FarmHousePatch.FarmHouseRealPos * 64f;
            Game1.xLocationAfterWarp = Game1.player.TilePoint.X;
            Game1.yLocationAfterWarp = Game1.player.TilePoint.Y;
        }

        internal static void OnConfigChanged(object sender, ConfigChangedEventArgs e)
        {
            if (Equals(e.OldValue, e.NewValue)) return;

            switch (e.ConfigName)
            {
                case nameof(ModConfig.EnablePlanting):
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/Locations");
                    break;
                case nameof(ModConfig.EnableFreeBuild):
                case nameof(ModConfig.EnableInstantBuild):
                case nameof(ModConfig.RemoveBuildConditions):
                case nameof(ModConfig.EnableGreenhouse):
                    _buildingConfigChanged = true; // Doesn't work if I don't do this
                    break;
            }

            if (!ModEntry.IsRelocateFarmAnimalsLoaded) return;
            ModEntry.Config.EnableAnimalRelocate = false;
            ConfigManager.SaveAction.Invoke();
        }

        internal static void OnClick(ButtonClickEventData e)
        {
            if (e.FieldID.Equals("BlacklistCurrentLocation"))
            {
                if (!Context.IsWorldReady)
                {
                    Game1.playSound("thudStep");
                }
                else if (Game1.player.currentLocation.IsFarm)
                {
                    Game1.playSound("thudStep");
                }
                else if (ModEntry.Config.BlacklistedLocations!.Contains(Game1.player.currentLocation.NameOrUniqueName))
                {
                    Game1.playSound("thudStep");
                }
                else
                {
                    Game1.playSound("backpackIN");
                    ModEntry.Config.BlacklistedLocations.Add(Game1.player.currentLocation.NameOrUniqueName);
                    if (Game1.player.currentLocation.IsBuildableLocation())
                    {
                        Game1.currentLocation.Map.Properties.Remove("CanBuildHere");
                    }
                }
            }
            else
            {
                Game1.playSound("backpackIN");
                ConfigUtility.InitializeDefaultConfig(ModEntry.Config, e.FieldID);
                PageHelper.OpenPage?.Invoke(PageHelper.CurrPage ?? string.Empty);

                if (e.FieldID.Equals("Building"))
                {
                    ResetBlacklist();
                }
            }
        }
        #endregion

        #region Modify Building Data
        private static BuildingData ModifyBuildingData(BuildingData data, bool enableFreeBuild, bool enableInstantBuild, bool removeBuildConditions, bool enableGreenhouse)
        {
            // Add greenhouse first
            if (enableGreenhouse && IsGreenhouse(data))
                SetGreenhouseAttributes(data);

            if (enableFreeBuild)
                SetFreeBuildAttributes(data);

            if (enableInstantBuild)
                SetInstantBuildAttributes(data);

            if (removeBuildConditions)
                RemoveBuildConditions(data);

            return data;
        }

        private static bool IsGreenhouse(BuildingData data)
        {
            return TokenParser.ParseText(data.Name) == Game1.content.LoadString("Strings\\Buildings:Greenhouse_Name");
        }

        private static void SetGreenhouseAttributes(BuildingData data)
        {
            // Define greenhouse materials
            List<BuildingMaterial> greenhouseMaterials =
            [
                new BuildingMaterial { ItemId = "(O)709", Amount = 100 },
                new BuildingMaterial { ItemId = "(O)338", Amount = 20 },
                new BuildingMaterial { ItemId = "(O)337", Amount = 10 },
            ];

            // Set greenhouse attributes
            data.Builder = Game1.builder_robin;
            data.BuildCost = 150000;
            data.BuildDays = 3;
            data.BuildMaterials = greenhouseMaterials;
            data.BuildCondition = "PLAYER_HAS_MAIL Host ccPantry";
        }

        private static void SetFreeBuildAttributes(BuildingData data)
        {
            data.BuildCost = 0;
            data.BuildMaterials = [];
        }

        private static void SetInstantBuildAttributes(BuildingData data)
        {
            data.MagicalConstruction = true;
            data.BuildDays = 0;
        }

        private static void RemoveBuildConditions(BuildingData data)
        {
            data.BuildCondition = "";
        }
        #endregion

        #region Modify Location Data
        private static LocationData ModifyLocationData(LocationData data, bool enablePlanting)
        {
            if (enablePlanting)
            {
                data.CanPlantHere = true;
            }

            return data;
        }
        #endregion

        #region Activate Build Menu
        private static void HandleBuildButtonPress(string builder)
        {
            if (Context.IsPlayerFree && Game1.activeClickableMenu == null)
            {
                ActivateBuildAnywhereMenu(builder);
            }
            else if (Game1.activeClickableMenu is BuildAnywhereMenu buildAnywhereMenu)
            {
                ResetBlacklist();
                Game1.displayFarmer = true;
                buildAnywhereMenu.returnToCarpentryMenu();
                Game1.activeClickableMenu.exitThisMenu();
            }
        }
        private static void ActivateBuildAnywhereMenu(string builder)
        {
            if (!Game1.currentLocation.IsOutdoors && !ModEntry.Config.EnableBuildingIndoors)
            {
                Game1.addHUDMessage(new HUDMessage(I18n.Message("NoBuildingIndoors"), HUDMessage.error_type) { timeLeft = HUDMessage.defaultTime });
                return;
            }

            bool magicInkCheck = !(Game1.getFarmer(Game1.player.UniqueMultiplayerID).hasMagicInk || ModEntry.Config.BypassMagicInk);
            if (builder == "Wizard" && magicInkCheck && !ModEntry.Config.EnableInstantBuild)
            {
                Game1.addHUDMessage(new HUDMessage(I18n.Message("NoMagicInk"), HUDMessage.error_type) { timeLeft = HUDMessage.defaultTime });
                return;
            }

            if (!Game1.currentLocation.IsBuildableLocation())
            {
                Game1.currentLocation.Map.Properties.Add("CanBuildHere", "T");
                Game1.currentLocation.isAlwaysActive.Value = true;
            }

            Game1.activeClickableMenu = new BuildAnywhereMenu(builder, Game1.player.currentLocation);
        }

        #endregion

        #region Blacklist
        internal static void ResetBlacklist(bool setAlwaysActive = false)
        {
            foreach (GameLocation location in Game1.locations)
            {
                if (location.buildings.Count != 0)
                {
                    if (!location.Map.Properties.TryGetValue("CanBuildHere", out var value) || value != "T")
                    {
                        if (ModEntry.Config.BlacklistedLocations!.Contains(location.NameOrUniqueName))
                            continue;

                        location.Map.Properties["CanBuildHere"] = "T";
                    }

                    if (setAlwaysActive && !location.isAlwaysActive.Value)
                    {
                        location.isAlwaysActive.Value = true;
                    }
                }

                if (ModEntry.Config.BlacklistedLocations!.Contains(location.NameOrUniqueName))
                {
                    location.Map.Properties.Remove("CanBuildHere");
                }
            }
        }
        #endregion
    }
}
