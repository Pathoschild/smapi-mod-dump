/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Xml.Serialization;

using AtraCore.Framework.ReflectionManager;

using AtraShared.Utils.Extensions;

using GrowableGiantCrops.Framework.Assets;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Netcode;

using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace GrowableGiantCrops.Framework.InventoryModels;

/// <summary>
/// A class that represents a normal tree in the inventory.
/// </summary>
[XmlType("Mods_atravita_InventoryTree")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Keeping like methods together.")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Keeping like methods together.")]
public sealed class InventoryTree : SObject
{
    #region consts

    /// <summary>
    /// A prefix used on the name of a tree in the inventory.
    /// </summary>
    internal const string InventoryTreePrefix = "atravita.InventoryTree/";

    /// <summary>
    /// A mod data string used to mark trees planted from these items.
    /// </summary>
    internal const string ModDataKey = $"{InventoryTreePrefix}Type";

    /// <summary>
    /// The category number for inventory trees.
    /// </summary>
    internal const int InventoryTreeCategory = -645547;
    #endregion

    /// <summary>
    /// The growth stage.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Public for serializer.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Reviewed.")]
    public readonly NetInt growthStage = new(Tree.seedStage);

    /// <summary>
    /// The growth stage.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Public for serializer.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Reviewed.")]
    public readonly NetBool isStump = new(false);

    #region drawfields
    [XmlIgnore]
    private AssetHolder? holder;

