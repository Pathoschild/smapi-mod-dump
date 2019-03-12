using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using bwdyworks.Registry;

namespace bwdyworks
{
    internal class AssetEditor : IAssetEditor
    {
        internal List<MonsterLootEntry> MonsterLoot = new List<MonsterLootEntry>();

        public bool CanEdit<T>(IAssetInfo asset)
        {
            var assetsToEdit = new string[]
            {
                "Data\\Monsters",
                "Maps\\springobjects",
                "Data\\ObjectInformation"
            };
            string assetName = asset.AssetName;
            return Array.Exists(assetsToEdit, delegate (string s) { return s.Equals(assetName); });
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data\\Monsters"))
            {
                Modworks.Log.Trace("editing monsters");
                var data = asset.AsDictionary<string, string>().Data;
                foreach (MonsterLootEntry mle in MonsterLoot)
                {
                    AddMonsterLootEntry(data, mle);
                }
            }
            else if (asset.AssetNameEquals("Maps\\springobjects"))
            {
                if (!ItemRegistry.Loaded) ItemRegistry.Load();
                Modworks.Log.Trace("editing springobjects");
                var oldTex = asset.AsImage().Data;
                if (oldTex.Width < 4096) //only if JsonAssets didn't beat us to it
                {
                    Texture2D newTex = new Texture2D(StardewValley.Game1.graphics.GraphicsDevice, oldTex.Width, System.Math.Max(oldTex.Height, 4096));
                    asset.ReplaceWith(newTex);
                    asset.AsImage().PatchImage(oldTex);
                }
                foreach (int i in ItemRegistry.LocalRegistry.RegisteredItemIds.Values)
                {
                    BasicItemEntry bie = ItemRegistry.RegisteredItems[i];
                    try
                    {
                        asset.AsImage().PatchImage(bie.LoadTexture(), null, new Microsoft.Xna.Framework.Rectangle(bie.IntegerId % 24 * 16, bie.IntegerId / 24 * 16, 16, 16));
                        Modworks.Log.Trace("Installed texture for custom item: " + bie.GlobalId);
                    }
                    catch (Exception e)
                    {
                        Modworks.Log.Error("Failed to install texture for custom item: " + bie.GlobalId + "\n" + e.Message);
                    }
                }
            }
            else if (asset.AssetNameEquals("Data\\ObjectInformation"))
            {
                if (!ItemRegistry.Loaded) ItemRegistry.Load();
                Modworks.Log.Trace("editing objectinformation");
                foreach (int i in ItemRegistry.LocalRegistry.RegisteredItemIds.Values)
                {
                    BasicItemEntry bie = ItemRegistry.RegisteredItems[i];
                    try
                    {
                        asset.AsDictionary<int, string>().Data.Add(bie.IntegerId, bie.Compile());
                        Modworks.Log.Trace("Installed data for custom item: " + bie.GlobalId);
                    }
                    catch (Exception e)
                    {
                        Modworks.Log.Error("Failed to install data for custom item: " + bie.GlobalId + "\n" + e.Message);
                    }
                }
            }
        }

        private bool AddMonsterLootEntry(IDictionary<string, string> monsterdata, MonsterLootEntry entry)
        {
            if (monsterdata == null)
            {
                Modworks.Log.Error("Attempted to add monster data to null asset!");
                return false;
            }
            if (!monsterdata.ContainsKey(entry.MonsterID))
            {
                Modworks.Log.Error("Failed to add monster loot:\nCould not identify a monster with id: " + entry.MonsterID);
                return false;
            }
            try
            {
                int? integerId = Modworks.Items.GetModItemId(entry.Module, entry.ItemID);
                if (!integerId.HasValue) return false;
                string[] monster = monsterdata[entry.MonsterID].Split('/');
                monster[6] += " " + integerId + " " + entry.Weight.ToString().TrimStart('0');
                string monsterCompiled = string.Join("/", monster);
                monsterdata[entry.MonsterID] = monsterCompiled;
                Modworks.Log.Debug(entry.Module + " added monster loot :: " + monsterCompiled);
                return true;
            }
            catch (Exception e)
            {
                Modworks.Log.Error("Error while adding monster loot entry: " + e.Message);
                return false;
            }
        }
    }
}
