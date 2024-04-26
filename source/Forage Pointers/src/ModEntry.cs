/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/ForagePointers
**
*************************************************/

using System;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ForagePointers
{
    class ModEntry : Mod
    {
        #region variables
        private ModConfig config;
        private int scaleEveryNLevels;
        private int viewScalingFactor;
        private int minimumViewDistance;
        private static Vector2 offset = new(0f, -33f);
        private static bool shouldDraw = true;
        private static uint frameCounter = 0;
        private const float renderScale = 5f;
        #endregion

        #region modentry
        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();

            scaleEveryNLevels = Math.Clamp(config.ScaleEveryNLevels, 1, 10);
            viewScalingFactor = Math.Clamp(config.ScalingRadius, 0, 50);
            minimumViewDistance = Math.Clamp(config.MinimumViewDistance, 0, 100);
            if (config.NumFramesArrowsOff < 0) config.NumFramesArrowsOff = 0;
            if (config.NumFramesArrowsOn < 0) config.NumFramesArrowsOn = 0;

            Helper.Events.Display.RenderingHud += OnHudRendering;
            if(config.BlinkPointers)
            {
                Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            }

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;

        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;
            
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Always Show",
                tooltip: () => "If set, this will always show on-screen pointers no matter the range",
                getValue: () => this.config.AlwaysShow,
                setValue: value => this.config.AlwaysShow = value
            );
            
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show Artifact Spots",
                tooltip: () => "If set, Artifact Spots will also get a pointer",
                getValue: () => this.config.ShowArtifactSpots,
                setValue: value => this.config.ShowArtifactSpots = value
            );
            
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show Seed Spots",
                tooltip: () => "If set, Seed Spots will also get a pointer",
                getValue: () => this.config.ShowSeedSpots,
                setValue: value => this.config.ShowSeedSpots = value
            );
            
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Scaling Frequency",
                tooltip: () => "How many levels between scaling (i.e. 2 means that the radius increases upon reaching levels 2, 4, 6, etc.)",
                getValue: () => this.config.ScaleEveryNLevels,
                setValue: value => this.config.ScaleEveryNLevels = value,
                min: 1,
                max: 10
            );
            
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Scaling Amount",
                tooltip: () => "Radius increase when scaling occurs, 0 means no scaling, 1 means radius increases by 1 tile, values over 10 are impractical on most monitors",
                getValue: () => this.config.ScalingRadius,
                setValue: value => this.config.ScalingRadius = value,
                min: 0,
                max: 50
            );
            
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Minimum View Distance",
                tooltip: () => "Pointer will always show at this distance.",
                getValue: () => this.config.MinimumViewDistance,
                setValue: value => this.config.MinimumViewDistance = value,
                min: 0,
                max: 50
            );
            
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Blink Pointers",
                tooltip: () => "Determines if the pointers are either solid or blinking.",
                getValue: () => this.config.BlinkPointers,
                setValue: value => this.config.BlinkPointers = value
            );
            
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Number of Frames Arrows On",
                tooltip: () => "Approximate number of frames arrows will be on for before turning off.",
                getValue: () => this.config.NumFramesArrowsOn,
                setValue: value => this.config.NumFramesArrowsOn = value,
                min: 0
            );
            
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Number of Frames Arrows Off",
                tooltip: () => "Approximate number of frames arrows will be off for before turning on.",
                getValue: () => this.config.NumFramesArrowsOff,
                setValue: value => this.config.NumFramesArrowsOff = value,
                min: 0
            );
            
            
        }

        #endregion

        #region eventListeners
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            frameCounter++;
            shouldDraw = frameCounter < config.NumFramesArrowsOn;
            if (frameCounter >= (config.NumFramesArrowsOn + config.NumFramesArrowsOff)) frameCounter = 0;
        }

        private void OnHudRendering(object sender, RenderingHudEventArgs e)
        {
            if (!Context.IsPlayerFree || !shouldDraw) return; //Changing Context.IsWorldReady to Context.IsPlayerFree to avoid drawing pointer during event and when menu opened
            var loc = Game1.player.Tile; //getTileLocation removed
            var viewDist = Math.Pow(minimumViewDistance + (Game1.player.ForagingLevel / scaleEveryNLevels * viewScalingFactor), 2);

            foreach (var v in Game1.currentLocation.objects.Pairs)
            {
                if ((!v.Value.IsSpawnedObject && (!config.ShowArtifactSpots || v.Value.QualifiedItemId != "(O)590") &&
                     (!config.ShowSeedSpots || v.Value.QualifiedItemId != "(O)SeedSpot")) ||
                    !Utility.isOnScreen(v.Key * 64f + new Vector2(32f, 32f), 64)) continue;
                var drawArrow = false;
                if (Game1.player.professions.Contains(17) || config.AlwaysShow) drawArrow = true; //They have the Tracker profession, always show arrows.
                else if ((loc - v.Key).LengthSquared() <= viewDist) drawArrow = true;

                if (!drawArrow) continue;
                //Thanks to Esca for making this portion of the renderer easier to understand
                Rectangle srcRect = new(412, 495, 5, 4);
                Vector2 centerOfObject = new((v.Key.X * 64) + 32, (v.Key.Y * 64) + 32);
                Vector2 targetPixel = centerOfObject + offset;

                Vector2 trackerRenderPosition = Game1.GlobalToLocal(Game1.viewport, targetPixel); //get the target pixel's position relative to the viewport
                trackerRenderPosition = Utility.ModifyCoordinatesForUIScale(trackerRenderPosition); //adjust for UI scaling and/or zoom

                e.SpriteBatch.Draw(Game1.mouseCursors, trackerRenderPosition, new Rectangle?(srcRect), Color.White, (float)Math.PI, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
            }
        }

        #endregion
    }
}