    [XmlIgnore]
    private Rectangle sourceRect = default;
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryTree"/> class.
    /// This is for the serializer, do not use.
    /// </summary>
    public InventoryTree()
        : base()
    {
        this.NetFields.AddFields(this.growthStage, this.isStump);
        this.Category = InventoryTreeCategory;
        this.Price = 0;
        this.CanBeSetDown = true;
        this.Edibility = inedible;
        this.bigCraftable.Value = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryTree"/> class.
    /// </summary>
    /// <param name="idx">The index of the tree.</param>
    /// <param name="initialStack">The initial stack.</param>
    /// <param name="growthStage">The growth stage.</param>
    /// <param name="isStump">Whether or not the tree is a stump.</param>
    public InventoryTree(TreeIndexes idx, int initialStack, int growthStage, bool isStump = false)
        : this()
    {
        if (!TreeIndexesExtensions.IsDefined(idx))
        {
            ModEntry.ModMonitor.Log($"Tree {idx.ToStringFast()} doesn't seem to be a valid tree. Setting to pine tree.", LogLevel.Error);
            idx = TreeIndexes.Pine;
        }

        this.ParentSheetIndex = (int)idx;
        this.growthStage.Value = Math.Clamp(growthStage, Tree.seedStage, Tree.treeStage);
        this.isStump.Value = isStump;
        this.Stack = initialStack;
        this.Name = InventoryTreePrefix + idx.ToStringFast();
    }

    #region reflection

    /// <summary>
    /// Stardew's Tree::shake.
    /// </summary>
    internal static readonly TreeShakeDel TreeShakeMethod = typeof(Tree)
        .GetCachedMethod("shake", ReflectionCache.FlagTypes.InstanceFlags)
        .CreateDelegate<TreeShakeDel>();

    /// <summary>
    /// A delegate that matches Tree.shake's call pattern.
    /// </summary>
    /// <param name="tree">The tree to shake.</param>
    /// <param name="tileLocation">the tile location of the tree.</param>
    /// <param name="doEvenIfStillShaking">Whether or not to shake the tree even if it's still shaking.</param>
    /// <param name="location">The relevant game location.</param>
    internal delegate void TreeShakeDel(
        Tree tree,
        Vector2 tileLocation,
        bool doEvenIfStillShaking,
        GameLocation location);

    /// <summary>
    /// Stardew's performSeedDestory.
    /// </summary>
    internal static readonly SeedDestroy SeedDestoryMethod = typeof(Tree)
        .GetCachedMethod("performSeedDestroy", ReflectionCache.FlagTypes.InstanceFlags)
        .CreateDelegate<SeedDestroy>();

    /// <summary>
    /// A delegate that matches the pattern of Stardew's performSeedDestroy.
    /// </summary>
    /// <param name="tree">The tree instance.</param>
    /// <param name="t">The tool used.</param>
    /// <param name="tileLocation">The tile location.</param>
    /// <param name="location">The game location.</param>
    internal delegate void SeedDestroy(
        Tree tree,
        Tool t,
        Vector2 tileLocation,
        GameLocation location);
    #endregion

    /// <summary>
    /// Gets the texture path of this inventory tree, if it was found.
    /// </summary>
    [XmlIgnore]
    internal string? TexturePath
    {
        get
        {
            if (this.sourceRect == default || this.holder is null)
            {
                this.PopulateDrawFields();
            }
            return this.holder?.TextureName;
        }
    }

    /// <summary>
    /// Gets the source rect associated with this inventory tree.
    /// </summary>
    [XmlIgnore]
    internal Rectangle SourceRect
    {
        get
        {
            if (this.sourceRect == default || this.holder is null)
            {
                this.PopulateDrawFields();
            }
            return this.sourceRect;
        }
    }

    #region placement

    /// <inheritdoc />
    public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        => this.CanPlace(l, tile, ModEntry.Config.RelaxedPlacement);

    /// <summary>
    /// Checks to see if a tree can be placed here.
    /// </summary>
    /// <param name="l">The game location.</param>
    /// <param name="tile">The tile to place at.</param>
    /// <param name="relaxed">Whether or not relaxed placement rules should be used.</param>
    /// <returns>True if placement should be allowed, false otherwise.</returns>
    internal bool CanPlace(GameLocation l, Vector2 tile, bool relaxed)
    {
        TreeIndexes tree = (TreeIndexes)this.ParentSheetIndex;
        if (tree == TreeIndexes.Invalid || !TreeIndexesExtensions.IsDefined(tree))
        {
            return false;
        }

        int x = (int)tile.X;
        int y = (int)tile.Y;

        return (GGCUtils.CanPlantTreesAtLocation(l, relaxed, x, y, true) || l.CanPlantTreesHere(309, x, y)) // 309 is one of the wild trees.
            && l.terrainFeatures?.ContainsKey(tile) == false
            && GGCUtils.IsTilePlaceableForResourceClump(l, x, y, relaxed)
            && (relaxed || this.isStump.Value || this.growthStage.Value < Tree.treeStage || !HasAdultTreesAround(l, x, y));
    }

    /// <inheritdoc />
    public override bool placementAction(GameLocation location, int x, int y, Farmer? who = null)
        => this.PlaceTree(location, x, y, ModEntry.Config.RelaxedPlacement);

    /// <summary>
    /// Places this tree at this location.
    /// </summary>
    /// <param name="location">Gamelocation to place at.</param>
    /// <param name="x">nonTileX.</param>
    /// <param name="y">nonTileY.</param>
    /// <param name="relaxed">Whether or not to use relaxed placement rules.</param>
    /// <returns>True for placed, false otherwise.</returns>
    internal bool PlaceTree(GameLocation location, int x, int y, bool relaxed)
    {
        Vector2 placementTile = new(x / Game1.tileSize, y / Game1.tileSize);
        if (!this.CanPlace(location, placementTile, relaxed))
        {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
            return false;
        }

        Tree tree = new(this.ParentSheetIndex, this.growthStage.Value)
        {
            currentTileLocation = placementTile,
        };
        tree.stump.Value = this.isStump.Value;
        if (ModEntry.Config.PreserveModData)
        {
            tree.modData.CopyModDataFrom(this.modData);
        }
        tree.modData?.SetEnum(ModDataKey, (TreeIndexes)this.ParentSheetIndex);

        if ((this.ParentSheetIndex == Tree.mushroomTree || (tree.IsPalmTree() && ModEntry.Config.PalmTreeBehavior.HasFlagFast(PalmTreeBehavior.Stump)))
            && this.growthStage.Value == Tree.treeStage
            && location.IsOutdoors && Game1.GetSeasonForLocation(location) == "winter")
        {
            tree.stump.Value = true;
        }

        location.terrainFeatures[placementTile] = tree;
        TreeShakeMethod(tree, placementTile, true, location);
        location.playSound("dirtyHit");
        DelayedAction.playSoundAfterDelay("coin", 100);
        return true;
    }

    #endregion

    #region draw

    /// <inheritdoc />
    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
    {
        float draw_layer = Math.Max(
            0f,
            ((y * Game1.tileSize) + 40) / 10000f) + (x * 1E-05f);
        this.draw(spriteBatch, x, y, draw_layer, alpha);
    }

    /// <inheritdoc />
    public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
    {
        if (this.sourceRect == default || this.holder is null)
        {
            this.PopulateDrawFields();
        }

        if (this.sourceRect != default && this.holder?.Get() is Texture2D tex)
        {
            Vector2 position = Game1.GlobalToLocal(
                Game1.viewport,
                new Vector2((xNonTile * Game1.tileSize) - (this.sourceRect.Width == 48 ? Game1.tileSize : 0), (yNonTile * Game1.tileSize) - (this.sourceRect.Height * Game1.pixelZoom) + Game1.tileSize));
            if (this.growthStage.Value == Tree.treeStage && !this.isStump.Value)
            {
                Vector2 stump = Game1.GlobalToLocal(
                    Game1.viewport,
                    new Vector2(xNonTile * Game1.tileSize, (yNonTile * Game1.tileSize) - Game1.tileSize));
                spriteBatch.Draw(
                    texture: tex,
                    position: stump,
                    sourceRectangle: new Rectangle(32, 96, 16, 32),
                    color: Color.White * alpha,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: Vector2.One * Game1.pixelZoom,
                    effects: SpriteEffects.None,
                    layerDepth);
            }
            spriteBatch.Draw(
                texture: tex,
                position,
                sourceRectangle: this.sourceRect,
                color: Color.White * alpha,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Vector2.One * Game1.pixelZoom,
                effects: SpriteEffects.None,
                layerDepth + 0.01f);
        }
    }

    /// <inheritdoc />
    public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
    {
        Vector2 grabTile = Game1.GetPlacementGrabTile();
        int x = (int)grabTile.X * Game1.tileSize;
        int y = (int)grabTile.Y * Game1.tileSize;
        Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
        if (Game1.isCheckingNonMousePlacement)
        {
            Vector2 nearbyValidPlacementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, location, this, x, y);
            x = (int)nearbyValidPlacementPosition.X;
            y = (int)nearbyValidPlacementPosition.Y;
        }

        bool canPlaceHere = Utility.playerCanPlaceItemHere(location, this, x, y, Game1.player) && Utility.withinRadiusOfPlayer(x, y, 1, Game1.player);
        spriteBatch.Draw(
            texture: Game1.mouseCursors,
            new Vector2(x - Game1.viewport.X, y - Game1.viewport.Y),
            new Rectangle(canPlaceHere ? 194 : 210, 388, 16, 16),
            color: Color.White,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: Game1.pixelZoom,
            effects: SpriteEffects.None,
            layerDepth: 0.01f);
        this.draw(spriteBatch, x / Game1.tileSize, y / Game1.tileSize, 0.5f);
    }

