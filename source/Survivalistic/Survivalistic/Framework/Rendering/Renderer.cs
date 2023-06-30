/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Survivalistic.Framework.Bars;
using Survivalistic.Framework.Common;
using System;

namespace Survivalistic.Framework.Rendering
{
    public class Renderer
    {
        public static void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.CurrentEvent != null) return;

            CheckMouseHovering();

            e.SpriteBatch.Draw(Textures.HungerSprite, new Rectangle((int)BarsPosition.barPosition.X, (int)BarsPosition.barPosition.Y - 240, Textures.HungerSprite.Width * 4, Textures.HungerSprite.Height * 4), Color.White);
            e.SpriteBatch.Draw(Textures.ThirstSprite, new Rectangle((int)BarsPosition.barPosition.X - 60, (int)BarsPosition.barPosition.Y - 240, Textures.ThirstSprite.Width * 4, Textures.ThirstSprite.Height * 4), Color.White);

            e.SpriteBatch.Draw(Textures.HungerFiller, new Vector2(BarsPosition.barPosition.X + 36, BarsPosition.barPosition.Y - 25), new Rectangle(0, 0, Textures.HungerFiller.Width * 6 * Game1.pixelZoom, (int)BarsInformations.hunger_percentage), BarsInformations.GetOffsetHungerColor(), 3.138997f, new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 1f);
            e.SpriteBatch.Draw(Textures.ThirstFiller, new Vector2(BarsPosition.barPosition.X - 24, BarsPosition.barPosition.Y - 25), new Rectangle(0, 0, Textures.ThirstFiller.Width * 6 * Game1.pixelZoom, (int)BarsInformations.thirst_percentage), BarsInformations.GetOffsetThirstyColor(), 3.138997f, new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 1f);
        
            if (BarsDatabase.render_numerical_hunger)
            {
                string information = $"{(int)ModEntry.data.actual_hunger}/{(int)ModEntry.data.max_hunger}";
                Vector2 text_size = Game1.dialogueFont.MeasureString(information);
                Vector2 text_position;
                if (BarsDatabase.right_side) text_position = new Vector2(-12, text_size.X);
                else text_position = new Vector2(12 + Textures.HungerSprite.Width * 4, 0);

                Game1.spriteBatch.DrawString(
                    Game1.dialogueFont,
                    information,
                    new Vector2(BarsPosition.barPosition.X + text_position.X, BarsPosition.barPosition.Y - 240 + ((Textures.HungerSprite.Height * 4) / 4) + 8),
                    new Color(255, 255, 255),
                    0f,
                    new Vector2(text_position.Y, 0),
                    1,
                    SpriteEffects.None,
                    0f);
            }

            if (BarsDatabase.render_numerical_thirst)
            {
                string information = $"{(int)ModEntry.data.actual_thirst}/{(int)ModEntry.data.max_thirst}";
                Vector2 text_size = Game1.dialogueFont.MeasureString(information);
                Vector2 text_position;
                if (BarsDatabase.right_side) text_position = new Vector2(-12, text_size.X);
                else text_position = new Vector2(12 + Textures.HungerSprite.Width * 4, 0);

                Game1.spriteBatch.DrawString(
                    Game1.dialogueFont,
                    information,
                    new Vector2(BarsPosition.barPosition.X - 60 + text_position.X, BarsPosition.barPosition.Y - 240 + ((Textures.HungerSprite.Height * 4) / 4) + 8),
                    new Color(255, 255, 255),
                    0f,
                    new Vector2(text_position.Y, 0),
                    1,
                    SpriteEffects.None,
                    0f);
            }
        }

        public static void CheckMouseHovering()
        {
            Vector2 _mouse_position = new Vector2(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y);

            if (_mouse_position.X >= BarsPosition.barPosition.X &&
                _mouse_position.X <= BarsPosition.barPosition.X + Textures.HungerSprite.Width * 4 &&
                _mouse_position.Y >= BarsPosition.barPosition.Y - 240 &&
                _mouse_position.Y <= BarsPosition.barPosition.Y - 240 + Textures.HungerSprite.Height * 4)
            {
                BarsDatabase.render_numerical_hunger = true;
            }
            else
            {
                BarsDatabase.render_numerical_hunger = false;
            }

            if (_mouse_position.X >= BarsPosition.barPosition.X - 60 &&
                _mouse_position.X <= BarsPosition.barPosition.X - 60 + Textures.HungerSprite.Width * 4 &&
                _mouse_position.Y >= BarsPosition.barPosition.Y - 240 &&
                _mouse_position.Y <= BarsPosition.barPosition.Y - 240 + Textures.HungerSprite.Height * 4)
            {
                BarsDatabase.render_numerical_thirst = true;
            }
            else
            {
                BarsDatabase.render_numerical_thirst = false;
            }
        }
    }
}
