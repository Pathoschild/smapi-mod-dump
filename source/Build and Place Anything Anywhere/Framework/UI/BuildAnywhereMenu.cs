/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Threading;

namespace AnythingAnywhere.Framework.UI
{
    internal class BuildAnywhereMenu : CarpenterMenu
    {
        public BuildAnywhereMenu(string builder, ModConfig config, IMonitor monitor) : base(builder, Game1.currentLocation)
        {
            UpdateTargetLocation(Game1.currentLocation);

            TargetLocation = Game1.currentLocation;
            Game1.player.forceCanMove();
            resetBounds();
            int index = 0;
            Blueprints.Clear();
            foreach (KeyValuePair<string, BuildingData> data in Game1.buildingData)
            {

                if ((data.Value.Builder != builder || !GameStateQuery.CheckConditions(data.Value.BuildCondition, TargetLocation) || data.Value.BuildingToUpgrade != null && TargetLocation.getNumberBuildingsConstructed(data.Value.BuildingToUpgrade) == 0 || !IsValidBuildingForLocation(data.Key, data.Value, TargetLocation)) &&
                    !config.EnableFreeBuild)
                {
                    continue;
                }
                else if (config.EnableFreeBuild)
                {
                    if (data.Value.Builder != builder)
                        continue;
                    data.Value.MagicalConstruction = true;
                    data.Value.BuildCost = 0;
                    data.Value.BuildDays = 0;
                    data.Value.BuildMaterials = new List<BuildingMaterial>();
                }
                Blueprints.Add(new BlueprintEntry(index++, data.Key, data.Value, null));
                /*monitor.LogOnce($"Blueprint Added. Index: {index}\nData: {data.Key}\nData: {data.Value}", LogLevel.Info);*/
                if (data.Value.Skins == null)
                {
                    continue;
                }
                foreach (BuildingSkin skin in data.Value.Skins)
                {
                    if (skin.ShowAsSeparateConstructionEntry && GameStateQuery.CheckConditions(skin.Condition, TargetLocation))
                    {
                        Blueprints.Add(new BlueprintEntry(index++, data.Key, data.Value, skin.Id));
                    }
                }
            }

            SetNewActiveBlueprint(0);
            if (Game1.options.SnappyMenus)
            {
                populateClickableComponentList();
                snapToDefaultClickableComponent();
            }
        }

        private void resetBounds()
        {
            xPositionOnScreen = Game1.uiViewport.Width / 2 - maxWidthOfBuildingViewer - spaceToClearSideBorder;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - maxHeightOfBuildingViewer / 2 - spaceToClearTopBorder + 32;
            width = maxWidthOfBuildingViewer + maxWidthOfDescription + spaceToClearSideBorder * 2 + 64;
            height = maxHeightOfBuildingViewer + spaceToClearTopBorder;
            initialize(xPositionOnScreen, yPositionOnScreen, width, height, showUpperRightCloseButton: true);
            okButton = new ClickableTextureComponent("OK", new Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - 192 - 12, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 64, 64), null, null, Game1.mouseCursors, new Rectangle(366, 373, 16, 16), 4f)
            {
                myID = 106,
                rightNeighborID = 104,
                leftNeighborID = 105,
                upNeighborID = 109
            };
            cancelButton = new ClickableTextureComponent("OK", new Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - 64, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f)
            {
                myID = 107,
                leftNeighborID = 104,
                upNeighborID = 109
            };
            backButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 64, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
            {
                myID = 101,
                rightNeighborID = 102,
                upNeighborID = 109
            };
            forwardButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + maxWidthOfBuildingViewer - 256 + 16, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
            {
                myID = 102,
                leftNeighborID = 101,
                rightNeighborID = -99998,
                upNeighborID = 109
            };
            demolishButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_Demolish"), new Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - 128 - 8, yPositionOnScreen + maxHeightOfBuildingViewer + 64 - 4, 64, 64), null, null, Game1.mouseCursors, new Rectangle(348, 372, 17, 17), 4f)
            {
                myID = 104,
                rightNeighborID = 107,
                leftNeighborID = 106,
                upNeighborID = 109
            };
            upgradeIcon = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + maxWidthOfBuildingViewer - 128 + 32, yPositionOnScreen + 8, 36, 52), Game1.mouseCursors, new Rectangle(402, 328, 9, 13), 4f)
            {
                myID = 103,
                rightNeighborID = 104,
                leftNeighborID = 105,
                upNeighborID = 109
            };
            moveButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings"), new Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - 256 - 20, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 64, 64), null, null, Game1.mouseCursors, new Rectangle(257, 284, 16, 16), 4f)
            {
                myID = 105,
                rightNeighborID = 106,
                leftNeighborID = -99998,
                upNeighborID = 109
            };
            paintButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_PaintBuildings"), new Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - 320 - 20, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 64, 64), null, null, Game1.mouseCursors2, new Rectangle(80, 208, 16, 16), 4f)
            {
                myID = 105,
                rightNeighborID = -99998,
                leftNeighborID = -99998,
                upNeighborID = 109
            };
            appearanceButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_ChangeAppearance"), new Rectangle(xPositionOnScreen + maxWidthOfBuildingViewer - 128 + 16, yPositionOnScreen + maxHeightOfBuildingViewer - 64 + 32, 64, 64), null, null, Game1.mouseCursors2, new Rectangle(96, 208, 16, 16), 4f)
            {
                myID = 109,
                downNeighborID = -99998
            };
            bool has_owned_buildings = false;
            bool has_paintable_buildings = false;
            foreach (Building building in TargetLocation.buildings)
            {
                if (building.hasCarpenterPermissions())
                {
                    has_owned_buildings = true;
                }
                if ((building.CanBePainted() || building.CanBeReskinned(ignoreSeparateConstructionEntries: true)) && HasPermissionsToPaint(building))
                {
                    has_paintable_buildings = true;
                }
            }
            demolishButton.visible = Game1.IsMasterGame;
            moveButton.visible = Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && has_owned_buildings;
            paintButton.visible = has_paintable_buildings;
            if (!demolishButton.visible)
            {
                upgradeIcon.rightNeighborID = demolishButton.rightNeighborID;
                okButton.rightNeighborID = demolishButton.rightNeighborID;
                cancelButton.leftNeighborID = demolishButton.leftNeighborID;
            }
            if (!moveButton.visible)
            {
                upgradeIcon.leftNeighborID = moveButton.leftNeighborID;
                forwardButton.rightNeighborID = -99998;
                okButton.leftNeighborID = moveButton.leftNeighborID;
            }
            UpdateAppearanceButtonVisibility();
        }

        public void UpdateTargetLocation(GameLocation newTargetLocation)
        {
            TargetLocation = newTargetLocation;
        }
    }
}
