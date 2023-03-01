/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using AtraBase.Toolkit;
using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.Reflection;

using AtraCore.Framework.ReflectionManager;

using AtraShared.Integrations.Interfaces;
using AtraShared.Utils.Extensions;
using AtraShared.Wrappers;

using GrowableGiantCrops.Framework.Assets;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Netcode;

using StardewValley;
using StardewValley.TerrainFeatures;

namespace GrowableGiantCrops.Framework.InventoryModels;

// NOTE: remember that the lower left corner is the placement corner!

/// <summary>
/// A class that represents a giant crop in the inventory.
/// </summary>
[XmlType("Mods_atravita_InventoryGiantCrop")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Keeping like methods together.")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Keeping like methods together.")]
public sealed class InventoryGiantCrop : SObject
{
    #region consts

    /// <summary>
    /// A prefix used on the name of a giant crop in the inventory.
    /// </summary>
    internal const string InventoryGiantCropPrefix = "atravita.GrowableGiantCrop/";

    /// <summary>
    /// Khloe's mod data key, used to identify her giant crops.
    /// </summary>
    internal const string GiantCropTweaksModDataKey = "leclair.giantcroptweaks/Id";

    internal const string ModDataKey = $"{InventoryGiantCropPrefix}Id";

    /// <summary>
    /// Numeric category ID used to identify Khloe's giant crops.
    /// </summary>
    internal const int GiantCropTweaksCategory = -13376523;

    /// <summary>
    /// Numeric category ID used to identify JA/vanilla giant crops.
    /// </summary>
    internal const int GiantCropCategory = -15577335; // set a large random negative number

    #endregion

    /// <summary>
    /// The string id, used to distinguish GiantCropTweaks giant crops.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Public for serializer.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Reviewed.")]
    public readonly NetString stringID = new(string.Empty);

    #region drawfields
    [XmlIgnore]
    private AssetHolder? holder;

    [XmlIgnore]
    private Rectangle sourceRect = default;

