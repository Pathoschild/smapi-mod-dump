/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;

using GrowableGiantCrops.Framework.Assets;
using GrowableGiantCrops.Framework.InventoryModels;
using GrowableGiantCrops.HarmonyPatches.GrassPatches;

using Microsoft.Xna.Framework;

using StardewValley.TerrainFeatures;

namespace GrowableGiantCrops.Framework;

/// <summary>
/// The API for this mod.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Reviewed.")]
public sealed class Api : IGrowableGiantCropsAPI
{
    #region shovel

    /// <inheritdoc />
    public bool IsShovel(Tool tool) => tool is ShovelTool;

    /// <inheritdoc />
    public Tool GetShovel() => new ShovelTool();

    #endregion

    #region config

    /// <inheritdoc/>
    public int MaxTreeStage => ModEntry.Config.MaxTreeStageInternal;

    /// <inheritdoc/>
    public int MaxFruitTreeStage => ModEntry.Config.MaxFruitTreeStageInternal;

    #endregion

    #region any

    /// <inheritdoc/>
    public bool CanPlace(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
    => obj switch
    {
        InventoryResourceClump clump => this.CanPlaceClump(clump, loc, tile, relaxed),
        InventoryGiantCrop crop => this.CanPlaceGiant(crop, loc, tile, relaxed),
        InventoryFruitTree fruitTree => this.CanPlaceFruitTree(fruitTree, loc, tile, relaxed),
        InventoryTree tree => this.CanPlaceTree(tree, loc, tile, relaxed),
        _ => false,
    };

    /// <inheritdoc/>
    public bool TryPlace(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
    => obj switch
        {
            InventoryResourceClump clump => this.TryPlaceClump(clump, loc, tile, relaxed),
            InventoryGiantCrop crop => this.TryPlaceGiant(crop, loc, tile, relaxed),
            InventoryFruitTree fruitTree => this.TryPlaceFruitTree(fruitTree, loc, tile, relaxed),
            InventoryTree tree => this.TryPlaceTree(tree, loc, tile, relaxed),
            _ => false,
        };

    /// <inheritdoc/>
    public bool CanPickUp(GameLocation loc, Vector2 tile, bool placedOnly = false)
    => loc.GetLargeObjectAtLocation(((int)tile.X * Game1.tileSize) + 32, ((int)tile.Y * Game1.tileSize) + 32, placedOnly) switch
    {
        GiantCrop crop => this.CanPickUpCrop(crop, placedOnly) is not null,
        ResourceClump clump => this.CanPickUpClump(clump, placedOnly) != ResourceClumpIndexes.Invalid,
        _ => false
    };

    /// <inheritdoc />
    public SObject? TryPickUpClumpOrGiantCrop(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        switch (loc.RemoveLargeObjectAtLocation(((int)tile.X * Game1.tileSize) + 32, ((int)tile.Y * Game1.tileSize) + 32, placedOnly))
        {
            case GiantCrop giant:
                if (this.GetMatchingCrop(giant) is InventoryGiantCrop inventoryGiantCrop)
                {
                    return inventoryGiantCrop;
                }
                else
                {
                    loc.resourceClumps.Add(giant);
                    return null;
                }
            case ResourceClump resource:
                if (this.GetMatchingClump(resource) is InventoryResourceClump inventoryResourceClump)
                {
                    return inventoryResourceClump;
                }
                else
                {
                    loc.resourceClumps.Add(resource);
                    return null;
                }
            case LargeTerrainFeature terrain:
                loc.largeTerrainFeatures.Add(terrain);
                return null;
            default:
                return null;
        }
    }

    /// <inheritdoc />
    public SObject? TryPickUpTreeOrFruitTree(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        if (loc.terrainFeatures?.TryGetValue(tile, out TerrainFeature? terrain) == true)
        {
            switch (terrain)
            {
                case FruitTree fruitTree:
                    if (this.CanPickUpFruitTree(fruitTree, placedOnly) != null)
                    {
                        fruitTree.shake(tile, true, loc);
                        if (this.GetMatchingFruitTree(fruitTree) is InventoryFruitTree inventoryFruitTree)
                        {
                            loc.terrainFeatures.Remove(tile);
                            return inventoryFruitTree;
                        }
                    }
                    break;
                case Tree tree:
                    if (this.CanPickUpTree(tree, placedOnly) != TreeIndexes.Invalid)
                    {
                        InventoryTree.TreeShakeMethod(tree, tile, true, loc);
                        if (this.GetMatchingTree(tree) is InventoryTree inventoryTree)
                        {
                            loc.terrainFeatures.Remove(tile);
                            return inventoryTree;
                        }
                    }
                    break;
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public SObject? TryPickUp(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        if (this.TryPickUpClumpOrGiantCrop(loc, tile, placedOnly) is SObject @object)
        {
            return @object;
        }
        if (this.TryPickUpTreeOrFruitTree(loc, tile, placedOnly) is SObject obj)
        {
            return obj;
        }
        return null;
    }

    /// <inheritdoc />
    public void DrawPickUpGraphics(SObject obj, GameLocation loc, Vector2 tile)
    {
        switch (obj)
        {
            case InventoryGiantCrop crop:
                ShovelTool.AddAnimations(loc, tile, crop.TexturePath, crop.SourceRect, crop.TileSize);
                break;
            case InventoryResourceClump clump:
                ShovelTool.AddAnimations(loc, tile, Game1.objectSpriteSheetName, clump.SourceRect, new Point(2, 2));
                break;
            case InventoryTree tree:
            {
                Vector2 offset = new(
                    x: tree.SourceRect.Width / 32,
                    y: tree.SourceRect.Height / 16);
                ShovelTool.AddAnimations(loc, tile - offset, tree.TexturePath, tree.SourceRect, new Point(((int)offset.X * 2) + 1, (int)offset.Y + 1));
                break;
            }
            case InventoryFruitTree fruitTree:
            {
                Vector2 offset = new(
                    x: fruitTree.SourceRect.Width / 32,
                    y: fruitTree.SourceRect.Height / 16);
                ShovelTool.AddAnimations(loc, tile - offset, "TileSheets/fruitTrees", fruitTree.SourceRect, new Point(((int)offset.X * 2) + 1, (int)offset.Y + 1));
                break;
            }
            case SObject @object when @object.bigCraftable.Value:
                ShovelTool.AddAnimations(loc, tile, Game1.objectSpriteSheetName, GameLocation.getSourceRectForObject(@object.ParentSheetIndex), new Point(1, 1));
                break;
        }
    }

    /// <inheritdoc />
    public void DrawAnimations(GameLocation loc, Vector2 tile, string? texturePath, Rectangle sourceRect, Point tileSize)
        => ShovelTool.AddAnimations(loc, tile, texturePath, sourceRect, tileSize);

    #endregion

    #region clumps

    /// <inheritdoc />
    public ResourceClumpIndexes GetIndexOfClumpIfApplicable(SObject obj)
    {
        if (obj is InventoryResourceClump clump)
        {
            ResourceClumpIndexes idx = (ResourceClumpIndexes)clump.ParentSheetIndex;
            if (ResourceClumpIndexesExtensions.IsDefined(idx))
            {
                return idx;
            }
        }
        return ResourceClumpIndexes.Invalid;
    }

    /// <inheritdoc />
    public SObject? GetResourceClump(ResourceClumpIndexes idx, int initialStack = 1)
        => ResourceClumpIndexesExtensions.IsDefined(idx)
        ? new InventoryResourceClump(idx, initialStack)
        : null;

    /// <inheritdoc />
    public bool CanPlaceClump(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryResourceClump clump && clump.CanPlace(loc, tile, relaxed);

    /// <inheritdoc />
    public bool TryPlaceClump(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryResourceClump clump && clump.PlaceResourceClump(loc, (int)tile.X * Game1.tileSize, (int)tile.Y * Game1.tileSize, relaxed);

    /// <inheritdoc />
    public ResourceClumpIndexes CanPickUpClump(GameLocation loc, Vector2 tile, bool placedOnly = false)
        => loc.GetLargeObjectAtLocation(((int)tile.X * Game1.tileSize) + 32, ((int)tile.Y * Game1.tileSize) + 32, placedOnly) switch
            {
                GiantCrop => ResourceClumpIndexes.Invalid,
                ResourceClump clump => this.CanPickUpClump(clump, placedOnly),
                _ => ResourceClumpIndexes.Invalid
            };

    /// <inheritdoc />
    public ResourceClumpIndexes CanPickUpClump(ResourceClump clump, bool placedOnly = false)
    {
        if (clump.GetType() != typeof(ResourceClump))
        {
            return ResourceClumpIndexes.Invalid;
        }
        if (!placedOnly && ResourceClumpIndexesExtensions.IsDefined((ResourceClumpIndexes)clump.parentSheetIndex.Value))
        {
            return (ResourceClumpIndexes)clump.parentSheetIndex.Value;
        }
        return clump.modData?.GetEnum(InventoryResourceClump.ResourceModdata, ResourceClumpIndexes.Invalid) ?? ResourceClumpIndexes.Invalid;
    }

    /// <inheritdoc />
    public SObject? GetMatchingClump(ResourceClump resource)
    {
        if (resource is GiantCrop crop)
        {
            return this.GetMatchingCrop(crop);
        }

        ResourceClumpIndexes idx = (ResourceClumpIndexes)resource.parentSheetIndex.Value;
        if (idx != ResourceClumpIndexes.Invalid && ResourceClumpIndexesExtensions.IsDefined(idx))
        {
            InventoryResourceClump clump = new(idx, 1) { TileLocation = resource.tile.Value };
            if (ModEntry.Config.PreserveModData)
            {
                clump.modData?.CopyModDataFrom(resource.modData);
            }
            return clump;
        }
        return null;
    }

    /// <inheritdoc />
    public SObject? TryPickUpClump(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        switch (loc.RemoveLargeObjectAtLocation(((int)tile.X * Game1.tileSize) + 32, ((int)tile.Y * Game1.tileSize) + 32, placedOnly))
        {
            case GiantCrop giant:
                loc.resourceClumps.Add(giant);
                return null;
            case ResourceClump resource:
                if (this.GetMatchingClump(resource) is InventoryResourceClump inventoryResourceClump)
                {
                    return inventoryResourceClump;
                }
                else
                {
                    loc.resourceClumps.Add(resource);
                    return null;
                }
            case LargeTerrainFeature terrain:
                loc.largeTerrainFeatures.Add(terrain);
                return null;
            default:
                return null;
        }
    }

    #endregion

    #region crops

    /// <inheritdoc />
    public (int idx, string? stringId)? GetIdentifiers(SObject obj)
    {
        if (obj is InventoryGiantCrop crop)
        {
            string? stringID = string.IsNullOrEmpty(crop.stringID.Value) ? null : crop.stringID.Value;
            return (crop.ParentSheetIndex, stringID);
        }
        return null;
    }

    /// <inheritdoc />
    public SObject? GetGiantCrop(int produceIndex, int initialStack)
    => InventoryGiantCrop.IsValidGiantCropIndex(produceIndex)
        ? new InventoryGiantCrop(produceIndex, initialStack)
        : null;

    /// <inheritdoc />
    public SObject? GetGiantCrop(string stringID, int produceIndex, int initialStack)
    => ModEntry.GiantCropTweaksAPI?.GiantCrops?.ContainsKey(stringID) == true
        ? new InventoryGiantCrop(stringID, produceIndex, initialStack)
        : null;

    /// <inheritdoc />
    public SObject? GetMatchingCrop(GiantCrop giant)
    {
        InventoryGiantCrop? inventoryGiantCrop = null;
        if (giant.modData.TryGetValue(InventoryGiantCrop.GiantCropTweaksModDataKey, out string? stringID)
            && ModEntry.GiantCropTweaksAPI?.GiantCrops.ContainsKey(stringID) == true)
        {
            inventoryGiantCrop = new InventoryGiantCrop(stringID, giant.parentSheetIndex.Value, 1);
        }
        else if (InventoryGiantCrop.IsValidGiantCropIndex(giant.parentSheetIndex.Value))
        {
            inventoryGiantCrop = new InventoryGiantCrop(giant.parentSheetIndex.Value, 1);
        }

        if (inventoryGiantCrop is not null)
        {
            if (ModEntry.Config.PreserveModData)
            {
                inventoryGiantCrop.modData?.CopyModDataFrom(giant.modData);
            }
            inventoryGiantCrop.TileLocation = giant.tile.Value;
            return inventoryGiantCrop;
        }
        return null;
    }

    /// <inheritdoc />
    public bool CanPlaceGiant(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryGiantCrop crop && crop.CanPlace(loc, tile, relaxed);

    /// <inheritdoc />
    public bool TryPlaceGiant(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryGiantCrop crop && crop.PlaceGiantCrop(loc, (int)tile.X * Game1.tileSize, (int)tile.Y * Game1.tileSize, relaxed);

    /// <inheritdoc />
    public (int idx, string? stringId)? CanPickUpCrop(GiantCrop crop, bool placedOnly = false)
    {
        if (!placedOnly || crop.modData?.ContainsKey(InventoryGiantCrop.ModDataKey) == true)
        {
            if (crop.modData?.TryGetValue(InventoryGiantCrop.GiantCropTweaksModDataKey, out string? stringID) == true
                && !string.IsNullOrEmpty(stringID))
            {
                return (crop.parentSheetIndex.Value, stringID);
            }
            else if (InventoryGiantCrop.IsValidGiantCropIndex(crop.parentSheetIndex.Value))
            {
                return (crop.parentSheetIndex.Value, null);
            }
        }
        return null;
    }

    /// <inheritdoc />
    public (int idx, string? stringId)? CanPickUpCrop(GameLocation loc, Vector2 tile, bool placedOnly = false)
        => loc.GetLargeObjectAtLocation(((int)tile.X * Game1.tileSize) + 32, ((int)tile.Y * Game1.tileSize) + 32, placedOnly) switch
        {
            GiantCrop crop => this.CanPickUpCrop(crop, placedOnly),
            _ => null,
        };

    /// <inheritdoc />
    public SObject? TryPickUpGiantCrop(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        switch (loc.RemoveLargeObjectAtLocation(((int)tile.X * Game1.tileSize) + 32, ((int)tile.Y * Game1.tileSize) + 32, placedOnly))
        {
            case GiantCrop giant:
                if (this.GetMatchingCrop(giant) is InventoryGiantCrop inventoryGiantCrop)
                {
                    return inventoryGiantCrop;
                }
                else
                {
                    loc.resourceClumps.Add(giant);
                    return null;
                }
            case ResourceClump resource:
                loc.resourceClumps.Add(resource);
                return null;
            case LargeTerrainFeature terrain:
                loc.largeTerrainFeatures.Add(terrain);
                return null;
            default:
                return null;
        }
    }

    #endregion

    #region trees

    /// <inheritdoc />
    public SObject? GetTree(TreeIndexes idx, int initialStack = 1, int growthStage = Tree.bushStage, bool isStump = false)
    {
        if (TreeIndexesExtensions.IsDefined(idx))
        {
            return new InventoryTree(idx, initialStack, growthStage, growthStage == Tree.treeStage && isStump);
        }
        return null;
    }

    /// <inheritdoc />
    public SObject? GetMatchingTree(Tree tree)
    {
        SObject? inventoryTree = this.GetTree((TreeIndexes)tree.treeType.Value, 1, tree.growthStage.Value, tree.stump.Value);
        if (inventoryTree is InventoryTree && ModEntry.Config.PreserveModData)
        {
            inventoryTree.modData?.CopyModDataFrom(tree.modData);
        }
        if (inventoryTree is not null)
        {
            inventoryTree.TileLocation = tree.currentTileLocation;
        }
        return inventoryTree;
    }

    /// <inheritdoc />
    public bool CanPlaceTree(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryTree tree && tree.CanPlace(loc, tile, relaxed);

    /// <inheritdoc />
    public bool TryPlaceTree(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryTree tree && tree.PlaceTree(loc, (int)tile.X * Game1.tileSize, (int)tile.Y * Game1.tileSize, relaxed);

    /// <inheritdoc />
    public TreeIndexes CanPickUpTree(Tree tree, bool placedOnly = false)
    {
        if (!placedOnly || tree.modData?.ContainsKey(InventoryTree.ModDataKey) == true)
        {
            TreeIndexes id = (TreeIndexes)tree.treeType.Value;
            if (TreeIndexesExtensions.IsDefined(id))
            {
                return id;
            }
        }
        return TreeIndexes.Invalid;
    }

    /// <inheritdoc />
    public TreeIndexes CanPickUpTree(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        if (loc?.terrainFeatures?.TryGetValue(tile, out TerrainFeature? terrainFeature) == true
            && terrainFeature is Tree tree)
        {
            return this.CanPickUpTree(tree, placedOnly);
        }
        return TreeIndexes.Invalid;
    }

    /// <inheritdoc />
    public SObject? TryPickUpTree(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        if (loc.terrainFeatures?.TryGetValue(tile, out TerrainFeature? terrain) == true
            && terrain is Tree tree && this.CanPickUpTree(tree, placedOnly) != TreeIndexes.Invalid)
        {
            InventoryTree.TreeShakeMethod(tree, tile, true, loc);
            SObject? inventoryTree = this.GetMatchingTree(tree);
            if (inventoryTree is InventoryTree)
            {
                loc.terrainFeatures.Remove(tile);
            }
            return inventoryTree;
        }

        return null;
    }

    #endregion

    #region fruit trees

    /// <inheritdoc />
    public SObject? GetFruitTree(int saplingIndex, int initialStack, int growthStage, int daysUntilMature, int struckByLightning = 0)
    => InventoryFruitTree.IsValidFruitTree(saplingIndex)
            ? new InventoryFruitTree(saplingIndex, initialStack, growthStage, daysUntilMature, struckByLightning)
            : null;

    /// <inheritdoc />
    public SObject? GetMatchingFruitTree(FruitTree tree)
    {
        if (tree.stump.Value)
        {
            return null;
        }
        int? saplingIndex = AssetManager.GetMatchingSaplingIndex(tree.treeType.Value);
        if (saplingIndex is null)
        {
            return null;
        }
        SObject? inventoryTree = this.GetFruitTree(
            saplingIndex: saplingIndex.Value,
            initialStack: 1,
            growthStage: tree.growthStage.Value,
            daysUntilMature: tree.daysUntilMature.Value,
            struckByLightning: tree.struckByLightningCountdown.Value);
        if (inventoryTree is InventoryFruitTree && ModEntry.Config.PreserveModData)
        {
            inventoryTree.modData?.CopyModDataFrom(tree.modData);
        }
        if (inventoryTree is not null)
        {
            inventoryTree.TileLocation = tree.currentTileLocation;
        }
        return inventoryTree;
    }

    /// <inheritdoc />
    public bool CanPlaceFruitTree(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryFruitTree tree && tree.CanPlace(loc, tile, relaxed);

    /// <inheritdoc />
    public bool TryPlaceFruitTree(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryFruitTree tree && tree.PlaceFruitTree(loc, (int)tile.X * Game1.tileSize, (int)tile.Y * Game1.tileSize, relaxed);

    /// <inheritdoc />
    public (int saplingIndex, int growthStage, int daysUntilMature, int struckByLightning)? CanPickUpFruitTree(FruitTree tree, bool placedOnly = false)
    {
        if (tree.stump.Value)
        {
            return null;
        }
        if (!placedOnly || tree.modData?.ContainsKey(InventoryFruitTree.ModDataKey) == true)
        {
            int? saplingID = AssetManager.GetMatchingSaplingIndex(tree.treeType.Value);
            if (saplingID is not null)
            {
                return (saplingID.Value, tree.growthStage.Value, tree.daysUntilMature.Value, tree.struckByLightningCountdown.Value);
            }
        }
        return null;
    }

    /// <inheritdoc />
    public (int saplingIndex, int growthStage, int daysUntilMature, int struckByLightning)? CanPickUpFruitTree(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        if (loc?.terrainFeatures?.TryGetValue(tile, out TerrainFeature? terrainFeature) == true
            && terrainFeature is FruitTree tree)
        {
            return this.CanPickUpFruitTree(tree, placedOnly);
        }
        return null;
    }

    /// <inheritdoc />
    public SObject? TryPickUpFruitTree(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        if (loc.terrainFeatures?.TryGetValue(tile, out TerrainFeature? terrain) == true
            && terrain is FruitTree tree && this.CanPickUpFruitTree(tree, placedOnly) is not null)
        {
            tree.shake(tile, true, loc);
            SObject? inventoryTree = this.GetMatchingFruitTree(tree);
            if (inventoryTree is InventoryFruitTree)
            {
                loc.terrainFeatures.Remove(tile);
            }
            return inventoryTree;
        }

        return null;
    }

    #endregion

    #region grass

    /// <inheritdoc />
    public Grass? GetMatchingGrass(SObject starter)
    {
        if (starter.ParentSheetIndex != SObjectPatches.GrassStarterIndex)
        {
            return null;
        }

        if (SObjectPatches.IsMoreGrassStarter?.Invoke(starter) == true
            && SObjectPatches.GetMoreGrassStarterIndex?.Invoke(starter) is int moreGrassIdx)
        {
            return SObjectPatches.InstantiateMoreGrassGrass?.Invoke(moreGrassIdx);
        }

        if (starter.modData?.GetInt(SObjectPatches.ModDataKey) is not int idx)
        {
            return new Grass(Grass.springGrass, 4);
        }

        Grass grass = new(idx, 1);
        grass.modData.SetBool(SObjectPatches.ModDataKey, true);

        if (ModEntry.Config.PreserveModData)
        {
            grass.modData?.CopyModDataFrom(starter.modData);
        }
        return grass;
    }

    /// <inheritdoc />
    public SObject GetMatchingStarter(Grass grass)
    {
        SObject? starter = null;
        if (SObjectPatches.IsMoreGrassGrass?.Invoke(grass) == true)
        {
            starter = SObjectPatches.InstantiateMoreGrassStarter?.Invoke(grass.grassType.Value);
        }
        starter ??= new(SObjectPatches.GrassStarterIndex, 1);
        if (ModEntry.Config.PreserveModData)
        {
            starter.modData?.CopyModDataFrom(grass.modData);
        }
        starter.modData?.SetInt(SObjectPatches.ModDataKey, grass.grassType.Value);
        return starter;
    }

    #endregion
}
