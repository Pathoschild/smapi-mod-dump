using System.Collections.Generic;
using NpcAdventure.Loader.ContentPacks.Data;
using NpcAdventure.Loader.ContentPacks.Provider;
using NpcAdventure.Utils;
using StardewModdingAPI;

namespace NpcAdventure.Loader.ContentPacks
{
    /// <summary>Handles loading assets from content packs.</summary>
    internal class ManagedContentPack
    {
        public static string[] SUPPORTED_FORMATS = { "1.1", "1.2", "1.3" };

        /// <summary>The managed content pack.</summary>
        public IContentPack Pack { get; }
        public IMonitor Monitor { get; }

        private readonly LegacyDataProvider legacyDataProvider;

        public Contents Contents { get; private set; }
        public ISemanticVersion FormatVersion { get; internal set; }

        /// <summary>Construct an instance.</summary>
        /// <param name="pack">The content pack to manage.</param>
        public ManagedContentPack(IContentPack pack, IMonitor monitor, bool paranoid = false)
        {
            this.Pack = pack;
            this.Monitor = monitor;
            this.legacyDataProvider = new LegacyDataProvider(this, paranoid);
        }

        public void Load()
        {
            this.Monitor.Log($"   Loading content pack `{this.Pack.Manifest.Name}`");

            if (!this.Pack.HasFile("content.json"))
                throw new ContentPackException("Declaration file `content.json` not found!");

            this.Contents = this.Pack.ReadJsonFile<Contents>("content.json");
            this.FormatVersion = new SemanticVersion(this.Contents.Format);
            this.VerifyContentPack();
        }

        /// <summary>
        /// Verify loaded content pack with their data definitions
        /// </summary>
        private void VerifyContentPack()
        {
            if (!this.CheckFormatVersion(this.FormatVersion))
                throw new ContentPackException($"Unsupported format `{this.Contents.Format}`");

            this.Monitor.Log($"      Detected format version {this.FormatVersion}");

            this.VerifyPatches(this.FormatVersion);
        }

        /// <summary>
        /// Verify defined patches in the content pack
        /// </summary>
        /// <param name="formatVersion"></param>
        private void VerifyPatches(ISemanticVersion formatVersion)
        {
            int num = 0; // For identify patches without log name
            for (int i = 0; i < this.Contents.Changes.Count; i++)
            {
                var change = this.Contents.Changes[i];
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
                    this.Contents.Changes.RemoveAt(i--);
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
        private List<string> ApplyPatchRewrites(LegacyChanges change, ISemanticVersion formatVersion)
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
                // Locales exists in format version 1.2 and newer. For older formats is locale undefined
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

            return notices;
        }

        /// <summary>
        /// Validate patch definitions, checks their fields
        /// </summary>
        /// <param name="change"></param>
        /// <returns></returns>
        private List<string> ValidatePatchDefinition(LegacyChanges change)
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

        /// <summary>
        /// Apply content pack to given target
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="target">Target to be patched</param>
        /// <param name="path">Which patch (asset path)</param>
        /// <returns>True if patch to the target was applied</returns>
        public bool Apply<TKey, TValue>(Dictionary<TKey, TValue> target, string path)
        {
            return this.legacyDataProvider.Apply(target, path);
        }
    }
}
