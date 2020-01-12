using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace BetterHitboxStardew
{
    public class ModEntry : Mod
    {

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += InputEvents_ButtonPressed;
            helper.Events.Input.CursorMoved += Input_CursorMoved;
        }

        // Update the hitbox when further away from character
        private void Input_CursorMoved(object sender, CursorMovedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree || Game1.player.isCharging || Game1.player.isRidingHorse() || Game1.player.UsingTool)
            {
                return;
            }
            UpdateGrabTileAndCursor(e.NewPosition);
        }

        // Update the hitbox when button is pressed
        private void InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree || !(e.Button == SButton.MouseLeft) || Game1.player.isCharging || Game1.player.isRidingHorse() || Game1.player.UsingTool)
            {
                return;
            }
            UpdateGrabTileAndCursor(e.Cursor);
        }

        private void UpdateGrabTileAndCursor(ICursorPosition cursor)
        {

            double angle = GetAngle(cursor.ScreenPixels);
            Vector2 grabTile = Game1.player.getTileLocation();

            angle += 22.5;

            int dir;
            if (angle < 45)
            {
                grabTile += new Vector2(1, 0);
                dir = Game1.right;
            }
            else if (angle < 90)
            {
                grabTile += new Vector2(1, -1);
                dir = Game1.up;
            }
            else if (angle < 135)
            {
                grabTile += new Vector2(0, -1);
                dir = Game1.up;
            }
            else if (angle < 180)
            {
                grabTile += new Vector2(-1, -1);
                dir = Game1.up;
            }
            else if (angle < 225)
            {
                grabTile += new Vector2(-1, 0);
                dir = Game1.left;
            }
            else if (angle < 270)
            {
                grabTile += new Vector2(-1, 1);
                dir = Game1.down;
            }
            else if (angle < 315)
            {
                grabTile += new Vector2(0, 1);
                dir = Game1.down;
            }
            else if (angle < 360)
            {
                grabTile += new Vector2(1, 1);
                dir = Game1.down;
            }
            else
            {
                grabTile += new Vector2(1, 0);
                dir = Game1.right;
            }
            Game1.player.lastClick = grabTile * 64 + new Vector2(32, 32);
            Game1.player.FacingDirection = dir;
        }

        private double GetAngle(Vector2 screenPixels)
        {
            Vector2 absolutePixels = screenPixels + new Vector2(Game1.viewport.X, Game1.viewport.Y);
            Vector2 position = new Vector2((int)Game1.player.Position.X + 30, (int)Game1.player.Position.Y);
            Vector2 delta = absolutePixels - position;
            double angle = Math.Atan2(-delta.Y, delta.X);
            double angle2;

            if (angle < 0.0)
            {
                angle2 = 2 * Math.PI + angle;
            }
            else
            {
                angle2 = angle;
            }
            angle2 = (angle2 / (2.0 * Math.PI)) * 360;
            return angle2;
        }
    }
}
