using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KrobusSellsLargerStacks
{
    public class KrobusSellsLargerStacks : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu menu && Game1.currentLocation.Name == "Sewer")
            {
                foreach (var krobusItem in GetKrobusItemsFromConfig(Config))
                {
                    KeyValuePair<ISalable, int[]>  sellable = GetSellable(menu, krobusItem);

                    if (!sellable.Equals(new KeyValuePair<ISalable, int[]>()))
                    {
                        var sellableItem = sellable.Key;
                        var priceAndStock = sellable.Value;

                        sellableItem.Stack = krobusItem.ItemQuantity;
                        priceAndStock[priceAndStock.Length - 1] = krobusItem.ItemQuantity;
                        menu.itemPriceAndStock[sellableItem] = priceAndStock;
                    }

                }
            }
        }

        public List<KrobusItem> GetKrobusItemsFromConfig(ModConfig modConfig)
        {
            List<KrobusItem> krobusItems = new List<KrobusItem>();

            foreach (var property in modConfig.GetType().GetProperties())
            {
                switch (property.Name)
                {
                    case "DarkEssenceQuantity":
                        krobusItems.Add(new KrobusItem(int.Parse(property.GetValue(modConfig, null)?.ToString()), 769));
                        break;
                    case "SolarEssenceQuantity":
                        krobusItems.Add(new KrobusItem(int.Parse(property.GetValue(modConfig, null)?.ToString()), 768));
                        break;
                    case "SlimeQuantity":
                        krobusItems.Add(new KrobusItem(int.Parse(property.GetValue(modConfig, null)?.ToString()), 766));
                        break;
                    case "OmniGeodeQuantity":
                        krobusItems.Add(new KrobusItem(int.Parse(property.GetValue(modConfig, null)?.ToString()), 749));
                        break;
                    case "MixedSeedsQuantity":
                        krobusItems.Add(new KrobusItem(int.Parse(property.GetValue(modConfig, null)?.ToString()), 770));
                        break;
                    case "IridiumSprinklerQuantity":
                        krobusItems.Add(new KrobusItem(int.Parse(property.GetValue(modConfig, null)?.ToString()), 645));
                        break;
                    case "BatWingQuantity":
                        krobusItems.Add(new KrobusItem(int.Parse(property.GetValue(modConfig, null)?.ToString()), 767));
                        break;
                    case "MagnetQuantity":
                        krobusItems.Add(new KrobusItem(int.Parse(property.GetValue(modConfig, null)?.ToString()), 703));
                        break;
                    case "FishQuantity":
                        krobusItems.Add(new KrobusItem(int.Parse(property.GetValue(modConfig, null)?.ToString()), 0, "Fish"));
                        break;
                    case "FoodQuantity":
                        krobusItems.Add(new KrobusItem(int.Parse(property.GetValue(modConfig, null)?.ToString()), 0, "Cooking"));
                        break;
                    default:
                        break;
                }
            }

            return krobusItems;
        }

        private static KeyValuePair<ISalable, int[]> GetSellable(ShopMenu menu, KrobusItem krobusItem)
        {
            KeyValuePair<ISalable, int[]> sellable = new KeyValuePair<ISalable, int[]>();

            try
            {
                sellable = menu.itemPriceAndStock.Where(kv => (kv.Key as StardewValley.Object).ParentSheetIndex == krobusItem.ItemId
                    || (kv.Key as StardewValley.Object).Type == krobusItem.Type).FirstOrDefault();

            }
            catch (NullReferenceException)
            {
                // Item was not found in shop inventory - not a problem as supply changes daily
            }

            return sellable;
        }
    }
}