/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace MaritimeSecrets
{
    internal struct SpeechType
    {
        internal const int dynamic = 0;
        internal const int modern = 1;
        internal const int sailor = 2;
    }

    public class MaritimeSecrets : Mod
    {
        internal string talkedToMarinerTodayKey;
        internal bool IsUsingMermaidMod { get; private set; }
        internal MaritimeSecretsConfig Config { get; private set; }

        private readonly List<string> mermaidMods = new()
        {
            "NayaSprites.ContentPatcher.MermaidMariner",
            "Naya.ContentPatcher.MermaidMariner",
            "mi.Mermaids",
            "DragonMaus.MermaidsReplaceOldMarinerRedux",
            "peaachdew.OldMarinerToMermaid",
            "JennaJuffuffles.MarinerToMermaid",
            "DolphINaF.MermaidMariner"
        };

        internal Dictionary<string, string> marinerDialogueOverride;

        private string translationOverrideKey;

        public override void Entry(IModHelper helper)
        {
            translationOverrideKey = $"Mods/{ModManifest.UniqueID}/MarinerDialogueOverride";
            talkedToMarinerTodayKey = $"{ModManifest.UniqueID}/TalkedToMarinerToday";

            Config = Helper.ReadConfig<MaritimeSecretsConfig>();

            MaritimeSecretsConfig.VerifyConfigValues(Config, this);

            Helper.Events.GameLoop.GameLaunched += delegate { MaritimeSecretsConfig.SetUpModConfigMenu(Config, this); };

            IsUsingMermaidMod = false;

            foreach (var mermaidModId in mermaidMods)
            {
                if (Helper.ModRegistry.IsLoaded(mermaidModId))
                {
                    IsUsingMermaidMod = true;
                    break;
                }
            }

            Helper.Events.GameLoop.DayEnding += delegate { ResetMarinerTalk(); };

            Helper.Events.Content.AssetRequested += this.OnAssetRequested;
            Helper.Events.Content.AssetReady += this.OnAssetReady;
            Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

            Patcher.PatchAll(this);
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo(translationOverrideKey))
            {
                e.LoadFromModFile<Dictionary<string, string>>("dialogueOverride/override-dummy.json", AssetLoadPriority.Medium);
            }
        }

        private void OnAssetReady(object sender, AssetReadyEventArgs e)
        {
            if (e.Name.IsEquivalentTo(translationOverrideKey))
            {
                this.marinerDialogueOverride = Game1.content.Load<Dictionary<string, string>>(translationOverrideKey);
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.marinerDialogueOverride = Game1.content.Load<Dictionary<string, string>>(translationOverrideKey);
        }

        private void ResetMarinerTalk()
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                farmer?.modData?.Remove(talkedToMarinerTodayKey);
            }
        }

        /// <summary>
        /// Small helper method to log to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        /// <summary>
        /// Small helper method to log an error to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        /// <param name="e">an optional error message to log additionally</param>
        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }
    }
}