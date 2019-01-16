using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace CustomCrops
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;

            helper.Events.Display.MenuChanged += OnMenuChanged;

            var savedIds = helper.Data.ReadJsonFile<Dictionary<string, CropData.Ids>>("saved-ids.json");
            if (savedIds != null)
            {
                CropData.savedIds = savedIds;
                foreach (var ids in savedIds)
                {
                    CropData.Ids.MostRecentObject = Math.Max(CropData.Ids.MostRecentObject, Math.Max(ids.Value.Product, ids.Value.Seeds));
                    CropData.Ids.MostRecentCrop = Math.Max(CropData.Ids.MostRecentCrop, ids.Value.Crop);
                }
            }

            CropData.crops.Clear();
            Log.info("Registering custom crops...");
            DirectoryInfo cropsFolder = new DirectoryInfo(Path.Combine(helper.DirectoryPath, "Crops"));
            if (!cropsFolder.Exists)
                cropsFolder.Create();
            foreach (var folderPath in Directory.EnumerateDirectories(cropsFolder.FullName))
            {
                IContentPack contentPack = this.Helper.ContentPacks.CreateFake(folderPath);
                try
                {
                    var data = contentPack.ReadJsonFile<CropData>("crop.json");
                    if (data == null)
                    {
                        Log.warn($"\tFailed to load crop data for {folderPath}");
                        continue;
                    }
                    else if (!File.Exists(Path.Combine(folderPath, "crop.png")))
                    {
                        Log.warn($"\tCrop {folderPath} has no crop image, skipping");
                        continue;
                    }
                    else if (!File.Exists(Path.Combine(folderPath, "product.png")))
                    {
                        Log.warn($"\tCrop {folderPath} has no product image, skipping");
                        continue;
                    }
                    else if (!File.Exists(Path.Combine(folderPath, "seeds.png")))
                    {
                        Log.warn($"\tCrop {folderPath} has no seeds image, skipping");
                        continue;
                    }

                    Log.info($"\tCrop: {data.Id}");
                    CropData.Register(data);
                }
                catch (Exception e)
                {
                    Log.warn($"\tFailed to load crop data for {folderPath}: {e}");
                    continue;
                }
            }
            helper.Data.WriteJsonFile("saved-ids.json", CropData.savedIds);
            helper.Content.AssetEditors.Add(new ContentInjector());
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            var menu = e.NewMenu as ShopMenu;
            if (menu?.portraitPerson == null)
                return;

            if (menu.portraitPerson.Name == "Pierre")
            {
                Log.trace("Adding crops to shop");

                List<Item> forSale = Helper.Reflection.GetField<List<Item>>(menu, "forSale").GetValue();
                Dictionary<Item, int[]> itemPriceAndStock = Helper.Reflection.GetField<Dictionary<Item, int[]>>(menu, "itemPriceAndStock").GetValue();

                IReflectedMethod precondMeth = Helper.Reflection.GetMethod(Game1.currentLocation, "checkEventPrecondition");
                foreach (KeyValuePair<string, CropData> crop in CropData.crops)
                {
                    if (!crop.Value.Seasons.Contains(Game1.currentSeason))
                        continue;
                    if (crop.Value.SeedPurchaseRequirements.Count > 0 && precondMeth.Invoke<int>(new object[] { crop.Value.GetSeedPurchaseRequirementString() }) == -1)
                        continue;
                    Item item = new StardewValley.Object(Vector2.Zero, crop.Value.GetSeedId(), int.MaxValue);
                    forSale.Add(item);
                    itemPriceAndStock.Add(item, new int[] { crop.Value.SeedPurchasePrice, int.MaxValue });
                }
            }
        }
    }
}
