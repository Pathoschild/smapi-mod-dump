/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingBubblesTracker
{
    public class ModEntry : Mod
    {
        private Texture2D SpriteSheet;
        private HashSet<GameLocation> LocationWithFishingPoints = new HashSet<GameLocation>();
        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            Helper.Events.Display.RenderedHud += RenderTracker;
        }

        private bool IsLocationAllowed(GameLocation location)
        {
            return location.IsOutdoors && !(
                location is BeachNightMarket ||
                (location is IslandLocation && !Game1.MasterPlayer.mailReceived.Contains("seenBoatJourney"))
            );
        }

        private void OnTimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            foreach (var location in Game1.locations.Where(IsLocationAllowed))
            {
                var splashPoint = location.fishSplashPoint.Value;
                if (splashPoint != Point.Zero)
                {
                    if (!LocationWithFishingPoints.Contains(location))
                    {
                        Game1.addHUDMessage(new HUDMessage($"A fishing point appeared at {location.Name} ({location.fishSplashPoint.Value.X}, {location.fishSplashPoint.Value.Y})", HUDMessage.newQuest_type));
                        LocationWithFishingPoints.Add(location);
                    }
                } 
                else
                {
                    if (LocationWithFishingPoints.Contains(location))
                    {
                        Game1.addHUDMessage(new HUDMessage($"The fishing point at \"{location.Name}\" disappeared...", HUDMessage.newQuest_type));
                        LocationWithFishingPoints.Remove(location);
                    }
                }
            }
        }

        private void OnDayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            SpriteSheet = Game1.content.Load<Texture2D>("LooseSprites/CustomTracker");
            LocationWithFishingPoints.Clear();
        }

        private void RenderTracker(object sender, StardewModdingAPI.Events.RenderedHudEventArgs e)
        {
            if (!(Context.IsPlayerFree && Game1.currentLocation.IsOutdoors && !Game1.eventUp && Game1.farmEvent == null)) return;

            var location = Game1.currentLocation;
            if (location.fishSplashPoint.Value != Point.Zero)
            {
                var splash = location.fishSplashPoint.Value;
                var spriteSource = new Rectangle(0, 0, SpriteSheet.Width, SpriteSheet.Height);
                var splashTile = new Vector2(splash.X, splash.Y);
                if (Utility.isOnScreen(splashTile * 64f + new Vector2(32f, 32f), 64)) return;

                var scale = 4f;
                Rectangle bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds; //get the boundaries of the screen

                //define relative minimum and maximum sprite positions
                float minX = 8f;
                float minY = 8f;
                float maxX = bounds.Right - 8;
                float maxY = bounds.Bottom - 8;

                Vector2 renderSize = new Vector2((float)SpriteSheet.Width * scale, (float)SpriteSheet.Height * scale); //get the render size of the sprite

                Vector2 centerOfObject = new Vector2((splashTile.X * 64) + 32, (splashTile.Y * 64) + 32); //get the center pixel of the object
                Vector2 targetPixel = new Vector2(centerOfObject.X - (renderSize.X / 2), centerOfObject.Y - (renderSize.Y / 2)); //get the top left pixel of the custom tracker's "intended" location

                Vector2 trackerRenderPosition = Game1.GlobalToLocal(Game1.viewport, targetPixel); //get the target pixel's position relative to the viewport
                trackerRenderPosition = Utility.ModifyCoordinatesForUIScale(trackerRenderPosition); //adjust for UI scaling and/or zoom
                trackerRenderPosition.X = Utility.Clamp(trackerRenderPosition.X, minX, maxX); //limit X to min/max
                trackerRenderPosition.Y = Utility.Clamp(trackerRenderPosition.Y, minY, maxY); //limit Y to min/max

                //define offsets to adjust for rotation 
                float offsetX = 0;
                float offsetY = 0;

                float rotation = 0f; //the rotation of the tracker sprite

                if (trackerRenderPosition.X == minX) //if the tracker is on the LEFT
                {
                    if (trackerRenderPosition.Y == minY) //if the tracker is on the TOP LEFT
                    {
                        offsetY = renderSize.X / 2f; //offset down by 1/2 sprite width
                        rotation = (float)Math.PI * 1.75f; //315 degrees
                    }
                    else if (trackerRenderPosition.Y == maxY) //if the tracker is on the BOTTOM LEFT
                    {
                        offsetX = renderSize.X / 2f; //offset right by 1/2 sprite width
                        offsetY = renderSize.X; //offset down by 1 sprite width
                        rotation = (float)Math.PI * 1.25f; //225 degrees
                    }
                    else
                    {
                        offsetY = renderSize.X; //offset down by 1 sprite width
                        rotation = (float)Math.PI * 1.5f; //270 degrees
                    }
                }
                else if (trackerRenderPosition.X == maxX) //if the tracker is on the RIGHT
                {
                    if (trackerRenderPosition.Y == minY) //if the tracker is on the TOP RIGHT
                    {
                        offsetX = -renderSize.X / 2f; //offset left by 1/2 sprite width
                        rotation = (float)Math.PI * 0.25f; //45 degrees
                    }
                    else if (trackerRenderPosition.Y == maxY) //if the tracker is on the BOTTOM RIGHT
                    {
                        offsetX = renderSize.X; //offset right by 1 sprite width
                        offsetY = -renderSize.X / 2f; //offset up by 1/2 sprite width
                        rotation = (float)Math.PI * 0.75f; //135 degrees
                    }
                    else
                    {
                        rotation = (float)Math.PI * 0.5f; //90 degrees
                    }
                }
                else if (trackerRenderPosition.Y == maxY) //if the tracker is on the BOTTOM
                {
                    offsetX = renderSize.X; //offset right by 1 sprite width
                    rotation = (float)Math.PI; //180 degrees
                }

                trackerRenderPosition.X = Utility.Clamp(trackerRenderPosition.X + offsetX, minX, maxX); //add offset to X (limited to min/max)
                trackerRenderPosition.Y = Utility.Clamp(trackerRenderPosition.Y + offsetY, minY, maxY); //add offset to Y (limited to min/max)

                Game1.spriteBatch.Draw(SpriteSheet, trackerRenderPosition, new Rectangle?(spriteSource), Color.White, rotation, new Vector2(2f, 2f), scale, SpriteEffects.None, 1f); //draw the spritesheet on the game's main sprite batch
            }
        }
    }
}
