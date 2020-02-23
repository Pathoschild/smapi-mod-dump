using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using SubterranianOverhaul.Crops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubterranianOverhaul
{
    class VoidshroomDataInjector : IAssetEditor
    {
        private IMonitor monitor;

        public VoidshroomDataInjector(IMonitor monitor)
        {
            this.monitor = monitor;
            if (this.monitor != null)
            {
                this.monitor.Log("Data Injector initialized");
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return (asset.AssetNameEquals("Maps\\springobjects") || 
                    asset.AssetNameEquals("Data\\ObjectInformation") || 
                    asset.AssetNameEquals("Data\\Crops") ||
                    asset.AssetNameEquals("TileSheets\\crops"));
        }

        public void Edit<T>(IAssetData asset)
        {
            VoidshroomSpore.setIndex(); //get an item index for voidshroom spores if one isn't already set.
            CaveCarrotSeed.setIndex();
            CaveCarrot.setIndex();
            CaveCarrot.setCropIndex();
            if (asset.AssetNameEquals("Maps\\springobjects"))
            {
                IAssetDataForImage editor = asset.AsImage();
                Texture2D data = editor.Data;
                Texture2D texture2D = new Texture2D(Game1.graphics.GraphicsDevice, data.Width, Math.Max(data.Height, 4096));
                editor.ReplaceWith(texture2D);
                editor.PatchImage(data, new Rectangle?(), new Rectangle?(), PatchMode.Replace);
                try
                {
                    editor.PatchImage(TextureSet.voidShroomSpore, new Rectangle?(), new Rectangle?(this.objectRect(VoidshroomSpore.getIndex())), PatchMode.Replace);
                    editor.PatchImage(TextureSet.caveCarrotSeed, new Rectangle?(), new Rectangle?(this.objectRect(CaveCarrotSeed.getIndex())), PatchMode.Replace);
                }
                catch (Exception)
                {
                }
            } else if (asset.AssetNameEquals("Data\\ObjectInformation"))
            {
                IAssetDataForDictionary<int, string> editor = asset.AsDictionary<int, string>();

                IDictionary<int, string> data = editor.Data;
                if (!data.ContainsKey(VoidshroomSpore.getIndex()))
                {
                    int voidShroomSporeIndex = VoidshroomSpore.getIndex();
                    String voidShroomSpore = VoidshroomSpore.getObjectData();
                    this.log("Add voidshroom spore object data: "+ voidShroomSporeIndex + ": " + voidShroomSpore);
                    data.Add(voidShroomSporeIndex, voidShroomSpore);
                    
                }

                if (!data.ContainsKey(CaveCarrotSeed.getIndex()))
                {
                    int caveCarrotSeedIndex = CaveCarrotSeed.getIndex();
                    String caveCarrotObject = CaveCarrotSeed.getObjectData();
                    this.log("Add cave carrot seed object data: "+ caveCarrotSeedIndex + ": " + caveCarrotObject);
                    data.Add(caveCarrotSeedIndex, caveCarrotObject);
                }

                if (!data.ContainsKey(CaveCarrotFlower.getIndex()))
                {
                    int caveCarrotFlowerIndex = CaveCarrotFlower.getIndex();
                    String caveCarrotFlowerObject = CaveCarrotFlower.getObjectData();
                    this.log("Add cave carrot flower 'seed' data: "+ caveCarrotFlowerIndex+": " + caveCarrotFlowerObject);
                    data.Add(caveCarrotFlowerIndex, caveCarrotFlowerObject);
                }
            } else if (asset.AssetNameEquals("Data\\Crops"))
            {
                IAssetDataForDictionary<int, string> editor = asset.AsDictionary<int, string>();
                IDictionary<int, string> data = editor.Data;

                int seedIndex = CaveCarrot.getIndex();
                this.log("seedIndex is: "+seedIndex);
                if (!data.ContainsKey(seedIndex)) {
                    String cropData = CaveCarrot.getCropData();
                    this.monitor.Log("Loading crop data: "+cropData);
                    data.Add(CaveCarrot.getIndex(), cropData);
                }

                int caveCarrotFlowerIndex = CaveCarrotFlower.getIndex();
                this.log("seedIndex is: " + caveCarrotFlowerIndex);
                if (!data.ContainsKey(caveCarrotFlowerIndex))
                {
                    String cropData = CaveCarrotFlower.getCropData();
                    this.monitor.Log("Loading crop data: " + cropData);
                    data.Add(caveCarrotFlowerIndex, cropData);
                }
            } else if (asset.AssetNameEquals("TileSheets\\crops"))
            {
                IAssetDataForImage editor = asset.AsImage();
                Texture2D data = editor.Data;
                Texture2D texture2D = new Texture2D(Game1.graphics.GraphicsDevice, data.Width, Math.Max(data.Height, 4096));
                editor.ReplaceWith(texture2D);
                editor.PatchImage(data, new Rectangle?(), new Rectangle?(), PatchMode.Replace);
                try
                {
                    int index = CaveCarrot.getCropIndex();
                    this.monitor.Log("Loading cave carrot crop texture.  Crop index: " + index);
                    editor.PatchImage(TextureSet.caveCarrotCrop, new Rectangle?(), new Rectangle?(this.cropRect(index)), PatchMode.Replace);

                    index = CaveCarrotFlower.getCropIndex();
                    this.monitor.Log("Loading cave carrot flower crop texture.  Crop index: " + index);
                    editor.PatchImage(TextureSet.caveCarrotFlowerCrop, new Rectangle?(), new Rectangle?(this.cropRect(index)), PatchMode.Replace);
                }
                catch (Exception)
                {
                }
            }
        }

        private Rectangle objectRect(int index)
        {   
            return getRectangle(index, 24, 16, 16);
        }

        private Rectangle cropRect(int index)
        {   
            return getRectangle(index, 2, 32, 128);
            
        }

        private Rectangle getRectangle(int index, int itemsPerRow, int pixelsVertical, int pixelsHorizontal)
        {
            return new Rectangle(index % itemsPerRow * pixelsHorizontal, index / itemsPerRow * pixelsVertical, pixelsHorizontal, pixelsVertical);
        }

        private void log(string message)
        {
            if(this.monitor != null)
            {
                this.monitor.Log(message);
            }
        }
    }
}
