using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCrops
{
    public class ContentInjector : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data\\ObjectInformation"))
                return true;
            if (asset.AssetNameEquals("Data\\Crops"))
                return true;
            if (asset.AssetNameEquals("Maps\\springobjects"))
                return true;
            if (asset.AssetNameEquals("TileSheets\\crops"))
                return true;
            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data\\ObjectInformation"))
            {
                var data = asset.AsDictionary<int, string>().Data;
                foreach (var crop_ in CropData.crops)
                {
                    var crop = crop_.Value;
                    Log.trace($"Injecting to objects: {crop.GetProductId()}: {crop.GetProductObjectInformation()}");
                    data.Add(crop.GetProductId(), crop.GetProductObjectInformation());
                    Log.trace($"Injecting to objects: {crop.GetSeedId()}: {crop.GetSeedObjectInformation()}");
                    data.Add(crop.GetSeedId(), crop.GetSeedObjectInformation());
                }
            }
            else if (asset.AssetNameEquals("Data\\Crops"))
            {
                var data = asset.AsDictionary<int, string>().Data;
                foreach (var crop_ in CropData.crops)
                {
                    var crop = crop_.Value;
                    Log.trace($"Injecting to crops: {crop.GetSeedId()}: {crop.GetCropInformation()}");
                    data.Add(crop.GetSeedId(), crop.GetCropInformation());
                }
            }
            else if (asset.AssetNameEquals("Maps\\springobjects"))
            {
                var oldTex = asset.AsImage().Data;
                Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
                asset.ReplaceWith(newTex);
                asset.AsImage().PatchImage(oldTex);

                foreach (var crop_ in CropData.crops)
                {
                    var crop = crop_.Value;
                    Log.trace($"Injecting {crop.Id} sprites");
                    asset.AsImage().PatchImage(Mod.instance.Helper.Content.Load<Texture2D>($"Crops/{crop.Id}/product.png"), null, objectRect(crop.GetProductId()));
                    if (crop.Colors != null && crop.Colors.Count > 0)
                        asset.AsImage().PatchImage(Mod.instance.Helper.Content.Load<Texture2D>($"Crops/{crop.Id}/product-color.png"), null, objectRect(crop.GetProductId() + 1));
                    asset.AsImage().PatchImage(Mod.instance.Helper.Content.Load<Texture2D>($"Crops/{crop.Id}/seeds.png"), null, objectRect(crop.GetSeedId()));
                }
            }
            else if (asset.AssetNameEquals("TileSheets\\crops"))
            {
                var oldTex = asset.AsImage().Data;
                Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
                asset.ReplaceWith(newTex);
                asset.AsImage().PatchImage(oldTex);

                foreach (var crop_ in CropData.crops)
                {
                    var crop = crop_.Value;
                    Log.trace($"Injecting {crop.Id} crop images");
                    asset.AsImage().PatchImage(Mod.instance.Helper.Content.Load<Texture2D>($"Crops/{crop.Id}/crop.png"), null, cropRect(crop.GetCropSpriteIndex()));
                }
            }
        }
        private Rectangle objectRect(int index)
        {
            return new Rectangle(index % 24 * 16, index / 24 * 16, 16, 16);
        }
        private Rectangle cropRect(int index)
        {
            return new Rectangle(index % 2 * 128, index / 2 * 32, 128, 32);
        }
    }
}
