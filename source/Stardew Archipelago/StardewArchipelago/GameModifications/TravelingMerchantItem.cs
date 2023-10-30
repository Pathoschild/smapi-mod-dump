/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Serialization;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.GameModifications
{
    internal class TravelingMerchantItem : Item
    {
        private bool _hasBeenPurchasedAlready;
        private Item _stardewItem;
        public Item StardewItem => _stardewItem;
        private ArchipelagoStateDto _archipelagoState;

        public TravelingMerchantItem(Item stardewItem, ArchipelagoStateDto archipelagoState) : base()
        {
            _hasBeenPurchasedAlready = false;
            _stardewItem = stardewItem;
            _archipelagoState = archipelagoState;
        }

        public override bool actionWhenPurchased()
        {
            if (_hasBeenPurchasedAlready)
            {
                return _stardewItem.actionWhenPurchased();
            }

            _hasBeenPurchasedAlready = true;
            _archipelagoState.TravelingMerchantPurchases++;
            TravelingMerchantInjections.SetTravelingMerchantFlair(Game1.activeClickableMenu as ShopMenu);
            return _stardewItem.actionWhenPurchased();
        }

        public override Item getOne()
        {
            return _stardewItem.getOne();
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth,
            StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            _stardewItem.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public override string getDescription()
        {
            return _stardewItem.getDescription();
        }

        public override bool isPlaceable()
        {
            return _stardewItem.isPlaceable();
        }

        public override int maximumStackSize()
        {
            return _stardewItem.maximumStackSize();
        }

        public override int addToStack(Item stack)
        {
            return _stardewItem.addToStack(stack);
        }

        public override int salePrice()
        {
            return _stardewItem.salePrice();
        }

        public override bool canStackWith(ISalable other)
        {
            if (other is TravelingMerchantItem otherTravelingMerchantItem)
            {
                return _stardewItem.canStackWith(otherTravelingMerchantItem.StardewItem);
            }

            return _stardewItem.canStackWith(other);
        }

        public override bool CanBuyItem(Farmer farmer)
        {
            return _stardewItem.CanBuyItem(farmer);
        }

        public override string DisplayName
        {
            get => _stardewItem.DisplayName;
            set => _stardewItem.DisplayName = value;
        }

        public override string Name => _stardewItem.Name;
        public override int Stack
        {
            get => _stardewItem.Stack;
            set => _stardewItem.Stack = value;
        }
    }
}
