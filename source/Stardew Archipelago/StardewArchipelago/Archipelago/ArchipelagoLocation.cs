/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace StardewArchipelago.Archipelago
{
    public class ArchipelagoLocation
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public string Region { get; set; }

        public ArchipelagoLocation(string name, long id, string region)
        {
            Name = name;
            Id = id;
            Region = region;
        }

        public static IEnumerable<ArchipelagoLocation> LoadLocations(IModHelper helper)
        {
            var pathToLocationTable = Path.Combine("IdTables", "stardew_valley_location_table.json");
            var locationsTable = helper.Data.ReadJsonFile<Dictionary<string, JObject>>(pathToLocationTable);
            var locations = locationsTable["locations"];
            foreach (var (key, jEntry) in locations)
            {
                yield return LoadLocation(key, jEntry);
            }
        }

        private static ArchipelagoLocation LoadLocation(string locationName, JToken locationJson)
        {
            var id = locationJson["code"].Value<long>();
            var region = locationJson["region"].Value<string>();
            var location = new ArchipelagoLocation(locationName, id, region);
            return location;
        }
    }
}
