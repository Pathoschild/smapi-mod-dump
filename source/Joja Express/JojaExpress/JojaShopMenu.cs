/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/SDV_JojaExpress
**
*************************************************/

using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.GameData.Shops;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using System.Reflection;

namespace JojaExpress
{
    public class JojaShopMenu : ShopMenu
    {
        Func<ISalable, string> getPostfix;
        ClickableTextureComponent searchTexture, previousButton, nextButton;
        Texture2D tmp;
        public TextBox searchBox;

        public JojaShopMenu(string shopId, ShopData shopData, ShopOwnerData ownerData, Func<ISalable, Farmer, int, bool> onPurchase, Func<ISalable, string> getPostfix)
            : base(shopId, shopData, ownerData, onPurchase: onPurchase)
        {
            this.getPostfix = getPostfix;
            tmp = ModEntry._Helper.ModContent.Load<Texture2D>("assets/Search");
            searchTexture = new ClickableTextureComponent(
                new(xPositionOnScreen - 8, yPositionOnScreen + height - inventory.height + 14 * 4, 65 * 4, 48 * 4), 
                tmp, new(0, 0, 65, 48) ,4f);
            searchBox = new(null, null, Game1.smallFont, Color.Red)
            {
                X = xPositionOnScreen + 18,
                Y = yPositionOnScreen + height - inventory.height + 32 * 4,
                Width = 48 * 4,
                Height = 64,
                Selected = false
            };
            searchBox.OnEnterPressed += nextMatch;
            Game1.keyboardDispatcher.Subscriber = searchBox;
            previousButton = upArrow.ShallowClone();
            nextButton = downArrow.ShallowClone();
            previousButton.bounds = new(
                xPositionOnScreen + 4 + 44, yPositionOnScreen + height - inventory.height + 44 * 4,
                previousButton.bounds.Width, previousButton.bounds.Height);
            nextButton.bounds = new(
                xPositionOnScreen + 4 + 132, yPositionOnScreen + height - inventory.height + 44 * 4,
                nextButton.bounds.Width, nextButton.bounds.Height);
        }

