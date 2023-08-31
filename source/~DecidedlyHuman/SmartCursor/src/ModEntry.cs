/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DecidedlyShared.APIs;
using DecidedlyShared.Constants;
using DecidedlyShared.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace SmartCursor
{
    public class ModEntry : Mod
    {
        private List<BreakableEntity> breakableResources;
        private SmartCursorConfig config;
        private Logger logger;
        private Vector2? targetedObject;
        private readonly int baseRange = 3;
        private byte cooldownThreshold = 30;
        private bool isHoldKeyDown;

        private Dictionary<int, int> toolRanges = new Dictionary<int, int>();

        public override void Entry(IModHelper helper)
        {
            // Read our config.
            this.config = helper.ReadConfig<SmartCursorConfig>();

            // And get our tool ranges.
            this.config.GetToolRanges(out this.toolRanges);

            this.breakableResources = new List<BreakableEntity>();
            this.targetedObject = new Vector2();
            this.logger = new Logger(this.Monitor, helper.Translation);
            I18n.Init(helper.Translation);

            helper.Events.Player.Warped += this.OnPlayerWarped;
            helper.Events.Input.ButtonPressed += this.InputOnButtonPressed;
            helper.Events.Input.ButtonReleased += this.InputOnButtonReleased;
            helper.Events.Display.RenderedWorld += this.DisplayOnRenderedWorld;
            helper.Events.World.ObjectListChanged += this.WorldOnObjectListChanged;
            helper.Events.World.TerrainFeatureListChanged += this.WorldOnTerrainFeatureListChanged;
            helper.Events.GameLoop.UpdateTicked += this.GameLoopOnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        /// <summary>
        /// For clearing our targeted object, and setting our hold bool appropriately.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputOnButtonReleased(object? sender, ButtonReleasedEventArgs e)
        {
            // If the cursor is released, we want to set our hold key bool to false, and null the targeted object.
            if (e.Button == this.config.SmartCursorHold)
            {
                this.isHoldKeyDown = false;
                this.targetedObject = null;
            }
        }

        /// <summary>
        /// This is where we register with GMCM, if installed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            this.RegisterWithGmcm();
        }

        private void RegisterWithGmcm()
        {
            var configMenuApi =
                this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenuApi != null)
            {
                configMenuApi.Register(
                    this.ModManifest,
                    () => this.config = new SmartCursorConfig(),
                    () => this.Helper.WriteConfig(this.config));

                configMenuApi.AddSectionTitle(
                    this.ModManifest,
                    () => I18n.Settings_Keybinds_Title());

                configMenuApi.AddKeybind(
                    this.ModManifest,
                    () => this.config.SmartCursorHold,
                    button => this.config.SmartCursorHold = button,
                    () => I18n.Settings_Keybinds_HoldToActivate());

                configMenuApi.AddSectionTitle(
                    this.ModManifest,
                    () => I18n.Settings_Ranges_TierRanges_Title());

                configMenuApi.AddParagraph(
                    this.ModManifest,
                    () => I18n.Settings_Ranges_TierAmountNotice());

                configMenuApi.AddNumberOption(
                    mod: this.ModManifest,
                    getValue: () => this.config.TierOneRange,
                    setValue: i =>
                    {
                        this.toolRanges[0] = i;
                        this.config.TierOneRange = i;
                    },
                    name: () => I18n.Settings_Ranges_Tier1Range(),
                    min: 1,
                    max: 20);

                configMenuApi.AddNumberOption(
                    mod: this.ModManifest,
                    getValue: () => this.config.TierTwoRange,
                    setValue: i =>
                    {
                        this.toolRanges[1] = i;
                        this.config.TierTwoRange = i;
                    },
                    name: () => I18n.Settings_Ranges_Tier2Range(),
                    min: 1,
                    max: 20);

                configMenuApi.AddNumberOption(
                    mod: this.ModManifest,
                    getValue: () => this.config.TierThreeRange,
                    setValue: i =>
                    {
                        this.toolRanges[2] = i;
                        this.config.TierThreeRange = i;
                    },
                    name: () => I18n.Settings_Ranges_Tier3Range(),
                    min: 1,
                    max: 20);

                configMenuApi.AddNumberOption(
                    mod: this.ModManifest,
                    getValue: () => this.config.TierFourRange,
                    setValue: i =>
                    {
                        this.toolRanges[3] = i;
                        this.config.TierFourRange = i;
                    },
                    name: () => I18n.Settings_Ranges_Tier4Range(),
                    min: 1,
                    max: 20);

                configMenuApi.AddNumberOption(
                    mod: this.ModManifest,
                    getValue: () => this.config.TierFiveRange,
                    setValue: i =>
                    {
                        this.toolRanges[4] = i;
                        this.config.TierFiveRange = i;
                    },
                    name: () => I18n.Settings_Ranges_Tier5Range(),
                    min: 1,
                    max: 20);

                configMenuApi.AddNumberOption(
                    mod: this.ModManifest,
                    getValue: () => this.config.TierSixRange,
                    setValue: i =>
                    {
                        this.toolRanges[5] = i;
                        this.config.TierSixRange = i;
                    },
                    name: () => I18n.Settings_Ranges_Tier6Range(),
                    min: 1,
                    max: 20);

                configMenuApi.AddNumberOption(
                    mod: this.ModManifest,
                    getValue: () => this.config.TierSevenRange,
                    setValue: i =>
                    {
                        this.toolRanges[6] = i;
                        this.config.TierSevenRange = i;
                    },
                    name: () => I18n.Settings_Ranges_Tier7Range(),
                    min: 1,
                    max: 20);

                configMenuApi.AddSectionTitle(
                    this.ModManifest,
                    () => I18n.Settings_Toggles_Title());

                configMenuApi.AddBoolOption(
                    this.ModManifest,
                    () => this.config.AllowTargetingBabyTrees,
                    newValue =>
                    {
                        this.config.AllowTargetingBabyTrees = newValue;
                        this.GatherResources(Game1.currentLocation);
                    },
                    () => I18n.Settings_Toggles_AllowTargetingBabyTrees(),
                    () => I18n.Settings_Toggles_AllowTargetingBabyTrees_Tooltip());

                configMenuApi.AddBoolOption(
                    this.ModManifest,
                    () => this.config.AllowTargetingTappedTrees,
                    newValue =>
                    {
                        this.config.AllowTargetingTappedTrees = newValue;
                        this.GatherResources(Game1.currentLocation);
                    },
                    () => I18n.Settings_Toggles_AllowTargetingTappedTrees(),
                    () => I18n.Settings_Toggles_AllowTargetingTappedTrees_Tooltip());

                configMenuApi.AddBoolOption(
                    this.ModManifest,
                    () => this.config.AllowTargetingGiantCrops,
                    newValue =>
                    {
                        this.config.AllowTargetingGiantCrops = newValue;
                        this.GatherResources(Game1.currentLocation);
                    },
                    () => I18n.Settings_Toggles_AllowTargetingGiantCrops(),
                    () => I18n.Settings_Toggles_AllowTargetingGiantCrops_Tooltip());
            }
            else
            {
                this.logger.Log(I18n.Messages_GmcmNotInstalled(), LogLevel.Info);
            }
        }

        //TODO we probably don't need to update this every updateTick
        // update when:
        // use tool button is pressed
        // after BreakObject()
        //
        // this is probably used for showing the target too, so update when:
        // after GatherResources()
        // after player moves
        // after player changes tool
        //
        // are there other events for which we should update?
        private void updateTargetedObject()
        {
            // We only want to process any of this if our smart cursor key is held down.
            if (this.isHoldKeyDown)
            {
                this.targetedObject = this.GetTileToTargetForPlayer(Game1.player);
            }
            else
            {
                this.targetedObject = null;
            }
        }

        /// <summary>
        /// Every tick while the smart cursor key is held down, we want to check distances.
        /// This could do with some heavy optimisation, but it doesn't seem to hurt performance for me.
        /// </summary>
        private void GameLoopOnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            //TODO should we update GamePadState and MouseState before checking isHoldKeyDown?
            //TODO don't call this from GameLoopOnUpdateTicked
            this.updateTargetedObject();

            GamePadState gamepadState = Game1.input.GetGamePadState();
            MouseState mouseState = Game1.input.GetMouseState();

            // Now, if our cooldown timer has passed and the correct keys are held, we want to hit again.
            if (this.isHoldKeyDown && (gamepadState.IsButtonDown(Buttons.X)
                                       || mouseState.LeftButton == ButtonState.Pressed) && !Game1.player.UsingTool)
            {
                // if (Game1.player.UsingTool)
                //     return;
                Game1.player.UsingTool = true;

                var dummy = new Farmer();

                if (this.targetedObject.HasValue && Game1.player.CurrentTool != null)
                {
                    this.BreakObject(Game1.player, Game1.player.CurrentTool, true);

                    this.GatherResources(Game1.currentLocation);
                }

                Game1.player.EndUsingTool();
            }
        }

        private void BreakObject(Farmer player, Tool tool, bool refundStamina)
        {
            float startingStamina = player.stamina;

            tool.DoFunction(
                Game1.currentLocation,
                (int)this.targetedObject.Value.X * 64,
                (int)this.targetedObject.Value.Y * 64, 1, player);

            if (refundStamina)
                player.stamina = startingStamina;
        }

        /// <summary>
        /// return the tile location of the closest resource that is targetable by the current tool
        /// </summary>
        /// <param name="player">The <see cref="Farmer"></param>
        private Vector2? GetTileToTargetForPlayer(Farmer player)
        {
            // Grab a reference for our player tile.
            Vector2 playerTile = Game1.player.getTileLocation();

            if (player.CurrentTool == null)
            {
                //NOTE: durring refactoring, I noticed this.targetedObject was set to null if !isHoldKeyDown,
                // but this.targetedObject was just unchanged when player.CurrentTool was null.
                // That seemed unintentional but I'm making a note because this is a functional change.
                return null;
            }

            BreakableType breakableType;
            switch (player.CurrentTool)
            {
                case Pickaxe:
                    breakableType = BreakableType.Pickaxe;
                    break;
                case Axe:
                    breakableType = BreakableType.Axe;
                    break;
                case Hoe:
                    breakableType = BreakableType.Hoe;
                    break;
                default:
                    breakableType = BreakableType.NotAllowed;
                    break;
            }

            return this.GetTileToTarget(playerTile, breakableType, this.breakableResources,
                this.toolRanges[player.CurrentTool.UpgradeLevel] + 1f);
        }

        /// <summary>
        /// Calculate the distance between the player and all resources, and return the closest resource, which we
        /// then want to target.
        /// </summary>
        /// <param name="playerTile">The <see cref="Vector2"> tile the player is standing on.</param>
        /// <param name="breakableType" The <see cref="BreakableEntity"> target type></param>
        /// <param name="resources" The <see cref="List<BreakableEntity>"> targets to consider></param>
        /// <param name="toolRange" The <see cref="float"> radius that the tool can reach></param>
        private Vector2? GetTileToTarget_Old(Vector2 playerTile, BreakableType breakableType,
            List<BreakableEntity> resources, float toolRange)
        {
            Dictionary<BreakableEntity, float> objectDistancesFromPlayer = new Dictionary<BreakableEntity, float>();

            // We don't need to do any of this if the player isn't holding a whitelisted tool.
            if (breakableType != BreakableType.NotAllowed)
            {
                // Loop through the breakable resources gathered for this map, and add them to a new Dictionary along
                // with the distance between the player and the resource.
                foreach (var resource in resources)
                    objectDistancesFromPlayer.Add(resource, Vector2.Distance(resource.Tile, playerTile));

                // If there's anything in our new distances dictionary...
                if (objectDistancesFromPlayer.Any())
                {
                    // Use LINQ to add only ones within a certain range, and of a certain BreakableType.
                    // Sorted in ascending order.
                    var sortedDistances =
                        from distance in objectDistancesFromPlayer
                        where distance.Key.Type == breakableType &&
                              distance.Value < toolRange
                        orderby distance.Value
                        select distance;

                    // Again, if there's anything in our sorted list...
                    if (sortedDistances.Any())
                    {
                        // We set our targeted object to the first, which is the closest.
                        return sortedDistances.First().Key.Tile;
                    }
                    else
                    {
                        // Otherwise, we return null.
                        return null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// How is this faster than before:
        /// This mod calculates range as a circle shape
        /// Calculating a Vector2.Distance uses Math.Sqrt which is slow.
        /// It is faster to compare Vector2.DistanceSquared to toolRangeSquared.
        ///
        /// If we were to calculate range as a square shape like the base game
        /// We we would need Math.abs and/or more value comparisons
        ///
        /// also creating and sorting a dictionary is slow, particularly when we only care about the nearest item result.
        /// </summary>
        /// <param name="playerTile">The <see cref="Vector2"> tile the player is standing on.</param>
        /// <param name="breakableType" The <see cref="BreakableEntity"> target type></param>
        /// <param name="resources" The <see cref="List<BreakableEntity>"> targets to consider></param>
        /// <param name="toolRange" The <see cref="float"> radius that the tool can reach></param>
        private Vector2? GetTileToTarget(Vector2 playerTile, BreakableType breakableType,
            List<BreakableEntity> resources, float toolRange)
        {
            float nearestDistanceSquared = float.MaxValue;
            Vector2? nearestTile = null;
            float toolRangeSquared = toolRange * toolRange;
            foreach (BreakableEntity resource in resources)
            {
                if (resource.Type == breakableType) //tile is valid
                {
                    float distanceSquared = Vector2.DistanceSquared(resource.Tile, playerTile);
                    if (distanceSquared < nearestDistanceSquared) //tile is the new closest
                    {
                        nearestDistanceSquared = distanceSquared;
                        nearestTile = resource.Tile;
                    }
                }
            }

            if (nearestDistanceSquared < toolRangeSquared)
            {
                return nearestTile;
            }
            else
            {
                return null; //no valid tiles were near enough for your tool
            }
        }


        /// <summary>
        ///     Triggered when the world's object list changes, so the current location's resources can be
        ///     re-gathered if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorldOnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
        {
            // this.logger.Log("Object event fired.", LogLevel.Info);
            if (e.IsCurrentLocation)
            {
                this.GatherResources(e.Location);
            }
        }

        private void WorldOnLargeTerrainFeatureListChanged(object? sender, LargeTerrainFeatureListChangedEventArgs e)
        {
            if (e.IsCurrentLocation)
            {
                this.GatherResources(e.Location);
            }
        }

        /// <summary>
        ///     Triggered when the world's terrain feature list changes, so the current location's resources can be
        ///     re-gathered if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorldOnTerrainFeatureListChanged(object? sender, TerrainFeatureListChangedEventArgs e)
        {
            // this.logger.Log("Terrain feature event fired.", LogLevel.Info);
            if (e.IsCurrentLocation)
            {
                this.GatherResources(e.Location);
            }
        }

        /// <summary>
        ///     Used to render the targeted tile on the screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayOnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (this.targetedObject != null)
            {
                e.SpriteBatch.Draw(Game1.mouseCursors,
                    new Vector2(
                        this.targetedObject.Value.X * Game1.tileSize - Game1.viewport.X,
                        this.targetedObject.Value.Y * Game1.tileSize - Game1.viewport.Y),
                    new Rectangle(194, 388, 16, 16),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    1f);
            }
        }

        /// <summary>
        /// Used to hit the tile targeted by the smart cursor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputOnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.config.SmartCursorHold)
                this.isHoldKeyDown = true;

            var dummy = new Farmer();
            this.isHoldKeyDown = e.IsDown(this.config.SmartCursorHold);

            if ((e.Button == SButton.MouseLeft || e.Button == SButton.ControllerX ||
                 Game1.input.GetMouseState().LeftButton == ButtonState.Pressed) && !Game1.player.UsingTool)
            {
                if (this.targetedObject.HasValue && Game1.player.CurrentTool != null)
                {
                    this.BreakObject(Game1.player, Game1.player.CurrentTool, true);

                    this.GatherResources(Game1.currentLocation);
                }
            }
        }

        /// <summary>
        ///     Triggered every time the player warps to a new map so its resources can be gathered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            this.targetedObject = null;
            this.GatherResources(e.NewLocation);
        }

        /// <summary>
        ///     Used to gather a location's resources on entry.
        /// </summary>
        /// <param name="location"></param>
        private void GatherResources(GameLocation location)
        {
            // If the world isn't ready, we return. This is a guard against this being called in GMCM setup.
            if (!Context.IsWorldReady)
                return;

            this.breakableResources.Clear();
            Stopwatch time = new();

            time.Start();

            // First, we loop through the location's objects and add them to our breakable resources list.
            foreach (KeyValuePair<Vector2, SObject> pair in location.Objects.Pairs)
            {
                if (pair.Value.Category == 0)
                    this.breakableResources.Add(new BreakableEntity(pair.Value, this.config));
            }

            // Then the same with terrain features.
            foreach (var feature in location.terrainFeatures.Values)
            {
                if (feature is Tree tree)
                    this.breakableResources.Add(new BreakableEntity(tree, this.config));
            }

            // Then with large terrain features.
            foreach (var feature in location.largeTerrainFeatures)
            {
                this.breakableResources.Add(new BreakableEntity(feature, this.config));
            }


            // And finally, resource clumps.
            foreach (var clump in location.resourceClumps)
            {
                this.breakableResources.Add(new BreakableEntity(clump, this.config));
                // this.logger.Log($"Clump parentSheetIndex: {clump.parentSheetIndex}");
            }

            // And, in case we're in the Secret Woods...
            if (location is Woods)
            {
                foreach (var clump in (location as Woods).stumps)
                {
                    this.breakableResources.Add((new BreakableEntity(clump, this.config)));
                }
            }

            time.Stop();

            if (time.ElapsedMilliseconds > 10)
                this.logger.Log(
                    $"Took {time.ElapsedMilliseconds}ms ({time.ElapsedTicks} ticks) to gather {location}'s breakable entities.",
                    LogLevel.Debug);
        }
    }
}
