using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectS
{
    public class ModEntry : Mod, IAssetLoader, IAssetEditor
    {
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if(asset.AssetNameEquals("Characters/Monsters/Skeleton"))
            {
                return true;
            }

            return false;
        }

       
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Characters/Monsters/Skeleton"))
            {
                return this.Helper.Content.Load<T>("assets/Skeleton.png", ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");

        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if(asset.AssetNameEquals("TileSheets/Projectiles"))
            {
                return true;
            }
            if(asset.AssetNameEquals("TileSheets/furniture"))
            {
                return true;
            }
            if(asset.AssetNameEquals("Data/Furniture"))
            {
                return true;
            }
            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if(asset.AssetNameEquals("TileSheets/Projectiles"))
            {
                Texture2D image = this.Helper.Content.Load<Texture2D>("assets/Bone.png", ContentSource.ModFolder);
                asset.AsImage().PatchImage(image, targetArea: new Microsoft.Xna.Framework.Rectangle(63, 0, 16, 16));
            }
            if (asset.AssetNameEquals("TileSheets/furniture"))
            {
                Texture2D image = this.Helper.Content.Load<Texture2D>("assets/SansStatue.png", ContentSource.ModFolder);
                asset.AsImage().PatchImage(image, targetArea: new Microsoft.Xna.Framework.Rectangle(384, 640, 16, 32));

            }

            if (asset.AssetNameEquals("Data/Furniture"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                data[1304] = "Sans Statue/decor/1 2/1 1/1/500";
            }
        }

        public override void Entry(IModHelper helper)
        {

        }

    }
}
