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

using AtraBase.Toolkit.Extensions;

using AtraShared.Utils.Extensions;
using AtraShared.Wrappers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Netcode;

using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace GrowableGiantCrops.Framework.InventoryModels;

/// <summary>
/// A class that represents a fruit tree in the inventory.
/// </summary>
[XmlType("Mods_atravita_InventoryFruitTree")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Keeping like methods together.")]
public sealed class InventoryFruitTree : SObject
{
    #region consts

    /// <summary>
    /// A prefix used on the name of a tree in the inventory.
    /// </summary>
    internal const string InventoryTreePrefix = "atravita.InventoryFruitTree/";

    /// <summary>
    /// The mod data key used to keep track of a placed tree.
    /// </summary>
    internal const string ModDataKey = $"{InventoryTreePrefix}Id";

    /// <summary>
    /// The category number for inventory trees.
    /// </summary>
    internal const int InventoryTreeCategory = -645548;
    #endregion

    /// <summary>
    /// The growth stage.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Public for serializer.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Reviewed.")]
    public readonly NetInt growthStage = new(FruitTree.seedStage);

    /// <summary>
    /// The number of days until the fruit tree is mature.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Public for serializer.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Reviewed.")]
    public readonly NetInt daysUntilMature = new(28);

    /// <summary>
    /// Whether or not the tree has been struck by lightning.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Public for serializer.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Reviewed.")]
    public readonly NetInt struckByLightning = new(0);

    [XmlIgnore]
    private Rectangle sourceRect = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryFruitTree"/> class.
    /// This is for the serializer, do not use.
    /// </summary>
    public InventoryFruitTree()
        : base()
    {
        this.NetFields.AddFields(this.daysUntilMature, this.struckByLightning, this.growthStage);
        this.Category = InventoryTreeCategory;
        this.Price = 0;
        this.CanBeSetDown = true;
        this.Edibility = inedible;
        this.bigCraftable.Value = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryFruitTree"/> class.
    /// </summary>
    /// <param name="saplingIndex">The index of the sapling the tree corresponds to.</param>
    /// <param name="initialStack">Initial stack.</param>
    /// <param name="growthStage">Growth stage of the tree.</param>
    /// <param name="daysUntilMature">Number of days until the tree is mature.</param>
    /// <param name="struckByLightning">Whether or not the tree has been struck by lightning.</param>
    public InventoryFruitTree(int saplingIndex, int initialStack, int growthStage,  int daysUntilMature, int struckByLightning)
        : this()
    {
        if (!IsValidFruitTree(saplingIndex))
        {
            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>(@"Data\fruitTrees");
            int replacement = data.Keys.FirstOrDefault();
            ModEntry.ModMonitor.Log($"Tree {saplingIndex} doesn't seem to be a valid tree. Setting to default: {replacement}", LogLevel.Error);
            saplingIndex = replacement;
        }

        this.ParentSheetIndex = saplingIndex;
        this.daysUntilMature.Value = daysUntilMature;
        this.struckByLightning.Value = struckByLightning;
        this.growthStage.Value = growthStage;
        this.Stack = initialStack;
        this.Name = InventoryTreePrefix + GGCUtils.GetNameOfSObject(saplingIndex);
    }

    /// <summary>
    /// Gets the source rect associated with this inventory fruit tree.
    /// </summary>
    [XmlIgnore]
    internal Rectangle SourceRect
    {
        get
        {
            if (this.sourceRect == default)
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
        if (!IsValidFruitTree(this.ParentSheetIndex))
        {
            return false;
        }

        int x = (int)tile.X;
        int y = (int)tile.Y;
        return (GGCUtils.CanPlantTreesAtLocation(l, relaxed, x, y, false) || l.CanPlantTreesHere(69, x, y)) // 69 - banana tree.
            && l.terrainFeatures?.ContainsKey(tile) == false
            && GGCUtils.IsTilePlaceableForResourceClump(l, x, y, relaxed)
            && (relaxed || (!FruitTree.IsGrowthBlocked(tile, l) && !l.HasTreeInRadiusTwo(x, y)));
    }

    /// <inheritdoc />
    public override bool placementAction(GameLocation location, int x, int y, Farmer? who = null)
        => this.PlaceFruitTree(location, x, y, ModEntry.Config.RelaxedPlacement);

    /// <summary>
    /// Places this fruit tree at this location.
    /// </summary>
    /// <param name="location">Gamelocation to place at.</param>
    /// <param name="x">nonTileX.</param>
    /// <param name="y">nonTileY.</param>
    /// <param name="relaxed">Whether or not to use relaxed placement rules.</param>
    /// <returns>True for placed, false otherwise.</returns>
    internal bool PlaceFruitTree(GameLocation location, int x, int y, bool relaxed)
    {
        Vector2 placementTile = new(x / Game1.tileSize, y / Game1.tileSize);
        if (!this.CanPlace(location, placementTile, relaxed))
        {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
            return false;
        }

        FruitTree fruitTree = new(this.ParentSheetIndex, this.growthStage.Value)
        {
            GreenHouseTree = location.IsGreenhouse || ((this.ParentSheetIndex == 69 || this.ParentSheetIndex == 835) && location is IslandWest),
            GreenHouseTileTree = location.doesTileHavePropertyNoNull((int)placementTile.X, (int)placementTile.Y, "Type", "Back") == "Stone",
            currentTileLocation = placementTile,
        };
        fruitTree.struckByLightningCountdown.Value = this.struckByLightning.Value;
        fruitTree.daysUntilMature.Value = this.daysUntilMature.Value;
        if (ModEntry.Config.PreserveModData)
        {
            fruitTree.modData.CopyModDataFrom(this.modData);
        }
        fruitTree.modData?.SetInt(ModDataKey, this.ParentSheetIndex);

        fruitTree.shake(placementTile, true, location);
        location.terrainFeatures[placementTile] = fruitTree;
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
        if (this.sourceRect == default)
        {
            this.PopulateDrawFields();
        }

        if (this.sourceRect != default)
        {
            Vector2 position = Game1.GlobalToLocal(
                Game1.viewport,
                new Vector2((xNonTile * 64) - Game1.tileSize, (yNonTile * 64) - (this.sourceRect.Height * 4) + 64));
            spriteBatch.Draw(
                texture: FruitTree.texture,
                position,
                sourceRectangle: this.sourceRect,
                color: (this.struckByLightning.Value > 0 ? Color.Gray : Color.White) * alpha,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Vector2.One * Game1.pixelZoom,
                effects: SpriteEffects.None,
                layerDepth);
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
        if (this.sourceRect == default)
        {
            this.PopulateDrawFields();
        }

        if (this.sourceRect != default)
        {
            spriteBatch.Draw(
                FruitTree.texture,
                location + new Vector2(20f, 16f),
                this.sourceRect,
                (this.struckByLightning.Value > 0 ? Color.Gray : color) * transparency,
                0f,
                new Vector2(8f, 16f),
                this.GetScaleSize() * scaleSize,
                SpriteEffects.None,
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
        if (this.sourceRect == default)
        {
            this.PopulateDrawFields();
        }

        if (this.sourceRect != default)
        {
            int xOffset = (this.sourceRect.Width - 16) * 2;
            objectPosition.X -= xOffset;
            int yOffset = Math.Max((this.sourceRect.Height * Game1.pixelZoom) - (2 * Game1.tileSize), 0);
            objectPosition.Y -= yOffset;
            spriteBatch.Draw(
                texture: FruitTree.texture,
                position: objectPosition,
                sourceRectangle: this.sourceRect,
                color: this.struckByLightning.Value > 0 ? Color.Gray : Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: Math.Max(0f, (f.getStandingY() + 3) / 10000f));
        }
    }

    private float GetScaleSize() => 0.8f;

    #endregion

    #region misc

    /// <inheritdoc />
    public override Item getOne()
    {
        InventoryFruitTree fruitTree = new(
            saplingIndex: this.ParentSheetIndex,
            initialStack: 1,
            growthStage: this.growthStage.Value,
            daysUntilMature: this.daysUntilMature.Value,
            struckByLightning: this.struckByLightning.Value);
        fruitTree._GetOneFrom(this);
        return fruitTree;
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
    public override string getCategoryName() => I18n.FruitTreeCategory();

    /// <inheritdoc />
    public override Color getCategoryColor() => Color.BlueViolet;

    /// <inheritdoc />
    public override bool isPlaceable() => true;

    /// <inheritdoc />
    public override bool canBePlacedInWater() => false;

    /// <inheritdoc />
    public override bool canStackWith(ISalable other)
    {
        if (other is not InventoryFruitTree otherFruitTree)
        {
            return false;
        }
        return this.ParentSheetIndex == otherFruitTree.ParentSheetIndex
            && this.growthStage.Value == otherFruitTree.growthStage.Value
            && this.daysUntilMature.Value == otherFruitTree.daysUntilMature.Value
            && this.struckByLightning.Value == otherFruitTree.struckByLightning.Value
            && (!ModEntry.Config.PreserveModData || this.modData.ModDataMatches(otherFruitTree.modData));
    }

    /// <inheritdoc />
    public override bool isForage(GameLocation location) => false;

    /// <inheritdoc />
    protected override string loadDisplayName() => I18n.FruitTree_Name(this.GetSaplingDisplayName());

    /// <inheritdoc />
    public override string getDescription() => I18n.FruitTree_Description(this.GetSaplingDisplayName());

    /// <inheritdoc />
    protected override void _PopulateContextTags(HashSet<string> tags)
    {
        tags.Add("category_inventory_fruit_tree");
        tags.Add($"id_inventoryFruitTree_{this.ParentSheetIndex}");
        tags.Add("quality_none");
        tags.Add("item_" + this.SanitizeContextTag(this.Name));
    }

    private string GetSaplingDisplayName()
    {
        if (Game1Wrappers.ObjectInfo.TryGetValue(this.ParentSheetIndex, out string? data))
        {
            return data.GetNthChunk('/', objectInfoDisplayNameIndex).ToString();
        }
        else
        {
            return "UNKNOWN";
        }
    }
    #endregion

    #region helpers

    /// <summary>
    /// Does this sapling index correspond to a valid fruit tree.
    /// </summary>
    /// <param name="saplingIndex">The index of the sapling.</param>
    /// <returns>True if it corresponds to a key in Data\fruitTrees, false otherwise.</returns>
    internal static bool IsValidFruitTree(int saplingIndex)
    {
        try
        {
            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>(@"Data\fruitTrees");
            return data.ContainsKey(saplingIndex);
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed to load fruit tree asset\n\n{ex}", LogLevel.Error);
        }
        return false;
    }

    /// <summary>
    /// resets the source rectangle, used to transition between maps of different seasons.
    /// </summary>
    internal void Reset()
    {
        if (this.growthStage.Value >= FruitTree.treeStage)
        {
            this.sourceRect = default;
        }
    }

    /// <summary>
    /// Populates the fields required for drawing for this fruit tree.
    /// </summary>
    /// <param name="loc">The game location, or null for current.</param>
    internal void PopulateDrawFields(GameLocation? loc = null)
    {
        loc ??= Game1.currentLocation;
        if (loc is null)
        {
            return;
        }

        Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>(@"Data\fruitTrees");
        if (!data.TryGetValue(this.ParentSheetIndex, out string? treeInfo)
            || !int.TryParse(treeInfo.GetNthChunk('/'), out int treeIndex))
        {
            return;
        }

        // derived from FruitTree.draw
        int season = Utility.getSeasonNumber(loc is Desert or MineShaft ? "spring" : Game1.GetSeasonForLocation(loc));

        const int HEIGHT = 80;
        const int WIDTH = 48;
        this.sourceRect = this.growthStage.Value switch
        {
            0 => new Rectangle(0, treeIndex * HEIGHT, WIDTH, HEIGHT),
            1 or 2 or 3 => new Rectangle(WIDTH * this.growthStage.Value, treeIndex * HEIGHT, WIDTH, HEIGHT),
            _ => new Rectangle((season * WIDTH) + (WIDTH * 4), treeIndex * HEIGHT, WIDTH, HEIGHT),
        };
    }

    #endregion
}
