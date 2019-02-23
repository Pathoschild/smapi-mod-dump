using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;

namespace Lockpicks
{
    public class Assets : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\Monsters") || asset.AssetNameEquals("Data\\ObjectInformation") || asset.AssetNameEquals("Maps\\springobjects");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data\\ObjectInformation"))
                foreach (var item in Item.items)
                    try
                    {
                        asset.AsDictionary<int, string>().Data.Add(item.Value.internal_id, item.Value.GetData());
                    }
                    catch { }
            else if (asset.AssetNameEquals("Data\\Monsters"))
                try
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    string[] slime = data["Green Slime"].Split('/');
                    slime[6] += " " + Item.base_id + " 0.02"; //2% chance for green slimes to drop a lockpick
                    string slimeCompiled = string.Join("/", slime);
                    data["Green Slime"] = slimeCompiled;
                }
                catch { }
            else
            {
                var oldTex = asset.AsImage().Data;
                if (oldTex.Width != 4096)
                {
                    Texture2D newTex = new Texture2D(StardewValley.Game1.graphics.GraphicsDevice, oldTex.Width, System.Math.Max(oldTex.Height, 4096));
                    asset.ReplaceWith(newTex);
                    asset.AsImage().PatchImage(oldTex);
                }
                foreach (var obj in Item.items)
                    try { asset.AsImage().PatchImage(obj.Value.texture, null, new Rectangle(obj.Value.internal_id % 24 * 16, obj.Value.internal_id / 24 * 16, 16, 16)); } catch { }
            }

        }
    }

    public class Item
    {
        public static int base_id = 1919;
        public static Dictionary<string, Item> items;

        public string unique_id;
        public string displayname;
        public string description;
        public int internal_id;
        public int price;
        public int edibility;
        public Texture2D texture;

        public static void Setup()
        {
            Mod.instance.Helper.Content.AssetEditors.Add(new Assets());
            items = new Dictionary<string, Item>
            {
                ["lx.lockpick"] = new Item("lx.lockpick", "lockpick", "Lockpick", "Used to bypass locked doors.", 30, -300)
            };
        }

        public Item(string uid, string tx, string name, string desc, int price, int edibility)
        {
            unique_id = uid;
            displayname = name;
            description = desc;
            internal_id = base_id;
            this.price = price;
            this.edibility = edibility;
            this.texture = Mod.instance.Helper.Content.Load<Texture2D>("./Assets/" + tx + ".png", ContentSource.ModFolder);
        }

        public string GetData()
        {
            return $"{displayname}/{price}/{edibility}/Basic {StardewValley.Object.junkCategory}/{displayname}/{description}";
        }
    }
}
