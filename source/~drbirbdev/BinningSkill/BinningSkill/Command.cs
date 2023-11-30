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
using System.Text;
using System.Threading.Tasks;
using BirbCore.Attributes;
using StardewValley.GameData.GarbageCans;
using StardewValley;
using StardewValley.Internal;
using SpaceCore;

namespace BinningSkill.BinningSkill;

[SCommand("binning")]
internal class Command
{

    [SCommand.Command]
    public static void SimulateGarbageDrops(string id, int rounds = 10000)
    {
        GarbageCanData allData = Game1.content.Load<GarbageCanData>("Data\\GarbageCans");
        string entryKey = null;
        if (!allData.GarbageCans.TryGetValue(id, out var data))
        {
            Log.Info("Command didn't find exact id match, searching...");
            foreach (var entry in allData.GarbageCans)
            {
                if (entry.Key.Contains(id))
                {
                    Log.Info($"Found {entry.Key}");
                    entryKey = entry.Key;
                    data = entry.Value;
                    break;
                }
            }
            if (data is null)
            {
                Log.Info($"Found no garbage cans, using only defaults.");
            }
        }

        GameLocation canLocation = Game1.player.currentLocation;
        if (entryKey is not null)
        {
            Utility.ForEachLocation(location =>
            {
                foreach (var tile in location.Map.GetLayer("Buildings").Tiles.Array)
                {
                    if (tile is null || tile.Properties.Count() == 0)
                    {
                        continue;
                    }

                    foreach (var property in tile.Properties)
                    {
                        if (property.Key == "Action" &&  property.Value == $"Garbage {entryKey}")
                        {
                            Log.Info($"Found map {location.Name} for garbage can {entryKey}");
                            canLocation = location;
                            return false;
                        }
                    }
                }
                return true;
            });
        }


        Dictionary<string, int> itemCounts = new Dictionary<string, int>();
        int noItem = 0;

        double dailyLuck = Game1.player.DailyLuck + ModEntry.Config.PerLevelBaseDropChanceBonus * Game1.player.GetCustomSkillLevel("drbirbdev.Binning");
        for (int i = 0; i < rounds; i++)
        {
            float baseChance = ((data != null && data.BaseChance > 0f) ? data.BaseChance : allData.DefaultBaseChance);
            baseChance += (float)dailyLuck;
            Random garbageRandom = Utility.CreateDaySaveRandom(i + Utility.GetDeterministicHashCode(id));
            GarbageCanItemData selected = null;
            Item item = null;
            bool baseChancePassed = garbageRandom.NextDouble() < (double)baseChance;
            ItemQueryContext itemQueryContext = new ItemQueryContext(canLocation, Game1.player, garbageRandom);
            List<GarbageCanItemData>[] array = new List<GarbageCanItemData>[3]
            {
                allData.BeforeAll,
                data?.Items,
                allData.AfterAll
            };
            foreach (List<GarbageCanItemData> itemList in array)
            {
                if (itemList == null)
                {
                    continue;
                }
                foreach (GarbageCanItemData entry in itemList)
                {
                    if ((baseChancePassed || entry.IgnoreBaseChance) && GameStateQuery.CheckConditions(entry.Condition, Game1.currentLocation))
                    {
                        Item result = ItemQueryResolver.TryResolveRandomItem(entry, itemQueryContext, avoidRepeat: false);
                        selected = entry;
                        item = result;
                        break;
                    }
                }
                if (selected != null)
                {
                    break;
                }
            }
            if (item == null)
            {
                noItem++;
                continue;
            }
            if (!itemCounts.ContainsKey(item.Name))
            {
                itemCounts[item.Name] = 0;
            }
            itemCounts[item.Name]++;
        }

        List<KeyValuePair<string, int>> sortedItemCounts = itemCounts.ToList();
        sortedItemCounts.Sort((a, b) => b.Value - a.Value);

        Log.Info($"{noItem,10}\t{100.0f * noItem / rounds,10}%\tnothing found...");
        foreach(var entry in sortedItemCounts)
        {
            Log.Info($"{entry.Value,10}\t{100.0f * entry.Value / rounds,10}%\t{entry.Key}");
        }

    }
}
