using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;

namespace NpcAdventure.Loader
{
    public class ContentLoader : IContentLoader
    {
        private readonly string root;
        private readonly string assetsDir;
        private readonly IMonitor monitor;
        private readonly Dictionary<string, object> assetsMap;

        private IContentHelper Helper { get; }

        public ContentLoader(IContentHelper helper, string root, string assetsDir, IMonitor monitor)
        {
            this.Helper = helper;
            this.root = root;
            this.assetsDir = assetsDir;
            this.assetsMap = new Dictionary<string, object>();
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }

        public bool CanLoad(string assetName)
        {
            string path = $"{this.root}/{this.assetsDir}/{assetName}.json";

            return File.Exists(path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar));
        }

        public T Load<T>(string assetName)
        {
            if (this.assetsMap.TryGetValue(assetName, out object asset))
                return (T)asset;

            try
            {
                T newAsset = this.Helper.Load<T>($"{this.assetsDir}/{assetName}.json");

                this.assetsMap.Add(assetName, (object)newAsset);
                this.monitor.Log($"Loaded asset {assetName}");

                return newAsset;
            }
            catch (ContentLoadException e)
            {
                this.monitor.Log($"Cannot load asset {assetName}", LogLevel.Error);
                throw e;
            }
        }

        public Dictionary<string, string> LoadStrings(string stringsAssetName)
        {
            return this.Load<Dictionary<string, string>>(stringsAssetName);
        }

        public string LoadString(string path)
        {
            string[] parsedPath = path.Split(':');

            if (parsedPath.Length != 2)
                throw new ArgumentException($"Unable to parse string path: {path}");

            if (this.LoadStrings(parsedPath[0]).TryGetValue(parsedPath[1], out string str))
                return str;

            return path;
        }

        public string LoadString(string path, params object[] substitutions)
        {
            string str = this.LoadString(path);

            return string.Format(str, substitutions);
        }

        public void InvalidateCache()
        {
            foreach (string assetName in this.assetsMap.Keys)
                this.Helper.InvalidateCache($"{this.assetsDir}/{assetName}.json");

            this.assetsMap.Clear();
        }
    }
}
