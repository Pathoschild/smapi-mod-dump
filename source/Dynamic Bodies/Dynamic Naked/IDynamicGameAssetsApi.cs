/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;

namespace DynamicBodies
{
    public interface IDynamicGameAssetsApi
    {

        /// <summary>
        /// Get the DGA item ID of this item, if it has one.
        /// </summary>
        /// <param name="item">The item to get the DGA item ID of.</param>
        /// <returns>The DGA item ID if it has one, otherwise null.</returns>
        public string GetDGAItemId(object item_);

        /// <summary>Returns the DGA item</summary>
        /// <param name="fullId">IDGAItem ID</param>
        /// <param name="color">Color of it</param>
        public object SpawnDGAItem(string fullId, Color? color);

        /// <summary>Returns the DGA item</summary>
        /// <param name="fullId">IDGAItem ID</param>
        public object SpawnDGAItem(string fullId);

        /// <summary>Returns the DGA content pack IDs</summary>
        public string[] ListContentPacks();

        /// <summary>Returns the DGA content pack IDGAItem IDs</summary>
        /// <param name="packname">Content Pack ID</param>
        public string[]? GetItemsByPack(string packname);

        /// <summary>Returns all IDGAItem IDs</summary>
        public string[] GetAllItems();

        /// <summary>
        /// Register a DGA pack embedded in another mod.
        /// Needs the standard DGA fields in the manifest. (See documentation.)
        /// Probably shouldn't use config-schema.json for these, because if you do it will overwrite your mod's config.json.
        /// </summary>
        /// <param name="manifest">The mod manifest.</param>
        /// <param name="dir">The absolute path to the directory of the pack.</param>
        public void AddEmbeddedPack(IManifest manifest, string dir);
    }
}