        public void nextMatch(TextBox sender)
        {
            int i = currentItemIndex;
            while (true)
            {
                i++;
                i %= forSale.Count;
                if (i == currentItemIndex) return;

                if(forSale[i].DisplayName.Contains(searchBox.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    currentItemIndex = i;
                    ModEntry._Helper.Reflection.GetMethod(this, "setScrollBarToCurrentIndex").Invoke();
                    updateSaleButtonNeighbors();
                    return;
                }
            }
        }

        public void previousMatch()
        {
            int i = currentItemIndex;
            while (true)
            {
                i--;
                if (i < 0) i = forSale.Count - 1;
                if (i == currentItemIndex) return;

                if (forSale[i].DisplayName.Contains(searchBox.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    currentItemIndex = i;
                    ModEntry._Helper.Reflection.GetMethod(this, "setScrollBarToCurrentIndex").Invoke();
                    updateSaleButtonNeighbors();
                    return;
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!searchBox.Selected && !Game1.options.doesInputListContain(Game1.options.menuButton, key))
            {
                base.receiveKeyPress(key);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            searchBox.Update();
            if(previousButton.containsPoint(x, y))
            {
                previousMatch();
            }
            else if(nextButton.containsPoint(x, y))
            {
                nextMatch(null);
            }
        }

        public T getValue<T>(string name)
        {
            return (T)typeof(ShopMenu).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
        }

        public T involkeMethod<T>(string name)
        {
            return (T)typeof(ShopMenu).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, null);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            searchTexture = new ClickableTextureComponent(
                new(xPositionOnScreen - 8, yPositionOnScreen + height - inventory.height - 12 + 17 * 4, 65 * 4, 48 * 4),
                tmp, new(0, 0, 65, 48), 4f);
            searchBox.X = xPositionOnScreen + 18;
            searchBox.Y = yPositionOnScreen + height - inventory.height + 32 * 4;
            searchBox.Width = 48 * 4;
            searchBox.Height = 64;
            previousButton.bounds = new(
                xPositionOnScreen + 4 + 44, yPositionOnScreen + height - inventory.height + 44 * 4,
                previousButton.bounds.Width, previousButton.bounds.Height);
            nextButton.bounds = new(
                xPositionOnScreen + 4 + 132, yPositionOnScreen + height - inventory.height + 44 * 4,
                nextButton.bounds.Width, nextButton.bounds.Height);
        }

        public override void draw(SpriteBatch b)
        {
            ShopMenu father = this as ShopMenu;
            IReflectionHelper helper = ModEntry.Instance.Helper.Reflection;
            Rectangle scrollBarRunner = getValue<Rectangle>("scrollBarRunner");
            bool scrolling = getValue<bool>("scrolling");
            TemporaryAnimatedSpriteList animations = getValue<TemporaryAnimatedSpriteList>("animations");
            TemporaryAnimatedSprite poof = getValue<TemporaryAnimatedSprite>("poof");

            if (!Game1.options.showMenuBackground && !Game1.options.showClearBackgrounds)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            }

            ShopCachedTheme visualTheme = VisualTheme;
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen + width - inventory.width - 32 - 24, yPositionOnScreen + height - 256 + 40, inventory.width + 56, height - 448 + 20, Color.White, 4f);
            drawTextureBox(b, visualTheme.WindowBorderTexture, visualTheme.WindowBorderSourceRect, xPositionOnScreen, yPositionOnScreen, width, height - 256 + 32 + 4, Color.White, 4f);
            
            drawCurrency(b);
            searchTexture.draw(b);
            b.DrawString(Game1.dialogueFont, ModEntry._Helper.Translation.Get("search"), new(
                xPositionOnScreen + 4, yPositionOnScreen + height - inventory.height + 18 * 4
                ), Color.Brown);
            string textt = searchBox.Text;
            Vector2 vectort = Game1.smallFont.MeasureString(searchBox.Text);
            while (vectort.X > (float)searchBox.Width)
            {
                textt = textt.Substring(1);
                vectort = Game1.smallFont.MeasureString(textt);
            }

            bool flagt = Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 >= 500.0;
            if (flagt && searchBox.Selected)
            {
                b.Draw(Game1.staminaRect, new Rectangle(searchBox.X + 10 + (int)vectort.X + 2, searchBox.Y + 2, 4, 32), Color.Red);
            }
            b.DrawString(Game1.smallFont, searchBox.Text, new Vector2(searchBox.X + 10, searchBox.Y + 2), Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
            previousButton.draw(b);
            nextButton.draw(b);

            for (int i = 0; i < forSaleButtons.Count; i++)
            {
                if (currentItemIndex + i >= forSale.Count)
                {
                    continue;
                }

                bool flag = canPurchaseCheck != null && !canPurchaseCheck(currentItemIndex + i);
                drawTextureBox(b, visualTheme.ItemRowBackgroundTexture, visualTheme.ItemRowBackgroundSourceRect, forSaleButtons[i].bounds.X, forSaleButtons[i].bounds.Y, forSaleButtons[i].bounds.Width, forSaleButtons[i].bounds.Height, (forSaleButtons[i].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) && !scrolling) ? visualTheme.ItemRowBackgroundHoverColor : Color.White, 4f, drawShadow: false);
                ISalable salable = forSale[currentItemIndex + i];
                ItemStockInformation stockInfo = itemPriceAndStock[salable];
                StackDrawType stackDrawType = GetStackDrawType(stockInfo, salable);
                string text = salable.DisplayName;
                if (salable.Stack > 1)
                {
                    text = text + " x" + salable.Stack;
                }
                text += getPostfix.Invoke(salable);
                b.Draw(visualTheme.ItemIconBackgroundTexture, new Vector2(forSaleButtons[i].bounds.X + 32 - 12, forSaleButtons[i].bounds.Y + 24 - 4), visualTheme.ItemIconBackgroundSourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                Vector2 vector = new Vector2(forSaleButtons[i].bounds.X + 32 - 8, forSaleButtons[i].bounds.Y + 24);
                Color color = Color.White * ((!flag) ? 1f : 0.25f);
                int num = 1;
                if (itemPriceAndStock.TryGetValue(salable, out var value))
                {
                    num = value.Stock;
                }

                salable.drawInMenu(b, vector, 1f, 1f, 0.9f, StackDrawType.HideButShowQuality, color, drawShadow: true);
                if (num != int.MaxValue && ShopId != "ClintUpgrade" && ((stackDrawType == StackDrawType.Draw && num > 1) || stackDrawType == StackDrawType.Draw_OneInclusive))
                {
                    Utility.drawTinyDigits(num, b, vector + new Vector2(64 - Utility.getWidthOfTinyDigitString(num, 3f) + 3, 47f), 3f, 1f, color);
                }

                string text2 = text;
                bool flag2 = itemPriceAndStock[forSale[currentItemIndex + i]].Price > 0;
                if (SpriteText.getWidthOfString(text2) > width - (flag2 ? (150 + SpriteText.getWidthOfString(itemPriceAndStock[forSale[currentItemIndex + i]].Price + " ")) : 100) && text2.Length > (flag2 ? 27 : 37))
                {
                    text2 = text2.Substring(0, flag2 ? 27 : 37);
                    text2 += "...";
                }

                SpriteText.drawString(b, text2, forSaleButtons[i].bounds.X + 96 + 8, forSaleButtons[i].bounds.Y + 28, 999999, -1, 999999, flag ? 0.5f : 1f, 0.88f, junimoText: false, -1, "", visualTheme.ItemRowTextColor);

                int num2 = forSaleButtons[i].bounds.Right;
                int num3 = forSaleButtons[i].bounds.Y + 28 - 4;
                int y = forSaleButtons[i].bounds.Y + 44;
                if (itemPriceAndStock[forSale[currentItemIndex + i]].Price > 0)
                {
                    SpriteText.drawString(b, itemPriceAndStock[forSale[currentItemIndex + i]].Price + " ", num2 - SpriteText.getWidthOfString(itemPriceAndStock[forSale[currentItemIndex + i]].Price + " ") - 60, forSaleButtons[i].bounds.Y + 28, 999999, -1, 999999, (getPlayerCurrencyAmount(Game1.player, currency) >= itemPriceAndStock[forSale[currentItemIndex + i]].Price && !flag) ? 1f : 0.5f, 0.88f, junimoText: false, -1, "", visualTheme.ItemRowTextColor);
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(forSaleButtons[i].bounds.Right - 52, forSaleButtons[i].bounds.Y + 40 - 4), new Rectangle(193 + currency * 9, 373, 9, 10), Color.White * ((!flag) ? 1f : 0.25f), 0f, Vector2.Zero, 4f, flipped: false, 1f, -1, -1, (!flag) ? 0.35f : 0f);
                    num2 -= SpriteText.getWidthOfString(itemPriceAndStock[forSale[currentItemIndex + i]].Price + " ") + 96;
                    num3 = forSaleButtons[i].bounds.Y + 20;
                    y = forSaleButtons[i].bounds.Y + 28;
                }

                if (itemPriceAndStock[forSale[currentItemIndex + i]].TradeItem != null)
                {
                    int count = 5;
                    string tradeItem = itemPriceAndStock[forSale[currentItemIndex + i]].TradeItem;
                    if (tradeItem != null && itemPriceAndStock[forSale[currentItemIndex + i]].TradeItemCount.HasValue)
                    {
                        count = itemPriceAndStock[forSale[currentItemIndex + i]].TradeItemCount.Value;
                    }

                    bool flag3 = HasTradeItem(tradeItem, count);
                    if (canPurchaseCheck != null && !canPurchaseCheck(currentItemIndex + i))
                    {
                        flag3 = false;
                    }

                    float num4 = SpriteText.getWidthOfString("x" + count);
                    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(tradeItem);
                    Texture2D texture = dataOrErrorItem.GetTexture();
                    Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
                    Utility.drawWithShadow(b, texture, new Vector2((float)(num2 - 88) - num4, num3), sourceRect, Color.White * (flag3 ? 1f : 0.25f), 0f, Vector2.Zero, -1f, flipped: false, -1f, -1, -1, flag3 ? 0.35f : 0f);
                    SpriteText.drawString(b, "x" + count, num2 - (int)num4 - 16, y, 999999, -1, 999999, flag3 ? 1f : 0.5f, 0.88f, junimoText: false, -1, "", visualTheme.ItemRowTextColor);
                }
            }

            if (IsOutOfStock())
            {
                SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11583"), xPositionOnScreen + width / 2 - SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11583")) / 2, yPositionOnScreen + height / 2 - 128);
            }

