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
using AtraCore.Utilities;

using AtraShared.Utils.Extensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

using XLocation = xTile.Dimensions.Location;

namespace GrowableBushes.Framework;

/// <summary>
/// A bush in the inventory.
/// </summary>
[XmlType("Mods_atravita_InventoryBush")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Keeping like methods together.")]
public sealed class InventoryBush : SObject
{
    [XmlIgnore]
    private Rectangle sourceRect = default;

    /// <summary>
    /// The prefix used for the internal name of these bushes.
    /// </summary>
    internal const string BushPrefix = "atravita.InventoryBush.";

    /// <summary>
    /// The moddata string used to mark the bushes planted with this mod.
    /// </summary>
    internal const string BushModData = $"{BushPrefix}Type";

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryBush"/> class.
    /// Constructor for the serializer.
    /// </summary>
    public InventoryBush()
        : base()
    {
        this.Edibility = inedible;
        this.Price = 0;
        this.Category = -15500057; // random negative integer.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryBush"/> class.
    /// </summary>
    /// <param name="whichBush">Which bush this inventory bush corresponds to.</param>
    /// <param name="initialStack">Initial stack of bushes.</param>
    public InventoryBush(BushSizes whichBush, int initialStack)
        : base((int)whichBush, initialStack, false, -1, 0)
    {
        if (!BushSizesExtensions.IsDefined(whichBush))
        {
            ModEntry.ModMonitor.Log($"Bush {whichBush.ToStringFast()} doesn't seem to be a valid bush? Setting as error bush.", LogLevel.Error);
            this.ParentSheetIndex = (int)BushSizes.Invalid;
        }

        this.bigCraftable.Value = true;
        this.CanBeSetDown = true;
        this.Name = BushPrefix + ((BushSizes)this.ParentSheetIndex).ToStringFast();
        this.Edibility = inedible;
        this.Price = 0;
        this.Category = -15500057; // random negative integer.

        // just to make sure the bush texture is loaded.
        _ = Bush.texture.Value;
    }

    #region reflection

    /// <summary>
    /// Stardew's Bush::shake.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Reflection delegate.")]
    internal static readonly BushShakeDel BushShakeMethod = typeof(Bush)
        .GetCachedMethod("shake", ReflectionCache.FlagTypes.InstanceFlags)
        .CreateDelegate<BushShakeDel>();

    internal delegate void BushShakeDel(
        Bush bush,
        Vector2 tileLocation,
        bool doEvenIfStillShaking);

    #endregion

    #region placement

    /// <inheritdoc />
    public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        => this.CanPlace(l, tile, ModEntry.Config.RelaxedPlacement);

    /// <summary>
    /// Checks for placement restrictions.
    /// </summary>
    /// <param name="l">Game location.</param>
    /// <param name="tile">Tile to check.</param>
    /// <param name="relaxed">If we should use relaxed restrictions.</param>
    /// <returns>Whether or not placement is allowed.</returns>
    internal bool CanPlace(GameLocation l, Vector2 tile, bool relaxed)
    {
        if (l.largeTerrainFeatures is null || Utility.isPlacementForbiddenHere(l))
        {
            return false;
        }

        BushSizes size = (BushSizes)this.ParentSheetIndex;
        if (size == BushSizes.Invalid || !BushSizesExtensions.IsDefined(size))
        {
            return false;
        }

        int width = size.GetWidth();
        for (int x = (int)tile.X; x < (int)tile.X + width; x++)
        {
            if (!IsTilePlaceableForBush(l, x, (int)tile.Y, relaxed))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override bool placementAction(GameLocation location, int x, int y, Farmer? who = null)
        => this.PlaceBush(location, x, y, ModEntry.Config.RelaxedPlacement);

    /// <summary>
    /// Places a bush.
    /// </summary>
    /// <param name="location">Game location.</param>
    /// <param name="x">non tile x.</param>
    /// <param name="y">non tile y.</param>
    /// <param name="relaxed">use relaxed placement or not.</param>
    /// <returns>True if placed, false otherwise.</returns>
    internal bool PlaceBush(GameLocation location, int x, int y, bool relaxed)
    {
        if (location.largeTerrainFeatures is null || Utility.isPlacementForbiddenHere(location))
        {
            return false;
        }

        BushSizes size = (BushSizes)this.ParentSheetIndex;
        if (size == BushSizes.Invalid || !BushSizesExtensions.IsDefined(size))
        {
            return false;
        }

        Vector2 placementTile = new(x / Game1.tileSize, y / Game1.tileSize);
        if (!this.CanPlace(location, placementTile, relaxed))
        {
            return false;
        }

        Bush bush = new(placementTile, size.ToStardewBush(), location);

        // set metadata.
        switch (size)
        {
            case BushSizes.SmallAlt:
                bush.tileSheetOffset.Value = 1;
                break;
            case BushSizes.Harvested:
                bush.tileSheetOffset.Value = 0;
                break;
            case BushSizes.Medium:
            case BushSizes.Large:
                bush.townBush.Value = false;
                bush.tileSheetOffset.Value = 0;
                break;
            case BushSizes.Town:
            case BushSizes.TownLarge:
                bush.townBush.Value = true;
                bush.tileSheetOffset.Value = 0;
                break;
        }

        bush.setUpSourceRect();
        bush.modData.SetEnum(BushModData, size);
        location.largeTerrainFeatures.Add(bush);
        location.playSound("thudStep");
        BushShakeMethod(bush, placementTile, true);
        return true;
    }

    #endregion

    #region draw

    /// <inheritdoc />
    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
    {
        float draw_layer = Math.Max(
            0f,
            ((y * 64) + 40) / 10000f) + (x * 1E-05f);
        this.draw(spriteBatch, x, y, draw_layer, alpha);
    }

    /// <inheritdoc />
    public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
    {
        if (this.sourceRect == default)
        {
            this.sourceRect = this.GetSourceRectForSeason(GetSeason(Game1.currentLocation));
        }

        if (this.sourceRect != default)
        {
            Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(xNonTile * 64, (yNonTile * 64) - (this.sourceRect.Height * 4) + 64));
            spriteBatch.Draw(
                texture: Bush.texture.Value,
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
        int x = (int)Game1.GetPlacementGrabTile().X * 64;
        int y = (int)Game1.GetPlacementGrabTile().Y * 64;
        Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
        if (Game1.isCheckingNonMousePlacement)
        {
            Vector2 nearbyValidPlacementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, location, this, x, y);
            x = (int)nearbyValidPlacementPosition.X;
            y = (int)nearbyValidPlacementPosition.Y;
        }

        int width = ((BushSizes)this.ParentSheetIndex).GetWidth();
        bool canPlaceHere = Utility.playerCanPlaceItemHere(location, this, x, y, Game1.player) && Utility.withinRadiusOfPlayer(x, y, 1, Game1.player);
        for (int x_offset = 0; x_offset < width; x_offset++)
        {
            spriteBatch.Draw(
                texture: Game1.mouseCursors,
                new Vector2(x + (x_offset * 64) - Game1.viewport.X, y - Game1.viewport.Y),
                new Rectangle(canPlaceHere ? 194 : 210, 388, 16, 16),
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: 0.01f);
        }
        this.draw(spriteBatch, x / 64, y / 64, 0.5f);
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
            this.sourceRect = this.GetSourceRectForSeason(GetSeason(Game1.currentLocation));
        }

        if (this.sourceRect != default)
        {
            spriteBatch.Draw(
                Bush.texture.Value,
                location + new Vector2(this.sourceRect.Width < this.sourceRect.Height && (BushSizes)this.ParentSheetIndex != BushSizes.Medium ? 32f : 16f, 32f),
                this.sourceRect,
                color * transparency,
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
            this.sourceRect = this.GetSourceRectForSeason(GetSeason(f.currentLocation));
        }
        if (this.sourceRect != default)
        {
            int xOffset = (this.sourceRect.Width - 16) * 2;
            objectPosition.X -= xOffset;
            int yOffset = Math.Max(this.sourceRect.Height - 32, 0);
            objectPosition.Y -= yOffset;
            spriteBatch.Draw(
                texture: Bush.texture.Value,
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

    /// <summary>
    /// Gets the DrawInMenu scaling for a bush.
    /// </summary>
    /// <returns>float scaling factor.</returns>
    private float GetScaleSize() =>
        (BushSizes)this.ParentSheetIndex switch
        {
            BushSizes.Large or BushSizes.TownLarge => 1f,
            BushSizes.Medium => 1.4f,
            _ => 2f
        };

    #endregion

    #region misc

    /// <inheritdoc />
    public override Item getOne()
    {
        InventoryBush bush = new((BushSizes)this.ParentSheetIndex, 1);
        bush._GetOneFrom(this);
        return bush;
    }

    /// <inheritdoc />
    public override int maximumStackSize() => ModEntry.Config.AllowBushStacking ? 999 : 1;

    /// <inheritdoc />
    public override bool canBeShipped() => false;

    /// <inheritdoc />
    public override bool canBeGivenAsGift() => false;

    /// <inheritdoc />
    public override bool canBeTrashed() => true;

    /// <inheritdoc />
    public override string getCategoryName() => I18n.Category();

    /// <inheritdoc />
    public override Color getCategoryColor() => Color.Green;

    /// <inheritdoc />
    public override bool isPlaceable() => true;

    /// <inheritdoc />
    public override bool canBePlacedInWater() => false;

    /// <inheritdoc />
    public override bool isForage(GameLocation location) => false;

    /// <inheritdoc />
    public override bool canStackWith(ISalable other)
    {
        if (other is not InventoryBush otherBush)
        {
            return false;
        }
        return this.ParentSheetIndex == otherBush.ParentSheetIndex
            && (!ModEntry.Config.PreserveModData || this.modData.ModDataMatches(otherBush.modData));
    }

    /// <inheritdoc />
    protected override string loadDisplayName() => (BushSizes)this.ParentSheetIndex switch
    {
        BushSizes.Small => I18n.Bush_Small(),
        BushSizes.Medium => I18n.Bush_Medium(),
        BushSizes.Large => I18n.Bush_Large(),
        BushSizes.SmallAlt => I18n.Bush_Small_Alt(),
        BushSizes.Town => I18n.Bush_Town(),
        BushSizes.TownLarge => I18n.Bush_Town_Big(),
        BushSizes.Walnut => I18n.Bush_Walnut(),
        BushSizes.Harvested => I18n.Bush_Harvested(),
        _ => "Error Bush",
    };

    /// <inheritdoc />
    public override string getDescription() => (BushSizes)this.ParentSheetIndex switch
    {
        BushSizes.Small => I18n.Bush_Small_Description(),
        BushSizes.Medium => I18n.Bush_Medium_Description(),
        BushSizes.Large => I18n.Bush_Large_Description(),
        BushSizes.SmallAlt => I18n.Bush_Small_Alt_Description(),
        BushSizes.Town => I18n.Bush_Town_Description(),
        BushSizes.TownLarge => I18n.Bush_Town_Big_Description(),
        BushSizes.Walnut => I18n.Bush_Walnut_Description(),
        BushSizes.Harvested => I18n.Bush_Harvested_Description(),
        _ => "This should have not have happened. What. You should probably just trash this."
    };

    /// <inheritdoc />
    protected override void _PopulateContextTags(HashSet<string> tags)
    {
        tags.Add("category_inventory_bush");
        tags.Add($"id_inventoryBush_{this.ParentSheetIndex}");
        tags.Add("quality_none");
        tags.Add("item_" + this.SanitizeContextTag(this.Name));
    }

    #endregion

    #region helpers

    /// <summary>
    /// Updates the sourceRect for a new location.
    /// </summary>
    /// <param name="location">The location to update for.</param>
    internal void UpdateForNewLocation(GameLocation location)
    {
        int season = GetSeason(location);
        this.sourceRect = this.GetSourceRectForSeason(season);
    }

    private static bool IsTilePlaceableForBush(GameLocation location, int tileX, int tileY, bool relaxed)
    {
        if (location is null || location.doesTileHaveProperty(tileX, tileY, "Water", "Back") is not null)
        {
            return false;
        }

        Rectangle position = new(tileX * 64, tileY * 64, 64, 64);

        foreach (Farmer farmer in location.farmers)
        {
            if (farmer.GetBoundingBox().Intersects(position))
            {
                return false;
            }
        }

        foreach (Character character in location.characters)
        {
            if (character.GetBoundingBox().Intersects(position))
            {
                return false;
            }
        }

        foreach (LargeTerrainFeature largeTerrainFeature in location.largeTerrainFeatures)
        {
            if (largeTerrainFeature.getBoundingBox().Intersects(position))
            {
                return false;
            }
        }

        if (location is IAnimalLocation hasAnimals)
        {
            foreach (FarmAnimal? animal in hasAnimals.Animals.Values)
            {
                if (animal.GetBoundingBox().Intersects(position))
                {
                    return false;
                }
            }
        }

        if (relaxed)
        {
            return true;
        }

        Vector2 tile = new(tileX, tileY);
        if (location is BuildableGameLocation buildable)
        {
            foreach (Building? building in buildable.buildings)
            {
                if (!building.isTilePassable(tile))
                {
                    return false;
                }
            }
        }

        if (location.terrainFeatures?.ContainsKey(tile) == true
            || location.Objects?.ContainsKey(tile) == true)
        {
            return false;
        }

        return !location.isTileOccupied(tile)
            && location.isTilePassable(new XLocation(tileX, tileY), Game1.viewport);
    }

    private static int GetSeason(GameLocation loc)
    {
        return Utility.getSeasonNumber(loc is Desert? "summer" : Game1.GetSeasonForLocation(loc));
    }

    // derived from Bush.setUpSourceRect
    private Rectangle GetSourceRectForSeason(int season)
    {
        switch ((BushSizes)this.ParentSheetIndex)
        {
            case BushSizes.Small:
                return new Rectangle(season * 32, 224, 16, 32);
            case BushSizes.SmallAlt:
                return new Rectangle((season * 32) + 16, 224, 16, 32);
            case BushSizes.Medium:
                int y = Math.DivRem(season * 64, Bush.texture.Value.Bounds.Width, out int x);
                return new Rectangle(x, y * 48, 32, 48);
            case BushSizes.Town:
                return new Rectangle(season * 32, 96, 32, 32);
            case BushSizes.Large:
                return season switch
                {
                    0 or 1 => new Rectangle(0, 128, 48, 48),
                    2 => new Rectangle(48, 128, 48, 48),
                    _ => new Rectangle(0, 176, 48, 48),
                };
            case BushSizes.TownLarge:
                return season switch
                {
                    0 or 1 => new Rectangle(48, 176, 48, 48),
                    2 => new Rectangle(48, 128, 48, 48),
                    _ => new Rectangle(0, 176, 48, 48),
                };
            case BushSizes.Harvested:
                return new Rectangle(0, 320, 32, 32);
            case BushSizes.Walnut:
                return new Rectangle(32, 320, 32, 32);
            default:
                return default;
        }
    }

    /// <summary>
    /// Draw some graphics for being picked up.
    /// </summary>
    /// <param name="location">game location picked up from.</param>
    /// <param name="tile">Tile pikced up from.</param>
    internal void DrawPickUpGraphics(GameLocation location, Vector2 tile)
    {
        if (this.sourceRect == default)
        {
            this.sourceRect = this.GetSourceRectForSeason(GetSeason(Game1.currentLocation));
        }
        if (this.sourceRect == default)
        {
            ModEntry.ModMonitor.Log($"Could not resolve source rect for {this.Name}", LogLevel.Error);
            return;
        }

        BushSizes size = (BushSizes)this.ParentSheetIndex;
        if (size == BushSizes.Invalid || !BushSizesExtensions.IsDefined(size))
        {
            return;
        }

        int width = size.GetWidth();

        Multiplayer mult = MultiplayerHelpers.GetMultiplayer();

        const float deltaY = -100;
        const float gravity = 0.0025f;

        float velocity = -0.7f - MathF.Sqrt(2 * 60f * gravity);
        float time = (MathF.Sqrt((velocity * velocity) - (gravity * deltaY * 2f)) / gravity) - (velocity / gravity);

        Vector2 landingPos = new Vector2(tile.X + Math.Max(0, (width / 2) - 1), tile.Y) * 64f;

        TemporaryAnimatedSprite bushTas = new(
            "TileSheets/bushes",
            this.sourceRect,
            (tile * 64f) - new Vector2(0, this.sourceRect.Y / 16),
            false,
            0f,
            Color.White)
        {
            totalNumberOfLoops = 1,
            interval = time,
            acceleration = new Vector2(0f, gravity),
            motion = new Vector2(0f, velocity),
            scale = Game1.pixelZoom,
            timeBasedMotion = true,
            rotation = 0.1f,
            rotationChange = 0.1f,
            scaleChange = -0.0015f,
            layerDepth = (landingPos.Y + 32f) / 10000f,
        };

        TemporaryAnimatedSprite? dustTas = new(
            textureName: Game1.mouseCursorsName,
            sourceRect: new Rectangle(464, 1792, 16, 16),
            animationInterval: 120f,
            animationLength: 5,
            numberOfLoops: 0,
            position: landingPos,
            flicker: false,
            flipped: Game1.random.NextDouble() < 0.5,
            layerDepth: (landingPos.Y + 40f) / 10000f,
            alphaFade: 0.01f,
            color: Color.White,
            scale: Game1.pixelZoom,
            scaleChange: 0.02f,
            rotation: 0f,
            rotationChange: 0f)
        {
            light = true,
            delayBeforeAnimationStart = Math.Max((int)time - 10, 0),
        };

        mult.broadcastSprites(location, bushTas, dustTas);
    }
    #endregion
}
