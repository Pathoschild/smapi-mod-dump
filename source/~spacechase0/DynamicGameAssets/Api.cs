/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using DynamicGameAssets.Game;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace DynamicGameAssets
{
    public class Api : IDynamicGameAssetsApi
    {
        /// <inheritdoc/>
        public string GetDGAItemId(object item_)
        {
            if (item_ is IDGAItem item)
                return item.FullId;
            else
                return null;
        }

        /// <inheritdoc/>
        public object SpawnDGAItem(string fullId, Color? color)
        {
            object spawnDgaItem = this.SpawnDGAItem(fullId);
            if(color.HasValue && spawnDgaItem is CustomObject obj)
            {
                obj.ObjectColor = color;
            }
            return spawnDgaItem;
        }

        /// <inheritdoc/>
        public object SpawnDGAItem(string fullId)
        {
            return Mod.Find(fullId)?.ToItem();
        }

        public string[] ListContentPacks()
        {
            return Mod.contentPacks.Keys.ToArray();
        }

        public string[]? GetItemsByPack(string packname)
        {
            if (Mod.contentPacks.TryGetValue(packname, out var pack))
                return pack.items.Where((kvp) => kvp.Value.Enabled).Select((kvp) => packname+"/"+kvp.Key).ToArray();
            return null;
        }

        public string[] GetAllItems()
        {
            List<string> ret = new(50);
            foreach (var(packname, pack) in Mod.contentPacks)
            {
                ret.AddRange(pack.items.Where((kvp) => kvp.Value.Enabled).Select((kvp) => packname + "/" + kvp.Key));
            }
            return ret.ToArray();
        }

        /// <inheritdoc/>
        public void AddEmbeddedPack(IManifest manifest, string dir)
        {
            Mod.AddEmbeddedContentPack(manifest, dir);
        }
    }
}
