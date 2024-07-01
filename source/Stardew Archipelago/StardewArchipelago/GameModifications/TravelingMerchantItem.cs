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
using StardewArchipelago.Serialization;
using StardewValley;

namespace StardewArchipelago.GameModifications
{
    internal class TravelingMerchantItem : ISalable
    {
        private bool _hasBeenPurchasedAlready;
        private ISalable _stardewItem;
        private ArchipelagoStateDto _archipelagoState;

        public TravelingMerchantItem(ISalable stardewItem, ArchipelagoStateDto archipelagoState)
        {
            _hasBeenPurchasedAlready = false;
            _stardewItem = stardewItem;
            _archipelagoState = archipelagoState;
        }

        public bool actionWhenPurchased(string shopId)
        {
            if (_hasBeenPurchasedAlready)
            {
                return _stardewItem.actionWhenPurchased(shopId);
            }

            _hasBeenPurchasedAlready = true;
            return _stardewItem.actionWhenPurchased(shopId);
        }

        public bool ShouldDrawIcon()
        {
            return _stardewItem.ShouldDrawIcon();
        }

        public string getDescription()
        {
            return _stardewItem.getDescription();
        }

        public int maximumStackSize()
        {
            return _stardewItem.maximumStackSize();
        }

        public bool appliesProfitMargins()
        {
            return _stardewItem.appliesProfitMargins();
        }

        public void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color,
            bool drawShadow)
        {
            _stardewItem.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public int addToStack(Item stack)
        {
            return _stardewItem.addToStack(stack);
        }

        public int sellToStorePrice(long specificPlayerID = -1)
        {
            return _stardewItem.sellToStorePrice();
        }

        public int salePrice(bool ignoreProfitMargins = false)
        {
            return _stardewItem.salePrice(ignoreProfitMargins);
        }

        public bool canStackWith(ISalable other)
        {
            if (other is TravelingMerchantItem otherTravelingMerchantItem)
            {
                return _stardewItem.canStackWith(otherTravelingMerchantItem._stardewItem);
            }

            return _stardewItem.canStackWith(other);
        }

        public bool CanBuyItem(Farmer farmer)
        {
            return _stardewItem.CanBuyItem(farmer);
        }

        public bool IsInfiniteStock()
        {
            return _stardewItem.IsInfiniteStock();
        }

        public ISalable GetSalableInstance()
        {
            return _stardewItem.GetSalableInstance();
        }

        public void FixStackSize()
        {
            _stardewItem.FixStackSize();
        }

        public void FixQuality()
        {
            _stardewItem.FixQuality();
        }

        public string TypeDefinitionId => _stardewItem.TypeDefinitionId;
        public string QualifiedItemId => _stardewItem.QualifiedItemId;

        public string DisplayName => _stardewItem.DisplayName;

        public string Name => _stardewItem.Name;

        public bool IsRecipe
        {
            get => _stardewItem.IsRecipe;
            set => _stardewItem.IsRecipe = value;
        }

        public int Stack
        {
            get => _stardewItem.Stack;
            set => _stardewItem.Stack = value;
        }

        public int Quality
        {
            get => _stardewItem.Quality;
            set => _stardewItem.Quality = value;
        }

        public string GetItemTypeId()
        {
            return _stardewItem.GetItemTypeId();
        }
    }
}
