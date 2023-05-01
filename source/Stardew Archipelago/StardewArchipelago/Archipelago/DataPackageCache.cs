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
using StardewModdingAPI;

namespace StardewArchipelago.Archipelago
{
    public class DataPackageCache
    {
        private Dictionary<string, ArchipelagoItem> _itemCacheByName { get; }
        private Dictionary<long, ArchipelagoItem> _itemCacheById { get; }
        private Dictionary<string, ArchipelagoLocation> _locationCacheByName { get; }
        private Dictionary<long, ArchipelagoLocation> _locationCacheById { get; }

        public DataPackageCache(IModHelper helper)
        {
            var items = ArchipelagoItem.LoadItems(helper);
            var locations = ArchipelagoLocation.LoadLocations(helper);

            _itemCacheByName = new Dictionary<string, ArchipelagoItem>();
            _itemCacheById = new Dictionary<long, ArchipelagoItem>();
            _locationCacheByName = new Dictionary<string, ArchipelagoLocation>();
            _locationCacheById = new Dictionary<long, ArchipelagoLocation>();

            foreach (var archipelagoItem in items)
            {
                _itemCacheByName.Add(archipelagoItem.Name, archipelagoItem);
                _itemCacheById.Add(archipelagoItem.Id, archipelagoItem);
            }

            foreach (var archipelagoLocation in locations)
            {
                _locationCacheByName.Add(archipelagoLocation.Name, archipelagoLocation);
                _locationCacheById.Add(archipelagoLocation.Id, archipelagoLocation);
            }
        }

        public string GetLocalItemName(long itemId)
        {
            if (!_itemCacheById.ContainsKey(itemId))
            {
                return null;
            }
            return _itemCacheById[itemId].Name;
        }

        public long GetLocalItemId(string itemName)
        {
            if (!_itemCacheByName.ContainsKey(itemName))
            {
                return -1;
            }
            return _itemCacheByName[itemName].Id;
        }

        public string GetLocalLocationName(long locationId)
        {
            if (!_locationCacheById.ContainsKey(locationId))
            {
                return null;
            }
            return _locationCacheById[locationId].Name;
        }

        public long GetLocalLocationId(string locationName)
        {
            if (!_locationCacheByName.ContainsKey(locationName))
            {
                return -1;
            }
            return _locationCacheByName[locationName].Id;
        }
    }
}
