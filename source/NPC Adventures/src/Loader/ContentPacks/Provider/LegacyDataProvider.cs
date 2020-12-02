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
            var patches = new List<ManagedPatch>();
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
                if (patch.Change.Action == "Replace")
                {
                    if (target.Count > 0)
                        this.Monitor.Log(
                            $"Content pack `{contentPackName}` patch `{patch.Change.LogName}` replaces all contents for `{path}`.", 
                            this.paranoid ? LogLevel.Alert : LogLevel.Trace);
                    target.Clear(); // Load replaces all content
                }

                var isLocalized = !string.IsNullOrEmpty(patch.Change.Locale);
                var patchData = patch.LoadData<TKey, TValue>();
                
                AssetPatchHelper.ApplyPatch(target, patchData);
                this.Monitor.Log($"Content pack `{contentPackName}` applied{(isLocalized ? $" `{patch.Change.Locale}` translation" : "")} patch `{patch.Change.LogName}` for `{path}`");
            }

            return true;
        }

        private List<ManagedPatch> GetPatchesForAsset(string path, string action)
        {
            return this.Managed.Patches
                .Where((p) => p.Change.Action.Equals(action) && p.Change.Target.Equals(path) && !p.Disabled)
                .Where((p) => string.IsNullOrEmpty(p.Change.Locale))
                .ToList();
        }

        private List<ManagedPatch> GetTranslationPatches(string path, string locale)
        {
            return this.Managed.Patches
                .Where((p) => p.Change.Action.Equals("Patch") && p.Change.Target.Equals(path) && !p.Disabled)
                .Where((p) => !string.IsNullOrEmpty(p.Change.Locale) && p.Change.Locale.ToLower().Equals(locale))
                .ToList();
        }
    }
}
