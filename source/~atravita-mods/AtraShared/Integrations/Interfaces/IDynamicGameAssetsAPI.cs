/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

/*********************************************
 * The following file was copied from: https://github.com/spacechase0/StardewValleyMods/blob/develop/DynamicGameAssets/IDynamicGameAssetsApi.cs.
 *
 * The original license is as follows:
 *
 * MIT License
 *
 * Copyright (c) 2021 Chase W
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 *
 * *******************************************/

using Microsoft.Xna.Framework;

namespace AtraShared.Integrations.Interfaces;

/// <summary>
/// The API for Dynamic Game Assets.
/// </summary>
/// <remarks>Copied from https://github.com/spacechase0/StardewValleyMods/blob/develop/DynamicGameAssets/IDynamicGameAssetsApi.cs .</remarks>
public interface IDynamicGameAssetsApi
{
    /// <summary>
    /// Get the DGA item ID of this item, if it has one.
    /// </summary>
    /// <param name="item">The item to get the DGA item ID of.</param>
    /// <returns>The DGA item ID if it has one, otherwise null.</returns>
    string? GetDGAItemId(object item);

    /// <summary>
    /// Spawn a DGA item, referenced with its full ID ("mod.id/ItemId").
    /// Some items, such as crafting recipes or crops, don't have an item representation.
    /// </summary>
    /// <param name="fullId">The full ID of the item to spawn.</param>
    /// <param name="color">The color of the item.</param>
    /// <returns>The DGA item.</returns>
    object SpawnDGAItem(string fullId, Color? color);

    /// <summary>
    /// Spawn a DGA item, referenced with its full ID ("mod.id/ItemId").
    /// Some items, such as crafting recipes or crops, don't have an item representation.
    /// </summary>
    /// <param name="fullId">The full ID of the item to spawn.</param>
    /// <returns>The DGA item.</returns>
    object SpawnDGAItem(string fullId);

    /// <summary>
    /// Gets the names of all installed packs.
    /// </summary>
    /// <returns>Array of all pack names.</returns>
    string[] ListContentPacks();

    /// <summary>
    /// Gets all items provided by a pack.
    /// </summary>
    /// <param name="packname">The name of the pack.</param>
    /// <returns>Namespaced item names.</returns>
    string[]? GetItemsByPack(string packname);

    /// <summary>
    /// Gets all the items (namespaced names).
    /// </summary>
    /// <returns>A list of all items.</returns>
    string[] GetAllItems();

    /// <summary>
    /// Register a DGA pack embedded in another mod.
    /// Needs the standard DGA fields in the manifest. (See documentation.)
    /// Probably shouldn't use config-schema.json for these, because if you do it will overwrite your mod's config.json.
    /// </summary>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="dir">The absolute path to the directory of the pack.</param>
    void AddEmbeddedPack(IManifest manifest, string dir);
}