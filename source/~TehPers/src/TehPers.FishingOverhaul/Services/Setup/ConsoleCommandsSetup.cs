/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StardewModdingAPI;
using System.IO;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Json;
using TehPers.Core.Api.Setup;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Config.ContentPacks;
using TehPers.FishingOverhaul.Parsing;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal class ConsoleCommandsSetup : ISetup
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly FishingApi fishingApi;
        private readonly IJsonProvider jsonProvider;
        private readonly IAssetProvider assetProvider;

        public ConsoleCommandsSetup(
            IModHelper helper,
            IMonitor monitor,
            FishingApi fishingApi,
            IJsonProvider jsonProvider,
            [ContentSource(ContentSource.ModFolder)] IAssetProvider assetProvider
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.fishingApi = fishingApi ?? throw new ArgumentNullException(nameof(fishingApi));
            this.jsonProvider =
                jsonProvider ?? throw new ArgumentNullException(nameof(jsonProvider));
            this.assetProvider =
                assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        }

        public void Setup()
        {
            this.helper.ConsoleCommands.Add(
                "tfo_reload",
                "Reloads the fishing information. Usage: 'tfo_reload'.",
                this.Reload
            );
            this.helper.ConsoleCommands.Add(
                "tfo_entries",
                "Lists the registered fishing information. Usage: 'tfo_list <fish|trash|treasure>'.",
                this.Entries
            );
            this.helper.ConsoleCommands.Add(
                "tfo_export",
                "Exports registered entries as JSON to a file. This is the final result of adding/removing entries and setting fish traits. Usage: 'tfo_export'.",
                this.Export
            );
            this.helper.ConsoleCommands.Add(
                "tfo_parse",
                "Parses an expression. Usage: 'tfo_parse expr'",
                (cmd, args) =>
                {
                    var raw = string.Join(" ", args);
                    if (!ExpressionParser.TryParse(raw, out var result, out var error))
                    {
                        this.monitor.Log(error, LogLevel.Error);
                        return;
                    }

                    this.monitor.Log(result.ToString(), LogLevel.Info);
                }
            );
        }

        private void Reload(string command, string[] args)
        {
            this.fishingApi.RequestReload();
            this.monitor.Log("Reloaded fishing information.", LogLevel.Info);
        }

        private void Entries(string command, string[] args)
        {
            if (args.FirstOrDefault() is not { } entryType)
            {
                this.monitor.Log("Missing entry type (fish, trash, treasure).", LogLevel.Error);
                return;
            }

            // Get table of data
            var table = entryType switch
            {
                "fish" => GetEntriesTable(
                    this.fishingApi.fishManagers,
                    "Item key",
                    entry => new(entry.Entry.FishKey.ToString()),
                    entry => entry.Entry.AvailabilityInfo
                ),
                "trash" => GetEntriesTable(
                    this.fishingApi.trashManagers,
                    "Item key",
                    entry => new(entry.Entry.ItemKey.ToString()),
                    entry => entry.Entry.AvailabilityInfo
                ),
                "treasure" => GetEntriesTable(
                    this.fishingApi.treasureManagers,
                    "Item keys",
                    entry => new(entry.Entry.ItemKeys.Select(k => k.ToString())),
                    entry => entry.Entry.AvailabilityInfo
                ),
                _ => new(ImmutableArray<Row>.Empty),
            };

            if (!table.Rows.Any())
            {
                return;
            }

            // Print out table
            table.Log(this.monitor, LogLevel.Info);

            Table GetEntriesTable<T>(
                IEnumerable<T> entries,
                string itemKeyHeader,
                Func<T, Cell> getItemKey,
                Func<T, AvailabilityInfo> getAvailabilityInfo
            )
            {
                var header = new Row(
                    ImmutableArray.Create(
                        new Cell(itemKeyHeader),
                        new Cell("Base chance"),
                        new Cell("Priority tier"),
                        new Cell("Time available"),
                        new Cell("Seasons"),
                        new Cell("Weathers"),
                        new Cell("Water types"),
                        new Cell("Locations")
                    )
                );
                var data = entries.Select(
                    entry =>
                    {
                        var availabilityInfo = getAvailabilityInfo(entry);
                        return new Row(
                            ImmutableArray.Create(
                                getItemKey(entry),
                                new Cell($"{availabilityInfo.BaseChance:F4}"),
                                new Cell($"{availabilityInfo.PriorityTier:F2}"),
                                new Cell(
                                    $"{availabilityInfo.StartTime:0000}-{availabilityInfo.EndTime:0000}"
                                ),
                                new Cell(availabilityInfo.SeasonsSplit.Select(s => s.ToString())),
                                new Cell(availabilityInfo.WeathersSplit.Select(w => w.ToString())),
                                new Cell(
                                    availabilityInfo.WaterTypesSplit.Select(w => w.ToString())
                                ),
                                new Cell(
                                    availabilityInfo.IncludeLocations.Select(loc => $"+{loc}")
                                        .Concat(
                                            availabilityInfo.ExcludeLocations.Select(
                                                loc => $"-{loc}"
                                            )
                                        )
                                )
                            )
                        );
                    }
                );

                return new(data.Prepend(header).ToImmutableArray());
            }
        }

        private void Export(string command, string[] args)
        {
            var contentPack = new FishingContentPack
            {
                AddFish =
                    this.fishingApi.fishManagers.Select(manager => manager.Entry)
                        .ToImmutableArray(),
                SetFishTraits = this.fishingApi.fishTraits.ToImmutableDictionary(),
                AddTrash =
                    this.fishingApi.trashManagers.Select(manager => manager.Entry)
                        .ToImmutableArray(),
                AddTreasure =
                    this.fishingApi.treasureManagers.Select(manager => manager.Entry)
                        .ToImmutableArray(),
                AddEffects = this.fishingApi.fishingEffectManagers
                    .Select(manager => manager.Entry)
                    .ToImmutableArray(),
            };
            var path = Path.Combine(
                Constants.DataPath,
                ".tehpers.fishingoverhaul",
                "entries.exported.json"
            );
            this.monitor.Log($"Writing entries to: {path}", LogLevel.Info);
            this.monitor.Log(
                "This file is for informational purposes only. Some entries may have special handling and cannot be created in a content pack.",
                LogLevel.Info
            );
            this.jsonProvider.WriteJson(
                contentPack,
                path,
                this.assetProvider,
                settings => settings.DefaultValueHandling = DefaultValueHandling.Ignore
            );
        }

        private record Cell(string Contents)
        {
            public int Width { get; } = Contents.Length;

            public Cell(IEnumerable<string> contents)
                : this(string.Join(", ", contents))
            {
            }
        }

        private record Row(ImmutableArray<Cell> Cells)
        {
            public ImmutableArray<int> CellWidths { get; } =
                Cells.Select(cell => cell.Width).ToImmutableArray();
        }

        private record Table(ImmutableArray<Row> Rows)
        {
            private ImmutableArray<int> ColWidths { get; } = Rows.Any()
                ? Rows[0]
                    .Cells.Select((_, colI) => Rows.Max(row => row.CellWidths[colI]))
                    .ToImmutableArray()
                : ImmutableArray<int>.Empty;

            public void Log(IMonitor monitor, LogLevel level)
            {
                var lines = this.Rows.Select(
                    row => row.Cells.Select(
                        (cell, colI) => cell.Contents.PadRight(this.ColWidths[colI])
                    )
                );
                foreach (var line in lines)
                {
                    monitor.Log(string.Join(" | ", line), level);
                }
            }
        }
    }
}
