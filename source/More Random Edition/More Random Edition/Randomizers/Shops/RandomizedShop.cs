/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Force.DeepCloner;
using StardewValley;
using StardewValley.GameData.Shops;
using System;
using System.Collections.Generic;
using System.Linq;
using static StardewValley.GameData.QuantityModifier;

namespace Randomizer
{
    public abstract class RandomizedShop
    {
        /// <summary>
        /// The key to the shop in Data/Shops
        /// </summary>
        public string ShopId { get; private set; }

        /// <summary>
        /// The default data in Data/Shops
        /// </summary>
        private static readonly Dictionary<string, ShopData> DefaultShopData = 
            DataLoader.Shops(Game1.content);

        /// <summary>
        /// The current shop data to use as a replacement - deep cloned from the default
        /// </summary>
        protected ShopData CurrentShopData { get; set; }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="shopId">The shop id</param>
        public RandomizedShop(string shopId) 
        {
            ShopId = shopId;
            CurrentShopData = DefaultShopData[shopId].DeepClone();
        }

        /// <summary>
        /// Used to determine whether the shop should be modified
        /// </summary>
        /// <returns>Whether the shop should be modified, based on settings</returns>
        public abstract bool ShouldModifyShop();

        /// <summary>
        /// The method that will modify the shop stock in some way
        /// </summary>
        /// <returns>The modified shop data</returns>
        public abstract ShopData ModifyShop();

        /// <summary>
        /// Gets the shop item by the given id, or null if not found
        /// </summary>
        /// <param name="id">The id to search - this is the "Id" field in the ShopItemData</param>
        /// <returns>The retrieved data</returns>
        protected ShopItemData GetShopItemById(string id)
        {
            return CurrentShopData.Items
                .FirstOrDefault(shopItem => shopItem.Id == id);
        }

        /// <summary>
        /// Gets all shop items matching any value in the list of given ids
        /// </summary>
        /// <param name="id">The id to search - this is the "ItemId" field in the ShopItemData</param>
        /// <returns>The retrieved data</returns>
        protected List<ShopItemData> GetShopItemsByItemIds(List<string> ids)
        {
            return CurrentShopData.Items
                .Where(shopItem => ids.Contains(shopItem.ItemId))
                .ToList();
        }

        /// <summary>
        /// Gets a new price for the item
        /// Takes the greater of the item's price and the fallback price and multiplies them by
        /// the multiplier and the difficulty level
        /// </summary>
        /// <param name="item">The item to get the price for</param>
        /// <param name="fallbackPrice">The price to use if the item costs too little</param>
        /// <param name="multiplier">The multiplier</param>
        /// <returns>The new item price</returns>
        protected static int GetAdjustedItemPrice(Item item, int fallbackPrice, int multiplier)
        {
            return GetAdjustedItemPrice(item.GetSaliableObject(), fallbackPrice, multiplier);
        }

        /// <summary>
        /// Gets a new price for the item
        /// Takes the greater of the item's price and the fallback price and multiplies them by
        /// the multiplier and the difficulty level
        /// </summary>
        /// <param name="item">The item to get the price for</param>
        /// <param name="fallbackPrice">The price to use if the item costs too little</param>
        /// <param name="multiplier">The multiplier</param>
        /// <returns>The new item price</returns>
        protected static int GetAdjustedItemPrice(ISalable item, int fallbackPrice, int multiplier)
        {
            return Math.Max(fallbackPrice, (int)(item.salePrice() * Game1.MasterPlayer.difficultyModifier)) * multiplier;
        }

