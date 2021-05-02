/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/JojaOnline
**
*************************************************/

using JojaOnline.JojaOnline.Items;
using JojaOnline.JojaOnline.Mailing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JojaOnline.JojaOnline.UI
{
    public class JojaSite : IClickableMenu
    {
        public readonly float scale = 1f;
        public readonly int preferredWidth = 1000;
        private readonly int nextDayShippingFee = 10;
        private readonly int maxUniqueCartItems = 10;
        private readonly int buttonScrollingOffset = 8;
        private readonly Texture2D sourceSheet = JojaResources.GetJojaSiteSpriteSheet();
        private readonly Texture2D bannerAdSheet = JojaResources.GetJojaAdBanners();
        private readonly IMonitor monitor = JojaResources.GetMonitor();

        private Rectangle scrollBarRunner;
        private ClickableComponent nextDayShippingButton;
        private ClickableComponent twoDayShippingButton;
        private ClickableComponent purchaseButton;
        private List<ClickableComponent> forSaleButtons = new List<ClickableComponent>();

        private ClickableTextureComponent scrollBar;
        private ClickableTextureComponent cancelButton;
        private ClickableTextureComponent checkoutButton;
        private ClickableTextureComponent cartQuantity;
        private List<ClickableTextureComponent> clickables = new List<ClickableTextureComponent>();

        private bool scrolling = false;
        private bool isCheckingOut = false;
        private bool isNextDayShipping = hasPrimeShipping ? true : false; // Make next day shipping default selected if Joja Prime is purchased (as it will be free)
        private bool canAffordOrder = false;
        private int currentItemIndex = 0;

        // Tick related
        private float numberOfSecondsToDelayInput = 0.5f;
        private int lastTick = Game1.ticks;

        // Description related
        private ISalable hoveredItem;
        private int hoverPrice = -1;
        private string hoverText = "";
        private string boldTitleText = "";

        // Membership related
        private static bool isJojaMember = false;
        private static bool hasPrimeShipping = false;

        // Random sale item related
        private static int randomSaleItemId = -1;
        private static float randomSalePercentageOff = 0f;
        private ISalable randomSaleItem;
        private ClickableTextureComponent randomSaleButton;

        // Items for sale in shop
        private List<ISalable> forSale = new List<ISalable>();

        // Item: [price, quantity]
        private Dictionary<ISalable, int[]> itemsInCart = new Dictionary<ISalable, int[]>();
        private Dictionary<ISalable, int[]> itemPriceAndStock = new Dictionary<ISalable, int[]>();

        public JojaSite(int uiWidth, int uiHeight, float uiScale) : base(Game1.viewport.Width / 2 - (uiWidth) / 2, Game1.viewport.Height / 2 - (uiHeight) / 2, uiWidth, uiHeight, showUpperRightCloseButton: true)
        {
            // Get the items to be sold
            Dictionary<ISalable, int[]> jojaOnlineStock = GetItemsToSell();
            foreach (ISalable j in jojaOnlineStock.Keys)
            {
                if (j is StardewValley.Object && (bool)(j as StardewValley.Object).isRecipe)
                {
                    continue;
                }

                this.forSale.Add(j);
                this.itemPriceAndStock.Add(j, new int[2]
                {
                    // Increase sale price by 25% without membership
                    isJojaMember ? jojaOnlineStock[j][0] : jojaOnlineStock[j][0] + (jojaOnlineStock[j][0] / 4),
                    j.Stack
                });
            }

            scale = uiScale;
            //monitor.Log($"[{scale}] {uiWidth}x{uiHeight} vs {width}x{height} at ({xPositionOnScreen}, {yPositionOnScreen})", LogLevel.Debug);

            // Scroll bar
            scrollBar = new ClickableTextureComponent(new Rectangle(GetScaledXCoordinate(this.width / 2 - (int)(13 * scale), false), GetScaledYCoordinate(592), (int)(25 * scale), 40), sourceSheet, new Rectangle(0, 64, 6, 10), scale * 4f);
            scrollBarRunner = new Rectangle(GetScaledXCoordinate(this.width / 2 - (int)(13 * scale), false), GetScaledYCoordinate(592), (int)(25 * scale), this.height / 3);

            // Draw the clickables (buttons, etc)
            for (int i = 0; i < 4; i++)
            {//GetScaledSourceBounds((int)(35 + 16 * scale), 575 + 16 + i * ((scrollBarRunner.Height / 4)), (int)(this.width - 128 * scale) / 2, (scrollBarRunner.Height / 4) + 4, useScale: false)
                this.forSaleButtons.Add(new ClickableComponent(new Rectangle(scrollBarRunner.X - (int)(this.width / 2 - (scrollBarRunner.Width * 2)) + IClickableMenu.borderWidth / 4, GetScaledYCoordinate(592) + i * ((scrollBarRunner.Height / 4)), (int)(this.width / 2 - (scrollBarRunner.Width * 2)) - IClickableMenu.borderWidth / 4, (scrollBarRunner.Height / 4) + 4), string.Concat(i))
                {
                    myID = i + 3546,
                    fullyImmutable = true
                });
                //(GetScaledSourceBounds((int)(60 + 16 * scale + ((int)(this.width - 128 * scale) / 2)), 575 + 16 + i * ((scrollBarRunner.Height / 4)), (int)(this.width - 128 * scale) / 2, (scrollBarRunner.Height / 4) + 4, useScale: false), string.Concat(i))
                this.forSaleButtons.Add(new ClickableComponent(new Rectangle(scrollBarRunner.X + scrollBarRunner.Width, GetScaledYCoordinate(592) + i * ((scrollBarRunner.Height / 4)), (int)(this.width / 2 - (scrollBarRunner.Width * 2)) - IClickableMenu.borderWidth / 4, (scrollBarRunner.Height / 4) + 4), string.Concat(i))
                {
                    myID = (7 - i) + 3546,
                    fullyImmutable = true
                });
            }

            // Joja Ads
            //drawClickable("jojaLeftAd", 42, 248, sourceSheet, new Rectangle(608, 0, 208, 208));
            //drawClickable("jojaRightAd", 829, 260, sourceSheet, new Rectangle(0, 352, 201, 194));
            this.clickables.Add(new ClickableTextureComponent(new Rectangle(GetScaledXCoordinate((this.width / 2) - (int)(540 * scale) / 2, false), GetScaledYCoordinate(200), 540, 180), bannerAdSheet, new Rectangle(0, (180 * Game1.random.Next(0, 3)), 540, 180), scale));

            // Top header BG
            int headerY = IClickableMenu.borderWidth + 56;
            int headerHeight = (int)(70 * scale);

            // Logo
            this.clickables.Add(new ClickableTextureComponent(new Rectangle(GetScaledXCoordinate((int)(IClickableMenu.borderWidth + 5 * scale), false), GetScaledYCoordinate((int)(headerY + 5 * scale), false), 172, 52), sourceSheet, new Rectangle(0, 0, hasPrimeShipping ? 46 : 43, 13), scale * 4f));

            // Motto
            this.clickables.Add(new ClickableTextureComponent(new Rectangle(GetScaledXCoordinate((int)(this.width / 2 - (76 * 3f * scale) / 4), false), GetScaledYCoordinate((int)(headerY + headerHeight / 4), false), 228, 33), sourceSheet, new Rectangle(0, 16, 76, 11), scale * 3f));

            // Checkout button
            // Must have bound's x / y match the scale value to get button to appear correctly
            checkoutButton = new ClickableTextureComponent(new Rectangle(GetScaledXCoordinate(this.width - IClickableMenu.borderWidth - (int)(14 + 14 * scale * 4f), false), GetScaledYCoordinate(headerY + headerHeight / 4, false), (int)(14 * scale * 4f), (int)(8 * scale * 4f)), sourceSheet, new Rectangle(0, 32, 14, 8), scale * 4f);

            // Cancel button
            //cancelButton = new ClickableTextureComponent(GetScaledSourceBounds(45, 110, 42, 42), sourceSheet, new Rectangle(16, 48, 14, 14), scale * 3f);
            cancelButton = new ClickableTextureComponent(new Rectangle(GetScaledXCoordinate((int)(IClickableMenu.borderWidth + 5 * scale), false), GetScaledYCoordinate((int)(headerY + 5 * scale), false), (int)(scale * 42), (int)(scale * 42)), sourceSheet, new Rectangle(16, 48, 14, 14), scale * 3f);

            // Purchase button
            float textScale = scale == 1f ? 1f : 0.5f;
            purchaseButton = new ClickableComponent(GetScaledSourceBounds(this.width - (int)(Game1.dialogueFont.MeasureString($"Purchase Order") * textScale).X - (int)(75 * scale), this.height - (int)(115 * scale), (int)(Game1.dialogueFont.MeasureString($"Purchase Order") * textScale).X + (int)(24 * scale), (int)(Game1.dialogueFont.MeasureString($"Purchase Order") * textScale).Y + (int)(24 * scale), useScale: false), "");

            // TODO: Fix the buttons below
            // Shipping option buttons
            nextDayShippingButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + (int)(79 * scale), purchaseButton.bounds.Y - purchaseButton.bounds.Height - (int)(25 * scale), (int)(Game1.dialogueFont.MeasureString($"Next Day (+{nextDayShippingFee}%)") * textScale).X + (int)(24 * scale), (int)(Game1.dialogueFont.MeasureString($"Next Day (+{nextDayShippingFee}%)") * textScale).Y + (int)(24 * scale)), "");
            twoDayShippingButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + (int)(79 * scale), nextDayShippingButton.bounds.Y - nextDayShippingButton.bounds.Height - (int)(10 * scale), (int)(Game1.dialogueFont.MeasureString($"Two Day (FREE)") * textScale).X + (int)(24 * scale), (int)(Game1.dialogueFont.MeasureString($"Two Day (FREE)") * textScale).Y + (int)(24 * scale)), "");

            // Pick an item for sale
            randomSaleItem = forSale[randomSaleItemId];
            randomSaleButton = new ClickableTextureComponent(new Rectangle(GetScaledXCoordinate((this.width / 2) - 200 / 2, false), GetScaledYCoordinate(420), 200, 100), sourceSheet, new Rectangle(0, 96, 50, 25), 4f);

            // Override default close button position
            this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 50, this.yPositionOnScreen + 70, (int)(scale * 48), (int)(scale * 48)), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), scale * 4f);
        }

        public static bool GetPrimeShippingStatus()
        {
            return hasPrimeShipping;
        }

        public static bool GetMembershipStatus()
        {
            return isJojaMember;
        }

        public static void SetPrimeShippingStatus(bool givePrimeShippingOverride)
        {
            hasPrimeShipping = false;

            // If player has Joja Prime shipping or has overriden the config file, disable 10% shipping cost
            if (givePrimeShippingOverride || JojaResources.HasPrimeMembership())
            {
                hasPrimeShipping = true;
            }
        }

        public static void SetMembershipStatus(bool giveDiscountOverride)
        {
            isJojaMember = false;

            // If player is a Joja Member or has overriden the config file, disable the 25% price increase
            if (giveDiscountOverride || Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
            {
                isJojaMember = true;
            }
        }

        public static void PickRandomItemForDiscount(int minSalePercentage, int maxSalePercentage)
        {
            // Set the random percentage
            randomSalePercentageOff = Game1.random.Next(minSalePercentage, maxSalePercentage) / 100f;

            // Set the item id to be sold at discount
            randomSaleItemId = Game1.random.Next(GetItemsToSell().Count);
        }

        public static Dictionary<ISalable, int[]> GetItemsToSell()
        {
            return JojaResources.GetJojaOnlineStock();
        }

        public int GetScaledXCoordinate(int x, bool useScale = true)
        {
            if (!useScale)
            {
                return this.xPositionOnScreen + x;
            }
            return (int)((float)this.xPositionOnScreen + (float)x * this.scale);
        }

        public int GetScaledYCoordinate(int y, bool useScale = true)
        {
            if (!useScale)
            {
                return this.yPositionOnScreen + y;
            }
            return (int)((float)this.yPositionOnScreen + (float)y * this.scale);
        }

        public Vector2 GetScaledVector(int x, int y, bool offsetWithParentPosition = true)
        {
            if (offsetWithParentPosition)
            {
                return new Vector2((int)((float)this.xPositionOnScreen + (float)x * this.scale), (int)((float)this.yPositionOnScreen + (float)y * this.scale));
            }

            return new Vector2(x, y);
        }

        public Rectangle GetScaledSourceBounds(int x, int y, int width, int height, bool offsetWithParentPosition = true, bool useScale = true)
        {
            if (!useScale && offsetWithParentPosition)
            {
                return new Rectangle((int)((float)this.xPositionOnScreen + (float)x), (int)((float)this.yPositionOnScreen + (float)y), (int)((float)width), (int)((float)height));
            }
            else if (offsetWithParentPosition)
            {
                return new Rectangle((int)((float)this.xPositionOnScreen + (float)x * this.scale), (int)((float)this.yPositionOnScreen + (float)y * this.scale), (int)((float)width * this.scale), (int)((float)height * this.scale));
            }

            return new Rectangle(x, y, width, height);
        }

        public override void draw(SpriteBatch b)
        {
            // Fade the area
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // Draw the main box, along with the money box
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, speaker: false, drawOnlyBox: true, r: 80, g: 123, b: 186);
            Game1.dayTimeMoneyBox.drawMoneyBox(b);

            // See if we're checking out
            if (isCheckingOut)
            {
                b.Draw(JojaResources.GetJojaCheckoutBackground(), new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth - 8, this.yPositionOnScreen + IClickableMenu.borderWidth + 56, this.width - (IClickableMenu.borderWidth * 2) + 16, this.height - 128), new Rectangle(0, 0, 150, 200), Color.White);

                // Draw cancel button
                cancelButton.draw(b);

                // Draw item icon, name and quantity
                int subTotal = 0;
                int stackCount = 0;
                int uniqueItemCount = 1;

                int rowStart = scale == 1f ? 120 : 140;
                int rowSpacing = scale == 1f ? 45 : 40;
                float textScale = scale == 1f ? 1f : 0.5f;
                float itemScaling = scale == 1f ? 0.5f : 0.4f;
                foreach (ISalable item in itemsInCart.Keys)
                {
                    // Draw item
                    item.Stack = itemsInCart[item][1];
                    item.drawInMenu(b, GetScaledVector(80, rowStart + (rowSpacing * uniqueItemCount)), itemScaling, 1f, 0.9f, StackDrawType.Hide, Color.White, drawShadow: false);

                    // Draw name / quantity and price
                    Utility.drawTextWithShadow(b, $"{item.DisplayName} x{item.Stack}", Game1.dialogueFont, new Vector2(this.xPositionOnScreen + this.width / 4, GetScaledYCoordinate(rowStart + 10 + (rowSpacing * uniqueItemCount))), Color.White, textScale, 0.5f, numShadows: 0, shadowIntensity: 0);

                    int stackCost = item.Stack * itemsInCart[item][0];
                    Utility.drawTextWithShadow(b, stackCost + "g", Game1.dialogueFont, new Vector2((int)(this.xPositionOnScreen + this.width - this.width / 6 - IClickableMenu.borderWidth), GetScaledYCoordinate(rowStart + 10 + (rowSpacing * uniqueItemCount))), item == randomSaleItem ? Color.Cyan : Color.White, textScale, 0.5f, numShadows: 0, shadowIntensity: 0);

                    stackCount += item.Stack;
                    subTotal += stackCost;
                    uniqueItemCount++;
                }

                // Check if purchased item(s) contain Joja Prime, if so offer free shipping
                bool isPurchasingPrimeShipping = itemsInCart.Any(i => (i.Key as Item).ParentSheetIndex == JojaItems.GetJojaPrimeMembershipID()) ? true : false;

                // Draw the shipping options (free 2 day or next day with fee)
                Utility.drawTextWithShadow(b, "Options", Game1.dialogueFont, new Vector2(this.xPositionOnScreen + 85 * scale, (this.yPositionOnScreen + this.height - this.height / 3) - 20 * scale), Color.White, scale, 0.6f, numShadows: 0, shadowIntensity: 0);
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.twoDayShippingButton.bounds.X, this.twoDayShippingButton.bounds.Y, this.twoDayShippingButton.bounds.Width, this.twoDayShippingButton.bounds.Height, isNextDayShipping ? Color.White : Color.Gray, scale * 4f * twoDayShippingButton.scale, false);
                Utility.drawTextWithShadow(b, "Two Day (Free)", Game1.dialogueFont, new Vector2(this.twoDayShippingButton.bounds.X + 12 * scale, this.twoDayShippingButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12) * scale), Game1.textColor * (isNextDayShipping ? 1f : 0.25f), textScale, -1f, 0, 0, 0, 0);

                string nextDayShippingText = hasPrimeShipping || isPurchasingPrimeShipping ? $"Next Day (Free)" : $"Next Day (+{nextDayShippingFee}%)";
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.nextDayShippingButton.bounds.X, this.nextDayShippingButton.bounds.Y, this.nextDayShippingButton.bounds.Width, this.nextDayShippingButton.bounds.Height, isNextDayShipping ? Color.Gray : Color.White, scale * 4f * nextDayShippingButton.scale, false);
                Utility.drawTextWithShadow(b, nextDayShippingText, Game1.dialogueFont, new Vector2(this.nextDayShippingButton.bounds.X + 12 * scale, this.nextDayShippingButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12) * scale), Game1.textColor * (isNextDayShipping ? 0.25f : 1f), textScale, -1f, 0, 0, 0, 0);

                // Draw the subtotal
                //Utility.drawTextWithShadow(b, $"Subtotal:", Game1.dialogueFont, GetScaledVector(this.width - SpriteText.getWidthOfString($"Subtotal:{subTotal}g") - 50, this.height - 320), Color.LightGray, scale, 0.6f, numShadows: 0, shadowIntensity: 0);
                //Utility.drawTextWithShadow(b, $"{subTotal}g", Game1.dialogueFont, GetScaledVector(this.width - SpriteText.getWidthOfString($"{subTotal}g") - 75, this.height - 320), Color.LightGray, scale, 0.6f, numShadows: 0, shadowIntensity: 0);

                // Draw the shipping costs
                int shippingCosts = isNextDayShipping && !hasPrimeShipping && !isPurchasingPrimeShipping ? subTotal / nextDayShippingFee : 0;
                string shippingCostText = shippingCosts == 0 ? "FREE" : $"{shippingCosts}g";
                Utility.drawTextWithShadow(b, $"Shipping:", Game1.dialogueFont, new Vector2(this.xPositionOnScreen + this.width / 2 + 20 * scale, (this.yPositionOnScreen + this.height - this.height / 3) - 20 * scale), Color.White, scale, 0.6f, numShadows: 0, shadowIntensity: 0);
                Utility.drawTextWithShadow(b, shippingCostText, Game1.dialogueFont, new Vector2(this.xPositionOnScreen + this.width - SpriteText.getWidthOfString(shippingCostText) * scale - (shippingCostText == "FREE" ? 75 * scale : 25), (this.yPositionOnScreen + this.height - this.height / 3) - 20 * scale), Color.White, scale, 0.6f, numShadows: 0, shadowIntensity: 0);

                // Draw the total
                int total = subTotal + shippingCosts;
                Utility.drawTextWithShadow(b, $"Total:", Game1.dialogueFont, new Vector2(this.xPositionOnScreen + 85 * scale, this.purchaseButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12) * scale), Color.Gold, scale, 0.6f, numShadows: 0, shadowIntensity: 0);
                Utility.drawTextWithShadow(b, $"{total}g", Game1.dialogueFont, new Vector2(this.xPositionOnScreen + (85 * scale + SpriteText.getWidthOfString($"Total:") * scale), this.purchaseButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12) * scale), Color.Gold, scale, 0.6f, numShadows: 0, shadowIntensity: 0);

                // Draw the purchase button
                canAffordOrder = total <= Game1.player.Money ? true : false;
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.purchaseButton.bounds.X, this.purchaseButton.bounds.Y, this.purchaseButton.bounds.Width, this.purchaseButton.bounds.Height, canAffordOrder ? Color.White : Color.Gray, scale * 4f * purchaseButton.scale, false);
                Utility.drawTextWithShadow(b, $"Purchase Order", Game1.dialogueFont, new Vector2(this.purchaseButton.bounds.X + 12 * scale, this.purchaseButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12) * scale), Game1.textColor * (canAffordOrder ? 1f : 0.25f), textScale, -1f, 0, 0, 0, 0);

                // Draw the mouse
                this.drawMouse(b);

                return;
            }

            // Draw the custom store BG
            // this.width - (IClickableMenu.borderWidth * 2) - 12 to account for width shift, this.height - 128 due to drawDialogueBox method adding 128 to height
            b.Draw(JojaResources.GetJojaSiteBackground(), new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth - 8, this.yPositionOnScreen + IClickableMenu.borderWidth + 56, this.width - (IClickableMenu.borderWidth * 2) + 16, this.height - 128), new Rectangle(0, 0, 150, 200), Color.White);
            IClickableMenu.drawTextureBox(b, sourceSheet, new Rectangle(13, 64, 3, 3), this.xPositionOnScreen + IClickableMenu.borderWidth - 8, this.yPositionOnScreen + IClickableMenu.borderWidth + 56, this.width - (IClickableMenu.borderWidth * 2) + 16, (int)(70 * scale), Color.White, 1f, drawShadow: false);

            // Draw the current unique amount of items in cart
            cartQuantity = drawCartQuantity(sourceSheet, new Rectangle(0, 41, 5, 7));
            cartQuantity.draw(b);

            // Draw the usage instructions
            //IClickableMenu.drawTextureBox(b, sourceSheet, new Rectangle(144, 576, 76, 64), this.xPositionOnScreen + 100, this.yPositionOnScreen + 600, 76, 64, Color.White, scale, drawShadow: false);
            //SpriteText.drawString(b, "to order", this.xPositionOnScreen + 100, this.yPositionOnScreen + 600, 999999, -1, 999999, 1f, 0.88f, junimoText: false, -1, "", 4);
            //SpriteText.drawString(b, "to remove", (this.xPositionOnScreen + this.width) - SpriteText.getWidthOfString("Right Click to remove"), this.yPositionOnScreen + 600, 999999, -1, 999999, 1f, 0.88f, junimoText: false, -1, "", 4);

            // Draw the general clickables
            foreach (ClickableTextureComponent clickable in clickables)
            {
                clickable.draw(b);
            }

            // Draw the checkout button
            checkoutButton.draw(b, itemsInCart.Count > 0 ? Color.White : Color.Black, 0.99f);

            // Draw the scroll bar
            IClickableMenu.drawTextureBox(b, sourceSheet, new Rectangle(0, 74, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f, drawShadow: false);
            scrollBar.draw(b);

            // Draw the sale button
            randomSaleButton.draw(b);

            if (randomSaleItem is Furniture)
            {
                (randomSaleItem as Furniture).drawInMenu(b, new Vector2(randomSaleButton.bounds.Center.X - (25 * scale) + (scale == 1f ? -7 : -13), randomSaleButton.bounds.Center.Y - (25 * scale) + (scale == 1f ? -8 : -19)), 0.75f, 1f, 0.9f, StackDrawType.Hide, Color.White, drawShadow: false);
            }
            else if (randomSaleItem is Wallpaper)
            {
                if ((randomSaleItem as Wallpaper).isFloor)
                {
                    (randomSaleItem as Wallpaper).drawInMenu(b, new Vector2(randomSaleButton.bounds.Center.X - (25 * scale) + (scale == 1f ? -7 : -11), randomSaleButton.bounds.Center.Y - (25 * scale) + (scale == 1f ? -8 : -2)), 0.6f, 1f, 0.9f, StackDrawType.Hide, Color.White, drawShadow: false);
                }
                else
                {
                    (randomSaleItem as Wallpaper).drawInMenu(b, new Vector2(randomSaleButton.bounds.Center.X - (25 * scale) + (scale == 1f ? -7 : -14), randomSaleButton.bounds.Center.Y - (25 * scale) + (scale == 1f ? -8 : -7)), 0.6f, 1f, 0.9f, StackDrawType.Hide, Color.White, drawShadow: false);
                }
            }
            else
            {
                randomSaleItem.drawInMenu(b, new Vector2(randomSaleButton.bounds.Center.X - (25 * scale) + (scale == 1f ? -2 : -8), randomSaleButton.bounds.Center.Y - (25 * scale) + (scale == 1f ? -2 : -9)), 0.6f, 1f, 0.9f, StackDrawType.Hide, Color.White, drawShadow: false);
            }
            //randomSaleItem.drawInMenu(b, new Vector2(randomSaleButton.bounds.Center.X - 25 * scale, randomSaleButton.bounds.Center.Y - 25 * scale), scale * 0.5f, 1f, 0.9f, StackDrawType.Hide, Color.White, drawShadow: false);

            // Draw the individual store buttons
            foreach (ClickableComponent button in forSaleButtons)
            {
                int buttonPosition = forSaleButtons.IndexOf(button) + (currentItemIndex * 2);
                if (buttonPosition >= forSale.Count)
                {
                    continue;
                }

                // Draw the button grid
                IClickableMenu.drawTextureBox(b, sourceSheet, new Rectangle(0, 48, 15, 15), button.bounds.X, button.bounds.Y, button.bounds.Width, button.bounds.Height, Color.White, scale * 4f, drawShadow: false);

                // Get the quantity that is in the cart (if any)
                int currentlyInCart = 0;
                if (itemsInCart.ContainsKey(forSale[buttonPosition]))
                {
                    currentlyInCart = itemsInCart[forSale[buttonPosition]][1];
                }

                // Draw the item for sale
                float itemScale = scale == 1f ? 1f : 0.6f;
                Item itemForSale = forSale[buttonPosition] as Item;
                if (itemForSale is Furniture)
                {
                    (itemForSale as Furniture).drawInMenu(b, new Vector2(button.bounds.X + (button.bounds.Width / 12 * itemScale) + (itemScale == 1f ? 0 : -5), button.bounds.Y + (button.bounds.Height / 8 * itemScale) + (itemScale == 1f ? 2 : -5)), itemScale, 1f, 0.9f, StackDrawType.Hide, Color.White * (itemsInCart.Count < maxUniqueCartItems || currentlyInCart > 0 ? 1f : 0.25f), drawShadow: false);
                }
                else if (itemForSale is Wallpaper)
                {
                    (itemForSale as Wallpaper).drawInMenu(b, new Vector2(button.bounds.X + (button.bounds.Width / 12 * itemScale) + (itemScale == 1f ? 0 : -5), button.bounds.Y + (button.bounds.Height / 8 * itemScale) + (itemScale == 1f ? 2 : -5)), itemScale, 1f, 0.9f, StackDrawType.Hide, Color.White * (itemsInCart.Count < maxUniqueCartItems || currentlyInCart > 0 ? 1f : 0.25f), drawShadow: false);
                }
                else
                {
                    itemForSale.drawInMenu(b, new Vector2(button.bounds.X + (button.bounds.Width / 12 * itemScale), button.bounds.Y + (button.bounds.Height / 8 * itemScale) + (itemScale == 1f ? 2 : -2)), itemScale, 1f, 0.9f, StackDrawType.Hide, Color.White * (itemsInCart.Count < maxUniqueCartItems || currentlyInCart > 0 ? 1f : 0.25f), drawShadow: false);
                }

                // Draw the quantity in the cart
                Utility.drawTextWithShadow(b, $"In Cart: {currentlyInCart}", Game1.dialogueFont, new Vector2(button.bounds.X + (button.bounds.Width / 4) + 15 * scale, button.bounds.Y + 10), Color.White * (itemsInCart.Count < maxUniqueCartItems || currentlyInCart > 0 ? 1f : 0.5f), scale == 1f ? 0.9f : 0.5f, 0.88f, numShadows: 0, shadowIntensity: 0);

                // Check if item is on sale, if so then add visual marker
                float coinScale = scale == 1f ? 3.75f : 2f;
                float textScale = scale == 1f ? 0.9f : 0.5f;
                if (itemForSale == randomSaleItem)
                {
                    // Draw the (discounted) price
                    string price = ((int)(itemPriceAndStock[itemForSale][0] - (itemPriceAndStock[itemForSale][0] * randomSalePercentageOff))).ToString();
                    Utility.drawTextWithShadow(b, "-" + (randomSalePercentageOff * 100) + "%", Game1.dialogueFont, new Vector2(button.bounds.X + (button.bounds.Width / 4) + 15 * scale, button.bounds.Y + button.bounds.Height - (SpriteText.getHeightOfString("-" + (randomSalePercentageOff * 100) + "%") * textScale) - (6 * scale)), Color.Cyan * (itemsInCart.Count < maxUniqueCartItems || currentlyInCart > 0 ? 1f : 0.5f), textScale, 0.6f, numShadows: 0, shadowIntensity: 0);
                    Utility.drawTextWithShadow(b, price, Game1.dialogueFont, new Vector2(button.bounds.X + button.bounds.Width - (SpriteText.getWidthOfString(price) * textScale) - (9 * coinScale) - (6 * scale), button.bounds.Y + button.bounds.Height - (SpriteText.getHeightOfString(price) * textScale) - (6 * scale)), Color.Cyan * (itemsInCart.Count < maxUniqueCartItems || currentlyInCart > 0 ? 1f : 0.5f), textScale, 0.6f, numShadows: 0, shadowIntensity: 0);
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(button.bounds.X + button.bounds.Width - (9 * coinScale) - (10 * scale), button.bounds.Y + button.bounds.Height - (10 * coinScale) - (7 * scale)), new Rectangle(193, 373, 9, 10), Color.White * (itemsInCart.Count < maxUniqueCartItems || currentlyInCart > 0 ? 1f : 0.25f), 0f, Vector2.Zero, coinScale, flipped: false, 1f, -1, -1, 0f);

                    // Draw sale marker
                    Utility.drawWithShadow(b, sourceSheet, new Vector2(button.bounds.Right - (14 * scale * 3f) - (12 * scale), button.bounds.Y + (12 * scale)), new Rectangle(0, 80, 14, 7), Color.White, 0f, Vector2.Zero, scale * 3f, flipped: false, 1f, -1, -1, 0f);
                }
                else
                {
                    // Draw the price
                    string price = itemPriceAndStock[itemForSale][0].ToString();
                    Utility.drawTextWithShadow(b, price, Game1.dialogueFont, new Vector2(button.bounds.X + button.bounds.Width - (SpriteText.getWidthOfString(price) * textScale) - (9 * coinScale) - (6 * scale), button.bounds.Y + button.bounds.Height - (SpriteText.getHeightOfString(price) * textScale) - (6 * scale)), new Color(251, 249, 78) * (itemsInCart.Count < maxUniqueCartItems || currentlyInCart > 0 ? 1f : 0.5f), textScale, 0.6f, numShadows: 0, shadowIntensity: 0);
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(button.bounds.X + button.bounds.Width - (9 * coinScale) - (10 * scale), button.bounds.Y + button.bounds.Height - (10 * coinScale) - (7 * scale)), new Rectangle(193, 373, 9, 10), Color.White * (itemsInCart.Count < maxUniqueCartItems || currentlyInCart > 0 ? 1f : 0.25f), 0f, Vector2.Zero, coinScale, flipped: false, 1f, -1, -1, 0f);
                }
            }

            // Draw the tooltip
            if (!this.hoverText.Equals(""))
            {
                if (this.hoveredItem is StardewValley.Object && (bool)(this.hoveredItem as StardewValley.Object).isRecipe)
                {
                    IClickableMenu.drawToolTip(b, " ", this.boldTitleText, this.hoveredItem as Item, false, -1, 0, -1, -1, new CraftingRecipe(this.hoveredItem.Name.Replace(" Recipe", "")), (this.hoverPrice > 0) ? this.hoverPrice : (-1));
                }
                else
                {
                    IClickableMenu.drawToolTip(b, this.hoverText, this.boldTitleText, this.hoveredItem as Item, false, -1, 0, -1, -1, null, (this.hoverPrice > 0) ? this.hoverPrice : (-1));
                }
            }

            this.upperRightCloseButton.draw(b);
            this.drawMouse(b);
        }

        private bool tryToPurchaseItem(ISalable item, int numberToBuy, int x, int y, int indexInForSaleList)
        {
            if (itemPriceAndStock[item][1] < numberToBuy)
            {
                numberToBuy = Math.Max(0, itemPriceAndStock[item][1]);
            }

            if (!itemsInCart.ContainsKey(item) && numberToBuy > 0)
            {
                itemsInCart.Add(item, new int[] { item == randomSaleItem ? ((int)(itemPriceAndStock[item][0] - (itemPriceAndStock[item][0] * randomSalePercentageOff))) : this.itemPriceAndStock[item][0], numberToBuy });
            }
            else if (itemsInCart.ContainsKey(item) && itemPriceAndStock[item][1] >= itemsInCart[item][1] + numberToBuy)
            {
                itemsInCart[item][1] += numberToBuy;
            }

            return true;
        }


        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            lastTick = Game1.ticks;

            if (Game1.activeClickableMenu == null)
            {
                return;
            }

            if (isCheckingOut)
            {
                if (cancelButton.containsPoint(x, y))
                {
                    isCheckingOut = false;
                    Game1.playSound("cancel");
                }
                else if (nextDayShippingButton.containsPoint(x, y))
                {
                    isNextDayShipping = true;
                    Game1.playSound("select");
                }
                else if (twoDayShippingButton.containsPoint(x, y))
                {
                    isNextDayShipping = false;
                    Game1.playSound("select");
                }
                else if (purchaseButton.containsPoint(x, y))
                {
                    if (canAffordOrder)
                    {
                        // Close this menu
                        base.exitThisMenu();

                        // Create mail order
                        if (JojaMail.CreateMailOrder(Game1.player, isNextDayShipping ? 1 : 2, itemsInCart.Keys.Select(i => i as Item).ToList()))
                        {
                            this.monitor.Log("Order placed via JojaMail!", LogLevel.Debug);

                            // Display order success dialog
                            if (isNextDayShipping)
                            {
                                Game1.player.Money = Game1.player.Money - (itemsInCart.Keys.Sum(i => i.Stack * itemsInCart[i][0]) + (itemsInCart.Keys.Sum(i => i.Stack * itemsInCart[i][0]) / nextDayShippingFee));
                                Game1.activeClickableMenu = new DialogueBox("Your order has been placed! ^It will arrive tomorrow.");
                            }
                            else
                            {
                                Game1.player.Money = Game1.player.Money - itemsInCart.Keys.Sum(i => i.Stack * itemsInCart[i][0]);
                                Game1.activeClickableMenu = new DialogueBox("Your order has been placed! ^It will arrive in 2 days.");
                            }

                            Game1.playSound("moneyDial");
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                        }
                        else
                        {
                            this.monitor.Log("Issue ordering items, failed to dispatch JojaMail!", LogLevel.Error);

                            // Display order error dialog
                            Game1.activeClickableMenu = new DialogueBox($"Order failed to place! Please try again later.");
                        }
                    }
                    else
                    {
                        // Shake money bag
                        Game1.dayTimeMoneyBox.moneyShakeTimer = 2000;
                        Game1.playSound("cancel");
                    }
                }

                return;
            }

            if (this.scrollBar.containsPoint(x, y))
            {
                this.scrolling = true;
            }
            else if (randomSaleButton.containsPoint(x, y))
            {
                // Move the forSaleButtons until the randomSaleItem is displayed
                for (this.currentItemIndex = 0; this.currentItemIndex < Math.Max(0, this.forSale.Count - buttonScrollingOffset); currentItemIndex++)
                {
                    bool matchedItem = false;

                    for (int i = 0; i < this.forSaleButtons.Count; i++)
                    {
                        int index = (this.currentItemIndex * 2) + i;

                        if (this.forSale[index] == randomSaleItem)
                        {
                            matchedItem = true;
                            break;
                        }
                    }

                    if (matchedItem)
                    {
                        break;
                    }
                }

                this.setScrollBarToCurrentIndex();
                this.updateSaleButtonNeighbors();
            }
            else if ((checkoutButton.containsPoint(x, y) || cartQuantity.containsPoint(x, y)) && itemsInCart.Count > 0)
            {
                this.monitor.Log("Starting checkout...");
                isCheckingOut = true;
            }
            else
            {
                for (int i = 0; i < this.forSaleButtons.Count; i++)
                {
                    if ((this.currentItemIndex * 2) + i >= this.forSale.Count || !this.forSaleButtons[i].containsPoint(x, y))
                    {
                        continue;
                    }

                    int index = (this.currentItemIndex * 2) + i;
                    if (this.forSale[index] != null)
                    {
                        // Skip if we're at max for the cart size
                        if (itemsInCart.Count >= maxUniqueCartItems && !itemsInCart.ContainsKey(this.forSale[index]))
                        {
                            continue;
                        }

                        // DEBUG: monitor.Log($"{index} | {this.forSale[index].Name}");
                        int toBuy = (!Game1.oldKBState.IsKeyDown(Keys.LeftShift)) ? 1 : 5;
                        toBuy = Math.Min(toBuy, this.forSale[index].maximumStackSize());

                        // Skip if we're trying to buy more then what we can in a stack via mail
                        if (itemsInCart.ContainsKey(this.forSale[index]) && (itemsInCart[this.forSale[index]][1] >= this.forSale[index].maximumStackSize() || itemsInCart[this.forSale[index]][1] + toBuy > this.forSale[index].maximumStackSize()))
                        {
                            continue;
                        }

                        if (this.tryToPurchaseItem(this.forSale[index], toBuy, x, y, index))
                        {
                            DelayedAction.playSoundAfterDelay("coin", 100);
                        }
                        else
                        {
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                            Game1.playSound("cancel");
                        }
                    }

                    this.updateSaleButtonNeighbors();
                    this.setScrollBarToCurrentIndex();
                    return;
                }
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            if (this.scrolling)
            {
                int y2 = this.scrollBar.bounds.Y;
                this.scrollBar.bounds.Y = Math.Min(scrollBarRunner.Bottom - 35, Math.Max(y, scrollBarRunner.Top));
                float percentage = (float)(y - this.scrollBarRunner.Y) / (float)this.scrollBarRunner.Height;
                int correctedForSaleCount = (this.forSale.Count % 2 == 0 ? this.forSale.Count : this.forSale.Count + 1);
                this.currentItemIndex = Math.Min((correctedForSaleCount - buttonScrollingOffset) / 2, Math.Max(0, (int)(((float)correctedForSaleCount / 2) * percentage)));
                this.updateSaleButtonNeighbors();
            }
            else if (isCheckingOut)
            {
                return;
            }
            else if (Game1.ticks >= lastTick + (60 * numberOfSecondsToDelayInput))
            {
                for (int i = 0; i < this.forSaleButtons.Count; i++)
                {
                    if (this.currentItemIndex + i >= this.forSale.Count || !this.forSaleButtons[i].containsPoint(x, y))
                    {
                        continue;
                    }

                    int index = (this.currentItemIndex * 2) + i;
                    if (itemsInCart.Count >= maxUniqueCartItems && !itemsInCart.ContainsKey(this.forSale[index]))
                    {
                        continue;
                    }

                    if (this.forSale[index] != null)
                    {
                        // Skip if we're at max for the cart size
                        if (itemsInCart.Count >= maxUniqueCartItems && !itemsInCart.ContainsKey(this.forSale[index]))
                        {
                            continue;
                        }

                        // DEBUG: monitor.Log($"{index} | {this.forSale[index].Name}");
                        int toBuy = (!Game1.oldKBState.IsKeyDown(Keys.LeftShift)) ? 1 : 5;
                        toBuy = Math.Min(toBuy, this.forSale[index].maximumStackSize());

                        // Skip if we're trying to buy more then what we can in a stack via mail
                        if (itemsInCart.ContainsKey(this.forSale[index]) && (itemsInCart[this.forSale[index]][1] >= this.forSale[index].maximumStackSize() || itemsInCart[this.forSale[index]][1] + toBuy > this.forSale[index].maximumStackSize()))
                        {
                            continue;
                        }

                        if (this.tryToPurchaseItem(this.forSale[index], toBuy, x, y, index))
                        {
                            DelayedAction.playSoundAfterDelay("coin", 100);
                        }
                        else
                        {
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                            Game1.playSound("cancel");
                        }
                    }

                    this.updateSaleButtonNeighbors();
                    this.setScrollBarToCurrentIndex();
                    return;
                }
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            if (this.scrolling)
            {
                this.scrolling = false;
                this.setScrollBarToCurrentIndex();
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (Game1.activeClickableMenu == null)
            {
                return;
            }
            else if (isCheckingOut)
            {
                return;
            }

            for (int i = 0; i < this.forSaleButtons.Count; i++)
            {
                if (this.currentItemIndex + i >= this.forSale.Count || !this.forSaleButtons[i].containsPoint(x, y))
                {
                    continue;
                }

                int index = (this.currentItemIndex * 2) + i;
                if (this.forSale[index] != null && itemsInCart.ContainsKey(this.forSale[index]))
                {
                    int toRemove = (!Game1.oldKBState.IsKeyDown(Keys.LeftShift)) ? 1 : 5;
                    toRemove = Math.Min(toRemove, itemsInCart[this.forSale[index]][1]);

                    itemsInCart[this.forSale[index]][1] -= toRemove;
                    if (itemsInCart[this.forSale[index]][1] <= 0)
                    {
                        itemsInCart.Remove(this.forSale[index]);
                    }

                    Game1.playSound("cancel");
                }

                this.updateSaleButtonNeighbors();
                this.setScrollBarToCurrentIndex();
                return;
            }
        }

        public void drawClickable(string identifier, int x, int y, Texture2D sourceTexture, Rectangle sourceRect, float textureScale = 1f)
        {
            Rectangle bounds = GetScaledSourceBounds(x, y, sourceRect.Width, sourceRect.Height);
            ClickableTextureComponent clickable = new ClickableTextureComponent(bounds, sourceTexture, sourceRect, scale * textureScale)
            {
                name = identifier
            };

            clickables.Add(clickable);
        }

        public ClickableTextureComponent drawCartQuantity(Texture2D sourceTexture, Rectangle sourceRect)
        {
            // Shift sourceRect according to itemsInCart.Count() 
            int currentCount = itemsInCart.Count;
            sourceRect.X = sourceRect.X + (sourceRect.Width * currentCount);

            float x = checkoutButton.bounds.Center.X - 5 * scale;
            float y = checkoutButton.bounds.Center.Y - checkoutButton.bounds.Height / 2 - 10 * scale;

            if (currentCount >= maxUniqueCartItems)
            {
                sourceRect = new Rectangle(50, 38, 11, 10);
                x = checkoutButton.bounds.Center.X - 15 * scale;
                y = checkoutButton.bounds.Center.Y - checkoutButton.bounds.Height / 2 - 20 * scale;
            }

            // Create the ClickableTextureComponent object
            Rectangle bounds = new Rectangle((int)x, (int)y, sourceRect.Width, sourceRect.Height);
            ClickableTextureComponent quantityIcon = new ClickableTextureComponent(bounds, sourceTexture, sourceRect, scale * 4f)
            {
                name = "shoppingCartQuantity"
            };

            return quantityIcon;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && this.currentItemIndex > 0)
            {
                this.currentItemIndex--;
                this.setScrollBarToCurrentIndex();
                this.updateSaleButtonNeighbors();
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && this.currentItemIndex * 2 < Math.Max(0, this.forSale.Count - buttonScrollingOffset))
            {
                this.currentItemIndex++;
                this.setScrollBarToCurrentIndex();
                this.updateSaleButtonNeighbors();
                Game1.playSound("shiny4");
            }
        }

        private void setScrollBarToCurrentIndex()
        {
            if (forSale.Count > 0)
            {
                this.scrollBar.bounds.Y = this.scrollBarRunner.Y + this.scrollBarRunner.Height / Math.Max(1, this.forSale.Count - buttonScrollingOffset + 1) * (this.currentItemIndex * 2);
                if (this.currentItemIndex * 2 == this.forSale.Count - buttonScrollingOffset + (this.forSale.Count % 2 == 0 ? 0 : 1))
                {
                    this.scrollBar.bounds.Y = this.scrollBarRunner.Y + this.scrollBarRunner.Height - 35;
                }
            }
        }

        public void updateSaleButtonNeighbors()
        {
            ClickableComponent last_valid_button = this.forSaleButtons[0];
            for (int i = 0; i < this.forSaleButtons.Count; i++)
            {
                ClickableComponent button = this.forSaleButtons[i];
                button.upNeighborImmutable = true;
                button.downNeighborImmutable = true;
                button.upNeighborID = ((i > 0) ? (i + 3546 - 1) : (-7777));
                button.downNeighborID = ((i < 3 && i < this.forSale.Count - 1) ? (i + 3546 + 1) : (-7777));
                if (i >= this.forSale.Count)
                {
                    if (button == base.currentlySnappedComponent)
                    {
                        base.currentlySnappedComponent = last_valid_button;
                        if (Game1.options.SnappyMenus)
                        {
                            this.snapCursorToCurrentSnappedComponent();
                        }
                    }
                }
                else
                {
                    last_valid_button = button;
                }
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            base.currentlySnappedComponent = base.getComponentWithID(3546);
            this.snapCursorToCurrentSnappedComponent();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            this.hoverText = "";
            this.hoveredItem = null;
            this.boldTitleText = "";

            if (this.scrolling)
            {
                return;
            }

            for (int j = 0; j < this.forSaleButtons.Count; j++)
            {
                if (this.forSaleButtons[j].containsPoint(x, y))
                {
                    if (j + (currentItemIndex * 2) >= forSale.Count)
                    {
                        return;
                    }

                    ISalable item = this.forSale[j + (currentItemIndex * 2)];
                    this.hoverText = item.getDescription();
                    this.boldTitleText = item.DisplayName;
                    this.hoveredItem = item;
                }
            }
        }
    }
}
