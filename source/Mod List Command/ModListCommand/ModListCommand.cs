/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Xebeth/StardewValley-ListModsCommand
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI.Toolkit;
using System.Globalization;
using System.Diagnostics;
using StardewModdingAPI;
using System.Linq;
using System.IO;
using KBCsv;
using System;
using System.Collections.Immutable;
using System.Text;

namespace ModListCommand
{
    public sealed class ModListCommand : Mod
    {
        private const string CommandName = "list_mods";

        private const string HelpText = $"""
            Lists currently loaded mods.

            Usage: {CommandName} [console]
            - the console parameter is optional
            Lists mods to the console.

            Usage: {CommandName} csv <path>
            - path (required): the (absolute or relative) path to the file to be created. The base directory for relative paths is the game's installation directory (i.e. where Stardew Valley.dll is).

            Examples:
            {CommandName}
            {CommandName} console    (does the same as the previous example)
            {CommandName} csv c:\temp\mods.csv
            """;

        private readonly ModToolkit _toolkit = new();

        public override void Entry(IModHelper helper)
        {
            helper.ConsoleCommands.Add(CommandName, HelpText, ListMods);
        }

        private void ListMods(string _, string[] args)
        {
            if (args.Length == 0 || args[0] == "console")
            {
                ListModsToConsole();
            }
            else if (args.Length == 2 && args[0] == "csv" && !string.IsNullOrWhiteSpace(args[1]))
            {
                ListModsToCsv(csvPath: args[1]);
            }
            else
            {
                Monitor.Log($"Incorrect parameters!{Environment.NewLine}See the help:", LogLevel.Warn);
                Monitor.Log(HelpText, LogLevel.Info);
            }
        }
        private void ListModsToConsole()
        {
            StringBuilder outputBuilder = new();
            outputBuilder.AppendLine();

            foreach (var modInfo in EnumerateModInfos())
            {
                outputBuilder.Append(modInfo.Name);
                outputBuilder.Append(" v");
                outputBuilder.Append(modInfo.Version);
                outputBuilder.Append(" by ");
                outputBuilder.Append(modInfo.Author);
                outputBuilder.Append(':');
                outputBuilder.AppendLine();

                foreach (string url in modInfo.UpdateUrls)
                {
                    outputBuilder.Append("- ");
                    outputBuilder.AppendLine(url);
                }

                outputBuilder.AppendLine(modInfo.Description);
                outputBuilder.AppendLine();
            }

            Monitor.Log(outputBuilder.ToString(), LogLevel.Info);
        }

        private void ListModsToCsv(string csvPath)
        {
            using (var streamWriter = new StreamWriter(csvPath))
            using (var csvWriter = new CsvWriter(streamWriter)
            {
                ValueSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0],
                ValueDelimiter = '"',
                ForceDelimit = true
            })
            {
                csvWriter.WriteRecord("Name", "Version", "Author", "Links", "Description");

                foreach (var modInfo in EnumerateModInfos())
                {
                    csvWriter.WriteRecord(
                        modInfo.Name,
                        modInfo.Version.ToString(),
                        modInfo.Author,
                        string.Join(";", modInfo.UpdateUrls),
                        modInfo.Description);
                }

                Monitor.Log($"{csvWriter.RecordNumber} records written to `{Path.GetFullPath(csvPath)}`", LogLevel.Info);
                Monitor.Log("Opening the CSV file...", LogLevel.Info);
            }

            // Open the created CSV file
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(csvPath)
                {
                    UseShellExecute = true,
                }
            };
            process.Start();
        }

        private IEnumerable<ModInfo> EnumerateModInfos()
        {
            return Helper.ModRegistry.GetAll()
                .Select(x => CreateModInfo(x.Manifest));
        }

        private ModInfo CreateModInfo(IManifest manifest)
        {
            return new ModInfo
            {
                Name = manifest.Name,
                Version = manifest.Version,
                Author = manifest.Author,
                Description = manifest.Description,
                UpdateUrls = GetUrls(manifest.UpdateKeys).ToImmutableList(),
            };
        }

        private IEnumerable<string> GetUrls(string[] updateKeys)
        {
            if (updateKeys.Length == 0)
                return new[] { "no update key" };
            else
                return updateKeys.Select(u => _toolkit.GetUpdateUrl(u) ?? u);
        }
    }
}
