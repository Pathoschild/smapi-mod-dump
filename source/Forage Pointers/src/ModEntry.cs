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
        private ModConfig Config;
        private int ScaleEveryNLevels;
        private int ViewScalingFactor;
        private int MinimumViewDistance;
        private static Vector2 offset = new Vector2(0f, -33f);
        private static bool shouldDraw = true;
        private static uint frameCounter = 0;
        #endregion

        #region modentry
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            ScaleEveryNLevels = Clamp(Config.ScaleEveryNLevels, 1, 10);
            ViewScalingFactor = Clamp(Config.ScalingRadius, 0, 50);
            MinimumViewDistance = Clamp(Config.MinimumViewDistance, 0, 100);

            Helper.Events.Display.RenderingHud += OnHudRendering;
            if(Config.BlinkPointers)
            {
                Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            }

        }
        #endregion

        #region helperfunctions
        /// <summary>
        /// Clamps an integer between an inclusive lower and upper bounds. (Because .NET Framework doesn't have this function)
        /// </summary>
        /// <param name="val">The value to clamp.</param>
        /// <param name="min">The inclusive minimum to clamp to.</param>
        /// <param name="max">The inclusive Maximum to clamp to.</param>
        /// <returns></returns>
        private int Clamp(int val, int min, int max)
        {
            if (val < min) return min;
            if (val > max) return max;
            return val;
        }
        #endregion

        #region eventListeners
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            frameCounter++;
            if (frameCounter < Config.NumFramesArrowsOn) shouldDraw = true;
            else shouldDraw = false;
            if (frameCounter >= (Config.NumFramesArrowsOn + Config.NumFramesArrowsOff)) frameCounter = 0;
        }

        private void OnHudRendering(object sender, RenderingHudEventArgs e)
        {
            if (Context.IsWorldReady && shouldDraw)
            {
                var loc = Game1.player.getTileLocation();
                var viewDist = Math.Pow(MinimumViewDistance + (Game1.player.ForagingLevel / ScaleEveryNLevels * ViewScalingFactor), 2);

                foreach (var v in Game1.currentLocation.objects.Pairs)
                {
                    if ((v.Value.isSpawnedObject || (Config.ShowArtifactSpots && v.Value.ParentSheetIndex == 590)) && Utility.isOnScreen(v.Key * 64f + new Vector2(32f, 32f), 64))
                    {
                        bool drawArrow = false;
                        if (Game1.player.professions.Contains(17) || Config.AlwaysShow) drawArrow = true; //They have the Tracker profession, always show arrows.
                        else if ((loc - v.Key).LengthSquared() <= viewDist) drawArrow = true;

                        if (drawArrow)
                        {
                            //Thanks to Esca for making this portion of the renderer easier to understand
                            Rectangle srcRect = new Rectangle(412, 495, 5, 4);
                            float renderScale = 5f;
                            Vector2 centerOfObject = new Vector2((v.Key.X * 64) + 32, (v.Key.Y * 64) + 32);
                            Vector2 targetPixel = centerOfObject + offset;

                            Vector2 trackerRenderPosition = Game1.GlobalToLocal(Game1.viewport, targetPixel); //get the target pixel's position relative to the viewport
                            trackerRenderPosition = Utility.ModifyCoordinatesForUIScale(trackerRenderPosition); //adjust for UI scaling and/or zoom

                            e.SpriteBatch.Draw(Game1.mouseCursors, trackerRenderPosition, new Rectangle?(srcRect), Color.White, (float)Math.PI, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
