/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StardewModdingAPI;
using TehPers.Core.Api.Setup;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal class ConsoleCommandsSetup : ISetup
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly FishingApi fishingApi;

        public ConsoleCommandsSetup(IModHelper helper, IMonitor monitor, FishingApi fishingApi)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.fishingApi = fishingApi ?? throw new ArgumentNullException(nameof(fishingApi));
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
                    this.fishingApi.fishEntries,
                    "Item key",
                    entry => new(entry.Entry.FishKey.ToString()),
                    entry => entry.Entry.AvailabilityInfo
                ),
                "trash" => GetEntriesTable(
                    this.fishingApi.trashEntries,
                    "Item key",
                    entry => new(entry.Entry.ItemKey.ToString()),
                    entry => entry.Entry.AvailabilityInfo
                ),
                "treasure" => GetEntriesTable(
                    this.fishingApi.treasureEntries,
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
                                new Cell(availabilityInfo.BaseChance.ToString("F4")),
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
                var lines = this.Rows
                    .Select(
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