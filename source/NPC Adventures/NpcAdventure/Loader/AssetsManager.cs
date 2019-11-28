using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace NpcAdventure.Loader
{
    /// <summary>
    /// Mod's assets loader and editor.
    /// </summary>
    internal class AssetsManager : IAssetLoader, IAssetEditor
    {
        private readonly string modName;
        private readonly string modAssetDir;
        private readonly IMonitor monitor;

        public AssetsManager(string modName, string modAssetDir, IContentHelper helper, ContentPackProvider contentPacks, IMonitor monitor)
        {
            if (string.IsNullOrEmpty(modAssetDir))
            {
                throw new ArgumentException("Mod assets directory must be set!", nameof(modAssetDir));
            }

            this.modName = modName;
            this.modAssetDir = modAssetDir;
            this.Helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.ContentPacks = contentPacks;
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }

        public IContentHelper Helper { get; }
        public ContentPackProvider ContentPacks { get; }

        /// <summary>
        /// Can we load localisation for mod's asset?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            // Can we load localisation only for localised assets
            if (this.ContentPacks.CanEdit<T>(asset) || (asset.AssetName.StartsWith(this.modName) && !string.IsNullOrEmpty(asset.Locale)))
                return true;

            return false;
        }

        /// <summary>
        /// Is this asset mod's asset and we can load it?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (!asset.AssetName.StartsWith(this.modName))
                return false;

            return true;
        }

        /// <summary>
        /// Patch Mod's asset contents with localised content
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        public void Edit<T>(IAssetData asset)
        {
            string locale = asset.Locale;
            string fileName = $"{asset.AssetName.Replace(this.modName, this.modAssetDir)}.{locale}.json"; // Localised filename like Dialogue/Abigail.de-De.json for German localisation

            // Load translation for this asset if it's translated to current locale
            if (!string.IsNullOrEmpty(asset.Locale))
            {
                try
                {
                    this.monitor.VerboseLog($"Trying to load localised file {fileName} for {asset.AssetName}, locale {locale}");

                    var strings = asset.AsDictionary<string, string>().Data;
                    var localised = this.Helper.Load<Dictionary<string, string>>(fileName, ContentSource.ModFolder);

                    // Patch asset's content with translated contend and keep non-translated parts
                    foreach (var pair in localised)
                    {
                        strings[pair.Key] = pair.Value;
                    }
                }
                catch (ContentLoadException)
                {
                    this.monitor.Log($"No localization file {fileName} for {asset.AssetName}, locale {locale}");
                }
            }

            this.ContentPacks.Edit<T>(asset); // Apply patches from content packs
        }

        /// <summary>
        /// Load mod's asset contents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns></returns>
        public T Load<T>(IAssetInfo asset)
        {
            string fileName = $"{asset.AssetName.Replace(this.modName, this.modAssetDir)}.json";

            this.monitor.Log($"Trying to load asset {asset.AssetName} from file {fileName}");

            // Try load asset content cover from content pack (action load in patch) and replace original asset 
            // with it or load new content asset into mod. if original not exist in mod
            if (this.ContentPacks.CanLoad<T>(asset))
            {
                return this.ContentPacks.Load<T>(asset);
            }

            // Load original mod asset
            return this.Helper.Load<T>(fileName, ContentSource.ModFolder);
        }
    }
}
