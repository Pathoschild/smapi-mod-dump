/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Xml.Serialization;

using AtraBase.Toolkit.Reflection;

using AtraCore.Framework.ReflectionManager;
using AtraCore.Utilities;

using AtraShared.Utils.Extensions;

using GrowableGiantCrops.Framework.Assets;
using GrowableGiantCrops.Framework.InventoryModels;
using GrowableGiantCrops.HarmonyPatches.Compat;
using GrowableGiantCrops.HarmonyPatches.GrassPatches;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

using XLocation = xTile.Dimensions.Location;

namespace GrowableGiantCrops.Framework;

/// <summary>
/// A shovel.
/// </summary>
[XmlType("Mods_atravita_Shovel")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Like methods are grouped together.")]
public sealed class ShovelTool : GenericTool
{
    private static readonly Api Api = new();

    #region delegates

    /// <summary>
    /// Gets the mine rock count on a specific mineshaft level.
    /// </summary>
    internal static readonly Lazy<Func<MineShaft, int>> MineRockCountGetter = new(() =>
        (typeof(MineShaft).GetCachedProperty("stonesLeftOnThisLevel", ReflectionCache.FlagTypes.InstanceFlags)
             .GetGetMethod(nonPublic: true) ?? ReflectionThrowHelper.ThrowMethodNotFoundException<MethodInfo>("stonesLeftOnThisLevelGetter"))
             .CreateDelegate<Func<MineShaft, int>>());

    /// <summary>
    /// Sets the mine rock count on a specific mineshaft level.
    /// </summary>
    internal static readonly Lazy<Action<MineShaft, int>> MineRockCountSetter = new(() =>
        (typeof(MineShaft).GetCachedProperty("stonesLeftOnThisLevel", ReflectionCache.FlagTypes.InstanceFlags)
             .GetSetMethod(nonPublic: true) ?? ReflectionThrowHelper.ThrowMethodNotFoundException<MethodInfo>("stonesLeftOnThisLevelSetter"))
             .CreateDelegate<Action<MineShaft, int>>());

    private static readonly Lazy<Func<MineShaft, bool>> HasLadderSpawnedGetter = new(() =>
        typeof(MineShaft).GetCachedField("ladderHasSpawned", ReflectionCache.FlagTypes.InstanceFlags)
        .GetInstanceFieldGetter<MineShaft, bool>());

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ShovelTool"/> class.
    /// </summary>
    public ShovelTool()
        : base(I18n.Shovel_Name(), I18n.Shovel_Description(), 0, 0, 0)
    {
        this.Stackable = false;
    }

    /// <inheritdoc />
    public override Item getOne()
    {
        ShovelTool newShovel = new();
        newShovel._GetOneFrom(this);
        return newShovel;
    }

    #region functionality

    /// <inheritdoc />
    public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
    {
        // use the watering can arms.
        who.jitterStrength = 0.25f;
        who.FarmerSprite.setCurrentFrame(who.FacingDirection switch
        {
            Game1.down => 164,
            Game1.right => 172,
            Game1.up => 180,
            _ => 188,
        });
        this.Update(who.FacingDirection, 0, who);
        return false;
    }

    /// <inheritdoc />
    public override void endUsing(GameLocation location, Farmer who)
    {
        who.stopJittering();
        who.canReleaseTool = false;

        // use the watering can arms.
        int frame = who.FacingDirection switch
        {
            Game1.down => 164,
            Game1.right => 172,
            Game1.up => 180,
            _ => 188,
        };

        (who.Sprite as FarmerSprite)?.animateOnce(whichAnimation: frame, animationInterval: 125f, numberOfFrames: 3);
    }

    /// <summary>
    /// Does the actual tool function.
    /// </summary>
    /// <param name="location">The game location.</param>
    /// <param name="x">pixel x.</param>
    /// <param name="y">pixel y.</param>
    /// <param name="power">The power level of the tool</param>
    /// <param name="who">Last farmer to use.</param>
    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
        try
        {
            base.DoFunction(location, x, y, power, who);
            Vector2 pickupTile = new(x / Game1.tileSize, y / Game1.tileSize);

            location.performToolAction(this, x / Game1.tileSize, y / Game1.tileSize);
            GGCUtils.GetLargeObjectAtLocation(location, x, y, false)?.performToolAction(this, 0, pickupTile, location);

            // Handle bushes.
            if (ModEntry.GrowableBushesAPI?.TryPickUpBush(location, pickupTile, ModEntry.Config.PlacedOnly) is SObject bush)
            {
                ModEntry.ModMonitor.DebugOnlyLog($"Picking up bush {bush.Name}", LogLevel.Info);
                GiveItemOrMakeDebris(location, who, bush);
                ModEntry.GrowableBushesAPI.DrawPickUpGraphics(bush, location, bush.TileLocation);
                who.Stamina -= ModEntry.Config.ShovelEnergy;
                return;
            }

            // Handle clumps and giant crops.
            if (Api.TryPickUpClumpOrGiantCrop(location, pickupTile, ModEntry.Config.PlacedOnly) is SObject inventoryClump)
            {
                ModEntry.ModMonitor.DebugOnlyLog($"Picking up {inventoryClump.Name}.", LogLevel.Info);
                who.Stamina -= ModEntry.Config.ShovelEnergy;
                GiveItemOrMakeDebris(location, who, inventoryClump);
                Api.DrawPickUpGraphics(inventoryClump, location, inventoryClump.TileLocation);
                return;
            }

            // for small things we take only one energy, at most.
            int energy = Math.Min(ModEntry.Config.ShovelEnergy, 1);

            // objects go before terrain so tappers are removed before trees/fruit trees.
            if (location.objects.TryGetValue(pickupTile, out SObject? @object))
            {
                // special case terrain stuff.
                if (!@object.bigCraftable.Value && @object.GetType() == typeof(SObject))
                {
                    if (@object.ParentSheetIndex >= 0 &&
                        (@object.Name == "Stone" || @object.Name.Contains("Weeds") || @object.Name.Contains("Twig")))
                    {
                        HandleTerrainObject(location, who, pickupTile, energy, @object);
                        return;
                    }
                }

                // special case, ignore indoor pots with stuff in them.
                if (@object is IndoorPot pot && (pot.hoeDirt?.Value?.crop is not null || pot.bush?.Value is not null))
                {
                    pot.shakeTimer = 100;
                    return;
                }

                // special case: shovel pushes full chests.
                if (@object is Chest chest && !chest.isEmpty() && chest.playerChest.Value)
                {
                    chest.GetMutex().RequestLock(
                        acquired: () =>
                        {
                            location.playSound("hammer");
                            chest.shakeTimer = 100;
                            if (chest.TileLocation.X == 0f && chest.TileLocation.Y == 0f && location.getObjectAtTile((int)pickupTile.X, (int)pickupTile.Y) == chest)
                            {
                                chest.TileLocation = pickupTile;
                            }
                            chest.MoveToSafePosition(location, chest.TileLocation, 0, who.GetFacingDirection());
                            who.Stamina -= energy;
                            return;
                        },
                        failed: () => ModEntry.ModMonitor.Log($"Chest at {chest.TileLocation}: lock not acquired, skipping"));
                    return;
                }

                // special cases: Mushroom boxes, slime balls
                if (@object.bigCraftable.Value && @object.GetType() == typeof(SObject))
                {
                    if (@object.Name == "Mushroom Box")
                    {
                        who.Stamina -= energy;
                        @object.ParentSheetIndex = 128;
                        if (@object.readyForHarvest.Value)
                        {
                            location.debris.Add(new Debris(@object.heldObject.Value, who.Position));
                            @object.heldObject.Value = null;
                        }
                        @object.performRemoveAction(pickupTile, location);
                        GiveItemOrMakeDebris(location, who, @object);
                        AddAnimations(
                            loc: location,
                            tile: pickupTile - Vector2.UnitY,
                            texturePath: Game1.bigCraftableSpriteSheetName,
                            sourceRect: SObject.getSourceRectForBigCraftable(128),
                            new Point(1, 2));
                        location.objects.Remove(pickupTile);
                        return;
                    }
                    else if (@object.Name == "Slime Ball")
                    {
                        who.Stamina -= energy;
                        @object.ParentSheetIndex = 56;
                        @object.performRemoveAction(pickupTile, location);
                        GiveItemOrMakeDebris(location, who, @object);
                        AddAnimations(
                            loc: location,
                            tile: pickupTile - Vector2.UnitY,
                            texturePath: Game1.bigCraftableSpriteSheetName,
                            sourceRect: SObject.getSourceRectForBigCraftable(56),
                            new Point(1, 2));
                        location.objects.Remove(pickupTile);
                        return;
                    }
                }

                if (@object.performToolAction(this, location))
                {
                    who.Stamina -= energy;
                    if (FTMArtifactSpotPatch.IsBuriedItem?.Invoke(@object) != true)
                    {
                        GiveItemOrMakeDebris(location, who, @object);
                        AddAnimations(
                            loc: location,
                            tile: pickupTile,
                            texturePath: @object.bigCraftable.Value ? Game1.bigCraftableSpriteSheetName : Game1.objectSpriteSheetName,
                            sourceRect: @object.bigCraftable.Value ? SObject.getSourceRectForBigCraftable(@object.ParentSheetIndex) : GameLocation.getSourceRectForObject(@object.ParentSheetIndex),
                            new Point(1, 1));
                    }
                    location.objects.Remove(pickupTile);
                    return;
                }
            }

            if (location.terrainFeatures.TryGetValue(pickupTile, out TerrainFeature? terrain))
            {
                // block subclasses like Cosmetic Plant, which currently cannot be safely moved.
                if (terrain is Grass grass &&
                    (terrain.GetType() == typeof(Grass) || SObjectPatches.IsMoreGrassGrass?.Invoke(grass) == true))
                {
                    HandleGrass(location, who, pickupTile, energy, grass);
                    return;
                }

                if (terrain is Tree tree && terrain.GetType() == typeof(Tree))
                {
                    int effectiveStage = Math.Clamp(tree.growthStage.Value, 0, 5);
                    if (effectiveStage == 4)
                    {
                        effectiveStage--;
                    }
                    if (effectiveStage <= ModEntry.Config.MaxTreeStageInternal)
                    {
                        if (tree.growthStage.Value == 0 && tree.treeType.Value is not Tree.palmTree or Tree.palmTree2)
                        {
                            who.Stamina -= energy;
                            location.playSound("woodyHit");
                            location.playSound("axchop");
                            InventoryTree.SeedDestoryMethod(tree, this, pickupTile, location);
                            location.terrainFeatures.Remove(pickupTile);
                            return;
                        }
                        else if (Api.TryPickUpTree(location, pickupTile, ModEntry.Config.PlacedOnly) is InventoryTree inventoryTree)
                        {
                            ModEntry.ModMonitor.DebugOnlyLog($"Picking up {inventoryTree.Name}.", LogLevel.Info);
                            who.Stamina -= ModEntry.Config.ShovelEnergy;
                            GiveItemOrMakeDebris(location, who, inventoryTree);
                            Api.DrawPickUpGraphics(inventoryTree, location, inventoryTree.TileLocation);
                            return;
                        }
                    }
                    else
                    {
                        InventoryTree.TreeShakeMethod(tree, pickupTile, true, location);
                    }
                }

                if (terrain is FruitTree fruitTree && terrain.GetType() == typeof(FruitTree))
                {
                    if (fruitTree.growthStage.Value <= ModEntry.Config.MaxFruitTreeStageInternal)
                    {
                        if (Api.TryPickUpFruitTree(location, pickupTile, ModEntry.Config.PlacedOnly) is InventoryFruitTree inventoryFruitTree)
                        {
                            ModEntry.ModMonitor.DebugOnlyLog($"Picking up {inventoryFruitTree.Name}.", LogLevel.Info);
                            who.Stamina -= ModEntry.Config.ShovelEnergy;
                            GiveItemOrMakeDebris(location, who, inventoryFruitTree);
                            Api.DrawPickUpGraphics(inventoryFruitTree, location, inventoryFruitTree.TileLocation);
                            return;
                        }
                    }
                    else
                    {
                        fruitTree.shake(pickupTile, true, location);
                    }
                }

                if (terrain.performToolAction(this, 0, pickupTile, location))
                {
                    who.Stamina -= energy;
                    location.terrainFeatures.Remove(pickupTile);
                    return;
                }
            }

            // derived from Hoe - this makes hoedirt.
            if (location.doesTileHaveProperty((int)pickupTile.X, (int)pickupTile.Y, "Diggable", "Back") is null
                || location.isTileOccupied(pickupTile) || !location.isTilePassable(new XLocation((int)pickupTile.X, (int)pickupTile.Y), Game1.viewport))
            {
                return;
            }

            who.Stamina -= energy;
            location.makeHoeDirt(pickupTile, ignoreChecks: false);
            location.playSound("hoeHit");
            Game1.removeSquareDebrisFromTile((int)pickupTile.X, (int)pickupTile.Y);
            location.checkForBuriedItem((int)pickupTile.X, (int)pickupTile.Y, explosion: false, detectOnly: false, who);
            MultiplayerHelpers.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(
                rowInAnimationTexture: 12,
                new Vector2(pickupTile.X * 64f, pickupTile.Y * 64f),
                color: Color.White,
                animationLength: 8,
                flipped: Game1.random.Next(2) == 0,
                animationInterval: 50f));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Unexpected error in using shovel:\n\n{ex}", LogLevel.Error);
        }
    }

    /// <inheritdoc />
    public override bool onRelease(GameLocation location, int x, int y, Farmer who) => false;

    #endregion

    #region display

    /// <inheritdoc />
    public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        spriteBatch.Draw(
            texture: AssetManager.ToolTexture,
            position: location + new Vector2(32f, 32f),
            new Rectangle(96, 16, 16, 16),
            color: color * transparency,
            rotation: 0f,
            new Vector2(8f, 8f),
            scale: 4f * scaleSize,
            effects: SpriteEffects.None,
            layerDepth);
    }

    /// <inheritdoc />
    protected override string loadDisplayName() => I18n.Shovel_Name();

    /// <inheritdoc />
    protected override string loadDescription() => I18n.Shovel_Description();

    #endregion

    #region misc

    /// <inheritdoc />
    /// <remarks>disallow forging.</remarks>
    public override bool CanForge(Item item) => false;

    /// <inheritdoc />
    /// <remarks>disallow stacking.</remarks>
    public override int maximumStackSize() => -1;

    /// <inheritdoc />
    /// <remarks>nop this.</remarks>
    public override void actionWhenClaimed()
    {
    }

    /// <inheritdoc />
    /// <remarks>forbid attachments.</remarks>
    public override int attachmentSlots() => 0;

    /// <inheritdoc />
    /// <remarks>forbid attachments.</remarks>
    public override bool canThisBeAttached(SObject o) => false;

    /// <inheritdoc />
    /// <remarks>forbid attachments.</remarks>
    public override SObject attach(SObject o) => o;

    #endregion

    #region helpers

    /// <inheritdoc cref="IGrowableGiantCropsAPI.DrawAnimations(GameLocation, Vector2, string?, Rectangle, Point)"/>
    internal static void AddAnimations(GameLocation loc, Vector2 tile, string? texturePath, Rectangle sourceRect, Point tileSize)
    {
        if (texturePath is null)
        {
            return;
        }

        Multiplayer mp = MultiplayerHelpers.GetMultiplayer();

        float deltaY = -50f - (sourceRect.Height * 2);
        const float gravity = 0.0025f;

        float velocity = -0.7f - MathF.Sqrt(2 * 60f * gravity);
        float time = (MathF.Sqrt((velocity * velocity) - (gravity * deltaY * 2f)) / gravity) - (velocity / gravity);

        Vector2 landingPos = new Vector2(tile.X + (tileSize.X / 2f) - 1, tile.Y + tileSize.Y - 1) * 64f;

        TemporaryAnimatedSprite objTas = new(
            textureName: texturePath,
            sourceRect: sourceRect,
            position: tile * 64f,
            flipped: false,
            alphaFade: 0f,
            color: Color.White)
        {
            totalNumberOfLoops = 1,
            interval = time,
            acceleration = new Vector2(0f, gravity),
            motion = new Vector2(0f, velocity),
            scale = Game1.pixelZoom,
            timeBasedMotion = true,
            rotation = 0.1f,
            rotationChange = 0.1f,
            scaleChange = -0.0015f * (Math.Max(3, tileSize.X) / 3),
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

        int damage = (sourceRect.Height / 16) * (sourceRect.Width / 16);

        // if you somehow manage to hit a monster with the animation.....
        DelayedAction.functionAfterDelay(
            () => loc.damageMonster(new Rectangle((int)landingPos.X, (int)landingPos.Y, 64, 64), damage, damage * 3, false, Game1.player),
            (int)time);

        mp.broadcastSprites(loc, objTas, dustTas);
    }

    /// <summary>
    /// Tries to add an item to the player's inventory, dropping it at their feet if we can't.
    /// </summary>
    /// <param name="location">relevant location.</param>
    /// <param name="who">farmer to add to.</param>
    /// <param name="item">item to add.</param>
    private static void GiveItemOrMakeDebris(GameLocation location, Farmer who, Item item)
    {
        if (!who.addItemToInventoryBool(item))
        {
            location.debris.Add(new Debris(item, who.Position));
        }
    }

    private static void HandleTerrainObject(GameLocation location, Farmer who, Vector2 pickupTile, int energy, SObject @object)
    {
        who.Stamina -= energy;
        GiveItemOrMakeDebris(location, who, @object);
        AddAnimations(
            loc: location,
            tile: pickupTile,
            texturePath: Game1.objectSpriteSheetName,
            sourceRect: GameLocation.getSourceRectForObject(@object.ParentSheetIndex),
            new Point(1, 1));
        location.Objects.Remove(pickupTile);

        if (location is MineShaft shaft && @object.Name == "Stone")
        {
            int stonesLeft = MineRockCountGetter.Value(shaft);
            stonesLeft--;
            ModEntry.ModMonitor.DebugOnlyLog($"{stonesLeft} stones left on floor {shaft.mineLevel}", LogLevel.Info);
            if (stonesLeft <= 0 && !HasLadderSpawnedGetter.Value(shaft))
            {
                ModEntry.ModMonitor.DebugOnlyLog($"Last rock on {shaft.mineLevel}, creating ladder.", LogLevel.Info);
                shaft.createLadderDown((int)pickupTile.X, (int)pickupTile.Y);
            }
            MineRockCountSetter.Value(shaft, stonesLeft);
        }
        return;
    }

    private static void HandleGrass(GameLocation location, Farmer who, Vector2 pickupTile, int energy, Grass grass)
    {
        who.Stamina -= energy;
        SObject starter = Api.GetMatchingStarter(grass);
        GiveItemOrMakeDebris(location, who, starter);
        AddAnimations(
            loc: location,
            tile: pickupTile,
            texturePath: Game1.objectSpriteSheetName,
            sourceRect: GameLocation.getSourceRectForObject(SObjectPatches.GrassStarterIndex),
            new Point(1, 1));
        location.terrainFeatures.Remove(pickupTile);
    }

    #endregion
}
