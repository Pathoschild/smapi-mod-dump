/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace MoonShared.APIs
{
    public interface IDynamicGameAssetsApi
    {

        /// <inheritdoc/>
        public string GetDGAItemId(object item_);


        /// <inheritdoc/>
        public object SpawnDGAItem(string fullId, Color? color);


        /// <inheritdoc/>
        public object SpawnDGAItem(string fullId);


        public string[] ListContentPacks();


#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public string[]? GetItemsByPack(string packname);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        public string[] GetAllItems();

        /// <inheritdoc/>
        public void AddEmbeddedPack(IManifest manifest, string dir);
    }
}
