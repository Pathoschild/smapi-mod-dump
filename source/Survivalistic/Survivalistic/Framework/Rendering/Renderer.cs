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
using StardewValley.Menus;
using Survivalistic.Framework.Databases;
using System.Collections.Generic;
using System.Linq;

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

            if (Game1.player.ActiveObject != null)
            {
                if (Foods.foodDatabase.TryGetValue(Game1.player.ActiveObject.Name, out string food_status_string))
                {
                    Vector2 sizeUI = new Vector2(Game1.uiViewport.Width, Game1.uiViewport.Height);

                    List<string> food_status = food_status_string.Split('/').ToList();
                    string actualString = "";
                    if (Int32.Parse(food_status[1]) > 0)
                    {
                        actualString += $"+{food_status[1]} Hydration";
                    }
                    if (Int32.Parse(food_status[1]) > 0 && Int32.Parse(food_status[0]) > 0)
                    {
                        actualString += "\n";
                    }
                    if (Int32.Parse(food_status[0]) > 0)
                    {
                        actualString += $"+{food_status[0]} Hunger";
                    }



                    string currentText = actualString;
                    Vector2 text_size = Game1.smallFont.MeasureString(currentText);
                    SpriteBatch b = e.SpriteBatch;
                    IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)(sizeUI.X / 2) - (int)(text_size.X / 2 + 25), (int)(sizeUI.Y) - 125 - (int)(text_size.Y + 25), (int)(text_size.X + 50), (int)(text_size.Y + 40), Color.White * 1, 1, false, 1);
                    Utility.drawTextWithShadow(b, currentText, Game1.smallFont, new Vector2((int)(sizeUI.X / 2) - (int)(text_size.X / 2 + 25) + 25, (int)(sizeUI.Y) - 125 - (int)(text_size.Y + 25) + 20), Game1.textColor);
                }
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

        public static void OnActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            /*
            if (!Context.IsWorldReady)
                return;

            

            if (Game1.player.CursorSlotItem != null)
            {
                if (Foods.foodDatabase.TryGetValue(Game1.player.CursorSlotItem.Name, out string food_status_string))
                {
                    List<string> food_status = food_status_string.Split('/').ToList();
                    string actualString = "";
                    if (Int32.Parse(food_status[1]) > 0)
                    {
                        actualString += $"+{food_status[1]} Hydration";
                    }
                    if (Int32.Parse(food_status[1]) > 0 && Int32.Parse(food_status[0]) > 0)
                    {
                        actualString += "\n";
                    }
                    if (Int32.Parse(food_status[0]) > 0)
                    {
                        actualString += $"+{food_status[0]} Hunger";
                    }

                    string currentText = actualString;
                    Vector2 text_size = Game1.smallFont.MeasureString(currentText);

                    SpriteBatch b = e.SpriteBatch;
                    IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), Game1.getOldMouseX() - (int)(text_size.X + 82), Game1.getOldMouseY(), (int)(text_size.X + 50), (int)(text_size.Y + 40), Color.White * 1, 1, false, 1);
                    Utility.drawTextWithShadow(b, currentText, Game1.smallFont, new Vector2(Game1.getOldMouseX() - (int)(text_size.X + 82) + 25, Game1.getOldMouseY() + 20), Game1.textColor);
                }
            }
            */
        }
    }
}
