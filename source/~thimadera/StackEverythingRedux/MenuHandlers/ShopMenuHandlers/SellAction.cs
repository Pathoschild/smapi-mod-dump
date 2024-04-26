/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace StackEverythingRedux.MenuHandlers.ShopMenuHandlers
{
    public class SellAction : ShopAction
    {
        private const int SMALL_TILE = Game1.smallestTileSize;
        private const int HALF_TILE = Game1.tileSize / 2;
        private const int FULL_TILE = Game1.tileSize;

        private readonly Guid GUID = Guid.NewGuid();

        /// <summary>Constructs an instance.</summary>
        /// <param name="menu">The native shop menu.</param>
        /// <param name="item">The item to buy.</param>
        public SellAction(ShopMenu menu, Item item)
            : base(menu, item)
        {
            // Default amount
            // +1 before /2 ensures we get the number rounded UP
            Amount = (ClickedItem.Stack + 1) / 2;
            Log.TraceIfD($"[{nameof(SellAction)}] Instantiated for shop {menu} item {item}, GUID = {GUID}");
        }

        ~SellAction()
        {
            Log.TraceIfD($"[{nameof(SellAction)}] Finalized for GUID = {GUID}");
        }

        /// <summary>Verifies the conditions to perform the action.</summary>
        public override bool CanPerformAction()
        {
            return NativeShopMenu.highlightItemToSell(ClickedItem) && ClickedItem.Stack > 1;
        }

        /// <summary>Does the action.</summary>
        /// <param name="amount">Number of items.</param>
        /// <param name="clickLocation">Where the player clicked (i.e., when selecting item to split).</param>
        public override void PerformAction(int amount, Point clickLocation)
        {
            amount = Math.Min(amount, ClickedItem.Stack);
            Amount = amount; // Remove if we don't need to carry this around

            // Sell item
            int price = CalculateSalePrice(ClickedItem, amount);
            ShopMenu.chargePlayer(Game1.player, ShopCurrencyType, price);
            Log.Trace($"[{nameof(SellAction)}.{nameof(PerformAction)}] Charged player {price} for {amount} of {ClickedItem.Name}");

            // Update the stack amount/remove the item
            IList<Item> actualInventory = InvMenu.actualInventory;
            int index = actualInventory.IndexOf(ClickedItem);
            if (index >= 0 && index < actualInventory.Count)
            {
                int amountRemaining = ClickedItem.Stack - amount;
                if (amountRemaining > 0)
                {
                    actualInventory[index].Stack = amountRemaining;
                }
                else
                {
                    actualInventory[index] = null;
                }
            }

            _ = Game1.playSound("purchaseClick");
            if (amount < 1)
            {
                return;
            }

            Animate(amount, clickLocation);
        }

        /// <summary>
        /// Create animation of coins flying from the inventory slot to the shop moneybox
        /// </summary>
        /// <param name="amount">Number of items sold; determines the number of flying coins</param>
        /// <param name="clickLocation">Original click location (selection of item to sell).</param>
        public void Animate(int amount, Point clickLocation)
        {
            // This procedure is copied/adapted from game code
            // StardewValley.Menus.ShopMenu.receiveLeftClick

            // Might want to cap this ...
            int coins = (amount / 8) + 2;  // scale of "1/8" is from game code

            StardewModdingAPI.IReflectedField<List<TemporaryAnimatedSprite>> animationsField = StackEverythingRedux.Reflection.GetField<List<TemporaryAnimatedSprite>>(NativeShopMenu, "animations");
            List<TemporaryAnimatedSprite> animations = animationsField.GetValue();

            Vector2 snappedPosition = InvMenu.snapToClickableComponent(clickLocation.X, clickLocation.Y);

            Vector2 anim_pos = snappedPosition + new Vector2(HALF_TILE, HALF_TILE);

            Point startingPoint = new((int)snappedPosition.X + HALF_TILE, (int)snappedPosition.Y + HALF_TILE);

            int pos_x = NativeShopMenu.xPositionOnScreen;
            int pos_y = NativeShopMenu.yPositionOnScreen;
            int height = NativeShopMenu.height;
            Vector2 endingPoint = new(pos_x - 36, pos_y + height - InvMenu.height - 16);

            Vector2 accel1 = new(0f, 0.5f);
            Vector2 accel2 = Utility.getVelocityTowardPoint(startingPoint, endingPoint, 0.5f);
            Vector2 motion2 = Utility.getVelocityTowardPoint(startingPoint, endingPoint, 8f);

            for (int j = 0; j < coins; j++)
            {
                animations.Add(
                    new TemporaryAnimatedSprite(
                        textureName: Game1.debrisSpriteSheetName,
                        sourceRect: new Rectangle(Game1.random.Next(2) * SMALL_TILE, FULL_TILE, SMALL_TILE, SMALL_TILE),
                        animationInterval: 9999f,
                        animationLength: 1,
                        numberOfLoops: 999,
                        position: anim_pos,
                        flicker: false,
                        flipped: false
                        )
                    {
                        alphaFade = 0.025f,
                        motion = new Vector2(Game1.random.Next(-3, 4), -4f),
                        acceleration = accel1,
                        delayBeforeAnimationStart = j * 25,
                        scale = 2f
                    }
                    );
                animations.Add(
                    new TemporaryAnimatedSprite(
                        textureName: Game1.debrisSpriteSheetName,
                        sourceRect: new Rectangle(Game1.random.Next(2) * SMALL_TILE, FULL_TILE, SMALL_TILE, SMALL_TILE),
                        animationInterval: 9999f,
                        animationLength: 1,
                        numberOfLoops: 999,
                        position: anim_pos,
                        flicker: false,
                        flipped: false
                        )
                    {
                        alphaFade = 0.025f,
                        motion = motion2,
                        acceleration = accel2,
                        delayBeforeAnimationStart = j * 50,
                        scale = 4f
                    }
                    );
            }  // end_for j

            // Not sure if this is needed, but just to be safe
            animationsField.SetValue(animations);
        }

        // TODO: verify this is correct and Item.sellToShopPrice doesn't do the same thing
        /// <summary>Calculates the sale price of an item based on the algorithms used in the game source.</summary>
        /// <param name="item">Item to get the price for.</param>
        /// <param name="amount">Number being sold.</param>
        /// <returns>The sale price of the item * amount.</returns>
        private int CalculateSalePrice(Item item, int amount)
        {
            // Formula from ShopMenu.cs
            float sellPercentage = StackEverythingRedux.Reflection.GetField<float>(NativeShopMenu, "sellPercentage").GetValue();

            float pricef = sellPercentage * amount;
            pricef *= item is SObject sobj
                ? sobj.sellToStorePrice()
                : item.salePrice() * 0.5f
                ;

            // Invert so we give the player money instead (shitty but it's what the game does).
            return -(int)pricef;
        }

        /// <summary>Creates an instance of the action.</summary>
        /// <param name="shopMenu">Native shop menu.</param>
        /// <param name="mouse">Mouse position.</param>
        /// <returns>The instance or null if no valid item was selected.</returns>
        public static ShopAction Create(ShopMenu shopMenu, Point mouse)
        {
            InventoryMenu inventory = shopMenu.inventory;
            Item item = inventory.getItemAt(mouse.X, mouse.Y);
            return item != null ? new SellAction(shopMenu, item) : null;
        }
    }
}
