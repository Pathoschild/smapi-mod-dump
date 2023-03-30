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

using AtraBase.Toolkit;
using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.Reflection;

using AtraCore.Framework.ReflectionManager;

using AtraShared.Utils.Extensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

using XLocation = xTile.Dimensions.Location;

namespace CritterRings.Framework.Managers;

/// <summary>
/// Manages a jump for a player.
/// </summary>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Preference.")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Preference.")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1214:Readonly fields should appear before non-readonly fields", Justification = "Preference.")]
internal sealed class JumpManager : IDisposable
{
    private const int DEFAULT_TICKS = 200;

    private static bool hasSwim = false;

    // event handlers.
    private IGameLoopEvents gameEvents;
    private IDisplayEvents displayEvents;

    private bool disposedValue;
    private WeakReference<Farmer> farmerRef;
    private bool previousCollisionValue = false; // keeps track of whether or not the farmer had noclip on.
    private bool prevInvincibility; // we set invincibility because the farmer's sprite is not drawn anywhere
    private int prevInvincibilityTimer; // near the actual position, so this way the farmer can't appear to get hit out of nowhere.
    private readonly bool forceTimePass;

    private State state = State.Charging;
    private int ticks = DEFAULT_TICKS;
    private readonly Vector2 direction;
    private Keybind keybind;

    // charging fields.
    private int distance = 1;
    private readonly Vector2 startTile;
    private Vector2 currentTile = Vector2.Zero;
    private Vector2 openTile = Vector2.Zero;
    private bool isCurrentTileBlocked = false;
    private bool blocked = false;
    private NeedsBigJump needsBigJump = NeedsBigJump.False;

    // jumping fields.
    private JumpFrame frame;
    private float velocityX;

    #region delegates

