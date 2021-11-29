/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bouhm.Shared.Locations;
using Microsoft.Xna.Framework;
using NPCMapLocations.Framework.Models;
using StardewModdingAPI;
using StardewValley;

namespace NPCMapLocations.Framework
{
    /// <summary>Handles the <c>npc_map_locations_summary</c> console command.</summary>
    internal static class SummaryCommand
    {
        /********
        ** Accessors
        ********/
        /// <summary>The name of the console command/</summary>
        public static string Name { get; } = "npc_map_locations_summary";


        /********
        ** Public methods
        ********/
        /// <summary>Get the command description for the <c>help</c> command.</summary>
        public static string GetDescription()
        {
            return
                "patch summary\n"
                + "   Usage: npc_map_locations_summary\n"
                + "   Shows a summary of the current locations, context, and player position.";
        }

        /// <summary>Handle the console command.</summary>
        /// <param name="monitor">The monitor with which to write output..</param>
        /// <param name="locationUtil">Scans and maps locations in the game world.</param>
        /// <param name="customizations">Manages customized map recolors, NPCs, sprites, names, etc.</param>
        /// <param name="mapVectors">The in-world tile coordinates and map pixels which represent the same position for each location name.</param>
        /// <param name="npcMarkers">The tracked NPC markers.</param>
        /// <param name="locationsWithoutMapVectors">The outdoor location contexts which don't have any map vectors.</param>
        public static void Handle(IMonitor monitor, LocationUtil locationUtil, ModCustomizations customizations, IDictionary<string, MapVector[]> mapVectors, Dictionary<string, NpcMarker> npcMarkers, IEnumerable<LocationContext> locationsWithoutMapVectors)
        {
            if (!Context.IsWorldReady)
            {
                monitor.Log("You must load a save to use this command.", LogLevel.Error);
                return;
            }

            StringBuilder output = new();
            output.AppendLine();

            // current player
            {
                // collect info
                var player = Game1.player;
                var location = player.currentLocation;
                string locationName = locationUtil.GetLocationNameFromLevel(location.NameOrUniqueName) ?? location?.NameOrUniqueName;
                LocationContext context = locationUtil.TryGetContext(locationName, mapGeneratedLevels: false);
                Vector2 mapPixel = ModEntry.LocationToMap(locationName, player.getTileX(), player.getTileY(), customizations.MapVectors, customizations.LocationExclusions);

                // collect alternate location names
                List<string> altNames = new();
                if (location.NameOrUniqueName != location.Name)
                    altNames.Add($"unique: {location.NameOrUniqueName}");
                if (location.NameOrUniqueName != locationName)
                    output.Append($"context: {locationName}");

                // build output
                output.AppendLine();
                output.AppendLine("======================");
                output.AppendLine("==  Current player  ==");
                output.AppendLine("======================");
                output.AppendLine($"Player: {player.Name} ({player.UniqueMultiplayerID})");
                output.AppendLine($"Location: {location.Name}{(altNames.Any() ? $" ({string.Join(", ", altNames)})" : "")}");
                output.AppendLine($"Tile: ({player.getTileX()}, {player.getTileY()})");
                output.AppendLine($"Excluded: {customizations.LocationExclusions.Contains(locationName)}");
                output.AppendLine($"Map pixel: {(mapPixel != ModEntry.Unknown ? $"({mapPixel.X}, {mapPixel.Y})" : "unknown")}");
                output.AppendLine();

                output.AppendLine("Configured vectors:");
                if (customizations.MapVectors.TryGetValue(locationName, out MapVector[] vectors) && vectors.Any())
                {
                    output.Append(
                        SummaryCommand.BuildTable(
                            vectors,
                            "   ",
                            new[] { "tile", "map pixel" },
                            vector => $"{vector.TileX}, {vector.TileY}",
                            vector => $"{vector.MapX}, {vector.MapY}"
                        )
                    );
                }
                else
                    output.AppendLine("   (none)");
                output.AppendLine();

                if (context != null)
                {
                    output.AppendLine("Context:");
                    output.AppendLine($"   Type: {context.Type}");
                    if (context.Parent != null)
                        output.AppendLine($"   Parent: {context.Parent}");
                    output.AppendLine($"   Root: {context.Root}");
                    if (context.Children.Any())
                        output.AppendLine($"   Children: {string.Join(", ", context.Children.OrderBy(p => p))}");
                    if (context.Neighbors.Any())
                        output.AppendLine($"   Neighbors: {string.Join(", ", context.Neighbors.Keys.Distinct().OrderBy(p => p))}");
                }
                else
                    output.AppendLine("Context: unknown location!");

                output.AppendLine();
                output.AppendLine();
            }

            // excluded locations
            {
                output.AppendLine("=========================");
                output.AppendLine("==  Tracked locations  ==");
                output.AppendLine("=========================");
                output.AppendLine("These are the NPCs currently being tracked by the mod.");
                output.AppendLine();
                if (customizations.LocationExclusions.Any())
                {
                    output.AppendLine("If a location is marked \"excluded\", it's completely hidden from NPC Map Locations; players and NPCs in that location will disappear from the map.");
                    output.AppendLine("NPC Map Locations doesn't hide any locations itself, these are all excluded by other mods editing the `Mods/Bouhm.NPCMapLocations/Locations` asset.");
                    output.AppendLine();
                }

                // list locations by root
                output.AppendLine("   Known locations:");
                output.Append(
                    SummaryCommand.BuildTable(
                        records: locationUtil.LocationContexts.Values.OrderBy(p => p.Root ?? p.Name).ThenBy(p => p.Name),
                        linePrefix: "      ",
                        columnHeadings: new[] { "root", "name", "type", "notes" },
                        p => p.Root ?? p.Name,
                        p => p.Name,
                        p => p.Type.ToString(),
                        p => customizations.LocationExclusions.Contains(p.Name) ? "HIDDEN" : ""
                    )
                );

                // list exclusions not listed above
                if (customizations.LocationExclusions.Any())
                {
                    string[] otherExclusions = customizations.LocationExclusions.Where(name => !locationUtil.LocationContexts.ContainsKey(name)).OrderBy(p => p).ToArray();
                    if (otherExclusions.Any())
                    {
                        output.AppendLine();
                        output.AppendLine("These locations are excluded by mods, but don't match a known location:");
                        foreach (string name in otherExclusions)
                            output.AppendLine($"   - {name}");
                    }
                }

                output.AppendLine();
                output.AppendLine();
            }

            // NPC names
            {
                output.AppendLine("====================");
                output.AppendLine("==  Tracked NPCs  ==");
                output.AppendLine("====================");
                output.AppendLine("These are the the NPCs currently being tracked by the mod.");
                output.AppendLine();

                if (npcMarkers.Any())
                {
                    foreach (var typeGroup in npcMarkers.GroupBy(p => p.Value.Type).OrderBy(p => p.Key))
                    {
                        output.AppendLine($"   {typeGroup.Key}:");

                        output.Append(
                            SummaryCommand.BuildTable(
                                typeGroup.OrderBy(p => p.Key),
                                "      ",
                                new[] { "name", "location", "map pixel", "crop offset", "notes" },

                                marker => marker.Value.DisplayName != marker.Key ? $"{marker.Key} ({marker.Value.DisplayName})" : marker.Key,
                                marker => marker.Value.LocationName,
                                marker => $"{marker.Value.MapX}, {marker.Value.MapY}",
                                marker => marker.Value.CropOffset != 0 ? marker.Value.CropOffset.ToString() : "",
                                marker =>
                                {
                                    List<string> notes = new();
                                    if (marker.Value.IsHidden)
                                        notes.Add(marker.Value.ReasonHidden ?? "hidden (reason unknown)");
                                    if (marker.Value.IsBirthday)
                                        notes.Add("birthday");
                                    if (marker.Value.HasQuest)
                                        notes.Add("quest");
                                    return string.Join(", ", notes);
                                }
                            )
                        );
                    }
                }
                else
                    output.AppendLine("   (none)");
                output.AppendLine();
                output.AppendLine();
            }

            // map vectors
            {
                output.AppendLine("===================");
                output.AppendLine("==  Map vectors  ==");
                output.AppendLine("===================");
                output.AppendLine("A 'map vector' represents the same position both in-world (measured in tiles) and on the world map (measured in pixels).");
                output.AppendLine("These are used to calculate where any in-world character should be drawn on the map.");
                output.AppendLine();

                if (mapVectors.Any())
                {
                    var records = mapVectors
                        .SelectMany(group => group.Value
                            .Select(vector => new { Location = group.Key, Vector = vector, IsCustom = customizations.MapVectors.ContainsKey(group.Key) })
                        )
                        .OrderBy(p => p.Location)
                        .ThenBy(p => p.Vector.TileX)
                        .ThenBy(p => p.Vector.TileY);

                    output.Append(
                        SummaryCommand.BuildTable(
                            records,
                            "",
                            new[] { "location", "tile", "map pixel", "source" },
                            p => p.Location,
                            p => $"{p.Vector.TileX}, {p.Vector.TileY}",
                            p => $"{p.Vector.MapX}, {p.Vector.MapY}",
                            p => p.IsCustom ? "another mod" : "NPC Map Locations"
                        )
                    );
                }
                else
                    output.AppendLine("   (none)");

                output.AppendLine();
                output.AppendLine();
            }

            // unknown locations
            {
                output.AppendLine("=========================");
                output.AppendLine("==  Unknown Locations  ==");
                output.AppendLine("=========================");
                output.AppendLine("These locations have no map vectors defined, so NPCs and characters in that location won't appear on the world map.");
                output.AppendLine("For location mod authors, see the pinned post at https://www.nexusmods.com/stardewvalley/mods/239?tab=posts.");
                output.AppendLine();

                locationsWithoutMapVectors = locationsWithoutMapVectors.OrderBy(p => p.Name).ToArray();

                if (locationsWithoutMapVectors.Any())
                {
                    output.Append(
                        SummaryCommand.BuildTable(
                            locationsWithoutMapVectors,
                            "",
                            new[] { "name", "type", "root location" },
                            p => p.Name,
                            p => p.Type.ToString(),
                            p => p.Root
                        )
                    );
                }
                else
                    output.AppendLine("   (none)");

                output.AppendLine();
                output.AppendLine();
            }

            // render
            monitor.Log(output.ToString(), LogLevel.Info);
        }


