/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using stardew_access.Translation;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Text;

namespace stardew_access.Utils;

public static class TerrainUtils
{
    public static (bool IsWatered, bool IsFertilized, string? CropType, bool IsReadyForHarvest, bool IsDead) GetDirtInfo(HoeDirt dirt)
    {
        return (
            IsWatered: dirt.state.Value == HoeDirt.watered,
            IsFertilized: dirt.HasFertilizer(),
            CropType: dirt.crop != null
                ? dirt.crop.forageCrop.Value
                    ? Translator.Instance.Translate("terrain_util-forage_crop_types", new
                        { type = int.Parse(dirt.crop.whichForageCrop.Value) })
                    : ItemRegistry.GetDataOrErrorItem(dirt.crop.indexOfHarvest.Value).DisplayName
                : null,
            IsReadyForHarvest: dirt.crop != null && dirt.readyForHarvest(),
            IsDead: dirt.crop?.dead.Value ?? false
        );
    }

    public static string? GetDirtInfoString(HoeDirt dirt, bool ignoreIfEmpty = false)
    {
        var dirtDetails = GetDirtInfo(dirt);
        return GetDirtInfoString(dirtDetails, ignoreIfEmpty);
    }

    public static string? GetDirtInfoString((bool IsWatered, bool IsFertilized, string? CropType, bool IsReadyForHarvest, bool IsDead) dirtDetails, bool ignoreIfEmpty = false)
    {
        StringBuilder detailString = new();

        if (MainClass.Config.WateredToggle && dirtDetails.IsWatered)
            detailString.Append($"{Translator.Instance.Translate("terrain_util-crop-watered")} ");
        else if (!MainClass.Config.WateredToggle && !dirtDetails.IsWatered)
            detailString.Append($"{Translator.Instance.Translate("terrain_util-crop-unwatered")} ");

        if (dirtDetails.IsFertilized)
            detailString.Append($"{Translator.Instance.Translate("terrain_util-fertilized")} ");

        if (dirtDetails.CropType != null)
        {
            if (dirtDetails.IsReadyForHarvest)
                detailString.Append($"{Translator.Instance.Translate("terrain_util-harvestable")} ");
            if (dirtDetails.IsDead)
                detailString.Append($"{Translator.Instance.Translate("terrain_util-crop-dead")} ");
            detailString.Append(dirtDetails.CropType);
        }
        else if (!ignoreIfEmpty)
        {
            detailString.Append(Translator.Instance.Translate("terrain_util-crop-soil"));
        }

        string toReturn = detailString.ToString().Trim();
        return toReturn == "" ? null : toReturn;
    }

    public static (string TreeType, int GrowthStage, bool IsHarvestable) GetFruitTreeInfo(FruitTree fruitTree)
    {
        int stage = fruitTree.growthStage.Value;
        string treeType = fruitTree.GetDisplayName();
        bool isHarvestable = fruitTree.fruit.Count > 0;

        return (treeType, stage, isHarvestable);
    }

    public static string GetFruitTreeInfoString((string TreeType, int GrowthStage, bool IsHarvestable) fruitTreeDetails)
    {
        string growthStage = Translator.Instance.Translate("terrain_util-fruit_tree_growth_stage",
            new { stage = fruitTreeDetails.GrowthStage });

        string result = $"{fruitTreeDetails.TreeType} {growthStage}";
        if (fruitTreeDetails.IsHarvestable)
        {
            result = $"{Translator.Instance.Translate("terrain_util-harvestable")} {result}";
        }
        return result;
    }

    public static string GetFruitTreeInfoString(FruitTree fruitTree)
    {
        var fruitTreeDetails = GetFruitTreeInfo(fruitTree);
        return GetFruitTreeInfoString(fruitTreeDetails);
    }

    public static (string TreeType, int GrowthStage, string SeedName, bool IsFertilized, bool isStump, bool isMossy) GetTreeInfo(Tree tree)
    {
        var treeStage = tree.growthStage.Value;
        var treeType = tree.treeType.Value;
        string seedName = ItemRegistry.GetDataOrErrorItem(tree.GetData().SeedItemId).DisplayName;
        return (treeType, treeStage, seedName, tree.fertilized.Value, tree.stump.Value, tree.hasMoss.Value);
    }

