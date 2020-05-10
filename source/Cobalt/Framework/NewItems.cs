using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace Cobalt.Framework
{
    internal class ItemInjector : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Maps\\springobjects"))
                return true;
            if (asset.AssetNameEquals("Data\\ObjectInformation"))
                return true;
            if (asset.AssetNameEquals("Data\\CraftingRecipes"))
                return true;
            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Maps\\springobjects"))
            {
                // Make the original image larger - 1000 should be good? My stuff starts at 900.
                var oldTex = asset.AsImage().Data;
                Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 1000 / 24 * 16));
                asset.ReplaceWith(newTex);
                asset.AsImage().PatchImage(oldTex);

                asset.AsImage().PatchImage(ModEntry.instance.Helper.Content.Load<Texture2D>("assets/cobalt-bar.png"), null, imageRect(CobaltBarItem.INDEX));
                asset.AsImage().PatchImage(ModEntry.instance.Helper.Content.Load<Texture2D>("assets/cobalt-sprinkler.png"), null, imageRect(CobaltSprinklerObject.INDEX));
            }
            else if (asset.AssetNameEquals("Data\\ObjectInformation"))
            {
                asset.AsDictionary<int, string>().Data.Add(CobaltBarItem.INDEX, $"{CobaltBarItem.NAME}/{CobaltBarItem.PRICE}/{CobaltBarItem.EDIBILITY}/{CobaltBarItem.TYPE} {CobaltBarItem.CATEGORY}/{CobaltBarItem.NAME}/{CobaltBarItem.DESCRIPTION}");
                asset.AsDictionary<int, string>().Data.Add(CobaltSprinklerObject.INDEX, $"{CobaltSprinklerObject.NAME}/{CobaltSprinklerObject.PRICE}/{CobaltSprinklerObject.EDIBILITY}/{CobaltSprinklerObject.TYPE} {CobaltSprinklerObject.CATEGORY}/{CobaltSprinklerObject.NAME}/{CobaltSprinklerObject.DESCRIPTION}");
            }
            else if (asset.AssetNameEquals("Data\\CraftingRecipes"))
            {
                asset.AsDictionary<string, string>().Data.Add("Cobalt Bar", $"74 1 337 10/Home/{CobaltBarItem.INDEX} 3/false/null");
                asset.AsDictionary<string, string>().Data.Add("Cobalt Sprinkler", $"645 1 {CobaltBarItem.INDEX} 1/Home/{CobaltSprinklerObject.INDEX} 1/false/null");
            }
        }

        private Rectangle imageRect(int index)
        {
            return new Rectangle(index % 24 * 16, index / 24 * 16, 16, 16);
        }
    }
}
