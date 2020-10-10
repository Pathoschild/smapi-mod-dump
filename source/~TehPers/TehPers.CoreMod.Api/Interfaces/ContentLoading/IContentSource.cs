/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Content;

namespace TehPers.CoreMod.Api.ContentLoading {
    public interface IContentSource {
        /// <summary>The absolute path to this content source.</summary>
        string Path { get; }

        /// <summary>Loads an asset from this content source.</summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="path">The path to the asset relative to this content source.</param>
        /// <returns>The loaded asset.</returns>
        /// <exception cref="ContentLoadException">The asset failed to load.</exception>
        T Load<T>(string path);
    }
}