    [XmlIgnore]
    private Point tileSize = default;
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryGiantCrop"/> class.
    /// Used for the serializer, do not use.
    /// </summary>
    public InventoryGiantCrop()
        : base()
    {
        this.NetFields.AddField(this.stringID);
        this.Category = GiantCropCategory;
        this.Price = 0;
        this.CanBeSetDown = true;
        this.Edibility = inedible;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryGiantCrop"/> class for a GiantCropTweaks giant crop.
    /// </summary>
    /// <param name="stringID">the string id of the giantcroptweaks giant crop.</param>
    /// <param name="intID">int id of crop product.</param>
    /// <param name="initialStack">initial stack size.</param>
    public InventoryGiantCrop(string stringID, int intID, int initialStack)
        : this(intID, initialStack)
    {
        this.stringID.Value = stringID;
        this.Category = GiantCropTweaksCategory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryGiantCrop"/> class for vanilla/JA giant crops.
    /// </summary>
    /// <param name="intID">Integer ID to use.</param>
    /// <param name="initialStack">The initial size of the stack.</param>
    public InventoryGiantCrop(int intID, int initialStack)
        : this()
    {
        this.ParentSheetIndex = intID;
        this.Name = InventoryGiantCropPrefix + GGCUtils.GetNameOfSObject(intID);
        this.Stack = initialStack;
    }

    #region reflection

    /// <summary>
    /// A setter to shake a giant crop.
    /// </summary>
    private static readonly Action<GiantCrop, float> GiantCropSetShake = typeof(GiantCrop)
        .GetCachedField("shakeTimer", ReflectionCache.FlagTypes.InstanceFlags)
        .GetInstanceFieldSetter<GiantCrop, float>();

    /// <summary>
    /// Gets a texture path of this giant crop.
    /// </summary>
    [XmlIgnore]
    internal string TexturePath
    {
        get
        {
            if (this.sourceRect == default || this.holder is null)
            {
                this.PopulateTexture();
            }
            return this.holder?.TextureName ?? AssetManager.GiantCropPrefix + this.ParentSheetIndex;
        }
    }

    /// <summary>
    /// Gets the source rect to draw for this giant crop.
    /// </summary>
    [XmlIgnore]
    internal Rectangle SourceRect
    {
        get
        {
            if (this.sourceRect == default || this.holder is null)
            {
                this.PopulateTexture();
            }
            return this.sourceRect;
        }
    }

    /// <summary>
    /// Gets the tile size of this giant crop.
    /// </summary>
    [XmlIgnore]
    internal Point TileSize
    {
        get
        {
            this.PopulateTileSize();
            return this.tileSize;
        }
    }

    /// <summary>
    /// A method to shake a giant crop.
    /// </summary>
    /// <param name="crop">The giant crop to shake.</param>
    internal static void ShakeGiantCrop(GiantCrop crop)
    {
        GiantCropSetShake(crop, 100f);
        crop.NeedsUpdate = true;
    }
    #endregion

    #region placement

    /// <inheritdoc />
    public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        => this.CanPlace(l, tile, ModEntry.Config.RelaxedPlacement);

    /// <summary>
    /// Checks to see if a giant crop can be placed.
    /// </summary>
    /// <param name="l">The game location to check.</param>
    /// <param name="tile">The tile to check.</param>
    /// <param name="relaxed">Whether or not to use relaxed placement rules.</param>
    /// <returns>True if place-able, false otherwise.</returns>
    internal bool CanPlace(GameLocation l, Vector2 tile, bool relaxed)
    {
        if (l.resourceClumps is null || Utility.isPlacementForbiddenHere(l))
        {
            return false;
        }

        if (!IsValidGiantCropIndex(this.ParentSheetIndex))
        {
            return false;
        }

        this.PopulateTileSize();
        if (this.tileSize == default)
        {
            return false;
        }

        for (int x = (int)tile.X; x < (int)tile.X + this.tileSize.X; x++)
        {
            for (int y = (int)tile.Y - this.tileSize.Y + 1; y <= (int)tile.Y; y++)
            {
                if (!GGCUtils.IsTilePlaceableForResourceClump(l, x, y, relaxed)
                    && (!relaxed && !IsTileHoeable(l, x, y)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool IsTileHoeable(GameLocation l, int x, int y)
        => l.doesTileHaveProperty(x, y, "Diggable", "Back") is not null;

    /// <inheritdoc />
    public override bool placementAction(GameLocation location, int x, int y, Farmer? who = null)
        => this.PlaceGiantCrop(location, x, y, ModEntry.Config.RelaxedPlacement);

    /// <summary>
    /// Places a giant crop at this pixel location.
    /// </summary>
    /// <param name="location">Location to place at.</param>
    /// <param name="x">pixel x.</param>
    /// <param name="y">pixel y.</param>
    /// <param name="relaxed">whether or not to use relaxed placement rules.</param>
    /// <returns>True if placed, false otherwise.</returns>
    internal bool PlaceGiantCrop(GameLocation location, int x, int y, bool relaxed)
    {
        this.PopulateTileSize();
        if (this.tileSize == default)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(this.stringID.Value) && ModEntry.GiantCropTweaksAPI?.GiantCrops?.ContainsKey(this.stringID.Value) != true)
        {
            return false;
        }
        else if (!IsValidGiantCropIndex(this.ParentSheetIndex))
        {
            return false;
        }

        Vector2 placementTile = new(x / Game1.tileSize, y / Game1.tileSize);
        if (!this.CanPlace(location, placementTile, relaxed))
        {
            return false;
        }

        placementTile.Y -= this.tileSize.Y - 1;

        GiantCrop giant = new(this.ParentSheetIndex, placementTile);

        if (ModEntry.Config.PreserveModData)
        {
            giant.modData.CopyModDataFrom(this.modData);
        }
        giant.modData?.SetInt(ModDataKey, this.ParentSheetIndex);
        if (!string.IsNullOrEmpty(this.stringID.Value) && giant.modData is not null)
        {
            giant.modData[GiantCropTweaksModDataKey] = this.stringID.Value;
        }

        location.resourceClumps.Add(giant);
        location.playSound("thudStep");
        ShakeGiantCrop(giant);
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
            this.PopulateTexture();
        }

        if (this.sourceRect != default && this.holder?.Get() is Texture2D tex)
        {
            Vector2 position = Game1.GlobalToLocal(
                Game1.viewport,
                new Vector2(xNonTile * Game1.tileSize, (yNonTile * Game1.tileSize) - (this.sourceRect.Height * Game1.pixelZoom) + Game1.tileSize));
            spriteBatch.Draw(
                texture: tex,
                position,
                sourceRectangle: this.sourceRect,
                color: Color.White * alpha,
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
        this.PopulateTileSize();
        if (this.tileSize == default)
        {
            return;
        }

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

        for (int x_offset = 0; x_offset < this.tileSize.X; x_offset++)
        {
            for (int y_offset = 1 - this.tileSize.Y; y_offset <= 0; y_offset++)
            {
                spriteBatch.Draw(
                    texture: Game1.mouseCursors,
                    new Vector2(x + (x_offset * 64) - Game1.viewport.X, y + (y_offset * 64) - Game1.viewport.Y),
                    new Rectangle(canPlaceHere ? 194 : 210, 388, 16, 16),
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: Game1.pixelZoom,
                    effects: SpriteEffects.None,
                    layerDepth: 0.01f);
            }
        }
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
            this.PopulateTexture();
        }

        if (this.sourceRect != default && this.holder?.Get() is Texture2D tex)
        {
            spriteBatch.Draw(
                texture: tex,
                position: location + new Vector2(16f, 16f),
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
            this.PopulateTexture();
        }
        if (this.sourceRect != default && this.holder?.Get() is Texture2D tex)
        {
            int xOffset = (this.sourceRect.Width - 16) * 2;
            objectPosition.X -= xOffset;
            int yOffset = Math.Max((this.sourceRect.Height * Game1.pixelZoom) - Game1.tileSize, 0);
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
                layerDepth: Math.Max(0f, (f.getStandingY() + 3) / 10000f));
        }
    }

    private float GetScaleSize()
    {
        this.PopulateTileSize();
        return 3.5f / Math.Max(1, this.tileSize.X);
    }

    private void PopulateTileSize()
    {
        if (this.tileSize == default)
        {
            if (!string.IsNullOrEmpty(this.stringID.Value)
                && ModEntry.GiantCropTweaksAPI?.GiantCrops?.TryGetValue(this.stringID.Value, out IGiantCropData? data) == true)
            {
                this.tileSize = data.TileSize;
            }
            else
            {
                this.tileSize = new(3, 3);
            }
        }
    }

    /// <summary>
    /// Calculates the correct texture and rectangle to draw.
    /// </summary>
    private void PopulateTexture()
    {
        try
        {
            const int DEFAULT_WIDTH = 48;
            const int DEFAULT_HEIGHT = 64;

            if (!string.IsNullOrEmpty(this.stringID.Value)
                && ModEntry.GiantCropTweaksAPI?.GiantCrops?.TryGetValue(this.stringID.Value, out IGiantCropData? data) == true
                && AssetCache.Get(data.Texture) is AssetHolder holder)
            {
                this.holder = holder;
                _ = ModEntry.GiantCropTweaksAPI.TryGetSource(this.stringID.Value, out Rectangle? rect);
                this.sourceRect = rect ?? new Rectangle(data.Corner, new Point(data.TileSize.X * 16, (data.TileSize.Y * 16) + 16));
            }

            // Check More Giant Crops and Json Assets for texture data.
            else if (ModEntry.JaAPI?.TryGetGiantCropSprite(this.ParentSheetIndex, out Lazy<Texture2D>? jaTex) == true)
            {
                this.holder = new(jaTex.Value);
                this.sourceRect = new Rectangle(0, 0, DEFAULT_WIDTH, DEFAULT_HEIGHT);
            }
            else if (ModEntry.MoreGiantCropsAPI?.GetTexture(this.ParentSheetIndex) is Texture2D mgcTex)
            {
                this.holder = new(mgcTex);
                this.sourceRect = new Rectangle(0, 0, DEFAULT_WIDTH, DEFAULT_HEIGHT);
            }
            else
            {
                int idx = ProductToGameIndex(this.ParentSheetIndex);
                if (idx >= GiantCrop.cauliflower && idx <= GiantCrop.pumpkin)
                {
                    this.holder = AssetCache.Get("TileSheets/crops");
                    this.sourceRect = new Rectangle(112 + (idx * DEFAULT_WIDTH), 512, DEFAULT_WIDTH, DEFAULT_HEIGHT);
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in attempting to acquire texture and boundaries for {this.Name} - {this.ParentSheetIndex}, see log for details.", LogLevel.Error);
            ModEntry.ModMonitor.Log(ex.ToString());
        }
    }

    /// <summary>
    /// Resets the draw fields so they're calculated the next time the giant crop is drawn.
    /// </summary>
    internal void ResetDrawFields()
    {
        this.holder = null;
        this.sourceRect = default;
        this.tileSize = default;
    }

    #endregion

    #region misc

    /// <inheritdoc />
    public override Item getOne()
    {
        InventoryGiantCrop crop = string.IsNullOrEmpty(this.stringID.Value)
            ? new(this.ParentSheetIndex, 1)
            : new(this.stringID.Value, this.ParentSheetIndex, 1);
        crop._GetOneFrom(this);
        return crop;
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
    public override bool isForage(GameLocation location) => false;

    /// <inheritdoc />
    public override string getCategoryName() => I18n.GiantCropCategory();

    /// <inheritdoc />
    public override Color getCategoryColor() => Color.ForestGreen;

    /// <inheritdoc />
    public override bool isPlaceable() => true;

    /// <inheritdoc />
    public override bool canBePlacedInWater() => false;

    /// <inheritdoc />
    public override bool canStackWith(ISalable other)
    {
        if (other is not InventoryGiantCrop otherCrop)
        {
            return false;
        }
        return this.ParentSheetIndex == otherCrop.ParentSheetIndex
            && this.Category == otherCrop.Category
            && this.stringID.Value == otherCrop.stringID.Value
            && (!ModEntry.Config.PreserveModData || this.modData.ModDataMatches(otherCrop.modData));
    }

    /// <inheritdoc />
    protected override string loadDisplayName() => I18n.GiantCrop_Name(this.GetProductDisplayName());

    /// <inheritdoc />
    public override string getDescription() => I18n.GiantCrop_Description(this.GetProductDisplayName());

    /// <inheritdoc />
    protected override void _PopulateContextTags(HashSet<string> tags)
    {
        tags.Add("category_inventory_giant_crop");
        tags.Add($"id_inventoryGiantCrop_{this.ParentSheetIndex}");
        tags.Add("quality_none");
        tags.Add("item_" + this.SanitizeContextTag(this.Name));
    }

    private string GetProductDisplayName()
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
    /// Checks if an index is a valid giant crop index.
    /// </summary>
    /// <param name="idx">Index to check.</param>
    /// <returns>True if it's accounted for.</returns>
    [MethodImpl(TKConstants.Hot)]
    internal static bool IsValidGiantCropIndex(int idx)
    {
        switch (idx)
        {
            case 190:
            case 254:
            case 276:
                return true;
        }

        if (ModEntry.JACropIds.Contains(idx))
        {
            return true;
        }
        return ModEntry.MoreGiantCropsIds.Contains(idx);
    }

    private static int ProductToGameIndex(int productIndex)
        => productIndex switch
        {
            190 => GiantCrop.cauliflower,
            254 => GiantCrop.melon,
            276 => GiantCrop.pumpkin,
            _ => productIndex,
        };
    #endregion

}