        /********
        ** Private methods
        ********/
        /// <summary>Get an ASCII table which represents an arbitrary set of records.</summary>
        /// <typeparam name="TRecord">The record type.</typeparam>
        /// <param name="records">The records for which to build a table.</param>
        /// <param name="linePrefix">A prefix for each line in the table, e.g. for indentation.</param>
        /// <param name="columnHeadings">The column headings to show at the top of the table.</param>
        /// <param name="getValues">Get each column's values for a row.</param>
        private static StringBuilder BuildTable<TRecord>(IEnumerable<TRecord> records, string linePrefix, string[] columnHeadings, params Func<TRecord, string>[] getValues)
        {
            // validate
            if (columnHeadings.Length != getValues.Length)
                throw new InvalidOperationException($"You must specify an equal number of {nameof(columnHeadings)} and {nameof(getValues)} values.");

            // collect table data
            int columnCount = columnHeadings.Length;
            int[] sizes = columnHeadings.Select(p => p.Length).ToArray();

            string[][] rows = records
                ?.Select(record =>
                {
                    string[] row = new string[columnCount];

                    for (int i = 0; i < columnCount; i++)
                    {
                        string value = getValues[i](record) ?? "";

                        row[i] = value;
                        sizes[i] = Math.Max(sizes[i], value.Length);
                    }

                    return row;
                })
                .ToArray() ?? new string[columnCount][];

            // build table
            StringBuilder table = new();
            {
                void PrintRow(string[] values, char paddingChar = ' ')
                {
                    int last = columnCount - 1;
                    table.Append(linePrefix);
                    for (int i = 0; i <= last; i++)
                    {
                        table.Append(values[i].PadRight(sizes[i], paddingChar));
                        if (i < last)
                            table.Append(" | ");
                    }
                    table.AppendLine();
                }

                PrintRow(columnHeadings);
                PrintRow(columnHeadings.Select(p => "").ToArray(), '-');
                foreach (string[] row in rows)
                    PrintRow(row);
            }

            return table;
        }
    }
}
