/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Models.ContentPack;
using SolidFoundations.Framework.Utilities;
using SolidFoundations.Framework.Utilities.Backport;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Object = StardewValley.Object;

namespace SolidFoundations.Framework.Patches.Buildings
{
    // TODO: When updated to SDV v1.6, delete this patch
    internal class CarpenterMenuPatch : PatchTemplate
    {
        private readonly Type _object = typeof(CarpenterMenu);
        private static ClickableTextureComponent _appearanceButton;

        internal CarpenterMenuPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.setNewActiveBlueprint), null), prefix: new HarmonyMethod(GetType(), nameof(SetNewActiveBlueprintPrefix)));
            harmony.Patch(AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.receiveLeftClick), new[] { typeof(int), typeof(int), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(ReceiveLeftClickPostfix)));
            harmony.Patch(AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.tryToBuild), null), prefix: new HarmonyMethod(GetType(), nameof(TryToBuildPrefix)));

            harmony.Patch(AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.draw), new[] { typeof(SpriteBatch) }), transpiler: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(DrawTranspiler)));
            harmony.Patch(AccessTools.Constructor(typeof(CarpenterMenu), new[] { typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(CarpenterMenuPostfix)));
        }

        private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var list = instructions.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].opcode == OpCodes.Callvirt && list[i].operand is not null && list[i].operand.ToString().Contains("draw", StringComparison.OrdinalIgnoreCase))
                    {
                        if (list[i - 2].opcode == OpCodes.Ldfld && list[i - 2].operand.ToString().Contains("paintButton", StringComparison.OrdinalIgnoreCase))
                        {
                            list.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));
                            list.Insert(i + 2, new CodeInstruction(OpCodes.Ldarg_1));
                            list.Insert(i + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CarpenterMenuPatch), nameof(HandleSkinButtonDraw), new[] { typeof(CarpenterMenu), typeof(SpriteBatch) })));
                        }
                    }
                }

                return list;
            }
            catch (Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for CarpenterMenu.draw: {e}", LogLevel.Error);
                return instructions;
            }
        }

        private static void HandleSkinButtonDraw(CarpenterMenu menu, SpriteBatch b)
        {
            if (_appearanceButton is null)
            {
                _appearanceButton = new ClickableTextureComponent("Change Appearance", new Microsoft.Xna.Framework.Rectangle(menu.xPositionOnScreen + menu.maxWidthOfBuildingViewer - 128 + 16, menu.yPositionOnScreen + menu.maxHeightOfBuildingViewer - 64 + 32, 64, 64), null, null, SolidFoundations.assetManager.GetAppearanceButton(), new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16), 4f)
                {
                    myID = 109,
                    downNeighborID = -99998
                };
            }

            var building = _helper.Reflection.GetField<Building>(menu, "currentBuilding").GetValue();
            if (building is GenericBuilding genericBuilding && genericBuilding.Model is not null && genericBuilding.Model.Skins is not null && genericBuilding.Model.Skins.Count > 0)
            {
                _appearanceButton.draw(b);
            }
        }

        private static bool SetNewActiveBlueprintPrefix(CarpenterMenu __instance, int ___currentBlueprintIndex, List<BluePrint> ___blueprints, ref Building ___currentBuilding, ref int ___price, ref string ___buildingName, ref string ___buildingDescription, ref List<Item> ___ingredients)
        {
            if (SolidFoundations.buildingManager.GetSpecificBuildingModel(___blueprints[___currentBlueprintIndex].name) is ExtendedBuildingModel model && model is not null)
            {
                Type buildingTypeFromName = GetBuildingTypeFromName(model.BuildingType);
                ___currentBuilding = new GenericBuilding(model, ___blueprints[___currentBlueprintIndex]);
            }
            else
            {
                return true;
            }

            ___price = ___blueprints[___currentBlueprintIndex].moneyRequired;
            ___ingredients.Clear();

            foreach (KeyValuePair<int, int> item in ___blueprints[___currentBlueprintIndex].itemsRequired)
            {
                ___ingredients.Add(new Object(item.Key, item.Value));
            }

            ___buildingDescription = ___blueprints[___currentBlueprintIndex].description;
            ___buildingName = ___blueprints[___currentBlueprintIndex].displayName;
            //__instance.UpdateAppearanceButtonVisibility();
            if (Game1.options.SnappyMenus && __instance.currentlySnappedComponent != null)// && __instance.currentlySnappedComponent == ___appearanceButton && !___appearanceButton.visible)
            {
                __instance.setCurrentlySnappedComponentTo(102);
                __instance.snapToDefaultClickableComponent();
            }

            return false;
        }

        private static void ReceiveLeftClickPostfix(CarpenterMenu __instance, Building ___currentBuilding, bool ___onFarm, bool ___upgrading, int x, int y, bool playSound = true)
        {
            if (___onFarm && ___upgrading)
            {
                var targetLocation = FlexibleLocationFinder.GetBuildableLocationByName("Farm");
                GenericBuilding buildingAt = targetLocation.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64)) as GenericBuilding;
                if (buildingAt != null && __instance.CurrentBlueprint.name != null && buildingAt.buildingType.Equals(__instance.CurrentBlueprint.nameOfBuildingToUpgrade))
                {
                    buildingAt.upgradeName.Value = __instance.CurrentBlueprint.name;
                    buildingAt.daysUntilUpgrade.Value = __instance.CurrentBlueprint.daysToConstruct;

                    if (buildingAt.daysUntilUpgrade.Value <= 0)
                    {
                        buildingAt.dayUpdate(Game1.dayOfMonth);
                    }
                };
            }

            else if (_appearanceButton.containsPoint(x, y))
            {
                if (___currentBuilding is GenericBuilding genericBuilding && genericBuilding.Model is not null && genericBuilding.Model.Skins is not null && genericBuilding.Model.Skins.Count > 0)
                {
                    BuildingSkinMenu buildingSkinMenu = new BuildingSkinMenu(genericBuilding);
                    buildingSkinMenu.behaviorBeforeCleanup = (Action<IClickableMenu>)Delegate.Combine(buildingSkinMenu.behaviorBeforeCleanup, (Action<IClickableMenu>)delegate
                    {
                        if (Game1.options.SnappyMenus)
                        {
                            __instance.setCurrentlySnappedComponentTo(109);
                            __instance.snapCursorToCurrentSnappedComponent();
                        }

                        var skin = genericBuilding.Model.Skins.FirstOrDefault(s => s.ID == genericBuilding.skinID.Value);
                        if (skin is not null)
                        {
                            _helper.Reflection.GetField<string>(__instance, "buildingName").SetValue(genericBuilding.Model.GetTranslation(skin.Name));
                            _helper.Reflection.GetField<string>(__instance, "buildingDescription").SetValue(genericBuilding.Model.GetTranslation(skin.Description));
                        }
                        else
                        {
                            _helper.Reflection.GetField<string>(__instance, "buildingName").SetValue(genericBuilding.Model.Name);
                            _helper.Reflection.GetField<string>(__instance, "buildingDescription").SetValue(genericBuilding.Model.Description);
                        }
                    });
                    __instance.SetChildMenu(buildingSkinMenu);
                }
            }
        }

        private static bool TryToBuildPrefix(CarpenterMenu __instance, ref bool __result, Building ___currentBuilding)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(__instance.CurrentBlueprint.name) is false)
            {
                return true;
            }

            var targetLocation = FlexibleLocationFinder.GetBuildableLocationByName("Farm");
            __result = GameLocationPatch.AttemptToBuildStructure(targetLocation, __instance.CurrentBlueprint, ___currentBuilding);

            return false;
        }

        private static void CarpenterMenuPostfix(CarpenterMenu __instance, ref List<BluePrint> ___blueprints, ref ClickableTextureComponent ___upgradeIcon, bool magicalConstruction = false)
        {
            string builder = "Robin";
            if (magicalConstruction)
            {
                builder = "Wizard";
            }

            var targetLocation = FlexibleLocationFinder.GetBuildableLocationByName("Farm");
            foreach (var building in SolidFoundations.buildingManager.GetAllBuildingModels().Where(b => GameStateQuery.CheckConditions(b.BuildCondition) is true))
            {
                if (String.IsNullOrEmpty(building.Builder) || building.Builder.Equals(builder, StringComparison.OrdinalIgnoreCase))
                {
                    bool flag = false;
                    if (building.BuildingToUpgrade != null && targetLocation.getNumberBuildingsConstructed(building.BuildingToUpgrade) == 0 && targetLocation.getNumberBuildingsConstructed(building.BuildingToUpgrade.Replace(" ", null)) == 0)
                    {
                        flag = true;
                    }

                    if (!flag)
                    {
                        ___blueprints.Add(new BluePrint(building.ID));
                    }
                }
            }

            // Move the upgrade info button to the left, to make room for header
            ___upgradeIcon = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(__instance.xPositionOnScreen - 64, __instance.yPositionOnScreen + 8, 36, 52), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(402, 328, 9, 13), 4f)
            {
                myID = 103,
                rightNeighborID = 104,
                leftNeighborID = 105,
                upNeighborID = 109
            };
        }

        private static Type GetBuildingTypeFromName(string building_type_name)
        {
            Type type = null;
            if (building_type_name != null)
            {
                type = Type.GetType(building_type_name);
            }
            if (type == null)
            {
                type = typeof(Building);
            }
            return type;
        }

    }
}
