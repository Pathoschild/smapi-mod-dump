/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using NpcAdventure.Loader.ContentPacks.Data;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace NpcAdventure.Loader.ContentPacks.Provider
{
    class LegacyDataProvider : IDataProvider
    {
        private readonly bool paranoid;

        public LegacyDataProvider(ManagedContentPack managed, bool paranoid = false)
        {
            this.Managed = managed;
            this.paranoid = paranoid;
            this.Monitor = managed.Monitor;
        }

        public ManagedContentPack Managed { get; }
        public IMonitor Monitor { get; private set; }

        public bool Apply<TKey, TValue>(Dictionary<TKey, TValue> target, string path)
        {
            var patches = new List<LegacyChanges>();
            var contentPackName = this.Managed.Pack.Manifest.Name;

            patches.AddRange(this.GetPatchesForAsset(path, "Replace"));
            patches.AddRange(this.GetPatchesForAsset(path, "Patch"));
            patches.AddRange(this.GetTranslationPatches(path, this.Managed.Pack.Translation.Locale?.ToLower()));

            if (patches.Count() < 1)
            {
                return false;
            }

            foreach (var patch in patches)
            {
                if (patch.Action == "Replace")
                {
                    if (target.Count > 0)
                        this.Monitor.Log(
                            $"Content pack `{contentPackName}` patch `{patch.LogName}` replaces all contents for `{path}`.", 
                            this.paranoid ? LogLevel.Alert : LogLevel.Trace);
                    target.Clear(); // Load replaces all content
                }

                var isLocalized = !string.IsNullOrEmpty(patch.Locale);
                var patchData = this.Managed.Pack.LoadAsset<Dictionary<TKey, TValue>>(patch.FromFile);
                
                AssetPatchHelper.ApplyPatch(target, patchData);
                this.Monitor.Log($"Content pack `{contentPackName}` applied{(isLocalized ? $" `{patch.Locale}` translation" : "")} patch `{patch.LogName}` for `{path}`");
            }

            return true;
        }

        private List<LegacyChanges> GetPatchesForAsset(string path, string action)
        {
            return this.Managed.Contents.Changes
                .Where((p) => p.Action.Equals(action) && p.Target.Equals(path) && !p.Disabled)
                .Where((p) => string.IsNullOrEmpty(p.Locale))
                .ToList();
        }

        private List<LegacyChanges> GetTranslationPatches(string path, string locale)
        {
            return this.Managed.Contents.Changes
                .Where((p) => p.Action.Equals("Patch") && p.Target.Equals(path) && !p.Disabled)
                .Where((p) => !string.IsNullOrEmpty(p.Locale) && p.Locale.ToLower().Equals(locale))
                .ToList();
        }
    }
}