    // this exists because if currentAnimationFrames is 1 or lower,
    // the game will unset PauseForSingleAnimation on its own.
    // so we just manually set it to two.
    private static readonly Lazy<Action<FarmerSprite, int>> currentFramesSetter = new(() =>
        typeof(FarmerSprite).GetCachedField("currentAnimationFrames", ReflectionCache.FlagTypes.InstanceFlags).GetInstanceFieldSetter<FarmerSprite, int>()
    );
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="JumpManager"/> class.
    /// </summary>
    /// <param name="farmer">The farmer we're tracking.</param>
    /// <param name="gameEvents">The game event manager.</param>
    /// <param name="displayEvents">The display event manager.</param>
    /// <param name="keybind">The keybind that triggered this jump.</param>
    internal JumpManager(Farmer farmer, IGameLoopEvents gameEvents, IDisplayEvents displayEvents, Keybind keybind)
    {
        ModEntry.ModMonitor.DebugOnlyLog("(FrogRing) Starting -> Charging");
        this.farmerRef = new(farmer);
        this.gameEvents = gameEvents;
        this.displayEvents = displayEvents;
        this.keybind = keybind;

        this.gameEvents.UpdateTicked += this.OnUpdateTicked;
        this.displayEvents.RenderedWorld += this.OnRenderedWorld;

        // save the values here too in case we get interrupted.
        this.previousCollisionValue = Game1.player.ignoreCollisions;
        this.prevInvincibility = Game1.player.temporarilyInvincible;
        this.prevInvincibilityTimer = Game1.player.temporaryInvincibilityTimer;

        // forcing time to pass even while we're preparing to jump.
        this.forceTimePass = Game1.player.forceTimePass;
        Game1.player.forceTimePass = true;

        this.direction = Game1.player.FacingDirection switch
        {
            Game1.up => -Vector2.UnitY,
            Game1.left => -Vector2.UnitX,
            Game1.down => Vector2.UnitY,
            _ => Vector2.UnitX,
        };

        this.startTile = this.openTile = farmer.getTileLocation();
        this.RecalculateTiles(farmer, Game1.currentLocation);

        farmer.completelyStopAnimatingOrDoingAction();
        farmer.CanMove = false;
        SetCrouchAnimation(farmer);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="JumpManager"/> class.
    /// </summary>
    ~JumpManager() => this.Dispose(false);

    private enum State
    {
        Inactive,
        Charging,
        Jumping,
    }

    private enum JumpFrame
    {
        Start,
        Transition,
        Hold,
    }

    private enum NeedsBigJump
    {
        False,
        IfPastThisPoint,
        True,
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// sets some initial state.
    /// </summary>
    /// <param name="registry">Modregistry.</param>
    internal static void Initialize(IModRegistry registry)
        => hasSwim = registry.IsLoaded("aedenthorn.Swim");

    /// <summary>
    /// Checks to see if this JumpManager is valid (ie, not disposed, and has an active farmer associated).
    /// </summary>
    /// <param name="farmer">The relevant farmer.</param>
    /// <returns>True if valid.</returns>
    internal bool IsValid([NotNullWhen(true)] out Farmer? farmer)
    {
        farmer = null;
        return !this.disposedValue && this.state != State.Inactive
                    && this.farmerRef?.TryGetTarget(out farmer) == true && farmer is not null;
    }

    [MethodImpl(TKConstants.Hot)]
    private bool IsCurrentFarmer()
        => this.farmerRef?.TryGetTarget(out Farmer? farmer) == true && ReferenceEquals(farmer, Game1.player);

    [MethodImpl(TKConstants.Hot)]
    private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
    {
        if (!this.IsCurrentFarmer())
        {
            return;
        }

        e.SpriteBatch.Draw(
            texture: Game1.mouseCursors,
            new Vector2((this.openTile.X * Game1.tileSize) - Game1.viewport.X, (this.openTile.Y * Game1.tileSize) - Game1.viewport.Y),
            new Rectangle(194, 388, 16, 16),
            color: Color.White,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 4f,
            effects: SpriteEffects.None,
            layerDepth: 0.01f);

        if (this.isCurrentTileBlocked && this.state == State.Charging)
        {
            e.SpriteBatch.Draw(
                texture: Game1.mouseCursors,
                new Vector2((this.currentTile.X * Game1.tileSize) - Game1.viewport.X, (this.currentTile.Y * Game1.tileSize) - Game1.viewport.Y),
                new Rectangle(210, 388, 16, 16),
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: 0.01f);
        }
    }

    [MethodImpl(TKConstants.Hot)]
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!this.IsCurrentFarmer())
        {
            return;
        }

        // Thanks for the viewport movement code, DecidedlyHuman!
        if (this.state != State.Inactive && ModEntry.Config.ViewportFollowsTarget)
        {
            Vector2 position = Game1.player.Position + new Vector2(32, 32);
            Vector2 target = this.openTile * Game1.tileSize;
            Vector2 midpoint = Game1.player.FacingDirection switch
            {
                Game1.left or Game1.right => new Vector2(target.X + Math.Clamp((position.X - target.X) / 2, -320, 320), position.Y),
                _ => new Vector2(position.X, target.Y + Math.Clamp((position.Y - target.Y) / 2, -320, 320)),
            };
            Game1.moveViewportTo(midpoint, ModEntry.Config.JumpChargeSpeed / 2);
        }

        switch (this.state)
        {
            case State.Charging:
                if (this.keybind.GetState().IsDown())
                {
                    this.ticks -= ModEntry.Config.JumpChargeSpeed;
                    if (this.ticks <= 0)
                    {
                        if (this.distance < ModEntry.Config.MaxFrogJumpDistance && !this.blocked)
                        {
                            ++this.distance;
                            CRUtils.PlayChargeCue(this.distance);
                            this.RecalculateTiles(Game1.player, Game1.currentLocation);
                        }
                        this.ticks = DEFAULT_TICKS;
                        ModEntry.ModMonitor.TraceOnlyLog($"(Frog Ring) distance: {this.distance}");
                    }
                }
                else if (this.startTile == this.openTile)
                {
                    ModEntry.ModMonitor.DebugOnlyLog($"(Frog Ring) Switching Charging -> Invalid", LogLevel.Info);
                    Game1.player.synchronizedJump(3f); // a tiny little hop
                    this.state = State.Inactive;
                    this.Dispose();
                }
                else
                {
                    ModEntry.ModMonitor.DebugOnlyLog($"(Frog Ring) Switching Charging -> Jumping", LogLevel.Info);
                    this.state = State.Jumping;

                    CRUtils.PlayMeep();

                    #region velocity calculations

                    float initialVelocityY = 4f * MathF.Sqrt(this.distance);

                    // we're just gonna hope the big jump is enough to clear most buildings :(
                    if (this.needsBigJump == NeedsBigJump.True)
                    {
                        initialVelocityY = Math.Max(16f, initialVelocityY);
                    }

                    // okay, let's adjust for possible front tiles if needed
                    Vector2? tileToCheck = null;
                    if (this.direction == Vector2.UnitY)
                    {
                        tileToCheck = this.startTile;
                    }
                    else if (this.direction == -Vector2.UnitY)
                    {
                        tileToCheck = this.openTile;
                    }

                    if (tileToCheck is not null)
                    {
                        int verticalHeightNeeded = 4;
                        int startX = (int)tileToCheck.Value.X * Game1.tileSize;
                        int startY = (int)(tileToCheck.Value.Y - 3) * Game1.tileSize;
                        while (startY > 0 && (Game1.currentLocation.map.GetLayer("Front")?.PickTile(new XLocation(startX, startY), Game1.viewport.Size) is not null
                            || Game1.currentLocation.map.GetLayer("AlwaysFront")?.PickTile(new XLocation(startX, startY), Game1.viewport.Size) is not null))
                        {
                            ++verticalHeightNeeded;
                            startY -= 64;
                        }

                        ModEntry.ModMonitor.DebugOnlyLog($"Additional vertical height: {verticalHeightNeeded}");

                        initialVelocityY = Math.Max(initialVelocityY, 6 * MathF.Sqrt(verticalHeightNeeded));
                    }

                    // a little sanity here.
                    initialVelocityY = Math.Min(initialVelocityY, 128f);

                    float tileTravelDistance = this.openTile.ManhattanDistance(this.startTile);
                    if (ModEntry.Config.JumpCostsStamina && Game1.CurrentEvent is null)
                    {
                        Game1.player.Stamina -= tileTravelDistance;
                    }
                    float travelDistance = tileTravelDistance * Game1.tileSize;

                    // gravity is 0.5f, so total time is 2 * initialVelocityY / 0.5 = 4 * initialVelocityY;
                    this.velocityX = travelDistance / ((4 * initialVelocityY) - 1);
                    Game1.player.synchronizedJump(initialVelocityY);

                    #endregion

                    // track player state
                    this.previousCollisionValue = Game1.player.ignoreCollisions;
                    Game1.player.ignoreCollisions = true;

                    this.prevInvincibility = Game1.player.temporarilyInvincible;
                    this.prevInvincibilityTimer = Game1.player.temporaryInvincibilityTimer;
                    Game1.player.temporarilyInvincible = true;
                    Game1.player.temporaryInvincibilityTimer = int.MinValue;

                    StartJumpAnimation(Game1.player);
                }
                break;
            case State.Jumping:
                if (Game1.player.yJumpOffset == 0 && Game1.player.yJumpVelocity.WithinMargin(0f))
                {
                    ModEntry.ModMonitor.DebugOnlyLog($"(Frog Ring) Switching Jumping -> Inactive", LogLevel.Info);
                    this.state = State.Inactive;
                    this.Dispose();
                }
                else
                {
                    Game1.player.Position += this.velocityX * this.direction;
                    // Handle switching the jump frame.
                    switch (this.frame)
                    {
                        case JumpFrame.Start:
                        {
                            if (Game1.player.yJumpOffset < -20)
                            {
                                ModEntry.ModMonitor.TraceOnlyLog("(Frog Ring) Setting Jump Frame: START -> TRANSITION");
                                SetTransitionAnimation(Game1.player);
                                this.frame = JumpFrame.Transition;
                            }
                            break;
                        }
                        case JumpFrame.Transition:
                        {
                            if (Game1.player.yJumpVelocity < 0)
                            {
                                ModEntry.ModMonitor.TraceOnlyLog("(Frog Ring) Setting Jump Frame: TRANSITION -> HOLD");
                                HoldJumpAnimation(Game1.player);
                                this.frame = JumpFrame.Hold;
                            }
                            break;
                        }
                    }
                }
                break;
        }
    }

