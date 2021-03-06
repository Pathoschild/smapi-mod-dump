/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/UnkLegacy/QiSprinklers
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace QiSprinklers.Framework
{
    public class AssetEditor : IAssetEditor
    {

        private readonly string sprinklerName = QiSprinklers.ModHelper.Translation.Get("qiSprinkler.name");
        private readonly string sprinklerDesc = QiSprinklers.ModHelper.Translation.Get("qiSprinkler.description");

        public bool CanEdit<T>(IAssetInfo asset)
        {
            bool canEdit =
                   asset.AssetNameEquals("Maps/springobjects")
                || asset.AssetNameEquals("Data/ObjectInformation")
                || asset.AssetNameEquals("Data/CraftingRecipes");
            return canEdit;
        }

        public void Edit<T>(IAssetData asset)
        {

            if (asset.AssetNameEquals("Maps/springobjects"))
            {
                Texture2D sprinkler = QiSprinklers.ModHelper.Content.Load<Texture2D>("Assets/qiSprinkler.png");
                Texture2D old = asset.AsImage().Data;
                asset.ReplaceWith(new Texture2D(Game1.graphics.GraphicsDevice, old.Width, System.Math.Max(old.Height, 1200 / 24 * 16)));
                asset.AsImage().PatchImage(old);
                asset.AsImage().PatchImage(sprinkler, targetArea: this.GetRectangle(QiSprinklerItem.INDEX));
            }
            else if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                asset.AsDictionary<int, string>().Data.Add(QiSprinklerItem.INDEX, $"{sprinklerName}/{QiSprinklerItem.PRICE}/{QiSprinklerItem.EDIBILITY}/{QiSprinklerItem.TYPE} {QiSprinklerItem.CATEGORY}/{sprinklerName}/{sprinklerDesc}");
            }
            else if (asset.AssetNameEquals("Data/CraftingRecipes"))
            {
                IAssetDataForDictionary<string, string> oldDict = asset.AsDictionary<string, string>();
                Dictionary<string, string> newDict = new Dictionary<string, string>();
                // somehow the Dictionary maintains ordering, so reconstruct it with new sprinkler recipe immediately after Qi
                foreach (string key in oldDict.Data.Keys)
                {
                    newDict.Add(key, oldDict.Data[key]);
                    if (key.Equals("Iridium Sprinkler"))
                    {
                        if (asset.Locale != "en")
                            newDict.Add("Qi Sprinkler", $"645 1 913 1 915 1/Home/{QiSprinklerItem.INDEX}/false/Farming {QiSprinklerItem.CRAFTING_LEVEL}/{sprinklerName}");
                        else
                            newDict.Add("Qi Sprinkler", $"645 1 913 1 915 1/Home/{QiSprinklerItem.INDEX}/false/Farming {QiSprinklerItem.CRAFTING_LEVEL}");
                    }
                }
                asset.AsDictionary<string, string>().Data.Clear();
                foreach (string key in newDict.Keys)
                {
                    asset.AsDictionary<string, string>().Data.Add(key, newDict[key]);
                }
            }
        }

        public Rectangle GetRectangle(int id)
        {
            int x = (id % 24) * 16;
            int y = (id / 24) * 16;
            return new Rectangle(x, y, 16, 16);
        }
    }
}