        /// <summary>
        /// Gets a shop item to be added to the shop
        /// Uses mostly default values
        /// The stock values will use the limited mode that's player specific
        /// </summary>
        /// <param name="qualifiedId">The quantified id string</param>
        /// <param name="uniqueId">The id to use for the shop entry, must be unique to the shop</param>
        /// <param name="price">The price of the item - use -1 for a default price</param>
        /// <param name="availableStock">The stock of the item - use -1 for infinite</param>
        /// <param name="isRecipe">Whether the item is a recipe</param>
        /// <param name="condition">The condition for the item to show up in the shop</param>
        /// <param name="maxItems">Used when the item id is an array of some kind - the max items to choose from</param>
        /// <returns>The data to add to the shop</returns>
        protected static ShopItemData GetNewShopItem(
            string qualifiedId, 
            string uniqueId,
            int price = -1, 
            int availableStock = -1,
            bool isRecipe = false,
            string condition = null,
            int? maxItems = null)
        {
            return new ShopItemData()
            {
                TradeItemId = null,
                TradeItemAmount = 1,
                Price = price,
                ApplyProfitMargins = null,
                AvailableStock = availableStock,
                AvailableStockLimit = LimitedStockMode.Player,
                AvoidRepeat = false,
                UseObjectDataPrice = false,
                IgnoreShopPriceModifiers = true,
                PriceModifiers = null,
                PriceModifierMode = QuantityModifierMode.Stack,
                AvailableStockModifiers = null,
                AvailableStockModifierMode = QuantityModifierMode.Stack,
                Condition = condition,
                Id = $"{Globals.ModRef.ModManifest.UniqueID}-{uniqueId}", // This has to be a unique id in this list
                ItemId = qualifiedId,
                RandomItemId = null,
                MaxItems = maxItems,
                MinStack = -1,
                MaxStack = -1,
                Quality = -1,
                ObjectInternalName = null,
                ObjectDisplayName = null,
                ToolUpgradeLevel = -1,
                IsRecipe = isRecipe,
                StackModifiers = null,
                StackModifierMode = QuantityModifierMode.Stack,
                QualityModifiers = null,
                QualityModifierMode = QuantityModifierMode.Stack,
                PerItemCondition = null
            };
        }

		/// <summary>
		/// Adds a new item to the shop
		/// </summary>
		/// <param name="qualifiedId">The quantified id string</param>
		/// <param name="uniqueId">The id to use for the shop entry, must be unique to the shop</param>
		/// <param name="price">The price of the item - use -1 for a default price</param>
		/// <param name="availableStock">The stock of the item - use -1 for infinite</param>
		/// <param name="isRecipe">Whether the item is a recipe</param>
		/// <param name="condition">The condition for the item to show up in the shop</param>
		/// <param name="maxItems">Used when the item id is an array of some kind - the max items to choose from</param>
		protected void AddStock(
            string qualifiedId,
            string uniqueId,
            int price = -1,
            int availableStock = -1,
            bool isRecipe = false,
            string condition = null,
            int? maxItems = null)
        {
            AddStock(GetNewShopItem(qualifiedId, uniqueId, price, availableStock, isRecipe, condition, maxItems));
        }

        /// <summary>
        /// Adds an item into the shop at the end of the list
        /// </summary>
        /// <param name="itemToAdd">The item to add</param>
        protected void AddStock(ShopItemData itemToAdd)
        {
            CurrentShopData.Items.Add(itemToAdd);
        }

        /// <summary>
        /// Inserts an item into the shop data at the given index (the beginning, by default)
        /// </summary>
        /// <param name="qualifiedId">The quantified id string</param>
        /// <param name="uniqueId">The id to use for the shop entry, must be unique to the shop</param>
        /// <param name="price">The price of the item - use -1 for a default price</param>
        /// <param name="availableStock">The stock of the item - use -1 for infinite</param>
        /// <param name="index">The index to add to (defaults to 0)</param>
        protected void InsertStockAt(
            string qualifiedId,
            string uniqueId,
            int price = -1,
            int availableStock = -1,
            int index = 0)
        {
            InsertStockAt(
                GetNewShopItem(qualifiedId, uniqueId, price, availableStock),
                index
            );
        }

        /// <summary>
        /// Inserts an item into the shop data at the given index (the beginning, by default)
        /// </summary>
        /// <param name="itemToAdd">The item to add</param>
        /// <param name="index">The index to add to (defaults to 0)</param>
        protected void InsertStockAt(ShopItemData itemToAdd, int index = 0)
        {
            CurrentShopData.Items.Insert(index, itemToAdd);
        }
    }
}
