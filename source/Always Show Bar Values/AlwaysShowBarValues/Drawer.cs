/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sarahvloos/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Menus;
using AlwaysShowBarValues.Config;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AlwaysShowBarValues.UIElements;


namespace AlwaysShowBarValues
{
    internal static class Drawer
    {
        /// <summary>Handles drawing for all available boxes, including our main one's</summary>
        /// <param name="spriteBatch">The sprite batch this should be drawn with</param>
        /// <param name="boxes">A list of all available boxes</param>
        /// <param name="isTopLayer">Whether this sprite batch is in the top or bottom layer of the HUD</param>
        internal static void DrawAllBoxes(SpriteBatch spriteBatch, List<StatBox> boxes, bool isTopLayer)
        {
            foreach (StatBox box in boxes) if (ShouldDraw(box, isTopLayer)) Draw(spriteBatch, box);
        }

        /// <summary>Draws a box with stat values in it</summary>
        /// <param name="spriteBatch">The sprite batch this should be drawn with</param>
        /// <param name="statBox">The box being drawn</param>
        private static void Draw(SpriteBatch spriteBatch, StatBox statBox)
        {
            // draw background
            if (statBox.BoxStyle is null || statBox.BoxStyle == "Round") DrawRoundBackground(spriteBatch, statBox.MessageSize.X, statBox.BoxPosition);
            else if (statBox.BoxStyle == "Toolbar") DrawToolbarBackground(spriteBatch, statBox.MessageSize.X, statBox.MessageSize.Y, statBox.BoxPosition);
            // draw text
            DrawStat(spriteBatch, statBox, new Vector2(48f, 28f), true);
            DrawStat(spriteBatch, statBox, new Vector2(48f, statBox.TopValue.MaxStringSize.Y + 20f), false);
        }

        /// <summary>Decides whether there is anything to be drawn, or whether it should be drawn at all.</summary>
        /// <param name="statBox">The box to be drawn</param>
        /// <param name="isTopLayer">Whether it'll be drawn on the HUD's top layer or the bottom one.</param>
        /// <returns></returns>
        private static bool ShouldDraw(StatBox statBox, bool isTopLayer)
        {
            // Is the box valid?
            if (statBox is null || !statBox.IsValid) return false;
            // Should I be drawing it at all?
            if (!statBox.Enabled || statBox.IsHidden || !statBox.ShouldDraw || statBox.Above != isTopLayer) return false;
            // Are its stats valid?
            if (!statBox.UpdateCurrentStats()) return false;
            // Is everything working as intended?
            if (!statBox.IsValid || statBox.TopValue is null || statBox.BottomValue is null) return false;
            return true;
        }

        /// <summary>Draws a stat's information inside the box.</summary>
        /// <param name="b">The sprite batch this should be drawn with</param>
        /// <param name="statBox">The box this stat is being drawn on</param>
        /// <param name="offset">The offset from the box's starting position</param>
        /// <param name="isTopStat">Whether this stat is the top or bottom one.</param>
        private static void DrawStat(SpriteBatch b, StatBox statBox, Vector2 offset, bool isTopStat)
        {
            PlayerStat stat = isTopStat ? statBox.TopValue : statBox.BottomValue;
            Vector2 startingPos = statBox.BoxPosition + offset;
            // icon
            Vector2 iconPosition = statBox.IconsLeftOfString ? startingPos + stat.Offset + new Vector2(-12f, 16f) : startingPos + stat.Offset + new Vector2(stat.MaxStringSize.X - 8f, 16f);
            b.Draw(Game1.mouseCursors, iconPosition, stat.IconSourceRectangle, Color.White * 1f, 0f, new Vector2(8f, 8f), stat.IconScale, SpriteEffects.None, 1f);
            Vector2 textPosition = statBox.IconsLeftOfString ? startingPos : startingPos + new Vector2(-16f, 0f);
            // draw bottom string
            if (statBox.TextShadow)
                Utility.drawTextWithShadow(b, stat.StatusString, Game1.smallFont, textPosition, stat.GetTextColor(), 1f, 1f, -1, -1, 1f);
            else
                b.DrawString(Game1.smallFont, stat.StatusString, textPosition, stat.GetTextColor());
        }

        /// <summary>Draws the background of a box, in case it should be round.</summary>
        /// <param name="b">The sprite batch this should be drawn with</param>
        /// <param name="messageWidth">The width of the text to be drawn inside the box</param>
        /// <param name="itemBoxPosition">The box's starting position</param>
        private static void DrawRoundBackground(SpriteBatch b, float messageWidth, Vector2 itemBoxPosition)
        {
            // left rounded corners
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X, itemBoxPosition.Y), new Rectangle(323, 360, 6, 24), Color.White * 1f, 0f, Vector2.Zero, 4.5f, SpriteEffects.FlipHorizontally, 1f);
            // middle rectangle
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 24f, itemBoxPosition.Y), new Rectangle(319, 360, 1, 24), Color.White * 1f, 0f, Vector2.Zero, new Vector2(messageWidth, 4.5f), SpriteEffects.None, 1f);
            // right rounded corners
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 24f + messageWidth, itemBoxPosition.Y), new Rectangle(323, 360, 6, 24), Color.White * 1f, 0f, Vector2.Zero, 4.5f, SpriteEffects.None, 1f);
        }

        /// <summary>Draws the background of a box, in case it should match the toolbar.</summary>
        /// <param name="b">The sprite batch this should be drawn with</param>
        /// <param name="messageWidth">The width of the text to be drawn inside the box</param>
        /// <param name="itemBoxPosition">The box's starting position</param>
        private static void DrawToolbarBackground(SpriteBatch b, float messageWidth, float messageHeight, Vector2 itemBoxPosition)
            {
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)itemBoxPosition.X + 10, (int)itemBoxPosition.Y + 6, (int)messageWidth + 32, (int)messageHeight + 36, Color.White, 1f, drawShadow: false);
            }
    }
}
