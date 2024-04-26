/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/stardew-mods
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace EventBlackBars
{
    /// <summary> The mod entry class loaded by SMAPI. </summary>
    public class ModEntry : Mod
    {
        public static ModEntry Instance;
        public static Texture2D BlackRectangle;
        public static GraphicsDevice GraphicsDevice;
        
        public static bool RenderBars;
        public static float BarHeight;
        
        private bool _barsMovingIn;
        private bool _barsMovingOut;
        private ModConfig _config;


        /// <summary> The mod entry point, called after the mod is first loaded. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;

            helper.Events.Display.RenderingStep += OnRenderingStep;
            helper.Events.Display.WindowResized += OnWindowResized;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            
            _config = Helper.ReadConfig<ModConfig>();
            GraphicsDevice = Game1.graphics.GraphicsDevice;
            PrepareAssets(GraphicsDevice);
        }

        public void SaveConfig(ModConfig newConfig)
        {
            _config = newConfig;
            Helper.WriteConfig(newConfig);
        }

        /// <summary>
        /// "Move" bars in direction.
        /// </summary>
        public void StartMovingBars(Direction direction)
        {
            // don't start to move out the bars if they are not moved in
            if(direction == Direction.MoveOut && BarHeight <= 0) return;
            
            RenderBars = true;
            
            if (_config.MoveBarsInSmoothly)
            {
                BarHeight = direction == Direction.MoveIn ? 0 : GetMaxBarHeight(GraphicsDevice);

                _barsMovingIn = direction == Direction.MoveIn;
                _barsMovingOut = direction == Direction.MoveOut;
            }
            else
            {
                BarHeight = direction == Direction.MoveIn ? GetMaxBarHeight(GraphicsDevice) : 0;
            }
        }
        
        /// <summary>
        /// Prepare a black square for the bars.
        /// </summary>
        private void PrepareAssets(GraphicsDevice graphicsDevice)
        {
            BlackRectangle = new Texture2D(graphicsDevice, 1, 1);
            BlackRectangle.SetData(new [] { Color.Black });
        }

        /// <summary>
        /// Draw the bars.
        /// </summary>
        private void OnRenderingStep(object sender, RenderingStepEventArgs e)
        {
            
        }
        
        /// <summary>
        /// Smoothly "move" the bars when required.
        /// </summary>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!_barsMovingIn && !_barsMovingOut || !_config.MoveBarsInSmoothly) return;

            var maxBarHeight = GetMaxBarHeight(GraphicsDevice);
            var desiredBarHeight = _barsMovingIn ? maxBarHeight : 0;
            const float speed = 1f;
            
            // Quit resizing the bars when the desired height is about to be reached.
            if (Math.Abs(BarHeight - desiredBarHeight) <= 1f)
            {
                _barsMovingIn = _barsMovingOut = false;
                BarHeight = desiredBarHeight;

                RenderBars = desiredBarHeight != 0;
                
                return;
            }
            
            // Gradually change the bar height.
            BarHeight = desiredBarHeight > BarHeight ? BarHeight += speed : BarHeight -= speed;
        }
        
        /// <summary>
        /// Adjust the bar height when resized.
        /// </summary>
        private void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            if(_barsMovingIn || _barsMovingOut || BarHeight <= 0) return;

            BarHeight = GetMaxBarHeight(GraphicsDevice);
        }
        
        private void ApplyHarmonyPatches()
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                AccessTools.Method(typeof(Event), nameof(Event.exitEvent)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.EventEnd))
            );

            harmony.Patch(
                AccessTools.Method(typeof(Event), nameof(Event.drawAfterMap)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.DrawAfterMap))
            );
            
            harmony.Patch(
                AccessTools.Method(typeof(GameLocation), nameof(GameLocation.startEvent)),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.EventStart))
            );
        }

        /// <summary> Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ApplyHarmonyPatches();
            ModConfig.SetUpModConfigMenu(_config, this);
        }

        /// <summary>
        /// Given a GraphicsDevice, return the maximum size each bar should be.
        /// </summary>
        private int GetMaxBarHeight(GraphicsDevice graphicsDevice)
        {
            return Convert.ToInt16(graphicsDevice.Viewport.Height *
                                   MathHelper.Clamp((float)_config.BarHeightPercentage / 100f, 0f, 1f));
        }
    }

    public enum Direction
    {
        MoveIn,
        MoveOut
    }
}