    private void RecalculateTiles(Farmer farmer, GameLocation location)
    {
        this.currentTile = this.startTile + (this.direction * this.distance);
        bool isValidTile = location.isTileOnMap(this.currentTile)
            && location.isTilePassable(new XLocation((int)this.currentTile.X, (int)this.currentTile.Y), Game1.viewport)
            && !location.isWaterTile((int)this.currentTile.X, (int)this.currentTile.Y);

        // let the user jump into water if they have swim mod. But not lava.
        if (hasSwim && !isValidTile && location is not VolcanoDungeon or Caldera)
        {
            isValidTile = location.isOpenWater((int)this.currentTile.X, (int)this.currentTile.Y);
        }

        if (isValidTile)
        {
            Rectangle box = farmer.GetBoundingBox();
            box.X += (int)this.direction.X * this.distance * Game1.tileSize;
            box.Y += (int)this.direction.Y * this.distance * Game1.tileSize;
            isValidTile &= !location.isCollidingPosition(box, Game1.viewport, true, 0, false, farmer);
        }

        // check a tile property here, so mapmakers can block the jump.
        if (location.doesEitherTileOrTileIndexPropertyEqual((int)this.currentTile.X, (int)this.currentTile.Y, "atravita.FrogJump", "Back", "Forbidden"))
        {
            this.blocked = true;
            isValidTile = false;
            Game1.showRedMessage(I18n.FrogRing_Blocked());
        }

        if (this.needsBigJump == NeedsBigJump.False)
        {
            if (location.terrainFeatures.TryGetValue(this.currentTile, out TerrainFeature? feat)
                && feat is Tree or FruitTree)
            {
                this.needsBigJump = NeedsBigJump.IfPastThisPoint;
            }
            if (this.needsBigJump == NeedsBigJump.False && location is Farm farm)
            {
                foreach (Building? building in farm.buildings)
                {
                    if (building.occupiesTile(this.currentTile))
                    {
                        this.needsBigJump = NeedsBigJump.IfPastThisPoint;
                        break;
                    }
                }
            }
        }

        if (isValidTile)
        {
            this.openTile = this.currentTile;
            this.isCurrentTileBlocked = false;
            if (this.needsBigJump == NeedsBigJump.IfPastThisPoint)
            {
                this.needsBigJump = NeedsBigJump.True;
            }
        }
        else
        {
            this.isCurrentTileBlocked = true;
        }
    }

