/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK
{
    /// <summary>
    /// Provides a basic API framework to edit the game's content assets.
    /// </summary>
    public abstract class AssetEditor : IAssetEditor
    {
        /// <summary>Provides access to the <see cref="IContentHelper"/> API provided by SMAPI.</summary>
        private static readonly IContentHelper contentHelper = ToolkitMod.ModHelper.Content;

        /// <summary>A collection of names of the game's content assets this asset editor can edit.</summary>
        private readonly IDictionary<string, string> gameAssetNames = new Dictionary<string, string>();

        /// <summary>
        /// Create a new instance of the <see cref="AssetEditor"/> class.
        /// </summary>
        /// <param name="assetName">The name of the content asset this asset editor can edit.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="assetName"/> is <c>null</c>.</exception>
        public AssetEditor(string assetName)
            : this(assetName != null ? new List<string>() { assetName } : null) { }

        /// <summary>
        /// Create a new instance of the <see cref="AssetEditor"/> class.
        /// </summary>
        /// <param name="assetNames">A list of names of the game's content assets this asset editor can edit.</param>
        /// /// <exception cref="ArgumentNullException">The specified <paramref name="assetNames"/> is <c>null</c>.</exception>
        public AssetEditor(IList<string> assetNames)
        {
            if (assetNames == null)
            {
                throw new ArgumentNullException(nameof(assetNames));
            }

            foreach (var assetName in assetNames)
            {
                if (!this.gameAssetNames.ContainsKey(assetName))
                {
                    this.gameAssetNames[assetName] = assetName;
                }
            }

            contentHelper.AssetEditors.Add(this);
        }

        /// <summary>
        /// Get whether this instance can edit the given asset.
        /// </summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        /// <returns><c>true</c> if the specified <paramref name="asset"/> can be edited; otherwise, <c>false</c>.</returns>
        public virtual bool CanEdit<T>(IAssetInfo asset)
        {
            return this.gameAssetNames.Values.Any(s => asset.AssetNameEquals(s));
        }

        /// <summary>
        /// Edit a matched asset.
        /// </summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public abstract void Edit<T>(IAssetData asset);

        /// <summary>
        /// Remove a game asset from the content cache so it's reloaded on the next request. This will reload 
        /// core game assets if needed, but references to the former asset will still show the previous content.
        /// </summary>
        /// <param name="assetName">
        /// The name of the game asset whose game cache you want to invalidate. If you want to invalidate
        /// the game caches for all game assets this asset editor can edit, use <see cref="string.Empty"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="assetName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The specified <paramref name="assetName"/>is not the name of an asset this asset editor can edit.</exception>
        public void RequestAssetCacheRefresh(string assetName = "")
        {
            if (assetName == null)
            {
                throw new ArgumentNullException(nameof(assetName));
            }

            if (assetName.Equals(""))
            {
                RequestAssetCacheRefresh(this.gameAssetNames.Values.ToList());
            }
            else if (gameAssetNames.ContainsKey(assetName))
            {
                contentHelper.InvalidateCache(assetName);
            }

        }

        /// <summary>
        /// Remove a list of game assets from the content cache so they are reloaded on the next request. This will reload 
        /// core game assets if needed, but references to the former asset will still show the previous content.
        /// </summary>
        /// <param name="assetNames">A list of game asset names whose game caches you want to invalidate.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="assetNames"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The specified <paramref name="assetNames"/>contains names of assets this asset editor cannot edit.</exception>
        public void RequestAssetCacheRefresh(IList<string> assetNames)
        {
            if (assetNames == null)
            {
                throw new ArgumentNullException(nameof(assetNames));
            }

            var invalidAsset = assetNames.Where(s => !gameAssetNames.ContainsKey(s)).FirstOrDefault();
            if (invalidAsset != null)
            {
                throw new ArgumentException($"Cannot request a cache refresh for game asset \"{invalidAsset}\" as it was not passed to this asset editor on creation!", 
                    nameof(assetNames));
            }

            foreach (var assetName in assetNames)
            {
                contentHelper.InvalidateCache(assetName);
            }
        }
    }
}
