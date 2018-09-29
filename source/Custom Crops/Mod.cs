using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using System.IO;
using System.Reflection;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using StardewValley;

namespace CustomCrops
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        
        public override void Entry(IModHelper helper)
        {
            instance = this;

            MenuEvents.MenuChanged += menuChanged;

            var savedIds = helper.ReadJsonFile<Dictionary<string, CropData.Ids>>(Path.Combine(Helper.DirectoryPath, "saved-ids.json"));
            if (savedIds != null)
            {
                CropData.savedIds = savedIds;
                foreach ( var ids in savedIds )
                {
                    CropData.Ids.MostRecentObject = Math.Max(CropData.Ids.MostRecentObject, Math.Max(ids.Value.Product, ids.Value.Seeds));
                    CropData.Ids.MostRecentCrop = Math.Max(CropData.Ids.MostRecentCrop, ids.Value.Crop);
                }
            }

            CropData.crops.Clear();
            Log.info("Registering custom crops...");
            foreach (var file in Directory.EnumerateDirectories(Path.Combine(helper.DirectoryPath, "Crops")))
            {
                try
                {
                    var data = helper.ReadJsonFile<CropData>(Path.Combine(file, "crop.json"));
                    if (data == null)
                    {
                        Log.warn("\tFailed to load crop data for " + file);
                        continue;
                    }
                    else if (!File.Exists(Path.Combine(file, "crop.png")))
                    {
                        Log.warn("\tCrop " + file + " has no crop image, skipping");
                        continue;
                    }
                    else if (!File.Exists(Path.Combine(file, "product.png")))
                    {
                        Log.warn("\tCrop " + file + " has no product image, skipping");
                        continue;
                    }
                    else if (!File.Exists(Path.Combine(file, "seeds.png")))
                    {
                        Log.warn("\tCrop " + file + " has no seeds image, skipping");
                        continue;
                    }

                    Log.info("\tCrop: " + data.Id);
                    CropData.Register(data);
                }
                catch ( Exception e )
                {
                    Log.warn("\tFailed to load crop data for " + file + ": " + e );
                    continue;
                }
            }
            helper.WriteJsonFile(Path.Combine(Helper.DirectoryPath, "saved-ids.json"), CropData.savedIds);

            var editors = ((IList<IAssetEditor>)helper.Content.GetType().GetProperty("AssetEditors", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(Helper.Content));
            editors.Add(new ContentInjector());
        }

        private void menuChanged(object sender, EventArgsClickableMenuChanged args)
        {
            var menu = args.NewMenu as ShopMenu;
            if (menu == null || menu.portraitPerson == null)
                return;

            if (menu.portraitPerson.name == "Pierre")
            {
                Log.trace("Adding crops to shop");

                var forSale = Helper.Reflection.GetPrivateValue<List<Item>>(menu, "forSale");
                var itemPriceAndStock = Helper.Reflection.GetPrivateValue<Dictionary<Item, int[]>>(menu, "itemPriceAndStock");

                var precondMeth = Helper.Reflection.GetPrivateMethod(Game1.currentLocation, "checkEventPrecondition");
                foreach (var crop in CropData.crops)
                {
                    if (!crop.Value.Seasons.Contains(Game1.currentSeason))
                        continue;
                    if (crop.Value.SeedPurchaseRequirements.Count > 0 &&
                        precondMeth.Invoke<int>(new object[] { crop.Value.GetSeedPurchaseRequirementString() }) == -1)
                        continue;
                    Item item = new StardewValley.Object(Vector2.Zero, crop.Value.GetSeedId(), int.MaxValue);
                    forSale.Add(item);
                    itemPriceAndStock.Add(item, new int[] { crop.Value.SeedPurchasePrice, int.MaxValue });
                }
            }
        }
    }
}
