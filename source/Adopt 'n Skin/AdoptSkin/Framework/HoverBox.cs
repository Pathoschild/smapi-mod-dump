/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gathouria/Adopt-Skin
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace AdoptSkin.Framework
{
    class HoverBox
    {
        // Test to display on tooltip for Pet or Horse, if any
        internal static string HoverText;


        /// <summary>Renders the name hover tooltip if a pet or horse is being hovered over</summary>
        internal void RenderHoverTooltip(object sender, RenderingHudEventArgs e)
        {
            if (Context.IsPlayerFree && HoverText != null)
                this.DrawSimpleTooltip(Game1.spriteBatch, HoverText, Game1.smallFont);
        }


        /// <summary>Checks whether the user's cursor is currently over a Pet or Horse and updates the text to display in the tooltip</summary>
        internal void HoverCheck()
        {
            bool isHovering = false;
            Vector2 mousePos = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;

            // Show pet tooltip
            foreach (Pet pet in ModApi.GetAllPets())
                if (IsWithinSpriteBox(mousePos, pet))
                {
                    isHovering = true;
                    HoverText = pet.displayName;
                }
            // Show horse tooltip
            foreach (Horse horse in ModApi.GetAllHorses())
                if (IsWithinSpriteBox(mousePos, horse) && !ModEntry.BeingRidden.Contains(horse))
                {
                    isHovering = true;
                    HoverText = horse.displayName;
                }

            // Clear hover text when not hovering over a pet or horse
            if (!isHovering)
            {
                HoverText = null;
            }
        }


        /// <summary>Returns the tile position of the mouse</summary>
        private Vector2 GetMousePos() { return new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize; }


        /// <summary>Returns true if the given mouse cursor location is over the given pet or horse's location</summary>
        private bool IsWithinSpriteBox(Vector2 mousePos, Character creature)
        {
            // ** MAY NEED TO CHANGE FOR MULTIPLAYER **
            if (Game1.player.currentLocation == creature.currentLocation &&
                (int)mousePos.X >= creature.getTileX() && (int)mousePos.X <= (creature.getTileX() + creature.GetSpriteWidthForPositioning()) &&
                //1.4 CODE: (int)mousePos.X >= creature.getLeftMostTileX().X && (int)mousePos.X <= creature.getRightMostTileX().X &&
                    (int)mousePos.Y <= creature.getTileY() && (int)mousePos.Y >= (creature.getTileY() - 1))
                return true;

            return false;
        }


        /// <summary>Draw tooltip at the cursor position with the given message.</summary>
        /// <param name="b">The sprite batch to update.</param>
        /// <param name="hoverText">The tooltip text to display.</param>
        /// <param name="font">The tooltip font.</param>
        private void DrawSimpleTooltip(SpriteBatch b, string hoverText, SpriteFont font)
        {
            Vector2 textSize = font.MeasureString(hoverText);
            int width = (int)textSize.X + Game1.tileSize / 2;
            int height = Math.Max(60, (int)textSize.Y + Game1.tileSize / 2);
            int x = Game1.getOldMouseX() + Game1.tileSize / 2;
            int y = Game1.getOldMouseY() - Game1.tileSize / 2;
            if (x + width > Game1.viewport.Width)
            {
                x = Game1.viewport.Width - width;
                y += Game1.tileSize / 4;
            }
            if (y + height < 0)
            {
                x += Game1.tileSize / 4;
                y = Game1.viewport.Height + height;
            }
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, Color.White);
            if (hoverText.Length > 1)
            {
                Vector2 tPosVector = new Vector2(x + (Game1.tileSize / 4), y + (Game1.tileSize / 4 + 4));
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 2f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(0f, 2f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 0f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector, Game1.textColor * 0.9f, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }
    }
}