    /// <inheritdoc />
    public override void drawAsProp(SpriteBatch b)
    {
        this.draw(b, (int)this.TileLocation.X, (int)this.TileLocation.Y);
    }

    /// <inheritdoc />
    public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        if (this.sourceRect == default || this.holder is null)
        {
            this.PopulateDrawFields();
        }

        if (this.sourceRect != default && this.holder?.Get() is Texture2D tex)
        {
            Vector2 offset;
            if (this.isStump.Value)
            {
                offset = new Vector2(32f, 32f);
            }
            else
            {
                offset = this.growthStage.Value switch
                {
                    Tree.treeStage => scaleSize >= 0.95f ? new Vector2(16f, 0f) : new Vector2(16f, 8f),
                    Tree.bushStage or 4 => new Vector2(32f, 32f),
                    _ => new Vector2(32f, 64f),
                };
            }
            spriteBatch.Draw(
                texture: tex,
                position: location + offset,
                sourceRectangle: this.sourceRect,
                color: color * transparency,
                rotation: 0f,
                new Vector2(8f, 16f),
                scale: this.GetScaleSize() * scaleSize,
                effects: SpriteEffects.None,
                layerDepth);
            if (((drawStackNumber == StackDrawType.Draw && this.maximumStackSize() > 1 && this.Stack > 1) || drawStackNumber == StackDrawType.Draw_OneInclusive)
                && scaleSize > 0.3f && this.Stack != int.MaxValue)
            {
                Utility.drawTinyDigits(
                    toDraw: this.Stack,
                    b: spriteBatch,
                    position: location + new Vector2(64 - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize) + (3f * scaleSize), 64f - (18f * scaleSize) + 2f),
                    scale: 3f * scaleSize,
                    layerDepth: 1f,
                    c: Color.White);
            }
        }
    }

    /// <inheritdoc />
    public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
    {
        if (this.sourceRect == default || this.holder is null)
        {
            this.PopulateDrawFields();
        }
        if (this.sourceRect != default && this.holder?.Get() is Texture2D tex)
        {
            float layerDepth = Math.Max(0f, (f.getStandingY() + 3) / 10000f);
            objectPosition.Y -= 2 * Game1.tileSize;
            if (this.growthStage.Value == Tree.treeStage && !this.isStump.Value)
            {
                spriteBatch.Draw(
                    texture: tex,
                    position: objectPosition + new Vector2(0, 128),
                    sourceRectangle: new Rectangle(32, 96, 16, 32),
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: Vector2.One * Game1.pixelZoom,
                    effects: SpriteEffects.None,
                    layerDepth);
            }

            int xOffset = (this.sourceRect.Width - 16) * 2;
            objectPosition.X -= xOffset;
            int yOffset = (this.sourceRect.Height * Game1.pixelZoom) - (4 * Game1.tileSize);
            objectPosition.Y -= yOffset;
            spriteBatch.Draw(
                texture: tex,
                position: objectPosition,
                sourceRectangle: this.sourceRect,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: layerDepth + 0.01f);
        }
    }

    private float GetScaleSize()
    {
        if (this.sourceRect == default || this.holder is null)
        {
            this.PopulateDrawFields();
        }
        return 64 / Math.Clamp(this.sourceRect.Height, 1, 64);
    }
    #endregion

    #region misc

    /// <inheritdoc />
    public override Item getOne()
    {
        InventoryTree tree = new((TreeIndexes)this.ParentSheetIndex, 1, this.growthStage.Value, this.isStump.Value);
        tree._GetOneFrom(this);
        return tree;
    }

    /// <inheritdoc />
    public override int maximumStackSize() => ModEntry.Config.AllowLargeItemStacking ? 999 : 1;

    /// <inheritdoc />
    public override bool canBeShipped() => false;

    /// <inheritdoc />
    public override bool canBeGivenAsGift() => false;

    /// <inheritdoc />
    public override bool canBeTrashed() => true;

    /// <inheritdoc />
    public override string getCategoryName() => I18n.TreeCategory();

    /// <inheritdoc />
    public override Color getCategoryColor() => Color.ForestGreen;

    /// <inheritdoc />
    public override bool isPlaceable() => true;

    /// <inheritdoc />
    public override bool canBePlacedInWater() => false;

    /// <inheritdoc />
    public override bool isForage(GameLocation location) => false;

    /// <inheritdoc />
    public override bool canStackWith(ISalable other)
    {
        if (other is not InventoryTree tree)
        {
            return false;
        }
        return this.ParentSheetIndex == tree.ParentSheetIndex
            && this.growthStage.Value == tree.growthStage.Value
            && this.isStump.Value == tree.isStump.Value
            && (!ModEntry.Config.PreserveModData || this.modData.ModDataMatches(tree.modData));
    }

    /// <inheritdoc/>
    protected override string loadDisplayName()
        => (TreeIndexes)this.ParentSheetIndex switch
        {
            TreeIndexes.Maple => I18n.Maple_Name(),
            TreeIndexes.Oak => I18n.Oak_Name(),
            TreeIndexes.Pine => I18n.Pine_Name(),
            TreeIndexes.Palm => I18n.Palm_Name(),
            TreeIndexes.BigPalm => I18n.BigPalm_Name(),
            TreeIndexes.Mahogany => I18n.Mahogany_Name(),
            TreeIndexes.Mushroom => I18n.Mushroom_Name(),
            _ => I18n.TreeInvalid_Name(),
        };

    /// <inheritdoc/>
    public override string getDescription()
        => (TreeIndexes)this.ParentSheetIndex switch
        {
            TreeIndexes.Maple => I18n.Maple_Description(),
            TreeIndexes.Oak => I18n.Oak_Description(),
            TreeIndexes.Pine => I18n.Pine_Description(),
            TreeIndexes.Palm => I18n.Palm_Description(),
            TreeIndexes.BigPalm => I18n.BigPalm_Description(),
            TreeIndexes.Mahogany => I18n.Mahogany_Description(),
            TreeIndexes.Mushroom => I18n.Mushroom_Description(),
            _ => I18n.TreeInvalid_Description(),
        };

    /// <inheritdoc />
    protected override void _PopulateContextTags(HashSet<string> tags)
    {
        tags.Add("category_inventory_tree");
        tags.Add($"id_inventoryTree_{this.ParentSheetIndex}");
        tags.Add("quality_none");
        tags.Add("item_" + this.SanitizeContextTag(this.Name));
    }

    #endregion

    #region helpers

    /// <summary>
    /// Resets the draw fields.
    /// </summary>
    internal void Reset()
    {
        // this is necessary for seasons.
        this.holder = null;
        this.sourceRect = default;
    }

    /// <summary>
    /// Populates the fields required for drawing for this particular location.
    /// </summary>
    /// <param name="loc">Gamelocation.</param>
    internal void PopulateDrawFields(GameLocation? loc = null)
    {
        loc ??= Game1.currentLocation;
        if (loc is null)
        {
            return;
        }

        #warning probably need to fix this in 1.6

        // derived from Tree.loadTexture and Tree.draw
        string season = loc is Desert or MineShaft ? "spring" : Game1.GetSeasonForLocation(loc);

        string assetPath;
        switch (this.ParentSheetIndex)
        {
            case Tree.mushroomTree:
                assetPath = @"TerrainFeatures\mushroom_tree";
                break;
            case Tree.palmTree:
                if (ModEntry.Config.PalmTreeBehavior.HasFlagFast(PalmTreeBehavior.Seasonal))
                {
                    if (season == "fall")
                    {
                        assetPath = AssetManager.FallPalm.BaseName;
                        break;
                    }
                    if (season == "winter")
                    {
                        assetPath = AssetManager.WinterPalm.BaseName;
                        break;
                    }
                }
                assetPath = @"TerrainFeatures\tree_palm";
                break;
            case Tree.palmTree2:
                if (ModEntry.Config.PalmTreeBehavior.HasFlagFast(PalmTreeBehavior.Seasonal))
                {
                    if (season == "fall")
                    {
                        assetPath = AssetManager.FallBigPalm.BaseName;
                        break;
                    }
                    if (season == "winter")
                    {
                        assetPath = AssetManager.WinterBigPalm.BaseName;
                        break;
                    }
                }
                assetPath = @"TerrainFeatures\tree_palm2";
                break;
            case Tree.pineTree:
                if (season == "summer")
                {
                    assetPath = @"TerrainFeatures\tree3_spring";
                    break;
                }
                goto default;
            default:
                assetPath = $@"TerrainFeatures\tree{this.ParentSheetIndex}_{season}";
                break;
        }

        this.holder = AssetCache.Get(assetPath);
        if (this.holder is null)
        {
            return;
        }

        if (this.isStump.Value)
        {
            this.sourceRect = new Rectangle(32, 96, 16, 32);
        }
        else
        {
            this.sourceRect = this.growthStage.Value switch
            {
                0 => new Rectangle(32, 128, 16, 16),
                1 => new Rectangle(0, 128, 16, 16),
                2 => new Rectangle(16, 128, 16, 16),
                3 or 4 => new Rectangle(0, 96, 16, 32),
                _ => new Rectangle(0, 0, 48, 96),
            };
        }
    }

    private static bool HasAdultTreesAround(GameLocation l, int xTile, int yTile)
    {
        for (int x = xTile - 1; x <= xTile + 1; x++)
        {
            for (int y = yTile - 1; y <= yTile + 1; y++)
            {
                if (l.terrainFeatures.TryGetValue(new Vector2(x, y), out TerrainFeature? terrain)
                    && terrain is Tree tree && tree.growthStage.Value >= Tree.treeStage)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion
}
