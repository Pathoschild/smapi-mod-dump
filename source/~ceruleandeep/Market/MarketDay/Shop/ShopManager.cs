/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using MarketDay;
using MarketDay.ItemPriceAndStock;
using MarketDay.Utility;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using MarketDay.API;
using MarketDay.Data;
using Microsoft.Xna.Framework.Graphics;

namespace MarketDay.Shop
{
    /// <summary>
    /// This class holds and manages all the shops, loading content packs to create shops
    /// And containing methods to update everything that needs to
    /// </summary>
    internal static class ShopManager
    {
        public static readonly Dictionary<string, GrangeShop> GrangeShops = new();
        public static readonly Dictionary<string, AnimalShop> AnimalShops = new();

        /// <summary>
        /// Takes content packs and loads them as ItemShop and AnimalShop objects
        /// </summary>
        public static void LoadContentPacks()
        {
            MarketDay.Log("Clearing content packs (what's the worst that could happen, right?)", LogLevel.Debug);
            foreach (var grangeShop in GrangeShops.Keys) GrangeShops.Remove(grangeShop);
            foreach (var animalShop in AnimalShops.Keys) GrangeShops.Remove(animalShop);
            
            MarketDay.Log("Adding Content Packs...", LogLevel.Info);
            foreach (var contentPack in MarketDay.helper.ContentPacks.GetOwned())
            {
                if (!contentPack.HasFile("shops.json"))
                {
                    MarketDay.Log($"No shops.json found from the mod {contentPack.Manifest.UniqueID}. " +
                        $"Skipping pack.", LogLevel.Warn);
                    continue;
                }

                ContentPack data;
                try
                {
                    data = contentPack.ReadJsonFile<ContentPack>("shops.json");
                }
                catch (Exception ex)
                {
                    MarketDay.Log($"Invalid JSON provided by {contentPack.Manifest.UniqueID}.", LogLevel.Error);
                    MarketDay.Log(ex.Message + ex.StackTrace,LogLevel.Error);
                    continue;
                }

                MarketDay.Log($"Loading: {contentPack.Manifest.Name} by {contentPack.Manifest.Author} | " +
                    $"{contentPack.Manifest.Version} | {contentPack.Manifest.Description}", LogLevel.Info);

                RegisterShops(data, contentPack);
            }
        }

        /// <summary>
        /// Saves each shop as long as its has a unique name
        /// </summary>
        /// <param name="data"></param>
        /// <param name="contentPack"></param>
        private static void RegisterShops(ContentPack data, IContentPack contentPack)
        {
            MarketDay.Config.ShopsEnabled ??= new Dictionary<string, bool>();
            if (data.GrangeShops != null)
            {
                foreach (var shopPack in data.GrangeShops)
                {
                    shopPack.ContentPack = contentPack;

                    if (shopPack.ShopName is null)
                    {
                        MarketDay.Log($"{contentPack.Manifest.Name}: a shop needs a name", LogLevel.Error);
                        continue;
                    }

                    if (!MarketDay.Config.ShopsEnabled.ContainsKey(shopPack.ShopKey))
                    {
                        MarketDay.Config.ShopsEnabled[shopPack.ShopKey] = true;
                    }
                    
                    if (GrangeShops.ContainsKey(shopPack.ShopKey))
                    {
                        MarketDay.Log($"{contentPack.Manifest.Name} is trying to add a Shop {shopPack.ShopKey}" +
                            $" but a shop of this name has already been added. " +
                            $"It will not be added.", LogLevel.Warn);
                        continue;
                    }

                    MarketDay.Log($"{contentPack.Manifest.Name} adding {shopPack.ShopKey}", LogLevel.Trace);
                    GrangeShops.Add(shopPack.ShopKey, shopPack);
                    
                    // load sign assets
                    LoadAssets(contentPack, shopPack);
                }
            }

            if (data.AnimalShops != null)
            {
                foreach (AnimalShop animalShopPack in data.AnimalShops)
                {
                    if (AnimalShops.ContainsKey(animalShopPack.ShopName))
                    {
                        MarketDay.Log($"{contentPack.Manifest.Name} is trying to add an AnimalShop \"{animalShopPack.ShopName}\"," +
                            $" but a shop of this name has already been added. " +
                            $"It will not be added.", LogLevel.Warn);
                        continue;
                    }
                    AnimalShops.Add(animalShopPack.ShopName, animalShopPack);
                }
            }
        }

        private static void LoadAssets(IContentPack contentPack, GrangeShop shopPack)
        {
            if (shopPack.OpenSignPath is not null && shopPack.OpenSignPath.Length > 0)
            {
                try
                {
                    shopPack.OpenSign = contentPack.ModContent.Load<Texture2D>(shopPack.OpenSignPath);
                    MarketDay.Log($"[{shopPack.ShopName}] Loaded asset {shopPack.OpenSignPath}", LogLevel.Trace);
                }
                catch (Exception ex)
                {
                    MarketDay.Log($"[{shopPack.ShopName}] Could not load asset {shopPack.OpenSignPath}", LogLevel.Error);
                    MarketDay.Log(ex.Message + ex.StackTrace, LogLevel.Error);
                }
            }

            if (shopPack.ClosedSignPath is not null && shopPack.ClosedSignPath.Length > 0)
            {
                try
                {
                    shopPack.ClosedSign = contentPack.ModContent.Load<Texture2D>(shopPack.ClosedSignPath);
                    MarketDay.Log($"[{shopPack.ShopName}] Loaded asset {shopPack.ClosedSignPath}", LogLevel.Trace);
                }
                catch (Exception ex)
                {
                    MarketDay.Log($"[{shopPack.ShopName}] Could not load asset {shopPack.ClosedSignPath}", LogLevel.Error);
                    MarketDay.Log(ex.Message + ex.StackTrace, LogLevel.Error);
                }
            }
        }

        /// <summary>
        /// Update all translations for each shop when a save file is loaded
        /// </summary>
        public static void UpdateTranslations()
        {
            foreach (ItemShop itemShop in GrangeShops.Values)
            {
                itemShop.UpdateTranslations();
            }

            foreach (AnimalShop animalShop in AnimalShops.Values)
            {
                animalShop.UpdateTranslations();
            }
        }

        /// <summary>
        /// Initializes all shops once the game is loaded
        /// </summary>
        public static void InitializeShops()
        {
            foreach (var itemShop in GrangeShops.Values)
            {
                itemShop.Initialize();
            }
        }

        /// <summary>
        /// Initializes the stocks of each shop after the save file has loaded so that item IDs are available to generate items
        /// </summary>
        public static void InitializeItemStocks()
        {
            foreach (GrangeShop itemShop in GrangeShops.Values)
            {
                if (itemShop.StockManager is null)
                {
                    MarketDay.Log($"InitializeItemStocks: StockManager for {itemShop.ShopName} is null", LogLevel.Warn);
                    return;
                }
                itemShop.StockManager.Initialize();
            }
        }

        /// <summary>
        /// Updates the stock for all itemshops at the start of each day
        /// and updates their portraits too to match the current season
        /// </summary>
        internal static void UpdateStock()
        {
            if (GrangeShops.Count > 0)
                MarketDay.Log($"Refreshing stock for all custom shops...", LogLevel.Trace);

            foreach (var grangeShop in GrangeShops.Values)
            {
                if (! grangeShop.IsPlayerShop) grangeShop.UpdateItemPriceAndStock();
                grangeShop.UpdatePortrait();
            }
        }

    }
}
