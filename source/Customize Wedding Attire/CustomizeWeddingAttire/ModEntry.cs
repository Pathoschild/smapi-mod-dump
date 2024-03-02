/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/CustomizeWeddingAttire
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using HarmonyLib;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomizeWeddingAttire
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        // Add a config
        private ModConfig Config;
        // Add a translator
        private ITranslationHelper I18n;

        // Config option strings defined here as constants for use elsewhere
        public const string tuxOption = "weddingAttire.tuxOption";
        public const string dressOption = "weddingAttire.dressOption";
        public const string noneOption = "weddingAttire.noneOption";
        public const string defaultOption = "weddingAttire.defaultOption";

        // Remember if last tick was a wedding
        private bool wasWedding = false;
        private int timer = 0;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Read in config file and create if needed
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // Initialize the i18n helper
            this.I18n = this.Helper.Translation;

            // Initialize useful things from this class in WeddingPatcher
            WeddingPatcher.Initialize(this.Monitor, this.Config, this.I18n, this.ModManifest);

            // Apply the Harmony patches
            var harmony = new Harmony(this.ModManifest.UniqueID);
            WeddingPatcher.Apply(harmony);

            // Set up GMCM config when game is launched
            helper.Events.GameLoop.GameLaunched += SetUpConfig;

            // Update clothing during the wedding
            helper.Events.GameLoop.UpdateTicked += UpdateClothing;
        }

        private void SetUpConfig(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            // Try to get the GMCM menu
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                // Set up the modData recording wedding attire preferences even when GMCM is not installed
                Game1.player.modData[$"{this.ModManifest.UniqueID}/weddingAttirePref"] = this.Config.WeddingAttire;
                Monitor.Log("Saving player preferences into modData",LogLevel.Trace);
                return;
            }

            // Register the GMCM menu, and make sure write player preferences into moddata when the config is updated
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => {
                    Helper.WriteConfig(Config);
                    // Refresh the modData recording wedding attire preferences for this player
                    Game1.player.modData[$"{this.ModManifest.UniqueID}/weddingAttirePref"] = this.Config.WeddingAttire;
                    Monitor.Log("Saving player preferences into modData", LogLevel.Trace);
                }
            );
            // Add the mod description into the GMCM menu
            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => Helper.Translation.Get("mod.description")
                );
            // If GMCM Options is not installed, add a text dropdown for the config options
            // Otherwise, add in the fancy display options using GMCM Options
            var configMenuExt = Helper.ModRegistry.GetApi<IGMCMOptionsAPI>("jltaylor-us.GMCMOptions");
            if (configMenuExt is null) {
                configMenu.AddTextOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("weddingAttire.title"),
                    tooltip: () => this.Helper.Translation.Get("weddingAttire.description"),
                    getValue: () => this.Config.WeddingAttire,
                    setValue: value => this.Config.WeddingAttire = value,
                    allowedValues: new string[] {
                    tuxOption,
                    dressOption,
                    noneOption,
                    defaultOption
                    },
                    formatAllowedValue: (str) => this.Helper.Translation.Get(str)
                );
            } else {
                var values = new string[] {
                    tuxOption,
                    dressOption,
                    noneOption,
                    defaultOption
                    };
                configMenuExt.AddImageOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("weddingAttire.title"),
                    tooltip: () => this.Helper.Translation.Get("weddingAttire.description"),
                    getValue: () => (uint)Array.IndexOf(values, this.Config.WeddingAttire),
                    setValue: (idx) => this.Config.WeddingAttire = values[idx],
                    getMaxValue: () => (uint)values.Length - 1,
                    maxImageHeight: () => 128,
                    maxImageWidth: () => 64,
                    drawImage: (v, b, pos) => {
                        FarmerRenderer.isDrawingForUI = true;
                        var farmer = Game1.player;
                        var oldPantsColor = farmer.pantsColor.Value;
                        var oldDir = farmer.facingDirection.Value;
                        farmer.faceDirection(Game1.down);
                        if (v == 0 || v == 3 && farmer.IsMale) { // tux
                            WeddingPatcher.putInTux(farmer);

                        } else if (v == 1) { // dress
                            WeddingPatcher.putInDress(farmer);
                        }
                        farmer.FarmerRenderer.draw(b, farmer.FarmerSprite.CurrentAnimationFrame, farmer.FarmerSprite.CurrentFrame, farmer.FarmerSprite.SourceRect, pos, Vector2.Zero, 0.8f, Color.White, 0f, 1f, farmer);
                        farmer.changeShirt("-1");
                        farmer.changePantsColor(oldPantsColor);
                        farmer.changePantStyle("-1");
                        farmer.faceDirection(oldDir);
                        FarmerRenderer.isDrawingForUI = false;
                    },
                    label: (idx) => this.Helper.Translation.Get(values[idx]),
                    arrowLocation: (int)IGMCMOptionsAPI.ImageOptionArrowLocation.Sides,
                    labelLocation: (int)IGMCMOptionsAPI.ImageOptionLabelLocation.Top
                );

            }
        }

        private void UpdateClothing(object sender, UpdateTickedEventArgs e)
        {
            // Forcibly update the clothing during the wedding to fix the sleeves
            if (Game1.CurrentEvent is not null && Game1.CurrentEvent.isWedding)
            {
                Game1.player.UpdateClothing();
                wasWedding = true;
            }
            // Also update the clothing for 15 ticks after the wedding to get the normal sleeves back
            if (wasWedding)
            {
                Game1.player.UpdateClothing();
                timer++;
            }
            if (timer > 15)
            {
                wasWedding = false;
                timer = 0;
            }
        }
    }
}