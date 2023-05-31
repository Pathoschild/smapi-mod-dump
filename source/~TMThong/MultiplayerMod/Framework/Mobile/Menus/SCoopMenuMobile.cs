/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiplayerMod.Framework.Mobile.Menus;
using MultiplayerMod.Framework.Patch.Mobile;
using StardewValley.Menus;
using StardewValley;
using System.Collections.Generic;

namespace MultiplayerMod.Framework.Mobile.Menus
{
    // Token: 0x020002B6 RID: 694
    public class SCoopMenuMobile : IClickableMenu
    {
        // Token: 0x06002695 RID: 9877 RVA: 0x002CA5F8 File Offset: 0x002C87F8

        public List<IClickableMenu> pages = new List<IClickableMenu>();
        public List<ClickableComponent> tabs = new List<ClickableComponent>();
        public ClickableComponent hostBtn;
        public ClickableComponent joinBtn;
        public SCoopMenuMobile()
        {
            this.width = Game1.viewport.Width;
            this.height = Game1.viewport.Height;
            initialize(0, 0, width, height, true);
            this.widthMod = (float)this.width / 1280f;
            this.heightMod = (float)this.height / 720f;
            this.tabWidth = 0;
            this.tabWidth = (Game1.viewport.Width - Game1Patch.xEdge * 2 - 80) / 8;
            this.tabCollisionHeight = 90;
            this.edgeX = Game1Patch.xEdge;
            this.edgeY = 8;
            this.tabY = (int)(5f * this.heightMod);
            pages.Add(new SCoopGameMenu(false));
            this.joinTextSize = Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\UI:CoopMenu_Join"));
            this.hostTextSize = Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\UI:CoopMenu_Host"));
            this.hostBtn = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1Patch.xEdge, this.yPositionOnScreen + this.tabY + this.edgeY, this.tabWidth, this.tabCollisionHeight), "host", "");
            this.joinBtn = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.edgeX + this.tabWidth, this.yPositionOnScreen + this.edgeY + this.tabY, this.tabWidth, this.tabCollisionHeight), "join", "");
            this.tabs.Add(joinBtn);
            this.tabs.Add(hostBtn);
            if (this.height > 600)
            {
                SCoopMenuMobile.tabHeight = 72;
            }
            else
            {
                SCoopMenuMobile.tabHeight = 68;
            }
            this.initializeUpperRightCloseButton();
        }

        // Token: 0x06002696 RID: 9878 RVA: 0x002CA7C0 File Offset: 0x002C89C0
        public override void update(GameTime time)
        {
            this.pages[0].update(time);
        }

        // Token: 0x06002697 RID: 9879 RVA: 0x002CA7D4 File Offset: 0x002C89D4
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if(hostBtn.containsPoint(x, y))
            {
                changeTab(true);
            }
            else if(joinBtn.containsPoint(x, y))
            {
                changeTab(false);
            }
            else
            {
                this.pages[0].releaseLeftClick(x, y);
            }
            base.receiveLeftClick(x, y, playSound);
        }

        // Token: 0x06002698 RID: 9880 RVA: 0x002CA875 File Offset: 0x002C8A75
        public override void releaseLeftClick(int x, int y)
        {
            if (hostBtn.containsPoint(x, y))
            {
                changeTab(true);
            }
            else if (joinBtn.containsPoint(x, y))
            {
                changeTab(false);
            }
            else
            {
                this.pages[0].releaseLeftClick(x, y);
            }
            base.releaseLeftClick(x, y);
        }

        // Token: 0x06002699 RID: 9881 RVA: 0x002CA88A File Offset: 0x002C8A8A
        public override void leftClickHeld(int x, int y)
        {
            this.pages[0].leftClickHeld(x, y);
        }

        // Token: 0x0600269A RID: 9882 RVA: 0x002CA8A0 File Offset: 0x002C8AA0
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64;
        }

        // Token: 0x0600269B RID: 9883 RVA: 0x002CA8F4 File Offset: 0x002C8AF4


        // Token: 0x0600269C RID: 9884 RVA: 0x002CA926 File Offset: 0x002C8B26
        public override bool readyToClose()
        {
            return this.pages[0].readyToClose();
        }

        // Token: 0x0600269D RID: 9885 RVA: 0x002CA942 File Offset: 0x002C8B42
        public void changeTab(bool isHost)
        {
            (this.pages[0] as SCoopGameMenu).isHostMenu = isHost;
        }

        public bool isHostMenu
        {
            get
            {
                return (this.pages[0] as SCoopGameMenu).isHostMenu;
            }
        }
        public RasterizerState _rasterizerState = new RasterizerState
        {
            ScissorTestEnable = true
        };

        // Token: 0x0600269E RID: 9886 RVA: 0x002CA980 File Offset: 0x002C8B80
        public override void draw(SpriteBatch b)
        {
            try
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.8f);

                this.pages[0].draw(b);
                //this.upperRightCloseButton.draw(b);
                b.End();
                b.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, this._rasterizerState, null, null);
                Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
                b.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, this.width, tabHeight + 16);
                foreach (ClickableComponent clickableComponent in this.tabs)
                {
                    if (clickableComponent.name == "join")
                    {
                        drawTab(b, clickableComponent.bounds.X, clickableComponent.bounds.Y, this.tabWidth, SCoopMenuMobile.tabHeight - 4, !isHostMenu, isHostMenu);
                        if (!isHostMenu)
                        {
                            Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:CoopMenu_Join"), Game1.smallFont, new Vector2((float)clickableComponent.bounds.X + ((float)clickableComponent.bounds.Width - this.joinTextSize.X) / 2f, (float)clickableComponent.bounds.Y + ((float)clickableComponent.bounds.Height - this.joinTextSize.Y) / 2f), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                        }
                        else
                        {
                            b.DrawString(Game1.smallFont, Game1.content.LoadString("Strings\\UI:CoopMenu_Join"), UtilityPatch.To4(new Vector2((float)clickableComponent.bounds.X + ((float)clickableComponent.bounds.Width - this.joinTextSize.X) / 2f, (float)clickableComponent.bounds.Y + ((float)clickableComponent.bounds.Height - this.joinTextSize.Y) / 2f)), Game1.textColor);
                        }
                    }
                    else if (clickableComponent.name == "host")
                    {
                        drawTab(b, clickableComponent.bounds.X, clickableComponent.bounds.Y, this.tabWidth, SCoopMenuMobile.tabHeight - 4, isHostMenu, isHostMenu);
                        if (isHostMenu)
                        {
                            Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:CoopMenu_Host"), Game1.smallFont, new Vector2((float)clickableComponent.bounds.X + ((float)clickableComponent.bounds.Width - this.hostTextSize.X) / 2f, (float)clickableComponent.bounds.Y + ((float)clickableComponent.bounds.Height - this.hostTextSize.Y) / 2f), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                        }
                        else
                        {
                            b.DrawString(Game1.smallFont, Game1.content.LoadString("Strings\\UI:CoopMenu_Host"), UtilityPatch.To4(new Vector2((float)clickableComponent.bounds.X + ((float)clickableComponent.bounds.Width - this.hostTextSize.X) / 2f, (float)clickableComponent.bounds.Y + ((float)clickableComponent.bounds.Height - this.hostTextSize.Y) / 2f)), Game1.textColor);
                        }
                    }
                }
                b.End();
                b.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            }
            catch(Exception e)
            {
                ModUtilities.ModMonitor.Log(e.Message, StardewModdingAPI.LogLevel.Error);
            }
        }
        public void drawTab(SpriteBatch b, int x, int y, int width, int height, bool isSelected = false, bool leftSmooth = false)
        {
            width -= 8;
            Rectangle rectangle = new Rectangle(91, 80, 16, 19);
            float num = 4f;
            Texture2D mobileSpriteSheet = Game1Patch.mobileSpriteSheet;
            int num2 = 4;
            Color color = isSelected ? Color.White : Color.DarkGray;
            int num3 = isSelected ? 16 : 0;
            b.Draw(mobileSpriteSheet, new Rectangle((int)((float)num2 * num) + x, (int)((float)num2 * num) + y, width - (int)((float)num2 * num * 2f) + 4, SCoopMenuMobile.tabHeight - (int)((float)num2 * num) + num3), new Rectangle?(new Rectangle(num2 + rectangle.X, num2 + rectangle.Y, num2, 2)), color, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);
            b.Draw(mobileSpriteSheet, new Vector2((float)x, (float)y), new Rectangle?(new Rectangle(rectangle.X, rectangle.Y, num2, num2)), color, 0f, Vector2.Zero, num, SpriteEffects.None, 0.8f);
            b.Draw(mobileSpriteSheet, new Vector2((float)(x + width - (int)((float)num2 * num)), (float)y), new Rectangle?(new Rectangle(rectangle.X + num2 * 2, rectangle.Y, num2 + 2, num2)), color, 0f, Vector2.Zero, num, SpriteEffects.None, 0.8f);
            b.Draw(mobileSpriteSheet, new Rectangle(x + (int)((float)num2 * num), y, width - (int)((float)num2 * num) * 2, (int)((float)num2 * num)), new Rectangle?(new Rectangle(rectangle.X + num2, rectangle.Y, num2, num2)), color, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);
            if (!isSelected || !leftSmooth)
            {
                b.Draw(mobileSpriteSheet, new Rectangle(x, y + (int)((float)num2 * num), (int)((float)num2 * num), SCoopMenuMobile.tabHeight - (int)((float)num2 * num)), new Rectangle?(new Rectangle(rectangle.X, num2 + rectangle.Y, num2, 2)), color, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);
            }
            else
            {
                b.Draw(mobileSpriteSheet, new Rectangle(x, y + (int)((float)num2 * num), (int)((float)num2 * num), SCoopMenuMobile.tabHeight - (int)((float)num2 * num) + num3), new Rectangle?(new Rectangle(rectangle.X, num2 + rectangle.Y, num2, 2)), color, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);
            }
            b.Draw(mobileSpriteSheet, new Rectangle(x + width - (int)((float)num2 * num), y + (int)((float)num2 * num), (int)(6f * num), SCoopMenuMobile.tabHeight - (int)((float)num2 * num)), new Rectangle?(new Rectangle(rectangle.X + num2 * 2, num2 + rectangle.Y, 6, 1)), color, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);
            if (isSelected)
            {
                b.Draw(mobileSpriteSheet, new Rectangle(x + width - 20, this.yPositionOnScreen + this.edgeY + SCoopMenuMobile.tabHeight, 24, 20), new Rectangle?(new Rectangle(98, 92, 6, 5)), color, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);
                if (!leftSmooth)
                {
                    b.Draw(mobileSpriteSheet, new Rectangle(x, this.yPositionOnScreen + this.edgeY + SCoopMenuMobile.tabHeight, 24, 20), new Rectangle?(new Rectangle(91, 92, 6, 5)), color, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);
                }
            }
        }
        // Token: 0x04001E7D RID: 7805
        private DialogueBox tutorialDialog;

        // Token: 0x04001E7E RID: 7806
        private bool wizardSource;

        // Token: 0x04001E7F RID: 7807
        private static bool showTutorialDialog;

        // Token: 0x04001E80 RID: 7808
        private Vector2 joinTextSize;

        // Token: 0x04001E81 RID: 7809
        private Vector2 hostTextSize;
        private float widthMod;
        private float heightMod;
        private int tabWidth;
        private int tabCollisionHeight;
        private int edgeX;
        private int edgeY;
        private int tabY;
        private static int tabHeight;
    }
}
