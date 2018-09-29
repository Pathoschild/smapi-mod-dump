using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Menus;
using StardewValley.Objects;

namespace InfertileSoil
{
    public class ModEntry : Mod, IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset) {
            return asset.AssetNameEquals("Data/CraftingRecipes");
        }

        public void Edit<T>(IAssetData asset)
        {
            asset.AsDictionary<string, string>().Set((name, data) =>
            {
                if (name == "Garden Pot")
                {
                    return data.Replace("330 1 390 10 338 1", "390 10");
                }
                return data;
            });
        }

        public override void Entry(IModHelper helper)
        {
            Event e = new Event("string", 1);
            // Goals:
            // AfterDayStart, find every crop that's planted in the ground and kill it. owo
            TimeEvents.AfterDayStarted += this.KillCrops;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
        }

        public void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            if (!Game1.player.knowsRecipe("Garden Pot"))
            {
                Game1.player.craftingRecipes.Add("Garden Pot", 0);
            }
        }

        public void KillCrops(object sender, EventArgs e)
        {
            foreach (GameLocation location in Game1.locations)
            {
                for (int i = location.terrainFeatures.Count() - 1; i >= 0; --i)
                {
                    KeyValuePair<Vector2, TerrainFeature> keyValuePair = location.terrainFeatures.Pairs.ElementAt<KeyValuePair<Vector2, TerrainFeature>>(i);
                    if(location.isTileHoeDirt(keyValuePair.Key))
                    {
                        HoeDirt f = keyValuePair.Value as HoeDirt;
                        f.destroyCrop(keyValuePair.Key, false, location);
                    }
                }
            }
        }
    }
}