    public static string GetTreeInfoString((string TreeType, int GrowthStage, string SeedName, bool IsFertilized, bool isStump, bool isMossy) treeDetails)
    {
        string treeType = "";
        if (int.TryParse(treeDetails.TreeType, out int treeTypeInt))
        {
            treeType = Translator.Instance.Translate("terrain_util-tree_type", new { type = treeTypeInt });
        }
        else
        {
            treeType = Translator.Instance.Translate("terrain_util-tree_type", new { type = treeDetails.TreeType });
        }
        if (treeDetails.GrowthStage == 0)
        {
            if (treeDetails.TreeType is Tree.bushyTree or Tree.leafyTree or Tree.pineTree or Tree.mahoganyTree)
            {
                return (treeDetails.IsFertilized
                    ? $"{Translator.Instance.Translate("terrain_util-fertilized")} "
                    : "") + treeDetails.SeedName;
            }
            else
            {
                return (treeDetails.IsFertilized
                        ? $"{Translator.Instance.Translate("terrain_util-fertilized")} "
                        : "") + $"{treeType} {Translator.Instance.Translate("terrain_util-tree-seedling")}";
            }
        }

        string growthStage = treeDetails.isStump
            ? Translator.Instance.Translate("terrain_util-tree-stump")
            : Translator.Instance.Translate("terrain_util-tree_growth_stage", new { stage = treeDetails.GrowthStage });
        string fertilizedStatus = treeDetails.IsFertilized ? Translator.Instance.Translate("terrain_util-fertilized") + " " : "";
        string mossyStatus = treeDetails.isMossy ? Translator.Instance.Translate("terrain_util-tree-mossy") + " " : "";

        return $"{fertilizedStatus}{mossyStatus}{treeType} {growthStage}";
    }

    public static string GetTreeInfoString(Tree tree)
    {
        var treeDetails = GetTreeInfo(tree);
        return GetTreeInfoString(treeDetails);
    }

    public static string GetCosmeticPlantInfoString(CosmeticPlant cosmeticPlant)
    {
        string name = cosmeticPlant.textureName().ToLower();
        name = name.Replace("terrain", "").Replace("feature", "").Replace("  ", " ").Trim();
        return name;
    }

    public static string GetFlooringInfoString(Flooring flooring)
    {
        // TODO Needs to be checked
        bool isPathway = flooring.whichFloor.Value is Flooring.gravel or Flooring.cobblestone or Flooring.wood or Flooring.ghost;
        bool isSteppingStone = flooring.whichFloor.Value == Flooring.steppingStone;
        string description = isPathway ? "tile_name-pathway" : (isSteppingStone ? "tile_name-stepping_stone" : "tile_name-flooring");
        return Translator.Instance.Translate(description);
    }

    public static string GetGrassInfoString(Grass grass)
    {
        // Must convert to int for fluent to recognize
        var grass_type = (int)grass.grassType.Value;
        object tokens = new
        {
            grass_type
        };
        return Translator.Instance.Translate("tile-grass-name", tokens);
    }

    public static (bool IsTownBush, bool IsHarvestable, int BushType, int Age, string ShakeOff) GetBushInfo(Bush bush)
    {
        // TODO i18n
        string season = bush.Location.GetSeason().ToString();

        string shakeOff = bush.GetShakeOffItem();

        bool isHarvestable = !bush.townBush.Value && bush.tileSheetOffset.Value == 1 && bush.inBloom();
        int bushType = bush.size.Value;

        return (bush.townBush.Value, isHarvestable, bushType, bush.getAge(), shakeOff);
    }

