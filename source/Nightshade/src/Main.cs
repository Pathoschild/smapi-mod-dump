/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/Nightshade
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Mods;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Enums;
using System;
using System.IO;
using System.Reflection;

namespace ichortower
{
    internal sealed class Nightshade : Mod
    {
        public static string ModId = null;
        public static Effect ColorShader = null;
        public static Effect DofShader = null;

        private static SpriteBatch sb = null;

        private static RenderTarget2D uiScreen = null;
        private static RenderTarget2D sceneScreen = null;
        private static bool usingColorizeWorld = false;
        private static bool usingColorizeUI = false;
        private static bool usingDepthOfField = false;

        public static ModConfig Config;

        public static Nightshade instance;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            ModId = this.ModManifest.UniqueID;
            try {
                byte[] stream = File.ReadAllBytes(Path.Combine(
                        helper.DirectoryPath, "assets/colorizer.mgfx"));
                Nightshade.ColorShader = new Effect(Game1.graphics.GraphicsDevice, stream);
                stream = File.ReadAllBytes(Path.Combine(
                        helper.DirectoryPath, "assets/depthoffield.mgfx"));
                Nightshade.DofShader = new Effect(Game1.graphics.GraphicsDevice, stream);
            }
            catch(Exception e) {
                Monitor.Log("Could not load a required shader!" +
                        " This mod will be disabled.", LogLevel.Error);
                Monitor.Log(e.ToString(), LogLevel.Error);
                return;
            }
            Nightshade.Config = helper.ReadConfig<ModConfig>();
            ApplyConfig(Nightshade.Config);

            var harmony = new Harmony(this.ModManifest.UniqueID);
            MethodInfo Game1_ShouldDrawOnBuffer = typeof(Game1).GetMethod(
                    "ShouldDrawOnBuffer",
                    BindingFlags.Public | BindingFlags.Instance);
            var post = new HarmonyMethod(typeof(Nightshade),
                    "Game1_ShouldDrawOnBuffer_Postfix");
            harmony.Patch(Game1_ShouldDrawOnBuffer,
                    postfix: post);

            sb = new SpriteBatch(Game1.graphics.GraphicsDevice);
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.Content.AssetReady += this.OnAssetReady;
            helper.Events.Display.Rendered += this.OnRendered;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.Specialized.LoadStageChanged += this.OnLoadStageChanged;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.Player.Warped += this.OnPlayerWarped;
        }

