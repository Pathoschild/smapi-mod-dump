/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContentPatcher;
using Newtonsoft.Json;
using StardewModdingAPI;

namespace MultipleSpouseDialog
{
    public class ConfigDialog
    {
        public string Call;
        public string Callers;
        internal IManagedConditions conditions;
        public string Responders;
        public string Response;
        public Dictionary<string, string> When;
    }

    internal static class Dialog
    {
        private static List<ConfigDialog> Data;
        private static ConfigDialog NoData;
        private static Random rand;
        private static IMonitor monitor;
        private static IManifest manifest;
        public static IContentPatcherAPI cp_api;

        public static void Initialize(IMonitor m, IManifest mf)
        {
            monitor = m;
            manifest = mf;
            rand = new Random();

            NoData = new ConfigDialog
            {
                Call = "Hi, @!",
                Response = "O hey, @!",
                Callers = "",
                Responders = "",
                When = new Dictionary<string, string>()
            };

            Data = new List<ConfigDialog>();
        }

        public static void Load(string directory)
        {
            if (cp_api == null) monitor.Log("Content patcher API not available; this won't end well", LogLevel.Error);

            foreach (var filename in Directory.GetFiles(directory, "*.json"))
            {
                if (filename.EndsWith("manifest.json")) continue;
                LoadFile(filename);
            }
        }


        private static void LoadFile(string filename)
        {
            monitor.Log($"Reading dialog from {filename}");

            try
            {
                var fileContents = File.ReadAllText(filename);
                var entries = JsonConvert.DeserializeObject<List<ConfigDialog>>(fileContents);
                if (entries == null) return;
                foreach (var dialog in entries)
                {
                    dialog.Call ??= "";
                    dialog.Response ??= "";
                    dialog.Callers ??= "";
                    dialog.Responders ??= "";
                    Data.Add(dialog);

                    if (ModEntry.config.ExtraDebugOutput)
                        monitor.Log(
                            $"Dialog loaded: {dialog.Call}: {dialog.Response}. Callers: {dialog.Callers} Responders: {dialog.Responders}");
                }
            }
            catch (Exception e)
            {
                monitor.Log($"Error reading from config file {filename}: {e.Message}", LogLevel.Debug);
            }
        }

        public static ConfigDialog RandomDialog(string npc1name, string npc2name, bool to_farmer = false)
        {
            List<ConfigDialog> candidates;

            if (to_farmer)
                candidates = Data
                    .Where(dialog => dialog.Callers.Split(' ').Contains(npc1name) || dialog.Callers == "")
                    .Where(dialog =>
                        dialog.Responders.Split(' ').Contains(npc2name) ||
                        dialog.Responders.Split(' ').Contains("Farmer") || dialog.Responders == "")
                    .ToList();
            else
                candidates = Data
                    .Where(dialog => dialog.Callers.Split(' ').Contains(npc1name) || dialog.Callers == "")
                    .Where(dialog => dialog.Responders.Split(' ').Contains(npc2name) || dialog.Responders == "")
                    .ToList();

            foreach (var dialog in candidates)
                if (dialog.conditions == null)
                    dialog.conditions = cp_api.ParseConditions(
                        manifest,
                        dialog.When,
                        new SemanticVersion("1.20.0")
                    );
                else
                    dialog.conditions.UpdateContext();

            var matched = candidates.Where(Dialog => Dialog.conditions.IsMatch).ToList();

            if (!matched.Any())
            {
                if (ModEntry.config.ExtraDebugOutput)
                    monitor.Log($"Could not find any candidate dialog for {npc1name} and {npc2name} ", LogLevel.Error);
                return NoData;
            }

            if (ModEntry.config.ExtraDebugOutput)
                monitor.Log($"{matched.Count} candidate dialogs for {npc1name} and {npc2name}", LogLevel.Debug);
            var r = rand.Next(matched.Count);
            return matched[r];
        }
    }
}