    public static string GetBushInfoString((bool IsTownBush, bool IsHarvestable, int BushType, int Age, string ShakeOff) bushInfo)
    {
        StringBuilder bushInfoString = new();

        // Add the harvest status and item name if it's harvestable
        if (bushInfo.IsHarvestable)
        {
            bushInfoString.Append($"{Translator.Instance.Translate("terrain_util-harvestable")} {ItemRegistry.GetDataOrErrorItem(bushInfo.ShakeOff).DisplayName} ");
        }

        // Add the type of the bush
        if (!MainClass.Config.DisableBushVerbosity)
        {
            if (bushInfo.IsTownBush)
            {
                bushInfoString.Append($"{Translator.Instance.Translate("terrain_util-bush-town")} ");
            }

            if (!(bushInfo.BushType == 4 && bushInfo.IsHarvestable))
                bushInfoString.Append($"{Translator.Instance.Translate("terrain_util-bush_type", new { type = bushInfo.BushType, has_matured = (bushInfo.Age < Bush.daysToMatureGreenTeaBush) ? 1 : 0 })} ");
        } else {
            // only name Tea bushes as they're plantable / harvestable
            if (bushInfo.BushType == Bush.greenTeaBush)
                bushInfoString.Append($"{Translator.Instance.Translate("terrain_util-bush_type", new { type = 3, has_matured = (bushInfo.Age < Bush.daysToMatureGreenTeaBush) ? 1 : 0 })} ");
        }

        // Append the word "Bush" to all except for tea bush
        if (bushInfo.BushType != Bush.greenTeaBush)
            bushInfoString.Append(Translator.Instance.Translate("terrain_util-bush"));

        return bushInfoString.ToString().Trim();
    }
    public static string GetBushInfoString(Bush bush)
    {
        var bushInfo = GetBushInfo(bush);
        return GetBushInfoString(bushInfo);
    }

    public static (string? name, CATEGORY? category) GetTerrainFeatureInfoAndCategory(TerrainFeature? terrainFeature, bool ignoreIfEmpty = false)
    {
        if (terrainFeature == null) return (null, null);
        // Switch on the actual type of the terrain feature
        switch (terrainFeature)
        {
            case LargeTerrainFeature largeTerrainFeature:
                return GetTerrainFeatureInfoAndCategory(largeTerrainFeature, ignoreIfEmpty);
            case HoeDirt dirt:
                CATEGORY cropCategory = CATEGORY.Crops;
                if (dirt.crop != null && dirt.readyForHarvest())
                {
                    cropCategory = CATEGORY.Ready;
                }
                else if (dirt.state.Value != HoeDirt.watered)
                { 
                    cropCategory = CATEGORY.Pending;
                }
                return (GetDirtInfoString(dirt, ignoreIfEmpty), cropCategory);
            case CosmeticPlant cosmeticPlant:
                return (GetCosmeticPlantInfoString(cosmeticPlant), CATEGORY.Furniture);
            case Flooring flooring when MainClass.Config.ReadFlooring:
                return (GetFlooringInfoString(flooring), CATEGORY.Flooring);
            case Flooring _:
                return (null, null);  // Set to None or another suitable default to avoid logging
            case FruitTree fruitTree:
                return (GetFruitTreeInfoString(fruitTree), fruitTree.fruit.Count > 0 ? CATEGORY.Ready : CATEGORY.Trees);
            case Grass grass:
                return (GetGrassInfoString(grass), CATEGORY.Debris);
            case Tree tree:
                return (GetTreeInfoString(tree), CATEGORY.Trees);
            default:
                string name = terrainFeature.GetType().Name;
                Log.Warn($"Unknown terrain feature type: {name}", true);
                return (name, CATEGORY.Unknown);
        }
    }

    public static (string? name, CATEGORY? category) GetTerrainFeatureInfoAndCategory(LargeTerrainFeature? largeTerrainFeature, bool ignoreIfEmpty = false)
    {
        if (largeTerrainFeature == null) return (null, null);

        switch (largeTerrainFeature)
        {
            case Bush bush:
                return (GetBushInfoString(bush), (!bush.townBush.Value && bush.tileSheetOffset.Value == 1 && bush.inBloom()) ? CATEGORY.Ready : CATEGORY.Bushes);
            case Tent tent:
                return (Translator.Instance.Translate("terrain_util-tent"), CATEGORY.Buildings);
            // Add more cases for other types of LargeTerrainFeature here
            default:
                string name = largeTerrainFeature.GetType().Name;
                Log.Warn($"Unknown LargeTerrainFeature type: {name}", true);
                return (name, CATEGORY.Unknown);
        }
    }
}
