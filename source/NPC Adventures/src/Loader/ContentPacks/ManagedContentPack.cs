/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using NpcAdventure.Loader.ContentPacks.Data;
using NpcAdventure.Loader.ContentPacks.Provider;
using NpcAdventure.Utils;
using StardewModdingAPI;

namespace NpcAdventure.Loader.ContentPacks
{
    /// <summary>Handles loading assets from content packs.</summary>
    internal class ManagedContentPack
    {
        public static readonly string[] SUPPORTED_FORMATS = { "1.1", "1.2", "1.3", "1.4" };

        /// <summary>The managed content pack.</summary>
        public IContentPack Pack { get; }
        public IMonitor Monitor { get; }
        public ISemanticVersion FormatVersion { get; private set; }
        public IEnumerable<ManagedPatch> Patches { get; private set; }
        public string UniqueId { get => this.Pack.Manifest.UniqueID; }

        /// <summary>Construct an instance.</summary>
        /// <param name="pack">The content pack to manage.</param>
        public ManagedContentPack(IContentPack pack, IMonitor monitor)
        {
            this.Pack = pack ?? throw new System.ArgumentNullException(nameof(pack));
            this.Monitor = monitor ?? throw new System.ArgumentNullException(nameof(monitor));
        }

        public void Load()
        {
            this.Monitor.Log($"   Loading content pack `{this.Pack.Manifest.Name}`");

            if (!this.Pack.HasFile("content.json"))
                throw new ContentPackException("Declaration file `content.json` not found!");

            var contents = this.Pack.ReadJsonFile<Contents>("content.json");
            this.FormatVersion = new SemanticVersion(contents.Format);
            this.VerifyContentPack(contents);
            this.MapPatches(contents.Changes);
        }

        public bool IsLegacy()
        {
            if (this.FormatVersion == null)
                throw new ContentPackException($"Content pack {this.UniqueId} is not loaded yet!");

            return this.FormatVersion.IsOlderThan("1.3");
        }

        private void MapPatches(List<ContentChange> changes)
        {
            this.Patches = from change in changes select new ManagedPatch(change, this);
        }

        /// <summary>
        /// Verify loaded content pack with their data definitions
        /// </summary>
        private void VerifyContentPack(Contents contents)
        {
            if (!this.CheckFormatVersion(this.FormatVersion))
                throw new ContentPackException($"Unsupported format `{contents.Format}`");

            if (contents.Changes == null)
                throw new ContentPackException($"Mandatory field `Changes` is not defined in pack {this.UniqueId}");

            this.Monitor.Log($"      Detected format version {this.FormatVersion}");

            this.VerifyPatches(contents.Changes, this.FormatVersion);
        }

        /// <summary>
        /// Verify defined patches in the content pack
        /// </summary>
        /// <param name="formatVersion"></param>
        private void VerifyPatches(List<ContentChange> changes, ISemanticVersion formatVersion)
        {
            int num = 0; // For identify patches without log name
            for (int i = 0; i < changes.Count; i++)
            {
                var change = changes[i];
                var rewriteNotices = this.ApplyPatchRewrites(change, formatVersion);
                var errors = this.ValidatePatchDefinition(change);

                if (change.LogName == null)
                    change.LogName = $"Patch #{num}";

                if (rewriteNotices.Count > 0)
                {
                    rewriteNotices.ForEach(e => this.Monitor.Log($"      {e} in patch `{change.LogName}`"));
                }


                if (errors.Count > 0)
                {
                    this.Monitor.Log($"Skipped content pack `{this.Pack.Manifest.Name}` patch `{change.LogName}` due to errors:", LogLevel.Error);
                    errors.ForEach(e => this.Monitor.Log($"   - {e}", LogLevel.Error));
                    changes.RemoveAt(i--);
                }

                num++;
            }
        }

