/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kakashigr/stardew-radioactivetools
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace RadioactiveTools.Framework {
    public class AssetEditor : IAssetEditor {
        private readonly string sprinklerName = ModEntry.ModHelper.Translation.Get("radioactiveSprinkler.name");
        private readonly string sprinklerDesc = ModEntry.ModHelper.Translation.Get("radioactiveSprinkler.description");
        //private readonly string rfertName = ModEntry.ModHelper.Translation.Get("radioactiveFertilizer.name");
        //private readonly string rfertDesc = ModEntry.ModHelper.Translation.Get("radioactiveFertilizer.description");

        public bool CanEdit<T>(IAssetInfo asset) {
            bool canEdit =
                   asset.AssetNameEquals("Maps/springobjects")
                || asset.AssetNameEquals("Maos/ObjectInformation")
                || asset.AssetNameEquals("Data/ObjectInformation")
                || asset.AssetNameEquals("Data/CraftingRecipes")
                || asset.AssetNameEquals("TileSheets/tools")
                || asset.AssetNameEquals("TileSheets/weapons");
            return canEdit;
        }
        //  "918": "Hyper Speed-Gro/70/-300/Basic -19/Hyper Speed-Gro/Greatly stimulates leaf production. Guaranteed to increase growth rate by at least 33%. Mix into tilled soil.",
        //  "466": "Deluxe Speed-Gro/40/-300/Basic -19/Deluxe Speed-Gro/Stimulates leaf production. Guaranteed to increase growth rate by at least 25%. Mix into tilled soil.",



        public void Edit<T>(IAssetData asset) {

            if (asset.AssetNameEquals("Maps/springobjects")) {
                Texture2D sprinkler = ModEntry.ModHelper.Content.Load<Texture2D>("assets/radioactiveSprinkler.png");
                //Texture2D rfertimg = ModEntry.ModHelper.Content.Load<Texture2D>("assets/radioactiveFertilizer.png");
                Texture2D old = asset.AsImage().Data;
                asset.ReplaceWith(new Texture2D(Game1.graphics.GraphicsDevice, old.Width, System.Math.Max(old.Height, 1200 / 24 * 16)));
                asset.AsImage().PatchImage(old);
                asset.AsImage().PatchImage(sprinkler, targetArea: this.GetRectangle(RadioactiveSprinklerItem.INDEX));
                //asset.AsImage().PatchImage(rfertimg, targetArea: this.GetRectangle(RadioactiveFertilizerItem.INDEX));
            } else if (asset.AssetNameEquals("Data/ObjectInformation")) {
                asset.AsDictionary<int, string>().Data.Add(RadioactiveSprinklerItem.INDEX, $"{this.sprinklerName}/{RadioactiveSprinklerItem.PRICE}/{RadioactiveSprinklerItem.EDIBILITY}/{RadioactiveSprinklerItem.TYPE} {RadioactiveSprinklerItem.CATEGORY}/{this.sprinklerName}/{this.sprinklerDesc}");
                //asset.AsDictionary<int, string>().Data.Add(RadioactiveFertilizerItem.INDEX, $"{this.rfertName}/{RadioactiveFertilizerItem.PRICE}/{RadioactiveFertilizerItem.EDIBILITY}/{RadioactiveFertilizerItem.TYPE} {RadioactiveFertilizerItem.CATEGORY}/{this.rfertName}/{this.rfertDesc}");
            } else if (asset.AssetNameEquals("Data/CraftingRecipes")) {
                IAssetDataForDictionary<string, string> oldDict = asset.AsDictionary<string, string>();
                Dictionary<string, string> newDict = new Dictionary<string, string>();
                // somehow the Dictionary maintains ordering, so reconstruct it with new sprinkler recipe immediately after radioactive
                foreach (string key in oldDict.Data.Keys) {
                    newDict.Add(key, oldDict.Data[key]);
                    if (key.Equals("Iridium Sprinkler")) {
                        if (asset.Locale != "en") {
                            newDict.Add("Radioactive Sprinkler", $"910 2 787 2/Home/{RadioactiveSprinklerItem.INDEX}/false/Farming {RadioactiveSprinklerItem.CRAFTING_LEVEL}/{this.sprinklerName}");
                            //newDict.Add("Radioactive Fertilizer", $"910 2 787 2/Home/{RadioactiveFertilizerItem.INDEX}/false/Farming {RadioactiveFertilizerItem.CRAFTING_LEVEL}/{this.rfertName}");
                        } else {
                            newDict.Add("Radioactive Sprinkler", $"910 2 787 2/Home/{RadioactiveSprinklerItem.INDEX}/false/Farming {RadioactiveSprinklerItem.CRAFTING_LEVEL}");
                            //newDict.Add("Radioactive Fertilizer", $"910 2 787 2/Home/{RadioactiveFertilizerItem.INDEX}/false/Farming {RadioactiveFertilizerItem.CRAFTING_LEVEL}");
                        }
                    }
                }
                asset.AsDictionary<string, string>().Data.Clear();
                foreach (string key in newDict.Keys) {
                    asset.AsDictionary<string, string>().Data.Add(key, newDict[key]);
                }
            }
            else if (asset.AssetNameEquals("TileSheets\\tools")) {
                asset.AsImage().PatchImage(ModEntry.ToolsTexture, null, null, PatchMode.Overlay);
            }
            else if (asset.AssetNameEquals("TileSheets\\weapons")) {
                //asset.AsImage().PatchImage(ModEntry.WeaponsTexture, null, null, PatchMode.Overlay);
            }
        }

        public Rectangle GetRectangle(int id) {
            int x = (id % 24) * 16;
            int y = (id / 24) * 16;
            return new Rectangle(x, y, 16, 16);
        }
    }
}