            inventory.draw(b);
            for (int num5 = animations.Count - 1; num5 >= 0; num5--)
            {
                if (animations[num5].update(Game1.currentGameTime))
                {
                    animations.RemoveAt(num5);
                }
                else
                {
                    animations[num5].draw(b, localPosition: true);
                }
            }

            poof?.draw(b);
            upArrow.draw(b);
            downArrow.draw(b);
            for (int j = 0; j < tabButtons.Count; j++)
            {
                tabButtons[j].draw(b);
            }

            if (forSale.Count > 4)
            {
                drawTextureBox(b, visualTheme.ScrollBarBackTexture, visualTheme.ScrollBarBackSourceRect, scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f);
                scrollBar.draw(b);
            }

            if (!hoverText.Equals(""))
            {
                if (hoveredItem?.IsRecipe ?? false)
                {
                    drawToolTip(
                        b, " ", boldTitleText, hoveredItem as Item, heldItem != null, -1, currency,
                        involkeMethod<string>("getHoveredItemExtraItemIndex"),
                        involkeMethod<int>("getHoveredItemExtraItemAmount"),
                        new CraftingRecipe(hoveredItem.Name.Replace(" Recipe", "")),
                        (hoverPrice > 0) ? hoverPrice : (-1)
                    );
                }
                else
                {
                    drawToolTip(
                        b, hoverText, boldTitleText, hoveredItem as Item,
                        heldItem != null, -1, currency,
                        involkeMethod<string>("getHoveredItemExtraItemIndex"),
                        involkeMethod<int>("getHoveredItemExtraItemAmount"),
                        null, (hoverPrice > 0) ? hoverPrice : (-1)
                     );
                }
            }

            upperRightCloseButton.draw(b);
            drawMouse(b);
        }
    }
}