        /// <summary>
        /// Apply rewrites for fields in patches
        /// </summary>
        /// <param name="change"></param>
        /// <param name="formatVersion"></param>
        /// <returns></returns>
        private List<string> ApplyPatchRewrites(ContentChange change, ISemanticVersion formatVersion)
        {
            List<string> notices = new List<string>();

            if (formatVersion.IsOlderThan("1.2") && !string.IsNullOrEmpty(change.Locale))
            {
                // Locales exists in format version 1.2 and newer. For older formats is locale undefined
                change.Locale = null;
                notices.Add($"Ignore field `Locale` in format version `{formatVersion}`");
            }

            if (formatVersion.IsOlderThan("1.2") && !string.IsNullOrEmpty(change.LogName))
            {
                change.LogName = null;
                notices.Add($"Ignore field `LogName` in format version `{formatVersion}`");
            }

            if (!formatVersion.IsOlderThan("1.3") && string.IsNullOrEmpty(change.Action))
            {
                change.Action = "Patch"; // Action patch is a default action in format >=1.3
            }

            if (formatVersion.IsOlderThan("1.3") && (change.Action == "Load" || change.Action == "Edit"))
            {
                var replace = change.Action == "Load" ? "Replace" : "Patch";

                notices.Add($"Rewrite action `{change.Action}` -> `{replace}`");
                change.Action = replace;
            }

            if (change.Action == "Replace")
            {
                notices.Add($"Detected content replacer `{change.LogName}` for `{change.Target}`");
            }

            if (formatVersion.IsOlderThan("1.4") && change.Priority != 0)
            {
                change.Priority = 0;
                notices.Add($"Discard value for field `Priority` in format version `{formatVersion}` (unsupported field)");
            }

            return notices;
        }

        /// <summary>
        /// Validate patch definitions, checks their fields
        /// </summary>
        /// <param name="change"></param>
        /// <returns></returns>
        private List<string> ValidatePatchDefinition(ContentChange change)
        {
            List<string> problems = new List<string>();

            if (string.IsNullOrEmpty(change.Action))
                problems.Add($"Undefined action in patch");
            if (string.IsNullOrEmpty(change.Target))
                problems.Add($"Target is not defined in entry");
            if (string.IsNullOrEmpty(change.FromFile))
                problems.Add("No content defined! `FromFile` must be set in entry");
            if (change.Action != null && change.Action.Equals("Replace") && !string.IsNullOrEmpty(change.Locale))
                problems.Add("Locale can't be used for `Replace` action! Use action `Patch` instead for localization patches");
            if (change.Action != null && !change.Action.Equals("Replace") && !change.Action.Equals("Patch"))
                problems.Add($"Unknown action `{change.Action}`");

            return problems;
        }

        /// <summary>
        /// Validate format version of the content pack definition
        /// </summary>
        /// <param name="semanticVersion"></param>
        /// <returns></returns>
        private bool CheckFormatVersion(ISemanticVersion semanticVersion)
        {
            foreach (var compareTo in SUPPORTED_FORMATS)
            {
                if (semanticVersion.EqualsMajorMinor(new SemanticVersion(compareTo)))
                    return true;
            }

            return false;
        }

        private List<ManagedPatch> GetPatchesForAsset(string path, string action)
        {
            return this.Patches
                .Where((p) => p.Change.Action.Equals(action) && p.Change.Target.Equals(path) && !p.Disabled)
                .Where((p) => string.IsNullOrEmpty(p.Change.Locale))
                .ToList();
        }

        private List<ManagedPatch> GetTranslationPatches(string path, string locale)
        {
            return this.Patches
                .Where((p) => p.Change.Action.Equals("Patch") && p.Change.Target.Equals(path) && !p.Disabled)
                .Where((p) => !string.IsNullOrEmpty(p.Change.Locale) && p.Change.Locale.ToLower().Equals(locale))
                .ToList();
        }

        public IEnumerable<ManagedPatch> GetPatchesForTarget(string targetPath)
        {
            if (this.Patches == null)
                throw new ContentPackException($"Content pack {this.UniqueId} is not loaded yet!");

            var patches = new List<ManagedPatch>();

            patches.AddRange(this.GetPatchesForAsset(targetPath, "Replace"));
            patches.AddRange(this.GetPatchesForAsset(targetPath, "Patch"));
            patches.AddRange(this.GetTranslationPatches(targetPath, this.Pack.Translation.Locale?.ToLower()));

            return patches;
        }
    }
}
