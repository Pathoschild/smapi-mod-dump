/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using NpcAdventure.Loader.ContentPacks;
using NpcAdventure.Loader.ContentPacks.Data;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NpcAdventure.Loader
{
    class ContentPackManager
    {
        private readonly IMonitor monitor;
        private readonly bool paranoid;
        private readonly List<ManagedContentPack> packs;

        /// <summary>
        /// Provides patches from content packs into mod's content
        /// </summary>
        /// <param name="monitor"></param>
        public ContentPackManager(IMonitor monitor, bool paranoid = false)
        {
            this.monitor = monitor;
            this.paranoid = paranoid;
            this.packs = new List<ManagedContentPack>();
        }

        /// <summary>
        /// Loads and verify content packs.
        /// </summary>
        /// <returns></returns>
        public void LoadContentPacks(IEnumerable<IContentPack> contentPacks)
        {
            this.monitor.Log("Loading content packs ...");

            // Try to load content packs and their's patches
            foreach (var pack in contentPacks)
            {
                try
                {
                    var managedPack = new ManagedContentPack(pack, this.monitor, this.paranoid);

                    managedPack.Load();
                    this.packs.Add(managedPack);
                } catch (ContentPackException e)
                {
                    this.monitor.Log($"Unable to load content pack `{pack.Manifest.Name}`:", LogLevel.Error);
                    this.monitor.Log($"   {e.Message}", LogLevel.Error);
                }
            }

            this.monitor.Log($"Loaded {this.packs.Count} content packs:", LogLevel.Info);
            this.packs.ForEach(mp => this.monitor.Log($"   {mp.Pack.Manifest.Name} {mp.Pack.Manifest.Version} by {mp.Pack.Manifest.Author}", LogLevel.Info));
            this.CheckCurrentFormat(this.packs);
            this.CheckForUsingReplacers(this.packs);
        }

        /// <summary>
        /// Apply content packs to the target
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="target">Target to be patched/param>
        /// <param name="path">Which patch (asset path)</param>
        /// <returns>True if any patch was applied on the target</returns>
        public bool Apply<TKey, TValue>(Dictionary<TKey, TValue> target, string path)
        {
            bool applied = false;

            foreach (var pack in this.packs)
            {
                applied |= pack.Apply(target, path);
            }

            return applied;
        }

        /// <summary>
        /// Check format version of available content packs 
        /// and inform user if any pack uses old format
        /// </summary>
        /// <param name="packs"></param>
        private void CheckCurrentFormat(List<ManagedContentPack> packs)
        {
            var currentFormatVersion = ManagedContentPack.SUPPORTED_FORMATS[ManagedContentPack.SUPPORTED_FORMATS.Length - 1];
            var usesOldFormat = from pack in packs
                                where pack.FormatVersion.IsOlderThan(currentFormatVersion)
                                select pack;

            if (usesOldFormat.Count() > 0)
            {
                this.monitor.Log($"Detected {usesOldFormat.Count()} content packs which use old format:", LogLevel.Info);
                this.monitor.Log($"   It's recommended to update these content packs to the new format.", LogLevel.Info);
                usesOldFormat.ToList().ForEach(p => this.monitor.Log($"   - {p.Pack.Manifest.Name} (format {p.FormatVersion})", LogLevel.Info));
            }
        }

        /// <summary>
        /// Check if given patches are safe or unsafe 
        /// (may apply replaces and overrides)
        /// </summary>
        /// <param name="packs"></param>
        private void CheckForUsingReplacers(List<ManagedContentPack> packs)
        {
            var unsafePacks = (from pack in packs
                              from patch in pack.Contents.Changes
                              where patch.Action == "Replace"
                              select pack).Distinct();

            if (unsafePacks.Count() > 0)
            {
                var loglevel = this.paranoid ? LogLevel.Warn : LogLevel.Info;
                this.monitor.Log($"Detected {unsafePacks.Count()} content packs with replacers:", loglevel);
                this.monitor.Log("   These content packs can erase and replace all contents for some target(s) in the mod and/or in other content packs.", loglevel);
                unsafePacks.ToList().ForEach(p => this.monitor.Log($"   - {p.Pack.Manifest.Name}", loglevel));
            }
        }
    }
}
