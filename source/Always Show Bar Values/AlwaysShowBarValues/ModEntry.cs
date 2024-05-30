/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sarahvloos/StardewMods
**
*************************************************/

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using AlwaysShowBarValues.Config;
using AlwaysShowBarValues.Integrations;
using AlwaysShowBarValues.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Menus;

namespace AlwaysShowBarValues
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ModConfig Config;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>This will help register config options to GenericModConfigMenu</summary>
        private GenericModConfigMenuRegistry? ConfigRegistry;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Display.RenderingHud += this.OnRenderingHud;
            helper.Events.Display.RenderedHud += this.OnRenderedHud;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        }


        /*********
        ** Private methods
        *********/

        /// <summary>Right after the game is launched, create a config menu for the player.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Create all available boxes, according to which mods the player has
            this.LoadAllBoxes();
            // Create a Generic Mod Config Menu menu for the mod
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;
            // Register this mod on GMCM
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );
            // Creates a new instance of the class that adds configuration options to GMCM
            ConfigRegistry = new(this.Config, this.ModManifest, configMenu);
            // Registers all available options to GMCM
            ConfigRegistry.RegisterAll(this.Config.GetStatBoxes());

        }

        /// <summary>Decides whether drawing on the HUD is allowed at that moment.</summary>
        /// <returns>Whether drawing on the HUD is advisable</returns>
        private static bool ShouldDrawOnHud()
        {
            // Shouldn't draw any stats if the player hasn't loaded a save yet
            if (!Context.IsWorldReady) return false;
            // Shouldn't draw anything on the HUD if an event is happening
            if (Game1.gameMode != 3 || Game1.freezeControls || Game1.panMode) return false;
            // Shouldn't draw anything on the HUD if it's hidden
            if (Game1.game1.takingMapScreenshot || !Game1.displayHUD) return false;
            return true;
        }

        /// <summary>Whenever the HUD is rendering, render the health/stamina values as well.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            if (ShouldDrawOnHud()) Drawer.DrawAllBoxes(spriteBatch: e.SpriteBatch, boxes: this.Config.GetStatBoxes(), isTopLayer: false);
        }

        /// <summary>Whenever the HUD is rendered, render the health/stamina values as well.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            if (ShouldDrawOnHud()) Drawer.DrawAllBoxes(spriteBatch: e.SpriteBatch, boxes: this.Config.GetStatBoxes(), isTopLayer: true);
        }

        /// <summary>whenever the player presses a button, check if it was one of our registered keybinds</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            foreach (StatBox box in this.Config.GetStatBoxes()) box.TryToggleShouldDraw();
        }

        /// <summary>Gets the mod instance of an existing mod.</summary>
        /// <param name="modInfo">The target mod's metadata.</param>
        /// <returns>The target mod's instance.</returns>
        private static Mod? GetModFromInfo (IModInfo modInfo)
        {
            if (modInfo is null) return null;
            if (modInfo.GetType() is not Type modInfoType) return null;
            if (modInfoType.GetProperty("Mod") is not PropertyInfo modPropertyInfo) return null;
            if (modPropertyInfo.GetValue(modInfo) is not object mod) return null;
            return (Mod)mod;
        }

        /// <summary>Creates StatBox instances for each box we're able to draw</summary>
        private void LoadAllBoxes()
        {
            // Survivalistic - Continued
            if (this.Helper.ModRegistry.Get("Ophaneom.Survivalistic") is IModInfo survivalisticModInfo && GetModFromInfo(survivalisticModInfo) is Mod survivalisticMod)
                this.Config.AddInstanceToMod("Survivalistic", survivalisticMod);
        }
    }
}