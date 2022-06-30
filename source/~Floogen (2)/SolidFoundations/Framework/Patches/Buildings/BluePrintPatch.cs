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
using SolidFoundations.Framework.Utilities.Backport;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace SolidFoundations.Framework.Patches.Buildings
{
    // TODO: When updated to SDV v1.6, delete this patch
    internal class BluePrintPatch : PatchTemplate
    {
        private readonly Type _object = typeof(BluePrint);

        internal BluePrintPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Constructor(typeof(BluePrint), new[] { typeof(string) }), prefix: new HarmonyMethod(GetType(), nameof(BluePrintPrefix)));
        }

        private static bool BluePrintPrefix(BluePrint __instance, string name)
        {
            if (SolidFoundations.buildingManager.GetSpecificBuildingModel(name) is ExtendedBuildingModel buildingData && buildingData is not null)
            {
                __instance.name = name;
                __instance.displayName = TextParser.ParseText(buildingData.Name);
                __instance.description = TextParser.ParseText(buildingData.Description);

                var textureName = _helper.Reflection.GetField<string>(__instance, "textureName");
                textureName.SetValue(buildingData.Texture);

                __instance.tilesWidth = buildingData.Size.X;
                __instance.tilesHeight = buildingData.Size.Y;
                __instance.humanDoor = new Point(buildingData.HumanDoor.X, buildingData.HumanDoor.Y);
                __instance.animalDoor = buildingData.GetAnimalDoorRect().Location;
                __instance.moneyRequired = buildingData.BuildCost;
                __instance.nameOfBuildingToUpgrade = buildingData.BuildingToUpgrade;
                if (SolidFoundations.buildingManager.GetAllBuildingModels().FirstOrDefault(b => b.Name == buildingData.BuildingToUpgrade || b.ID == buildingData.BuildingToUpgrade) is ExtendedBuildingModel upgradeBuildingData && upgradeBuildingData is not null)
                {
                    __instance.nameOfBuildingToUpgrade = upgradeBuildingData.ID;
                }

                __instance.itemsRequired = new Dictionary<int, int>();
                if (buildingData.BuildMaterials != null)
                {
                    foreach (BuildingMaterial buildMaterial in buildingData.BuildMaterials)
                    {
                        // WARNING: Only handling Object (O) qualified types until SDV v1.6
                        if (int.TryParse(buildMaterial.ItemID, out int itemId))
                        {
                            __instance.itemsRequired.Add(itemId, buildMaterial.Amount);
                        }
                    }
                }

                __instance.daysToConstruct = buildingData.BuildDays;
                __instance.additionalPlacementTiles = new List<Point>();
                if (buildingData.AdditionalPlacementTiles != null)
                {
                    foreach (BuildingPlacementTile additionalPlacementTile in buildingData.AdditionalPlacementTiles)
                    {
                        __instance.additionalPlacementTiles.Add(additionalPlacementTile.Tile);
                    }
                }
                __instance.magical = buildingData.Builder == "Wizard";

                try
                {
                    var textureImage = _helper.Reflection.GetField<Texture2D>(__instance, "texture");
                    textureImage.SetValue(Game1.content.Load<Texture2D>(__instance.textureName));
                }
                catch (Exception ex)
                {
                    // Flag failure here
                    SolidFoundations.monitor.Log($"There was an issue setting the texture for {buildingData.ID}", LogLevel.Warn);
                    SolidFoundations.monitor.Log($"Failed to set blueprint.texture via reflection: {ex}", LogLevel.Trace);
                }

                return false;
            }

            return true;
        }
    }
}
