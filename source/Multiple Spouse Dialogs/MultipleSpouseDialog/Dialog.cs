/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/MultipleSpouseDialogs
**
*************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StardewModdingAPI;

namespace MultipleSpouseDialog
{
    public class ConfigDialog
    {
        public string Call;
        public string Response;
        public string Callers;
        public string Responders;
        public Dictionary<string, string> When;
        internal ContentPatcher.IManagedConditions conditions;
    }

    class Dialog
    {
        public static List<ConfigDialog> Data;
        private static ConfigDialog NoData;
        public static bool ready = false;
        private static Random rand;
        private static IMonitor monitor;
        public static IModHelper helper;
        public static IManifest manifest;
        public static ContentPatcher.IContentPatcherAPI cp_api;

        public static void Initialize(IMonitor m, IModHelper h, IManifest mf)
        {
            monitor = m;
            helper = h;
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
            if (cp_api == null)
            {
                monitor.Log($"Content patcher API not available; this won't end well", LogLevel.Error);
            } 

            foreach (string filename in Directory.GetFiles(directory, "*.json"))
            {
                if (filename.EndsWith("manifest.json")) continue;
                LoadFile(filename);
            }
            ready = true;
        }


        private static void LoadFile(string filename)
        {
            List<ConfigDialog> entries;
            monitor.Log($"Reading dialog from {filename}", LogLevel.Debug);

            try
            {
                string filecontents = File.ReadAllText(filename);
                entries = JsonConvert.DeserializeObject<List<ConfigDialog>>(filecontents);
                foreach (ConfigDialog dialog in entries)
                {
                    if (dialog.Call == null) dialog.Call = "";
                    if (dialog.Response == null) dialog.Response = "";
                    if (dialog.Callers == null) dialog.Callers = "";
                    if (dialog.Responders == null) dialog.Responders = "";
                    Data.Add(dialog);

                    if (ModEntry.config.ExtraDebugOutput) monitor.Log($"Dialog loaded: {dialog.Call}: {dialog.Response}. Callers: {dialog.Callers} Responders: {dialog.Responders}", LogLevel.Trace);
                }
            }
            catch (Exception e)
            {
                monitor.Log($"Error reading from config file {filename}: {e.Message}", LogLevel.Debug);
            }
        }

        public static ConfigDialog RandomDialog(string npc1name, string npc2name, bool to_farmer=false)
        {
            List<ConfigDialog> candidates;

            if (to_farmer)
            {
                 candidates = Data
                  .Where(dialog => dialog.Callers.Split(' ').Contains(npc1name) || dialog.Callers == "")
                  .Where(dialog => dialog.Responders.Split(' ').Contains(npc2name) || dialog.Responders.Split(' ').Contains("Farmer") || dialog.Responders == "")
                  .ToList();
            }
            else
            {
                candidates = Data
                  .Where(dialog => dialog.Callers.Split(' ').Contains(npc1name) || dialog.Callers == "")
                  .Where(dialog => dialog.Responders.Split(' ').Contains(npc2name) || dialog.Responders == "")
                  .ToList();
            }

            foreach (ConfigDialog dialog in candidates)
            {
                if (dialog.conditions == null)
                {
                    dialog.conditions = cp_api.ParseConditions(
                        manifest: manifest,
                        rawConditions: dialog.When,
                        formatVersion: new SemanticVersion("1.20.0")
                    );
                }
                else
                {
                    dialog.conditions.UpdateContext();
                }
            }

            List<ConfigDialog> matched = candidates.Where(Dialog => Dialog.conditions.IsMatch).ToList();

            if (!matched.Any())
            {
                if (ModEntry.config.ExtraDebugOutput) monitor.Log($"Could not find any candidate dialog for {npc1name} and {npc2name} ", LogLevel.Error);
                return NoData;
            }
            if (ModEntry.config.ExtraDebugOutput) monitor.Log($"{matched.Count} candidate dialogs for {npc1name} and {npc2name}", LogLevel.Debug);
            int r = rand.Next(matched.Count);
            return matched[r];
        }
    }
}