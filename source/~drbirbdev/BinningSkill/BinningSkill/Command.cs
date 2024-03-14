/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using BirbCore.Attributes;
using StardewValley;
using StardewValley.GameData.GarbageCans;

namespace BinningSkill.BinningSkill;

[SCommand("binning")]
internal class Command
{
    [SCommand.Command]
    public static void CanInfo(string id)
    {
        if (!TryGetGarbageCan(id, out string key, out GameLocation canLocation))
        {
            Log.Error($"Could not find garbage can {id}");
            return;
        }

        GarbageCanData allData = DataLoader.GarbageCans(Game1.content);
        var data = allData.GarbageCans.GetValueOrDefault(key);

        List<GarbageCanItemData> items = [];
        items.AddRange(allData.BeforeAll);
        if (data?.Items != null)
        {
            items.AddRange(data.Items);
        }

        items.AddRange(allData.AfterAll);

        float chance = data?.BaseChance - -1 < 0.0001 ? allData.DefaultBaseChance : data?.BaseChance ?? 1;
        Log.Info($"Can {key} at location {canLocation?.Name ?? "unknown"}");
        Log.Info($"Base chance: {chance}");
        Log.Info("Item Data (including global item data):");
        foreach (GarbageCanItemData item in items)
        {
            if (item.IsMegaSuccess)
            {
                Log.Warn($"\t{item.Id}");
            }
            else if (item.IsDoubleMegaSuccess)
            {
                Log.Error($"\t{item.Id}");
            }
            else
            {
                Log.Info($"\t{item.Id}");
            }

            Log.Info($"\t\tPossible Items: {item.ItemId ?? string.Join(" ", item.RandomItemId)}");
            bool pass = GameStateQuery.CheckConditions(item.Condition, canLocation);
            if (pass)
            {
                Log.Info($"\t\tConditions:");
            }
            else
            {
                Log.Trace($"\t\tConditions:");
            }

            string[] rawQueries = GameStateQuery.SplitRaw(item.Condition);
            foreach (string query in rawQueries)
            {
                pass = GameStateQuery.CheckConditions(query, canLocation);
                if (pass)
                {
                    Log.Info($"\t\t\t{query}");
                }
                else
                {
                    Log.Trace($"\t\t\t{query}");
                }
            }

            Log.Info("");
        }

        Log.Info("Custom Data:");
        foreach (KeyValuePair<string, string> values in data?.CustomFields ?? [])
        {
            Log.Info($"\t\"{values.Key}\": \"{values.Value}\"");
        }
    }

    [SCommand.Command]
    public static void Simulate(string id, int rounds = 10000)
    {
        if (!TryGetGarbageCan(id, out string key, out GameLocation canLocation))
        {
            Log.Error($"Could not find garbage can {id}");
            return;
        }

        Dictionary<string, int> itemDataCounts = new Dictionary<string, int>();
        Dictionary<string, int> itemCounts = new Dictionary<string, int>();

        uint daysPlayed = Game1.stats.DaysPlayed;
        uint noItem = 0;
        for (uint i = daysPlayed; i < rounds + daysPlayed; i++)
        {
            Game1.stats.DaysPlayed = i;
            if (!canLocation.TryGetGarbageItem(key, Game1.player.DailyLuck, out Item item,
                    out GarbageCanItemData selected, out Random _))
            {
                Log.Warn(
                    "Checking garbage failed, make sure Binning level is appropriate, and no NPCs are near garbage can.");
                Game1.stats.DaysPlayed = daysPlayed;
                return;
            }

            if (item == null)
            {
                noItem++;
                continue;
            }

            itemDataCounts.TryAdd(selected.Id, 0);
            itemDataCounts[selected.Id]++;

            itemCounts.TryAdd(item.Name, 0);
            itemCounts[item.Name]++;
        }

        Game1.stats.DaysPlayed = daysPlayed;

        Log.Info("==== Selected Item Entries ====");
        List<KeyValuePair<string, int>> sortedItemDataCounts = itemDataCounts.ToList();
        sortedItemDataCounts.Sort((a, b) => b.Value - a.Value);

        Log.Info($"{noItem,10}\t{100.0f * noItem / rounds,10}%\tnothing found...");
        foreach (var entry in sortedItemDataCounts)
        {
            Log.Info($"{entry.Value,10}\t{100.0f * entry.Value / rounds,10}%\t{entry.Key}");
        }

        Log.Info("==== Selected Items ====");
        List<KeyValuePair<string, int>> sortedItemCounts = itemCounts.ToList();
        sortedItemCounts.Sort((a, b) => b.Value - a.Value);

        Log.Info($"{noItem,10}\t{100.0f * noItem / rounds,10}%\tnothing found...");
        foreach (var entry in sortedItemCounts)
        {
            Log.Info($"{entry.Value,10}\t{100.0f * entry.Value / rounds,10}%\t{entry.Key}");
        }
    }

    private static bool TryGetGarbageCan(string search, out string entryKey, out GameLocation entryLocation)
    {
        GarbageCanData allData = DataLoader.GarbageCans(Game1.content);
        entryKey = null;
        entryLocation = null;
        if (!allData.GarbageCans.TryGetValue(search, out GarbageCanEntryData entryData))
        {
            Log.Debug($"Command didn't find exact id match for {search}, searching...");
            foreach (var entry in allData.GarbageCans)
            {
                if (!entry.Key.Contains(search))
                {
                    continue;
                }

                Log.Debug($"Found {entry.Key}");
                entryKey = entry.Key;
                entryData = entry.Value;
                break;
            }

            if (entryData is null)
            {
                Log.Debug($"Found no garbage cans...");
                return false;
            }
        }
        else
        {
            entryKey = search;
        }

        string id = entryKey;
        Log.Debug($"Searching for location of {entryKey}");
        GameLocation canLocation = null;
        if (entryKey is not null)
        {
            Utility.ForEachLocation(location =>
            {
                foreach (var tile in location.Map.GetLayer("Buildings").Tiles.Array)
                {
                    if (tile is null || tile.Properties.Count == 0)
                    {
                        continue;
                    }

                    foreach (var property in tile.Properties)
                    {
                        if (property.Key != "Action" || property.Value != $"Garbage {id}")
                        {
                            continue;
                        }

                        Log.Debug($"Found map {location.Name} for garbage can {id}");
                        canLocation = location;
                        return false;
                    }
                }

                return true;
            });
        }

        entryLocation = canLocation;
        return true;
    }
}
