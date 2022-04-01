/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Reflection;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;

namespace DeluxeJournal.Util
{
    /// <summary>Facilitates attaching callbacks to ShopMenus.</summary>
    public class ShopHelper
    {
        private static readonly FieldInfo AnimationsField;
        private static readonly FieldInfo SellPercentageField;

        static ShopHelper()
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

            AnimationsField = ReflectionHelper.TryGetField<ShopMenu>("animations", flags);
            SellPercentageField = ReflectionHelper.TryGetField<ShopMenu>("sellPercentage", flags);
        }

        /// <summary>Attach an onPurchase callback to a ShopMenu.</summary>
        /// <param name="shop">The ShopMenu.</param>
        /// <param name="onPurchase">The callback to be attached. A return value of true exits the menu.</param>
        public static void AttachPurchaseCallback(ShopMenu shop, Func<ISalable, Farmer, int, bool> onPurchase)
        {
            Func<ISalable, Farmer, int, bool> origOnPurchase = shop.onPurchase;

            shop.onPurchase = delegate (ISalable salable, Farmer player, int amount)
            {
                bool exit = onPurchase(salable, player, amount);

                if (origOnPurchase != null)
                {
                    return origOnPurchase(salable, player, amount) || exit;
                }

                return exit;
            };
        }

        /// <summary>Attach an onSell callback to a ShopMenu, while keeping the default sell behavior.</summary>
        /// <param name="shop">The ShopMenu.</param>
        /// <param name="onSell">The callback to be attached. A return value of true exits the menu.</param>
        public static void AttachQuietSellCallback(ShopMenu shop, Func<ISalable, bool> onSell)
        {
            Func<ISalable, bool> origOnSell = shop.onSell;

            shop.onSell = delegate (ISalable salable)
            {
                bool exit = onSell(salable);

                if (origOnSell != null)
                {
                    return origOnSell(salable) || exit;
                }
                else if (salable is Item item)
                {
                    ISalable? buybackItem = null;
                    float sellPercentage = (float?)SellPercentageField.GetValue(shop) ?? 1f;
                    int sellPrice;

                    if (item is SObject obj)
                    {
                        sellPrice = (int)(obj.sellToStorePrice() * sellPercentage);
                    }
                    else
                    {
                        sellPrice = (int)(item.salePrice() * sellPercentage / 2);
                    }

                    ShopMenu.chargePlayer(Game1.player, shop.currency, -sellPrice * item.Stack);

                    if (shop.CanBuyback())
                    {
                        buybackItem = shop.AddBuybackItem(item, sellPrice, item.Stack);
                    }

                    if (item is SObject && ((SObject)item).Edibility != -300)
                    {
                        if (buybackItem != null && shop.buyBackItemsToResellTomorrow.ContainsKey(buybackItem))
                        {
                            shop.buyBackItemsToResellTomorrow[buybackItem].Stack += item.Stack;
                        }
                        else if (Game1.currentLocation is ShopLocation location)
                        {
                            Item clone = item.getOne();
                            clone.Stack = item.Stack;

                            if (buybackItem != null)
                            {
                                shop.buyBackItemsToResellTomorrow[buybackItem] = clone;
                            }

                            location.itemsToStartSellingTomorrow.Add(clone);
                        }
                    }

                    if (AnimationsField.GetValue(shop) is List<TemporaryAnimatedSprite> animations)
                    {
                        Vector2 animationPosition = shop.inventory.snapToClickableComponent(Game1.getMouseX(), Game1.getMouseY()) + new Vector2(32f, 32f);
                        Vector2 animationMotionTarget = new Vector2(shop.xPositionOnScreen - 36, shop.yPositionOnScreen + shop.height - shop.inventory.height - 16);
                        int coins = item.Stack / 8 + 2;

                        for (int i = 0; i < coins; i++)
                        {
                            animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16, 64, 16, 16), 9999f, 1, 999, animationPosition, false, false)
                            {
                                scale = 2f,
                                alphaFade = 0.025f,
                                delayBeforeAnimationStart = i * 25,
                                motion = new Vector2(Game1.random.Next(-3, 4), -4f),
                                acceleration = new Vector2(0, 0.5f)
                            });

                            animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16, 64, 16, 16), 9999f, 1, 999, animationPosition, false, false)
                            {
                                scale = 4f,
                                alphaFade = 0.025f,
                                delayBeforeAnimationStart = i * 25,
                                motion = Utility.getVelocityTowardPoint(animationPosition, animationMotionTarget, 8f),
                                acceleration = Utility.getVelocityTowardPoint(animationPosition, animationMotionTarget, 0.5f)
                            });
                        }

                        if (shop.inventory.getItemAt(Game1.getMouseX(), Game1.getMouseY()) == null)
                        {
                            animations.Add(new TemporaryAnimatedSprite(5, animationPosition, Color.White)
                            {
                                motion = new Vector2(0, -0.5f)
                            });
                        }
                    }

                    Game1.playSound("sell");
                    Game1.playSound("purchase");
                }

                return exit;
            };
        }
    }
}
