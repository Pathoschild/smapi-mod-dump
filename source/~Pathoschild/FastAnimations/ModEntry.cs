/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pathoschild.Stardew.Common;
using Pathoschild.Stardew.FastAnimations.Framework;
using Pathoschild.Stardew.FastAnimations.Handlers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Pathoschild.Stardew.FastAnimations
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config = null!; // set in Entry

        /// <summary>The animation handlers which skip or accelerate specific animations.</summary>
        private IAnimationHandler[] Handlers = null!; // set in Entry

        /// <summary>The <see cref="Handlers"/> filtered to those which need to be updated when the object list changes.</summary>
        private IAnimationHandlerWithObjectList[] HandlersWithObjectList = null!; // set in Entry


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            CommonHelper.RemoveObsoleteFiles(this, "FastAnimations.pdb"); // removed in 1.11.6

            this.Config = helper.ReadConfig<ModConfig>();
            this.UpdateConfig();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Events
        ****/
        /// <inheritdoc cref="IGameLoopEvents.GameLaunched" />
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // add Generic Mod Config Menu integration
            new GenericModConfigMenuIntegrationForFastAnimations(
                getConfig: () => this.Config,
                reset: () =>
                {
                    this.Config = new ModConfig();
                    this.Helper.WriteConfig(this.Config);
                    this.UpdateConfig();
                },
                saveAndApply: () =>
                {
                    this.Helper.WriteConfig(this.Config);
                    this.UpdateConfig();
                },
                modRegistry: this.Helper.ModRegistry,
                monitor: this.Monitor,
                manifest: this.ModManifest
            ).Register();
        }

        /// <inheritdoc cref="IGameLoopEvents.SaveLoaded" />
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            // initialize handlers
            foreach (IAnimationHandler handler in this.Handlers)
                handler.OnNewLocation(Game1.currentLocation);
        }

        /// <inheritdoc cref="IPlayerEvents.Warped" />
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.eventUp || !this.Handlers.Any() || !e.IsLocalPlayer)
                return;

            foreach (IAnimationHandler handler in this.Handlers)
                handler.OnNewLocation(e.NewLocation);
        }

        /// <inheritdoc cref="IWorldEvents.ObjectListChanged" />
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
        {
            if (e.IsCurrentLocation)
            {
                foreach (IAnimationHandlerWithObjectList handler in this.HandlersWithObjectList)
                    handler.OnObjectListChanged(e);
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.UpdateTicked" />
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!this.Handlers.Any())
                return;

            int playerAnimationId = Game1.player.FarmerSprite.currentSingleAnimation;
            foreach (IAnimationHandler handler in this.Handlers)
            {
                if (handler.IsEnabled(playerAnimationId))
                {
                    handler.Update(playerAnimationId);
                    break;
                }
            }
        }

        /****
        ** Methods
        ****/
        /// <summary>Apply the mod configuration if it changed.</summary>
        [MemberNotNull(nameof(ModEntry.Handlers))]
        private void UpdateConfig()
        {
            this.Handlers = this.GetHandlers(this.Config).ToArray();
            this.HandlersWithObjectList = this.Handlers.OfType<IAnimationHandlerWithObjectList>().ToArray();

            GameLocation location = Game1.currentLocation;
            if (location != null)
            {
                foreach (IAnimationHandler handler in this.Handlers)
                    handler.OnNewLocation(location);
            }
        }

        /// <summary>Get the enabled animation handlers.</summary>
        private IEnumerable<IAnimationHandler> GetHandlers(ModConfig config)
        {
            // player animations
            if (config.EatAndDrinkSpeed > 1 || config.DisableEatAndDrinkConfirmation)
                yield return new EatingHandler(config.EatAndDrinkSpeed, config.DisableEatAndDrinkConfirmation);
            if (config.FishingSpeed > 1)
                yield return new FishingHandler(config.FishingSpeed);
            if (config.HarvestSpeed > 1)
                yield return new HarvestHandler(config.HarvestSpeed);
            if (config.HoldUpItemSpeed > 1)
                yield return new HoldUpItemHandler(config.HoldUpItemSpeed);
            if (config.HorseFluteSpeed > 1)
                yield return new HorseFluteHandler(config.HorseFluteSpeed);
            if (config.MilkSpeed > 1)
                yield return new MilkingHandler(config.MilkSpeed);
            if (config.MountOrDismountSpeed > 1)
                yield return new MountHorseHandler(config.MountOrDismountSpeed);
            if (config.ReadBookSpeed > 1)
                yield return new ReadBookHandler(config.ReadBookSpeed);
            if (config.ShearSpeed > 1)
                yield return new ShearingHandler(config.ShearSpeed);
            if (config.ToolSwingSpeed > 1)
                yield return new ToolSwingHandler(config.ToolSwingSpeed);
            if (config.UseSlingshotSpeed > 1)
                yield return new SlingshotHandler(config.UseSlingshotSpeed);
            if (config.UseTotemSpeed > 1)
                yield return new UseTotemHandler(config.UseTotemSpeed);
            if (config.WeaponSwingSpeed > 1)
                yield return new WeaponSwingHandler(config.WeaponSwingSpeed);

            // world animations
            if (config.BreakGeodeSpeed > 1)
                yield return new BreakingGeodeHandler(config.BreakGeodeSpeed);
            if (config.CasinoSlotsSpeed > 1)
                yield return new CasinoSlotsHandler(config.CasinoSlotsSpeed);
            if (config.FishingTreasureSpeed > 1)
                yield return new FishingTreasureHandler(config.FishingTreasureSpeed);
            if (config.ForgeSpeed > 1)
                yield return new ForgeHandler(config.ForgeSpeed, this.Helper.Reflection);
            if (config.OpenChestSpeed > 1)
                yield return new OpenChestHandler(config.OpenChestSpeed);
            if (config.PamBusSpeed > 1)
                yield return new PamBusHandler(config.PamBusSpeed);
            if (config.PrizeTicketMachineSpeed > 1)
                yield return new PrizeTicketMachineHandler(config.PrizeTicketMachineSpeed, this.Helper.Reflection);
            if (config.TreeFallSpeed > 1)
                yield return new TreeFallingHandler(config.TreeFallSpeed);
            if (config.WheelSpinSpeed > 1)
                yield return new WheelSpinHandler(config.WheelSpinSpeed);

            // UI animations
            if (config.TitleMenuTransitionSpeed > 1)
                yield return new TitleMenuHandler(config.TitleMenuTransitionSpeed);
            if (config.LoadGameBlinkSpeed > 1)
                yield return new LoadGameMenuHandler(config.LoadGameBlinkSpeed);
        }
    }
}
