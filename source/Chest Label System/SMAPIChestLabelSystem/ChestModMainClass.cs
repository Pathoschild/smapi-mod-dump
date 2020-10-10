/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/speeder1/ChestNameWithHoverLabel
**
*************************************************/

/*
    Copyright 2016 Maurício Gomes (Speeder)

    Speeder's Chest Labeling System is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Speeder's Chest Labeling System are distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Speeder's Chest Labeling System.  If not, see <http://www.gnu.org/licenses/>.
 */

using StardewValley;
using StardewModdingAPI;
using System;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using SpeederSDVUIUtils;

namespace SMAPIChestLabelSystem
{
    public class ChestModMainClass : Mod
    {
        public override void Entry(IModHelper helper)
        {            
            helper.Events.GameLoop.UpdateTicked += this.EventUpdateTick;
            helper.Events.Display.RenderedActiveMenu += this.PostEventGUIDrawTick;
            helper.Events.Display.RenderedHud += this.PostEventHUDDrawTick;
        }

        Vector2 chestKey;
        ClickableTextureComponent labelButton;
        IClickableMenu lastMenu;
        SpeederSDVUIUtils.TextBox chestNameBox;
        bool leftClickWasDown = false;
        bool hovered = false;
        InputButton[] oldMenuButton;
        String hoverChestName;

        void EventUpdateTick(object sender, EventArgs e)
        {            
            if (Game1.currentLocation == null) return;            
            hoverChestName = null;
            if(lastMenu == null || Game1.activeClickableMenu == null || lastMenu != Game1.activeClickableMenu)
            {
                if(chestNameBox != null && Game1.keyboardDispatcher.Subscriber == chestNameBox) Game1.keyboardDispatcher.Subscriber = null;
                labelButton = null;
                leftClickWasDown = false;
                chestNameBox = null;                
                if(oldMenuButton != null)
                {
                    Game1.options.menuButton = oldMenuButton;
                    oldMenuButton = null;
                }
            }
            GameLocation currentLocation = Game1.currentLocation;
                     
            StardewValley.Objects.Chest openChest = null;            

            foreach (KeyValuePair<Vector2, StardewValley.Object> keyPair in currentLocation.Objects.Pairs)
            {
                if (keyPair.Value is StardewValley.Objects.Chest)
                {
                    openChest = keyPair.Value as StardewValley.Objects.Chest;                    
                    if (openChest.mutex.IsLocked() && Game1.activeClickableMenu is ItemGrabMenu)
                    {
                        
                        lastMenu = Game1.activeClickableMenu;
                        chestKey = keyPair.Key;
                        break;
                    }                    
                    if(openChest.getBoundingBox(keyPair.Key).Contains(Game1.getMouseX()+Game1.viewport.X, Game1.getMouseY()+Game1.viewport.Y))
                    {
                        hoverChestName = openChest.name;
                    }

                    openChest = null;
                }
            }

            if (openChest == null) return;

            if(labelButton == null)
            {                
                chestNameBox = new SpeederSDVUIUtils.TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor);
                chestNameBox.Text = openChest.name;
                chestNameBox.X = lastMenu.xPositionOnScreen;
                chestNameBox.Y = lastMenu.yPositionOnScreen + lastMenu.height - Game1.tileSize*2;
                chestNameBox.Width = Game1.tileSize * 5;
                labelButton = new ClickableTextureComponent("label-chest", new Rectangle(chestNameBox.X + chestNameBox.Width + Game1.tileSize/2, chestNameBox.Y, Game1.tileSize, Game1.tileSize), "", "Label Chest", Game1.mouseCursors, new Rectangle(128, 256, 64, 64), (float)Game1.pixelZoom/6f);
            }

            int mouseX = Game1.getOldMouseX();
            int mouseY = Game1.getOldMouseY();
            bool mouseJustReleased = false;
            if(leftClickWasDown == true && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                mouseJustReleased = true;                
            }

            labelButton.tryHover(mouseX, mouseY);
            
            Rectangle chestNameBoxBoundingBox = new Rectangle(chestNameBox.X, chestNameBox.Y, chestNameBox.Width, chestNameBox.Height);

            if (chestNameBoxBoundingBox.Contains(mouseX, mouseY))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed) leftClickWasDown = true;
                else leftClickWasDown = false;

                chestNameBox.Highlighted = true;        

                if (mouseJustReleased)
                {
                    chestNameBox.SelectMe();
                    if (oldMenuButton == null)
                    {
                        oldMenuButton = Game1.options.menuButton;
                        Game1.options.menuButton = new InputButton[] { };
                    }
                    //Game1.freezeControls = true;
                }                
            }
            else if(chestNameBox.Selected == false)
            {
                chestNameBox.Highlighted = false;
            }

            if (labelButton.containsPoint(mouseX, mouseY))
            {                
                if (Mouse.GetState().LeftButton == ButtonState.Pressed) leftClickWasDown = true;
                else leftClickWasDown = false;
                hovered = true;

                if (mouseJustReleased && chestNameBox.Selected)
                {
                    chestNameBox.Selected = false;
                    chestNameBox.Highlighted = false;
                    Game1.keyboardDispatcher.Subscriber = null;
                    currentLocation.objects[chestKey].name = chestNameBox.Text;
                    Game1.playSound("smallSelect");
                    Game1.options.menuButton = oldMenuButton;
                    oldMenuButton = null;                    
                }
            }
            else
            {
                hovered = false;
            }
        }
        
        void PostEventGUIDrawTick(object sender, EventArgs e)
        {            
            if (labelButton != null)
            {                                
                Utility.drawWithShadow(Game1.spriteBatch, labelButton.texture, new Vector2((float)labelButton.bounds.X + (float)(labelButton.sourceRect.Width / 2) * labelButton.baseScale, (float)labelButton.bounds.Y + (float)(labelButton.sourceRect.Height / 2) * labelButton.baseScale), labelButton.sourceRect, Color.White, 0f, new Vector2((float)(labelButton.sourceRect.Width / 2), (float)(labelButton.sourceRect.Height / 2)), labelButton.scale, false, 0, -1, -1, 0.35f);
                chestNameBox.Draw(Game1.spriteBatch, false);
                if (hovered)
                {
                    SpeederIClickableMenu.drawSimpleTooltip(Game1.spriteBatch, labelButton.hoverText, Game1.smallFont);
                }
                Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16)), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0);                                
            }            
        }

        void PostEventHUDDrawTick(object sender, EventArgs e)
        {            
            if (hoverChestName != null && hoverChestName != "Chest" && hoverChestName != "" && Game1.activeClickableMenu == null)
            {                
                SpeederIClickableMenu.drawSimpleTooltip(Game1.spriteBatch, hoverChestName, Game1.smallFont);                
            }
        }
    }
}