    #region cleanup

    private void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            this.Unhook();
            if (this.farmerRef?.TryGetTarget(out Farmer? farmer) == true && farmer is not null)
            {
                farmer.CanMove = true;
                farmer.ignoreCollisions = this.previousCollisionValue;
                farmer.temporarilyInvincible = this.prevInvincibility;
                farmer.temporaryInvincibilityTimer = this.prevInvincibilityTimer;
                farmer.forceTimePass = this.forceTimePass;
                if (disposing)
                {
                    Game1.moveViewportTo(farmer.Position, 5f);
                }
                farmer.completelyStopAnimatingOrDoingAction();
            }
            this.keybind = null!;
            this.farmerRef = null!;
            this.gameEvents = null!;
            this.displayEvents = null!;
            this.disposedValue = true;
        }
    }

    private void Unhook()
    {
        if (this.gameEvents is not null)
        {
            this.gameEvents.UpdateTicked -= this.OnUpdateTicked;
        }
        if (this.displayEvents is not null)
        {
            this.displayEvents.RenderedWorld -= this.OnRenderedWorld;
        }
    }

    #endregion

    #region animationFrames

    /***************************************************************************
     * Animations here are mostly from the watering and hoe-ing animations
     * and are made basically by inspecting FarmerSprite.getAnimationFromIndex
     * and Tool.endUsing.
     ***************************************************************************/

    private static void SetCrouchAnimation(Farmer farmer)
    {
        farmer.FarmerSprite.setCurrentSingleFrame(
            which: farmer.FacingDirection switch
            {
                Game1.down => 54,
                Game1.right => 58,
                Game1.up => 62,
                _ => 58,
            }, flip: farmer.FacingDirection == Game1.left,
            interval: 2000);
        farmer.FarmerSprite.PauseForSingleAnimation = true;
        farmer.FarmerSprite.timer = 0f;
        currentFramesSetter.Value(farmer.FarmerSprite, 2);
    }

    private static void StartJumpAnimation(Farmer farmer)
    {
        farmer.FarmerSprite.setCurrentSingleFrame(
            which: farmer.FacingDirection switch
            {
                Game1.down => 55,
                Game1.right => 59,
                Game1.up => 63,
                _ => 59,
            }, flip: farmer.FacingDirection == Game1.left);
        farmer.FarmerSprite.PauseForSingleAnimation = true;
        currentFramesSetter.Value(farmer.FarmerSprite, 2);
    }

    private static void SetTransitionAnimation(Farmer farmer)
    {
        farmer.FarmerSprite.setCurrentSingleFrame(
            which: farmer.FacingDirection switch
            {
                Game1.down => 25,
                Game1.right => 45,
                Game1.up => 46,
                _ => 45,
            }, flip: farmer.FacingDirection == Game1.left,
            secondaryArm: true);
        farmer.FarmerSprite.PauseForSingleAnimation = true;
        currentFramesSetter.Value(farmer.FarmerSprite, 2);
    }

    private static void HoldJumpAnimation(Farmer farmer)
    {
        farmer.FarmerSprite.setCurrentSingleFrame(
        which: farmer.FacingDirection switch
        {
            Game1.down => 62,
            Game1.right => 52,
            Game1.up => 70,
            _ => 52,
        },
        flip: farmer.FacingDirection == Game1.left);
        farmer.FarmerSprite.PauseForSingleAnimation = true;
        currentFramesSetter.Value(farmer.FarmerSprite, 2);
    }

    #endregion
}
