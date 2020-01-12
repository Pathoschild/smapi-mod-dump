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
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>The mod's config.json settings.</summary>
        ModConfig MConfig = null;

        /// <summary>The "address" of the custom tracker's texture in Stardew's content manager.</summary>
        string TrackerLoadString = "LooseSprites\\CustomTracker";

        /// <summary>If true, the custom tracker's image couldn't be loaded. Used to avoid repeating checks and error messages.</summary>
        bool FailedToLoadTracker = false;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MConfig = helper.ReadConfig<ModConfig>(); //load the mod's config.json file

            if (MConfig == null)
                return;
            
            if (MConfig.DrawBehindInterface) //if the tracker should be drawn behind the HUD
            {
                helper.Events.Display.RenderingHud += Display_RenderingHud; //use the "rendering" event
            }
            else //if the tracker should be drawn in front of the HUD
            {
                helper.Events.Display.RenderedHud += Display_RenderedHud; //use the "rendered" event
            }
        }

        /// <summary>Tasks performed before rendering the HUD.</summary>
        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
        {
            RenderCustomTracker(MConfig.ReplaceTrackersWithForageIcons);
        }

        /// <summary>Tasks performed after rendering the HUD.</summary>
        private void Display_RenderedHud(object sender, RenderedHudEventArgs e)
        {
            RenderCustomTracker(MConfig.ReplaceTrackersWithForageIcons);
        }

        /// <summary>Draws the custom tracker to Game1.spriteBatch, imitating code from the Game1.drawHUD method.</summary>
        /// <param name="forageIcon">If true, render the targeted forage object instead of the custom tracker icon.</param>
        private void RenderCustomTracker(bool forageIcon = false)
        {
            if (!Context.IsPlayerFree) //if the world isn't ready or the player isn't free
                return;

            if (!MConfig.EnableTrackersWithoutProfession && !Game1.player.professions.Contains(17)) //if the player needs to unlock the Tracker profession
                return;

            if (!Game1.currentLocation.IsOutdoors || Game1.eventUp || Game1.farmEvent != null) //if the player is indoors or an event is happening
                return;

            Texture2D spritesheet;
            Rectangle spriteSource;

            if (forageIcon || FailedToLoadTracker) //if forage icons are enabled OR the tracker has failed to load
            {
                spritesheet = Game1.objectSpriteSheet; //get the object spritesheet
            }
            else
            {
                try
                {
                    spritesheet = Game1.content.Load<Texture2D>(TrackerLoadString); //load the custom tracker spritesheet
                }
                catch (Exception ex)
                {
                    FailedToLoadTracker = true;
                    Monitor.Log($"Failed to load the custom tracker texture \"{TrackerLoadString}\". There may be a problem with the Content Patcher pack or its settings.", LogLevel.Warn);
                    Monitor.Log($"Forage icons will be displayed instead. Original error message:", LogLevel.Warn);
                    Monitor.Log($"{ex.Message}", LogLevel.Warn);
                    return;
                }
            }

            //define the tracker's rendering geometry
            const float scale = 4f; //the intended scale of the sprite

            //define relative minimum and maximum sprite positions
            Rectangle bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds; //get the boundaries of the screen
            float minX = 8f;
            float minY = 8f;
            float maxX = bounds.Right - 8;
            float maxY = bounds.Bottom - 8;

            //imitate SDV's Game1.drawHUD method to render a custom tracker icon
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in Game1.currentLocation.objects.Pairs)
            {
                if (((bool)((NetFieldBase<bool, NetBool>)pair.Value.isSpawnedObject) || pair.Value.ParentSheetIndex == 590) && !Utility.isOnScreen(pair.Key * 64f + new Vector2(32f, 32f), 64))
                {
                    if (forageIcon || FailedToLoadTracker) //if this is rendering forage icons
                    {
                        spriteSource = GameLocation.getSourceRectForObject(pair.Value.ParentSheetIndex); //get this object's spritesheet source rectangle
                    }
                    else //if this is rendering the custom tracker
                    {
                        spriteSource = new Rectangle(0, 0, spritesheet.Width, spritesheet.Height); //create a source rectangle covering the entire tracker spritesheet
                    }

                    Vector2 renderSize = new Vector2((float)spriteSource.Width * scale, (float)spriteSource.Height * scale); //get the render size of the sprite

                    Vector2 trackerRenderPosition = new Vector2();
                    float rotation = 0.0f;

                    Vector2 centerOfObject = new Vector2((pair.Key.X * 64) + 32, (pair.Key.Y * 64) + 32); //get the center pixel of the object
                    Vector2 targetPixel = new Vector2(centerOfObject.X - (renderSize.X / 2), centerOfObject.Y - (renderSize.Y / 2)); //get the top left pixel of the custom tracker's "intended" location

                    if (targetPixel.X > (double)(Game1.viewport.MaxCorner.X - 64)) //if the object is RIGHT of the screen
                    {
                        trackerRenderPosition.X = maxX; //use the predefined max X
                        rotation = 1.570796f;
                        targetPixel.Y = centerOfObject.Y - (renderSize.X / 2); //adjust Y for rotation
                    }
                    else if (targetPixel.X < (double)Game1.viewport.X) //if the object is LEFT of the screen
                    {
                        trackerRenderPosition.X = minX; //use the predefined min X
                        rotation = -1.570796f;
                        targetPixel.Y = centerOfObject.Y + (renderSize.X / 2); //adjust Y for rotation
                    }
                    else
                        trackerRenderPosition.X = targetPixel.X - (float)Game1.viewport.X; //use the target X (adjusted for viewport)

                    if (targetPixel.Y > (double)(Game1.viewport.MaxCorner.Y - 64)) //if the object is DOWN from the screen
                    {
                        trackerRenderPosition.Y = maxY; //use the predefined max Y
                        rotation = 3.141593f;
                        if (trackerRenderPosition.X > minX) //if X is NOT min (i.e. this is NOT the bottom left corner)
                        {
                            trackerRenderPosition.X = Math.Min(centerOfObject.X + (renderSize.X / 2) - (float)Game1.viewport.X, maxX); //adjust X for rotation (using renderPos, clamping to maxX, and adjusting for viewport)
                        }
                    }
                    else
                    {
                        trackerRenderPosition.Y = targetPixel.Y >= (double)Game1.viewport.Y ? targetPixel.Y - (float)Game1.viewport.Y : minY; //if the object is UP from the screen, use the predefined min Y; otherwise, use the target Y (adjusted for viewport)
                    }

                    if (trackerRenderPosition.X == minX && trackerRenderPosition.Y == minY) //if X and Y are min (TOP LEFT corner)
                    {
                        trackerRenderPosition.Y += spriteSource.Height; //adjust DOWN based on sprite size
                        rotation += 0.7853982f;
                    }
                    else if (trackerRenderPosition.X == minX && trackerRenderPosition.Y == maxY) //if X is min and Y is max (BOTTOM LEFT corner)
                    {
                        trackerRenderPosition.X += spriteSource.Width; //adjust RIGHT based on sprite size
                        rotation += 0.7853982f;
                    }
                    else if (trackerRenderPosition.X == maxX && trackerRenderPosition.Y == minY) //if X is max and Y is min (TOP RIGHT corner)
                    {
                        trackerRenderPosition.X -= spriteSource.Width; //adjust LEFT based on sprite size
                        rotation -= 0.7853982f;
                    }
                    else if (trackerRenderPosition.X == maxX && trackerRenderPosition.Y == maxY) //if X and Y are max (BOTTOM RIGHT corner)
                    {
                        trackerRenderPosition.Y -= spriteSource.Height; //adjust UP based on sprite size
                        rotation -= 0.7853982f;
                    }

                    Game1.spriteBatch.Draw(spritesheet, trackerRenderPosition, new Rectangle?(spriteSource), Color.White, rotation, new Vector2(2f, 2f), scale, SpriteEffects.None, 1f); //draw the trackerSheet on the game's main sprite batch
                }
            }
        }

        public class ModConfig
        {
            /// <summary>If true, trackers will be enabled even if the player doesn't have the Tracker profession.</summary>
            public bool EnableTrackersWithoutProfession = false;

            /// <summary>If true, an image of the forage being tracked will be displayed instead of the tracker icon.</summary>
            public bool ReplaceTrackersWithForageIcons = false;

            /// <summary>If true, trackers will be drawn behind the HUD. If false, they will be drawn in front of the HUD.</summary>
            public bool DrawBehindInterface = false;

            public ModConfig()
            {

            }
        }
    }
}
