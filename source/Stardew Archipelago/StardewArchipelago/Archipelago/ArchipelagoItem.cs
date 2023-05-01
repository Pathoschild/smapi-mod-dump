/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace StardewArchipelago.Archipelago
{
    public class ArchipelagoItem
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public ItemClassification Classification { get; set; }

        public ArchipelagoItem(string name, long id, ItemClassification classification)
        {
            Name = name;
            Id = id;
            Classification = classification;
        }

        public static IEnumerable<ArchipelagoItem> LoadItems(IModHelper helper)
        {
            var pathToItemTable = Path.Combine("IdTables", "stardew_valley_item_table.json");
            var itemsTable = helper.Data.ReadJsonFile<Dictionary<string, JObject>>(pathToItemTable);
            var items = itemsTable["items"];
            foreach (var (key, jEntry) in items)
            {
                yield return LoadItem(key, jEntry);
            }
        }

        private static ArchipelagoItem LoadItem(string itemName, JToken itemJson)
        {
            var id = itemJson["code"].Value<long>();
            var classification = Enum.Parse<ItemClassification>(itemJson["classification"].Value<string>(), true);
            var item = new ArchipelagoItem(itemName, id, classification);
            return item;
        }
    }

    public enum ItemClassification
    {
        Progression,
        Useful,
        Filler,
        Trap,
    }
}
