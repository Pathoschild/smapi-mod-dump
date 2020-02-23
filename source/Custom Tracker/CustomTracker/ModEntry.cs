using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTracker
{
    /// <summary>The mod's main class.</summary>
    public partial class ModEntry : Mod
    {
        /// <summary>The "address" of the custom tracker's texture in Stardew's content manager.</summary>
        string TrackerLoadString { get; } = "LooseSprites\\CustomTracker";
        /// <summary>The "address" of the "forage mode" tracker's background texture in Stardew's content manager.</summary>
        string BackgroundLoadString { get; } = "LooseSprites\\CustomTrackerForageBG";

        /// <summary>The loaded texture for the custom tracker.</summary>
        Texture2D Spritesheet { get; set; } = null;
        /// <summary>The loaded texture for the "forage mode" tracker's background.</summary>
        Texture2D Background { get; set; } = null;
        /// <summary>A rectangle describing the spritesheet location of the custom tracker.</summary>
        Rectangle SpriteSource { get; set; }
        /// <summary>A rectangle describing the spritesheet location of the "forage mode" tracker's background.</summary>
        Rectangle BackgroundSource { get; set; }

        /// <summary>True after this mod has attempted to load its textures. Used to avoid unnecessary reloading.</summary>
        bool TexturesLoaded { get; set; } = false;
        /// <summary>True if forage icons are being displayed instead of the custom tracker. If the tracker texture cannot be loaded, this may be used as a fallback.</summary>
        bool ForageIconMode { get; set; } = false;

        /// <summary>The mod's config.json settings.</summary>
        ModConfig MConfig { get; set; } = null;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            /*** load config settings ***/

            MConfig = helper.ReadConfig<ModConfig>(); //load the mod's config.json file
            if (MConfig == null) //if loading failed
                return;

            //register the mod's SMAPI events
            helper.Events.Display.RenderedHud += Display_RenderedHud;
            helper.Events.Display.RenderingHud += Display_RenderingHud;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.GameLaunched += EnableGMCM;
        }
    }
}
