using System;
using System.Collections.Generic;
using InfiniteBackpack.Framework.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace InfiniteBackpack
{
    public class InfiniteBackpack : Mod
    {
        private IbConfig _config; //Sets up the config file.
        private SButton _prevTabKey;
        private SButton _nextTabKey;
        private Texture2D backpack;
        private bool _drawSlider;
        private Vector2 _tabLoc = new Vector2(-1, -1);
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<IbConfig>();
            backpack = helper.Content.Load<Texture2D>("assets/backpack.png");

            //Events
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.Rendered += OnRendered;

        }



        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            
            if (e.IsDown(_nextTabKey))
            {
                //Lets do the changing of tabs, we will use the in-game option
                Game1.player.shiftToolbar(true);//Changes to the next tab
            }

            if (e.IsDown(_prevTabKey))
            {
                Game1.player.shiftToolbar(false);//Changes to the previous tab
            }

            if (e.IsDown(SButton.R))
            {
                Game1.player.increaseBackpackSize(12);
            }
            if (e.IsDown(SButton.MouseLeft) && Game1.activeClickableMenu is GameMenu)
            {
                List<IClickableMenu> tabs = Helper.Reflection.GetField<List<IClickableMenu>>(Game1.activeClickableMenu, "pages").GetValue();
                IClickableMenu curTab = tabs[(Game1.activeClickableMenu as GameMenu).currentTab];


                int xval = curTab.xPositionOnScreen + curTab.width;
                int yval = curTab.yPositionOnScreen + curTab.height;
                if (e.Cursor.ScreenPixels.X > (xval - 100) && e.Cursor.ScreenPixels.X < (xval - 50) &&
                    e.Cursor.ScreenPixels.Y > (yval - 100) && e.Cursor.ScreenPixels.Y < (yval - 50))
                {
                    //Game1.player.increaseBackpackSize(12);
                    Response yes = new Response("Yes", $"Buy tab # for  coins?");
                    Response no = new Response("No", "No thanks.");
                    Response[] responses = {yes, no};
                    Game1.currentLocation.createQuestionDialogue("Would you like to buy an extra inventory tab?", responses, "do_inv_increase");
                }
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Enum.TryParse<SButton>(_config.NextTabButton, true, out _nextTabKey))
            {
                _nextTabKey = SButton.NumPad1;
            }
            if (!Enum.TryParse<SButton>(_config.PrevTabButton, true, out _prevTabKey))
            {
                _prevTabKey = SButton.NumPad2;
            }
        }

        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (Context.IsWorldReady &&
                (Game1.activeClickableMenu is GameMenu || Game1.activeClickableMenu is ItemGrabMenu) &&
                Game1.player.MaxItems >= 36)
            {
                float f = 1.0f;
                if (Game1.activeClickableMenu is GameMenu gm)
                {
                    List<IClickableMenu> tabs = Helper.Reflection
                        .GetField<List<IClickableMenu>>(Game1.activeClickableMenu, "pages").GetValue();
                    IClickableMenu curTab = tabs[((GameMenu) Game1.activeClickableMenu).currentTab];

                    if (curTab is InventoryPage inv)
                    {
                        f = Math.Min(0.1f + (Vector2.Distance(new Vector2(Game1.getMouseX(), Game1.getMouseY()), new Vector2(curTab.xPositionOnScreen + curTab.width - 100, curTab.yPositionOnScreen + curTab.height - 280)) / 200.0f), 1.0f);
                        //Lets try hovering stuff
                        int xval = curTab.xPositionOnScreen + curTab.width;
                        int yval = curTab.yPositionOnScreen + curTab.height;
                        if (Game1.getMouseX() > (xval - 100) && Game1.getMouseX() < (xval - 50) &&
                            Game1.getMouseY() > (yval - 100) && Game1.getMouseX() < (yval - 50))
                        {
                            IClickableMenu c = Game1.activeClickableMenu;
                            var b = Game1.spriteBatch;
                            DrawSimpleTooltip(b, "Test", Game1.smallFont);
                            //c.drawHoverText(b, "Test Hover", Game1.smallFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 0, (CraftingRecipe)null, (IList<Item>)null);
                        }

                        Game1.spriteBatch.Draw(backpack, new Vector2(curTab.xPositionOnScreen + curTab.width - 100, curTab.yPositionOnScreen + curTab.height - 100), new Rectangle?(new Rectangle(0, 0, backpack.Width, backpack.Height)), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.5f);
                        
                        Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);
                    }
                }
            }
        }

        //Custom voids 
        private void DrawSimpleTooltip(SpriteBatch b, string hoverText, SpriteFont font)
        {
            Vector2 textSize = font.MeasureString(hoverText);
            int width = (int)textSize.X + this.backpack.Width + Game1.tileSize / 2;
            int height = Math.Max(60, (int)textSize.Y + Game1.tileSize / 2);
            int x = Game1.getOldMouseX() + Game1.tileSize / 2;
            int y = Game1.getOldMouseY() + Game1.tileSize / 2;
            if (x + width > Game1.viewport.Width)
            {
                x = Game1.viewport.Width - width;
                y += Game1.tileSize / 4;
            }
            if (y + height > Game1.viewport.Height)
            {
                x += Game1.tileSize / 4;
                y = Game1.viewport.Height - height;
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
            float halfHeartSize = backpack.Width * 0.5f;
            int sourceY =32;
            Vector2 heartPos = new Vector2(x + textSize.X + halfHeartSize, y + halfHeartSize);
            b.Draw(backpack, heartPos, new Rectangle(0, sourceY, 32, 32), Color.White);
        }
    }
}
