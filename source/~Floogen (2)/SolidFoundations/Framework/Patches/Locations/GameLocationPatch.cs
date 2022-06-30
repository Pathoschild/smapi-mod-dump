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
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Object = StardewValley.Object;

namespace SolidFoundations.Framework.Patches.Buildings
{
    // TODO: When updated to SDV v1.6, delete this patch
    internal class GameLocationPatch : PatchTemplate
    {

        private readonly Type _object = typeof(GameLocation);

        internal GameLocationPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.performAction), new[] { typeof(string), typeof(Farmer), typeof(xTile.Dimensions.Location) }), postfix: new HarmonyMethod(GetType(), nameof(PerformActionPostfix)));
            harmony.Patch(AccessTools.Method(typeof(BuildableGameLocation), nameof(BuildableGameLocation.isBuildingConstructed), new[] { typeof(string) }), postfix: new HarmonyMethod(GetType(), nameof(IsBuildingConstructedPostfix)));
            harmony.Patch(AccessTools.Method(typeof(BuildableGameLocation), nameof(BuildableGameLocation.buildStructure), new[] { typeof(BluePrint), typeof(Vector2), typeof(Farmer), typeof(bool), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(BuildStructurePrefix)));
        }

        private static void PerformActionPostfix(GameLocation __instance, ref bool __result, string action, Farmer who, xTile.Dimensions.Location tileLocation)
        {
            if (__instance is BuildableGameLocation buildableLocation is false || buildableLocation is null || __result is true)
            {
                return;
            }

            if (action != null && who.IsLocalPlayer)
            {
                string[] array = action.Split(' ');
                switch (array[0])
                {
                    case "BuildingChest":
                        {
                            GenericBuilding buildingAt2 = buildableLocation.getBuildingAt(new Vector2(tileLocation.X, tileLocation.Y)) as GenericBuilding;
                            if (buildingAt2 != null)
                            {
                                buildingAt2.PerformBuildingChestAction(array[1], who);
                                __result = true;
                            }
                            break;
                        }
                    case "BuildingToggleAnimalDoor":
                        {
                            GenericBuilding buildingAt = buildableLocation.getBuildingAt(new Vector2(tileLocation.X, tileLocation.Y)) as GenericBuilding;
                            if (buildingAt != null)
                            {
                                if (Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true))
                                {
                                    buildingAt.ToggleAnimalDoor(who);
                                }
                                __result = true;
                            }
                            break;
                        }
                }
            }
        }

        private static void IsBuildingConstructedPostfix(BuildableGameLocation __instance, ref bool __result, string name)
        {
            if (__result is true)
            {
                return;
            }

            foreach (GenericBuilding building in __instance.buildings.Where(b => b is GenericBuilding))
            {
                if (building is null || building.Model is null || building.Model.ValidOccupantTypes is null)
                {
                    continue;
                }

                if (building.Model.ValidOccupantTypes.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    __result = true;
                    return;
                }
            }
        }

        private static bool BuildStructurePrefix(BuildableGameLocation __instance, ref bool __result, BluePrint structureForPlacement, Vector2 tileLocation, Farmer who, bool magicalConstruction = false, bool skipSafetyChecks = false)
        {
            if (SolidFoundations.buildingManager.GetSpecificBuildingModel(structureForPlacement.name) is ExtendedBuildingModel model && model is not null)
            {
                __result = AttemptToBuildStructure(__instance, structureForPlacement, new GenericBuilding(model, structureForPlacement));
                return false;
            }

            return true;
        }

        internal static bool AttemptToBuildStructure(BuildableGameLocation farm, BluePrint blueprint, Building currentBuilding)
        {
            Vector2 tileLocation = new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
            if (!CanBuildHere(farm, blueprint, tileLocation))
            {
                return false;
            }

            var buildingModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(blueprint.name);
            if (buildingModel is null)
            {
                return false;
            }

            var customBuilding = new GenericBuilding(buildingModel, blueprint, tileLocation) { LocationName = farm.NameOrUniqueName };
            customBuilding.buildingLocation.Value = farm;
            customBuilding.owner.Value = Game1.player.UniqueMultiplayerID;
            if (currentBuilding is GenericBuilding genericBuilding)
            {
                customBuilding.skinID = genericBuilding.skinID;
                customBuilding.resetTexture();
            }

            string finalCheckResult = customBuilding.isThereAnythingtoPreventConstruction(farm, tileLocation);
            if (finalCheckResult != null)
            {
                Game1.addHUDMessage(new HUDMessage(finalCheckResult, Color.Red, 3500f));
                return false;
            }
            for (int y = 0; y < blueprint.tilesHeight; y++)
            {
                for (int x = 0; x < blueprint.tilesWidth; x++)
                {
                    Vector2 currentGlobalTilePosition = new Vector2(tileLocation.X + (float)x, tileLocation.Y + (float)y);
                    farm.terrainFeatures.Remove(currentGlobalTilePosition);
                }
            }

            farm.buildings.Add(customBuilding);
            customBuilding.RefreshModel();
            customBuilding.performActionOnConstruction(farm);
            customBuilding.updateInteriorWarps();

            SolidFoundations.multiplayer.globalChatInfoMessage("BuildingBuild", Game1.player.Name, Utility.AOrAn(blueprint.displayName), blueprint.displayName, Game1.player.farmName);

            return true;
        }


        public static bool CanBuildHere(BuildableGameLocation farm, BluePrint blueprint, Vector2 tileLocation)
        {
            for (int y5 = 0; y5 < blueprint.tilesHeight; y5++)
            {
                for (int x2 = 0; x2 < blueprint.tilesWidth; x2++)
                {
                    farm.pokeTileForConstruction(new Vector2(tileLocation.X + (float)x2, tileLocation.Y + (float)y5));
                }
            }
            foreach (Point additionalPlacementTile in blueprint.additionalPlacementTiles)
            {
                int x5 = additionalPlacementTile.X;
                int y4 = additionalPlacementTile.Y;
                farm.pokeTileForConstruction(new Vector2(tileLocation.X + (float)x5, tileLocation.Y + (float)y4));
            }
            for (int y3 = 0; y3 < blueprint.tilesHeight; y3++)
            {
                for (int x3 = 0; x3 < blueprint.tilesWidth; x3++)
                {
                    Vector2 currentGlobalTilePosition2 = new Vector2(tileLocation.X + (float)x3, tileLocation.Y + (float)y3);
                    if (!farm.isBuildable(currentGlobalTilePosition2))
                    {
                        return false;
                    }
                    foreach (Farmer farmer in farm.farmers)
                    {
                        if (farmer.GetBoundingBox().Intersects(new Rectangle(x3 * 64, y3 * 64, 64, 64)))
                        {
                            return false;
                        }
                    }
                }
            }
            foreach (Point additionalPlacementTile2 in blueprint.additionalPlacementTiles)
            {
                int x4 = additionalPlacementTile2.X;
                int y2 = additionalPlacementTile2.Y;
                Vector2 currentGlobalTilePosition3 = new Vector2(tileLocation.X + (float)x4, tileLocation.Y + (float)y2);
                if (!farm.isBuildable(currentGlobalTilePosition3))
                {
                    return false;
                }
                foreach (Farmer farmer2 in farm.farmers)
                {
                    if (farmer2.GetBoundingBox().Intersects(new Rectangle(x4 * 64, y2 * 64, 64, 64)))
                    {
                        return false;
                    }
                }
            }
            if (blueprint.humanDoor != new Point(-1, -1))
            {
                Vector2 doorPos = tileLocation + new Vector2(blueprint.humanDoor.X, blueprint.humanDoor.Y + 1);
                if (!farm.isBuildable(doorPos) && !farm.isPath(doorPos))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
