/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;
using StardewValley.Internal;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace MobileCatalogues
{
    public class Catalogues
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static ModConfig Config;

        // call this method from your Entry class
        public static void Initialize(IModHelper helper, IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }
        internal static void OpenCatalogue(string id)
        {
            switch (id)
            {
                case "catalogue":
                    OpenCatalogue();
                    break;
                case "furniture-catalogue":
                    OpenFurnitureCatalogue();
                    break;
                case "seed-catalogue":
                    OpenSeedCatalogue();
                    break;
                case "travel-catalogue":
                    OpenTravelingCatalogue();
                    break;
                case "desert-catalogue":
                    OpenDesertCatalogue();
                    break;
                case "hat-catalogue":
                    OpenHatMouseCatalogue();
                    break;
                case "clothing-catalogue":
                    OpenClothingCatalogue();
                    break;
                case "dwarf-catalogue":
                    OpenDwarfCatalogue();
                    break;
                case "krobus-catalogue":
                    OpenKrobusCatalogue();
                    break;
                case "guild-catalogue":
                    OpenGuildCatalogue();
                    break;
            }
        }

        public static void OpenCatalogue()
        {
            Monitor.Log("Opening catalogue");
            // ShopMenuFacade
            DelayedOpen(new ShopMenu("Catalogue", GetAllWallpapersAndFloors()));
            
        }

        public static void OpenFurnitureCatalogue()
        {
            Monitor.Log("Opening furniture catalogue");
            DelayedOpen(new ShopMenu("Furniture Catalogue", GetAllFurnitures()));
        }

        public static void OpenSeedCatalogue()
        {
            Monitor.Log("Opening seed catalogue");
            DelayedOpen(new ShopMenu("Seed Catalogue", GetAllSeeds()));
        }

        public static void OpenTravelingCatalogue()
        {
            if (Config.LimitTravelingCatalogToInTown && Game1.getLocationFromName("Forest") != null && !(Game1.getLocationFromName("Forest") as Forest).travelingMerchantDay)
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("traveling-merchant-not-here"));
                return;
            }

            var dict = ShopBuilder.GetShopStock("Traveler");

            AdjustPrices(ref dict, Config.FreeTravelingCatalogue);

            Monitor.Log("Opening traveling catalogue");
            // TODO: Fix
            DelayedOpen(new ShopMenu("Traveler", dict)); // on_purchase: new Func<ISalable, Farmer, int, bool>(Utility.onTravelingMerchantShopPurchase), null, null
        }

        public static void OpenDesertCatalogue()
        {
            if (Config.LimitDesertCatalogToBusFixed && !Game1.player.mailReceived.Contains("ccVault"))
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("desert-merchant-cannot-ship"));
                return;
            }

            var dict = ShopBuilder.GetShopStock("DesertTrade");
            AdjustPrices(ref dict, Config.FreeDesertCatalogue);

            Monitor.Log("Opening desert catalogue");
            // TODO: Fix
            DelayedOpen(new ShopMenu("DesertTrade", dict)); // on_purchase: boughtTraderItem
        }


        public static void OpenHatMouseCatalogue()
        {
            if (Game1.player.achievements.Count == 0)
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("catalog-not-available"));
                return;
            }
            var dict = ShopBuilder.GetShopStock("HatMouse");
            AdjustPrices(ref dict, Config.FreeHatCatalogue);

            Monitor.Log("Opening hat catalogue");
            DelayedOpen(new ShopMenu("HatMouse", dict));
        }

        public static void OpenClothingCatalogue()
        {
            Monitor.Log("Opening clothing catalogue");
            DelayedOpen(new ShopMenu("Clothing", GetAllClothing()));
        }


        private static void OpenDwarfCatalogue()
        {
            var dict = ShopBuilder.GetShopStock("Dwarf");
            AdjustPrices(ref dict, false);
            Game1.activeClickableMenu = new ShopMenu("Dwarf", dict);
        }

        private static void OpenKrobusCatalogue()
        {
            var dict = ShopBuilder.GetShopStock("Krobus");
            AdjustPrices(ref dict, false);
            Game1.activeClickableMenu = new ShopMenu("Krobus", dict);
        }

        private static void OpenGuildCatalogue()
        {
            var dict = ShopBuilder.GetShopStock("Marlon");
            AdjustPrices(ref dict, false);
            Game1.activeClickableMenu = new ShopMenu("Marlon", dict);
        }


        private static async void DelayedOpen(ShopMenu menu)
        {
            await Task.Delay(100);
            Monitor.Log("Really opening catalogue");
            Game1.activeClickableMenu = menu;
        }

        private static Dictionary<ISalable, ItemStockInformation> GetAllWallpapersAndFloors()
        {
            Dictionary<ISalable, ItemStockInformation> decors = new();
            Wallpaper f;
            for (int i = 0; i < 112; i++)
            {
                f = new Wallpaper(i, false);
                decors.Add(new Wallpaper(i, false)
                {
                    Stack = int.MaxValue
                }, new ItemStockInformation(
                    Config.FreeCatalogue  ? 0 : (int)Math.Round(f.salePrice() * Config.PriceMult),
                    int.MaxValue
                ));
            }
            for (int j = 0; j < 56; j++)
            {
                f = new Wallpaper(j, false);
                decors.Add(new Wallpaper(j, true)
                {
                    Stack = int.MaxValue
                }, new ItemStockInformation(
                    Config.FreeCatalogue  ? 0 : (int)Math.Round(f.salePrice() * Config.PriceMult),
                    int.MaxValue
                ));
            }
            return decors;
        }

        private static Dictionary<ISalable, ItemStockInformation> GetAllFurnitures()
        {
            Dictionary<ISalable, ItemStockInformation> decors = new();
            Furniture f;
            foreach (KeyValuePair<string, string> v in Game1.content.Load<Dictionary<string, string>>("Data\\Furniture"))
            {
                if(v.Value.Split('/')[1] == "fishtank")
                    f = new FishTankFurniture(v.Key, Vector2.Zero);
                else if(v.Value.Split('/')[1] == "bed")
                    f = new BedFurniture(v.Key, Vector2.Zero);
                else if(v.Value.Split('/')[0].EndsWith("TV"))
                    f = new TV(v.Key, Vector2.Zero);
                else
                    f = new Furniture(v.Key, Vector2.Zero);
                decors.Add(f, new ItemStockInformation(
                    Config.FreeFurnitureCatalogue ? 0 : (int)Math.Round(f.salePrice() * Config.PriceMult),
                    int.MaxValue
                ));
            }
            return decors;
        }

        private struct SeedProductData
        {
            public string SeedId { get; }
            public string CropId { get; }
            public Season[] Seasons { get; }

            public SeedProductData(string id, string cropId, Season[]? seasons)
            {
                SeedId = id;
                CropId = cropId;
                Seasons = seasons;
            }

            public bool IsInSeason(GameLocation location)
            {
                if (Seasons == null)
                {
                    return true;
                }

                return Seasons.Contains(location.GetSeason());
            }
        }

        private static Dictionary<ISalable, ItemStockInformation> GetAllSeeds()
        {
            Dictionary<ISalable, ItemStockInformation> items = new();
            List<SeedProductData> seedProducts = new();

            foreach (KeyValuePair<string, CropData> kvp in Game1.cropData)
            {
                seedProducts.Add(new SeedProductData(kvp.Key, kvp.Value.HarvestItemId, kvp.Value.Seasons?.ToArray()));
            }
            foreach (KeyValuePair<string, FruitTreeData> kvp in Game1.fruitTreeData)
            {
                foreach (var fruit in kvp.Value.Fruit)
                {
                    var seasons = fruit.Season is null ? null : new[] { fruit.Season!.Value };
                    seedProducts.Add(new SeedProductData(kvp.Key, fruit.Id, seasons));
                }
            }

            HashSet<string> addedSeeds = new();
            foreach (SeedProductData data in seedProducts)
            {
                bool include = true;
                if(Config.SeedsToInclude.ToLower() == "shipped")
                {
                    include = Game1.player.basicShipped.ContainsKey(data.CropId);
                }
                else if (Config.SeedsToInclude.ToLower() == "season")
                {
                    include = data.IsInSeason(Game1.currentLocation);
                }

                if (include && !addedSeeds.Contains(data.SeedId))
                {
                    Object item = ItemRegistry.Create<Object>(data.SeedId);
                    if (item == null)
                    {
                        continue;
                    }

                    if (!item.bigCraftable.Value && item.ParentSheetIndex == 745)
                    {
                        item.Price = (int)Math.Round(50 * Config.PriceMult);
                    }

                    items.Add(item, new ItemStockInformation(
                        Config.FreeSeedCatalogue ? 0 : (int)Math.Round(item.salePrice() * Config.PriceMult),
                        int.MaxValue
                    ));
                    addedSeeds.Add(data.SeedId);
                }
            }
            return items;
        }

        private static Dictionary<ISalable, ItemStockInformation> GetAllClothing()
        {
            Dictionary<ISalable, ItemStockInformation> stock = new();
            foreach (var key in Game1.shirtData.Keys.Concat(Game1.pantsData.Keys))
            {
                Clothing c = new(key);
                if (c != null)
                {
                    stock.Add(c, new ItemStockInformation(
                        Config.FreeClothingCatalogue ? 0 : (int)Math.Round(c.salePrice() * Config.PriceMult),
                        int.MaxValue
                    ));
                }
            }
            return stock;
        }

        private static void AdjustPrices(ref Dictionary<ISalable, ItemStockInformation> dict, bool free)
        {
            foreach (var key in dict.Keys)
            {
                dict[key] = new ItemStockInformation(
                    free ? 0 : (int)Math.Round(dict[key].Price * Config.PriceMult),
                    dict[key].Stock
                );
            }
        }
        public static bool boughtTraderItem(ISalable s, Farmer arg2, int arg3)
        {
            if (s.Name == "Magic Rock Candy")
            {
                // TODO: Fix
                // Desert.boughtMagicRockCandy = true;
            }
            return false;
        }

    }
}
