using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;

namespace NpcAdventure.Loader
{
    /// <summary>
    /// Content loader for NPC Adventure mod's contents
    /// All asset files must be in JSON format
    /// </summary>
    public class ContentLoader : IContentLoader
    {
        private readonly string modRootDirectory;
        private readonly string assetsDir;
        private readonly IMonitor monitor;
        private readonly Dictionary<string, object> assetCache;

        private IContentHelper Helper { get; }
        private string ModName { get; }

        /// <summary>
        /// Creatre new instance of custom service ContentLoader
        /// </summary>
        /// <param name="helper">SMAPI's Content helper</param>
        /// <param name="modName">Path to mod's root directory, Like path/to/mod.</param>
        /// <param name="assetsDir">Path to mod's assets dir, like `assets`. Thats mean assets dir is `path/to/mod/assets` </param>
        /// <param name="monitor"></param>
        public ContentLoader(IContentHelper helper, IContentPackHelper contentPacks, string modName, string assetsDir, string modRootDir, IMonitor monitor)
        {
            this.Helper = helper;
            this.ModName = modName;
            this.modRootDirectory = modRootDir;
            this.assetsDir = assetsDir;
            this.assetCache = new Dictionary<string, object>();
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));

            // Assign asset source manager to load and edit mod's assets
            AssetsManager assetsManager = new AssetsManager(modName, assetsDir, helper, new ContentPackProvider(modName, contentPacks, monitor), monitor);
            this.Helper.AssetLoaders.Add(assetsManager);
            this.Helper.AssetEditors.Add(assetsManager);
        }

        /// <summary>
        /// Checks for content asset exists and can be loaded
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        [Obsolete("Try to load asset and catch exception.")]
        public bool CanLoad(string assetName)
        {
            string path = $"{this.modRootDirectory}/{this.assetsDir}/{assetName}.json";

            return File.Exists(path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar));
        }

        /// <summary>
        /// Load an mod content asset
        /// </summary>
        /// <typeparam name="T">Type of asset to be loaded</typeparam>
        /// <param name="assetName">Name of asset, like `Strings/Strings` or `Dialogue/Abigail` and etc</param>
        /// <returns>Loaded content of asset</returns>
        public T Load<T>(string assetName)
        {
            string assetPath = $"{this.ModName}/{assetName}";
            // Try to get asset from our map cache
            if (this.assetCache.TryGetValue(assetPath, out object asset))
                return (T)asset;

            try
            {
                T newAsset = this.Helper.Load<T>(assetPath, ContentSource.GameContent);

                this.assetCache.Add(assetPath, (object)newAsset);

                return newAsset;
            }
            catch (ContentLoadException e)
            {
                this.monitor.Log($"Cannot load asset {assetName}", LogLevel.Error);
                throw e;
            }
        }

        /// <summary>
        /// Loads a string dictionary asset
        /// </summary>
        /// <param name="stringsAssetName">Name of asset, like `Strings/Strings` or `Dialogue/Abigail` and etc</param>
        /// <returns>Loaded dictionary of strings</returns>
        public Dictionary<string, string> LoadStrings(string stringsAssetName)
        {
            return this.Load<Dictionary<string, string>>(stringsAssetName);
        }

        /// <summary>
        /// Load one string from strings dictionary asset
        /// </summary>
        /// <param name="path">Path to string in asset with whole asset name (like `Strings/Strings:companionRecruited.yes`</param>
        /// <returns>A loaded string from asset dictionary</returns>
        public string LoadString(string path)
        {
            string[] parsedPath = path.Split(':');

            if (parsedPath.Length != 2)
                throw new ArgumentException($"Unable to parse string path: {path}");

            if (this.LoadStrings(parsedPath[0]).TryGetValue(parsedPath[1], out string str))
                return str;

            return path;
        }

        /// <summary>
        /// Load one string from strings dictionary asset with substituions.
        /// Placeholders `{%number%}` in string wil be replaced with substitution.
        /// </summary>
        /// <param name="path">Path to string in asset with whole asset name (like `Strings/Strings:companionRecruited.yes`)</param>
        /// <param name="substitutions">A substitution for replace placeholder in string</param>
        /// <returns>A loaded string from asset dictionary</returns>
        public string LoadString(string path, params object[] substitutions)
        {
            string str = this.LoadString(path);

            return string.Format(str, substitutions);
        }

        /// <summary>
        /// Invalidate mod's cached assets
        /// </summary>
        public void InvalidateCache()
        {
            foreach (string assetPath in this.assetCache.Keys)
                this.Helper.InvalidateCache(assetPath); // Remove file from SMAPI's file cache

            // And clear our assets map
            this.assetCache.Clear();
        }
    }
}
