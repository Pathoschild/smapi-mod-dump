using NpcAdventure.Model;
using StardewModdingAPI;
using System.Collections.Generic;

namespace NpcAdventure.Loader.ContentPacks
{
    /// <summary>
    /// Asset patch for mod's original asset
    /// </summary>
    internal class AssetPatch
    {
        private readonly ContentPackData.DataChanges meta;
        private readonly ManagedContentPack contentPack;

        public AssetPatch(ContentPackData.DataChanges meta, ManagedContentPack contentPack, string logName)
        {
            this.meta = meta;
            this.contentPack = contentPack;
            this.LogName = logName;
        }

        public string Action { get => this.meta.Action; }
        public string Target { get => this.meta.Target; }
        public string LogName { get; private set; }
        public string FromFile { get => this.meta.FromFile; }
        public string Locale { get => this.meta.Locale; }

        /// <summary>
        /// Load content pack patch data
        /// </summary>
        /// <returns></returns>
        public T LoadData<T>()
        {
            return this.contentPack.Load<T>(this.meta.FromFile);
        }

        public bool FromAssetExists()
        {
            return !string.IsNullOrEmpty(this.meta.FromFile) && this.contentPack.HasFile(this.meta.FromFile);
        }
    }
}