        public void ApplyConfig(ModConfig conf)
        {
            int index = conf.ColorizerActiveProfile;
            if (conf.ColorizeBySeason) {
                index = Game1.currentLocation?.GetSeasonIndex() ?? Game1.seasonIndex;
            }
            ColorizerPreset active = conf.ColorizerProfiles[index];
            ColorShader.Parameters["Saturation"].SetValue(active.Saturation);
            ColorShader.Parameters["Lightness"].SetValue(active.Lightness);
            ColorShader.Parameters["Contrast"].SetValue(active.Contrast);
            ColorShader.Parameters["ShadowRgb"].SetValue(new Vector3(
                    active.ShadowR, active.ShadowG, active.ShadowB));
            ColorShader.Parameters["MidtoneRgb"].SetValue(new Vector3(
                    active.MidtoneR, active.MidtoneG, active.MidtoneB));
            ColorShader.Parameters["HighlightRgb"].SetValue(new Vector3(
                    active.HighlightR, active.HighlightG, active.HighlightB));
            DofShader.Parameters["Field"].SetValue(conf.DepthOfFieldSettings.Field);
            DofShader.Parameters["Intensity"].SetValue(conf.DepthOfFieldSettings.Intensity);

            usingColorizeWorld = conf.ColorizeWorld;
            usingColorizeUI = conf.ColorizeUI;
            usingDepthOfField = conf.DepthOfFieldEnabled;
        }

        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            GMCMIntegration.Setup();
        }

        public void OnLoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == LoadStage.Preloaded) {
                ApplyConfig(Nightshade.Config);
            }
        }

        public void OnSaved(object sender, SavedEventArgs e)
        {
            ApplyConfig(Nightshade.Config);
        }

        public void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer) {
                ApplyConfig(Nightshade.Config);
            }
        }

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo($"Mods/{ModId}/Icons")) {
                e.LoadFromModFile<Texture2D>("assets/icons.png", AssetLoadPriority.Medium);
            }
        }

        public void OnAssetReady(object sender, AssetReadyEventArgs e)
        {
            if (e.Name.IsEquivalentTo($"Mods/{ModId}/Icons")) {
                ui.ShaderMenu.LoadIcons();
            }
        }

        /*
         * worldSource: the current render target for the world layer.
         * during map screenshots, the game uses a separate buffer of a
         * different size (UI layer not affected).
         */
        public void EnsureBuffers(RenderTarget2D worldSource, bool reallocate = false)
        {
            // we probably don't need to null coalesce here, but better safe
            // than sorry
            int sw = (worldSource ?? Game1.game1.screen).Width;
            int sh = (worldSource ?? Game1.game1.screen).Height;
            if (reallocate || sceneScreen is null || 
                    (sceneScreen.Width != sw || sceneScreen.Height != sh)) {
                sceneScreen?.Dispose();
                sceneScreen = new(Game1.graphics.GraphicsDevice, sw, sh);
            }
            int uw = Game1.game1.uiScreen.Width;
            int uh = Game1.game1.uiScreen.Height;
            if (reallocate || uiScreen is null || 
                    (uiScreen.Width != uw || uiScreen.Height != uh)) {
                uiScreen?.Dispose();
                uiScreen = new(Game1.graphics.GraphicsDevice, uw, uh);
            }
        }

        // attempting to run the shader after other mods do their OnRendered
        // drawing (GMCM, AT, FS...)
        [EventPriority(EventPriority.Low - 10)]
        public void OnRendered(object sender, RenderedEventArgs e)
        {
            if (!usingColorizeWorld && !usingColorizeUI) {
                return;
            }
            // call End/Begin to flush any pending draws in the spritebatch.
            // otherwise, they won't be drawn until after our shader.
            // the parameters to Begin are known and are the same as the ones
            // SMAPI uses to open the spritebatch when it raises the Rendered
            // event.
            e.SpriteBatch.End();
            e.SpriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp);
            // get current render target. we need to restore it later, and
            // we need to read from and write back to it.
            RenderTarget2D savedTarget = null;
            RenderTargetBinding[] rt = Game1.graphics.GraphicsDevice.GetRenderTargets();
            if (rt.Length > 0) {
                savedTarget = rt[0].RenderTarget as RenderTarget2D;
            }
            EnsureBuffers(savedTarget);

            // each layer (world, UI) is drawn to a separate back buffer and
            // then back to where it was, due to infelicities in how the game
            // handles its layer buffers. only one draw uses the shader: the
            // other one is just a blit.
            //
            // it is important to do the world layer second: during map
            // screenshots, the game renders to a target which is set to
            // RenderTargetUsage.DiscardContents, so it is automatically cleared
            // when it is set as the active render target; therefore we must
            // only do so once.
            if (usingColorizeUI) {
                Game1.SetRenderTarget(uiScreen);
                Game1.game1.GraphicsDevice.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        effect: Nightshade.ColorShader);
                sb.Draw(texture: Game1.game1.uiScreen,
                        position: Vector2.Zero,
                        color: Color.White);
                sb.End();
                Game1.SetRenderTarget(Game1.game1.uiScreen);
                Game1.game1.GraphicsDevice.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp);
                sb.Draw(texture: uiScreen,
                        position: Vector2.Zero,
                        color: Color.White);
                sb.End();
            }

            // the world layer runs the shader on the render back in, instead
            // of the render out. I believe Game1.lightmap leaves the layer in
            // a different state with alpha not premultiplied, so blitting it
            // first puts it in the state the shader expects.
            if (usingColorizeWorld) {
                Game1.SetRenderTarget(sceneScreen);
                Game1.game1.GraphicsDevice.Clear(Game1.bgColor);
                sb.Begin(SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp);
                sb.Draw(texture: savedTarget,
                        position: Vector2.Zero,
                        color: Color.White);
                sb.End();
                Game1.SetRenderTarget(savedTarget);
                sb.Begin(SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        effect: Nightshade.ColorShader);
                sb.Draw(texture: sceneScreen,
                        position: Vector2.Zero,
                        color: Color.White);
                sb.End();
            }
        }

        // this is much like OnRendered, but it's for the depth-of-field
        // shader, which applies only to the world layer.
        [EventPriority(EventPriority.Low - 10)]
        public void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!usingDepthOfField) {
                return;
            }
            if (Game1.game1.takingMapScreenshot) {
                return;
            }
            // flush pending draws
            e.SpriteBatch.End();
            e.SpriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp);
            // save current render target for restoration later
            RenderTarget2D savedTarget = null;
            RenderTargetBinding[] rt = Game1.graphics.GraphicsDevice.GetRenderTargets();
            if (rt.Length > 0) {
                savedTarget = rt[0].RenderTarget as RenderTarget2D;
            }
            EnsureBuffers(savedTarget);

            Nightshade.DofShader.Parameters["PitchX"]?.SetValue(
                    1f / (float)sceneScreen.Width);
            Nightshade.DofShader.Parameters["PitchY"]?.SetValue(
                    1f / (float)sceneScreen.Height);
            float ypos = Game1.player.getLocalPosition(Game1.viewport).Y;
            ypos += Game1.player.GetBoundingBox().Height / 2;
            ypos /= Game1.viewport.Height;
            if (ypos < 0f || ypos > 1.0f) {
                ypos = 0.5f;
            }
            Nightshade.DofShader.Parameters["Center"].SetValue(ypos);

            DofShader.CurrentTechnique = DofShader.Techniques[0];
            Game1.SetRenderTarget(sceneScreen);
            sb.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    effect: Nightshade.DofShader);
            sb.Draw(texture: Game1.game1.screen,
                    position: Vector2.Zero,
                    color: Color.White);
            sb.End();
            DofShader.CurrentTechnique = DofShader.Techniques[1];
            Game1.SetRenderTarget(Game1.game1.screen);
            sb.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    effect: Nightshade.DofShader);
            sb.Draw(texture: sceneScreen,
                    position: Vector2.Zero,
                    color: Color.White);
            sb.End();

            Game1.SetRenderTarget(savedTarget);
        }

        public static void Game1_ShouldDrawOnBuffer_Postfix(
                ref bool __result)
        {
            if (Game1.gameMode == Game1.playingGameMode) {
                __result = true;
            }
        }

        public void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (Game1.activeClickableMenu != null) {
                return;
            }
            if (Config.MenuKeybind.JustPressed()) {
                ui.ShaderMenu cfg = new();
                Game1.activeClickableMenu = cfg;
            }
        }

    }

}
