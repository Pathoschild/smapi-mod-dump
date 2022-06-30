/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using MarketDay.API;
using MarketDay.Data;
using MarketDay.ItemPriceAndStock;
using MarketDay.Utility;

namespace MarketDay.Shop
{
    /// <summary>
    /// This class holds all the information for each custom item shop
    /// </summary>
    public class ItemShop : ItemShopModel
    {
        protected Texture2D _portrait;
        public ItemPriceAndStockManager StockManager { get; set; }

        public IContentPack ContentPack { set; get; }
        
        /// <summary>
        /// This is used to make sure that JA only adds items to this shop the first time it is opened each day
        /// or else items will be added every time the shop is opened
        /// </summary>
        protected bool _shopOpenedToday;

        /// <summary>
        /// Initializes the stock manager, done at game loaded so that content packs have finished loading in
        /// </summary>
        public void Initialize()
        {
            StockManager = new ItemPriceAndStockManager(this);
        }

        /// <summary>
        /// Loads the portrait, if it exists, and use the seasonal version if one is found for the current season
        /// </summary>
        public void UpdatePortrait()
        {
            if (PortraitPath == null)
                return;

            //construct seasonal path to the portrait
            string seasonalPath = PortraitPath.Insert(PortraitPath.IndexOf('.'), "_" + Game1.currentSeason);
            try
            {
                //if the seasonal version exists, load it
                if (ContentPack.HasFile(seasonalPath)) 
                {
                    _portrait = ContentPack.ModContent.Load<Texture2D>(seasonalPath);
                }
                //if the seasonal version doesn't exist, try to load the default
                else if (ContentPack.HasFile(PortraitPath))
                {
                    _portrait = ContentPack.ModContent.Load<Texture2D>(PortraitPath);
                }
            }
            catch (Exception ex) //couldn't load the image
            {
                MarketDay.Log(ex.Message+ex.StackTrace, LogLevel.Error);
            }
        }
        
        /// <summary>
        /// Refreshes the contents of all stores
        /// and sets the flag for if the store has been opened yet today to false
        /// </summary>
        public void UpdateItemPriceAndStock()
        {
            _shopOpenedToday = false;
            MarketDay.Log($"Generating stock for {ShopName}", LogLevel.Trace, true);
            StockManager.Update();
        }

        /// <summary>
        /// Translate what needs to be translated on game saved, in case of the language being changed
        /// </summary>
        internal void UpdateTranslations()
        {
            Quote = Translations.Localize(Quote, LocalizedQuote);
            ClosedMessage = Translations.Localize(ClosedMessage, LocalizedClosedMessage);
        }
    }
}
