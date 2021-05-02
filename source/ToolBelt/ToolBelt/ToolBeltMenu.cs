/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/yuri0r/toolbelt
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace ToolBelt
{
    internal class ToolBeltMenu : IClickableMenu
    {
        private List<ToolBeltButton> buttons = new List<ToolBeltButton>();
        protected int buttonRadius;
        protected int lowestButtonY = -1;

        public bool gamepadMode;

        protected ToolBeltButton selectedButton;
        protected int wheelIndex = -1;

        public ToolBeltMenu()
        {
            buttonRadius = 24;

            width = 256;
            height = 256;
            xPositionOnScreen = (int)((float)(Game1.viewport.Width / 2) - (float)width / 2f);
            yPositionOnScreen = (int)((float)(Game1.viewport.Height / 2) - (float)height / 2f);
            snapToPlayerPosition();
        }

        public void updateToolList(Dictionary<Tool, int> dict)
        {
            //essentially re init the menu
            int count = 0;
            gamepadMode = false;
            buttons.Clear();
            selectedButton = null;

            foreach (KeyValuePair<Tool, int> kv in dict)
            {
                buttons.Add(makeToolButton(kv.Key, kv.Value));
                count++;
            }

            updateButtonRadius();
            repositionButtons();
        }

        private ToolBeltButton makeToolButton(Tool tool, int index)
        {
            Rectangle toolRect;
            Texture2D baseTexture;
            if (tool is MeleeWeapon | tool is Sword | tool is Slingshot)
            {
                baseTexture = Tool.weaponsTexture;
                toolRect = Game1.getSquareSourceRectForNonStandardTileSheet(baseTexture, 16, 16, tool.indexOfMenuItemView);
            }
            else
            {
                baseTexture = Game1.toolSpriteSheet;
                toolRect = Game1.getSquareSourceRectForNonStandardTileSheet(baseTexture, 16, 16, tool.indexOfMenuItemView);
            }
            return new ToolBeltButton(new Rectangle(0, 0, 64, 64), baseTexture, toolRect, 4f, index, tool, true);
        }

        public int closeAndReturnSelected()
        {
            exitThisMenu();
            if (wheelIndex < 0) return wheelIndex;
            return buttons[wheelIndex].getIndex();
        }

        public override void update(GameTime time)
        {
            snapToPlayerPosition();
            Vector2 offset = default(Vector2);
            if (!gamepadMode && Game1.options.gamepadControls && (Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Right.X) > 0.5f || Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Right.Y) > 0.5f))
            {
                gamepadMode = true;
            }

            if (gamepadMode)
            {
                if (Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Right.X) > 0.5f || Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Right.Y) > 0.5f)
                {
                    offset = new Vector2(Game1.input.GetGamePadState().ThumbSticks.Right.X, Game1.input.GetGamePadState().ThumbSticks.Right.Y);
                    offset.Y *= -1f;
                    offset.Normalize();
                    float highest_dot = -1f;
                    for (int j = 0; j < buttons.Count; j++)
                    {
                        buttons[j].deSelect();
                        float dot = Vector2.Dot(value2: new Vector2((float)buttons[j].bounds.Center.X - ((float)xPositionOnScreen + (float)width / 2f), (float)buttons[j].bounds.Center.Y - ((float)yPositionOnScreen + (float)height / 2f)), value1: offset);
                        if (dot > highest_dot)
                        {
                            highest_dot = dot;
                            wheelIndex = j;
                        }
                    }
                    selectedButton = buttons[wheelIndex];
                    selectedButton.select();
                }
                else
                {
                    ModEntry.swapItem(closeAndReturnSelected());
                }
            }

        }

        public override void performHoverAction(int x, int y)
        {
            if (gamepadMode)
            {
                return;
            }

            x = (int)Utility.ModifyCoordinateFromUIScale(x);
            y = (int)Utility.ModifyCoordinateFromUIScale(y);
            
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].containsPoint(x, y))
                {
                    wheelIndex = i;
                    selectedButton = buttons[i];
                    selectedButton.select();
                    return;
                }
                else
                {
                    buttons[i].deSelect();
                }
            }
            selectedButton = null;
            wheelIndex = -1;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            x = (int)Utility.ModifyCoordinateFromUIScale(x);
            y = (int)Utility.ModifyCoordinateFromUIScale(y);
            ModEntry.swapItem(closeAndReturnSelected());
            base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            receiveLeftClick(x, y, playSound);
        }

        protected void snapToPlayerPosition()
        {
            if (Game1.player != null)
            {
                Vector2 player_position = Game1.player.getLocalPosition(Game1.viewport) + new Vector2((float)(-width) / 2f, (float)(-height) / 2f);
                xPositionOnScreen = (int)player_position.X + 32;
                yPositionOnScreen = (int)player_position.Y - 64;
                if (xPositionOnScreen + width > Game1.viewport.Width)
                {
                    xPositionOnScreen -= xPositionOnScreen + width - Game1.viewport.Width;
                }
                if (xPositionOnScreen < 0)
                {
                    xPositionOnScreen -= xPositionOnScreen;
                }
                if (yPositionOnScreen + height > Game1.viewport.Height)
                {
                    yPositionOnScreen -= yPositionOnScreen + height - Game1.viewport.Height;
                }
                if (yPositionOnScreen < 0)
                {
                    yPositionOnScreen -= yPositionOnScreen;
                }
                repositionButtons();
            }
        }

        protected void repositionButtons()
        {
            lowestButtonY = -1;
            int x;
            int y;
            for (int i = 0; i < buttons.Count; i++)
            {
                ClickableTextureComponent button = buttons[i];
                float radians = Utility.Lerp(0f, (float)Math.PI * 2f, (float)i / (float)buttons.Count);
                x = (int)((float)(xPositionOnScreen + width / 2 + (int)(Math.Cos(radians) * (double)buttonRadius) * 4) - (float)button.bounds.Width / 2f);
                y = (int)((float)(yPositionOnScreen + height / 2 + (int)((0.0 - Math.Sin(radians)) * (double)buttonRadius) * 4) - (float)button.bounds.Height / 2f);                
                button.bounds.X = x;
                button.bounds.Y = y;
                if (lowestButtonY < y) lowestButtonY = y;
            }
        }

        private void updateButtonRadius()
        {
            buttonRadius = 25;
            if(buttons.Count > 7)
            {
                buttonRadius = buttons.Count * 4;
            }
        }

        public override void draw(SpriteBatch b)
        {
            Game1.StartWorldDrawInUI(b);
            
            if (!gamepadMode)
            {
                Game1.mouseCursorTransparency = 1f;
                drawMouse(b);
            }
            
            foreach (ClickableTextureComponent button in buttons)
            {
                button.draw(b);
            }

            if (selectedButton != null)
            {
                SpriteText.drawStringWithScrollCenteredAt(b, selectedButton.toolName(), xPositionOnScreen + width / 2, lowestButtonY + selectedButton.bounds.Height + 20); ;
            }
            Game1.EndWorldDrawInUI(b);
        }
    }
}