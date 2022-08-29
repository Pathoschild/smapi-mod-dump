/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewJsonProcessor
**
*************************************************/

// // Copyright 2022 Jamie Taylor
using System;
using System.IO;
using System.Reflection;
using JsonProcessor.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace JsonProcessor {
    public class ModEntry : Mod {
        public ModEntry() {
        }

        public override void Entry(IModHelper helper) {
            helper.ConsoleCommands.Add("process-json", "Transform a JSON file.  Run with \"help\" for details", CliCommand);
        }

        public override object? GetApi() {
            return new JsonProcessorAPI(Monitor);
        }

        private void CliCommand(string command, string[] args) {
            string usage = $@"USAGE: {command} <in-file> [<out-file>]";
            string help = $@"Processes the JSON file <in-file>.
If <out-file> is supplied then the result is written to that file
(WHICH IS REPLACED WITHOUT WARNING), otherwise the result is
written to the console.

File paths can be fully qualified or relative to the JsonProcessor mod folder.
(Hint: file paths with spaces must be in double quotes.)";
            if (args.Length < 1 || args.Length > 2) {
                Monitor.Log($"type \"{command} help\" for usage and help", LogLevel.Warn);
                return;
            }
            if (args[0] == "help" || args[0] == "--help") {
                Monitor.Log($"\n{usage}\n\n{help}", LogLevel.Info);
                return;
            }
            string inFilename = args[0];
            string? outFilename = null;
            if (!Path.IsPathFullyQualified(inFilename)) {
                inFilename = Path.Combine(Helper.DirectoryPath, inFilename);
            }
            FileInfo inFile = new FileInfo(inFilename);
            if (!inFile.Exists) {
                Monitor.Log($"Input file {inFilename} does not exist.", LogLevel.Error);
                return;
            }
            if (args.Length > 1) {
                outFilename = args[1];
                if (!Path.IsPathFullyQualified(outFilename)) {
                    outFilename = Path.Combine(Helper.DirectoryPath, outFilename);
                }
                string outDir = Path.GetDirectoryName(outFilename) ?? "";
                Monitor.Log($"outDir = {outDir}", LogLevel.Debug);
                if (!Directory.Exists(outDir)) {
                    Monitor.Log($"output directory {outDir} does not exist; please create it first", LogLevel.Error);
                    return;
                }
                if (Directory.Exists(outFilename)) {
                    Monitor.Log($"A directory already exists at {outFilename}; can't use that as an output file", LogLevel.Error);
                    return;
                }
            }

            Monitor.Log($"Reading from {inFilename}", LogLevel.Debug);
            JToken json;
            try {
                using (StreamReader file = inFile.OpenText())
                using (JsonTextReader reader = new JsonTextReader(file)) {
                    json = JToken.ReadFrom(reader);
                }
            } catch (Exception e) {
                Monitor.Log($"Exception while parsing JSON: {e}", LogLevel.Error);
                return;
            }
            //if (json is not JObject jsonObj) {
            //    Monitor.Log($"Input file was a json token of type {json.Type}, not an object.", LogLevel.Error);
            //    return;
            //}

            Monitor.Log($"Processing JSON", LogLevel.Debug);
            IJsonProcessor processor = new JsonProcessorAPI(Monitor).NewProcessor(Path.GetFileName(inFilename));
            if (!processor.Transform(json)) {
                Monitor.Log("Errors were encountered while transforming JSON", LogLevel.Warn);
            }

            if (outFilename is not null) {
                Monitor.Log($"Writing to {outFilename}", LogLevel.Debug);
                try {
                    using (StreamWriter file = File.CreateText(outFilename))
                    using (JsonTextWriter writer = new JsonTextWriter(file)) {
                        writer.Formatting = Formatting.Indented;
                        json.WriteTo(writer);
                    }
                    Monitor.Log($"Output written to {outFilename}", LogLevel.Info);
                } catch (Exception e) {
                    Monitor.Log($"Exception while writing to {outFilename}: {e}", LogLevel.Error);
                    return;
                }
            } else {
                Monitor.Log($"Result:\n{json.ToString()}", LogLevel.Info);
            }
        }
    }
}

