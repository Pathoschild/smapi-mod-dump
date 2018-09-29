using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Events;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace FarmAutomation.Common
{
    public static class ItemFinder
    {
        public static List<string> ConnectorItems { get; set; }

        public static List<int> ConnectorFloorings { get; set; }

        public static IEnumerable<Object> FindItemsofType(GameLocation location, Type itemType)
        {
            return location?.objects.Values.Where(o => o.GetType() == itemType);
        }

        public static IEnumerable<KeyValuePair<Vector2, Object>> FindObjectsWithName(GameLocation location, IEnumerable<string> names)
        {
            return location?.objects.Where(o => names.Contains(o.Value.Name));
        }

        public static Chest FindChestInLocation(GameLocation location)
        {
            if (location == null)
            {
                return null;
            }
            return FindItemsofType(location, typeof(Chest)).FirstOrDefault() as Chest;
        }

        public static void FindConnectedLocations(GameLocation location, Vector2 startPosition,
            List<ConnectedTile> processedLocations, bool allowDiagonals)
        {
            var adjecantTiles = GetAdjecantTiles(location, startPosition, allowDiagonals);
            foreach (var adjecantTile in adjecantTiles.Where(t => processedLocations.All(l => l.Location != t)))
            {
                if (location.objects.ContainsKey(adjecantTile) && (location.objects[adjecantTile] is Chest || ConnectorItems.Contains(location.objects[adjecantTile].Name)))
                {
                    var connectedTile = new ConnectedTile
                    {
                        Location = adjecantTile,
                    };
                    var chest = location.objects[adjecantTile] as Chest;
                    if (chest != null)
                    {
                        connectedTile.Chest = chest;
                    }
                    else
                    {
                        connectedTile.Object = location.objects[adjecantTile];
                    }
                    processedLocations.Add(connectedTile);
                    FindConnectedLocations(location, adjecantTile, processedLocations, allowDiagonals);

                }
                else if (location.terrainFeatures.ContainsKey(adjecantTile))
                {
                    var feature = location.terrainFeatures[adjecantTile] as Flooring;
                    if (feature == null)
                    {
                        continue;
                    }
                    if (ConnectorFloorings.Contains(feature.whichFloor))
                    {
                        processedLocations.Add(new ConnectedTile { Location = adjecantTile });
                        FindConnectedLocations(location, adjecantTile, processedLocations, allowDiagonals);
                    }
                }
            }
        }


        private static IEnumerable<Vector2> GetAdjecantTiles(GameLocation location, Vector2 startPosition, bool allowDiagonals = false)
        {
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    if (!allowDiagonals && (y == x || y == -x))
                    {
                        //ignore diagonals
                        continue;
                    }
                    var vector = new Vector2(startPosition.X + x, startPosition.Y + y);
                    if (vector != startPosition && LocationHelper.IsTileOnMap(location, vector))
                    {
                        yield return vector;
                    }
                }
            }
        }

        public static bool HaveConnectorsInInventoryChanged(EventArgsInventoryChanged inventoryChange)
        {
            var changes = inventoryChange.Added.Concat(inventoryChange.QuantityChanged).Concat(inventoryChange.Removed);
            if (changes.Any(i => ConnectorItems.Contains(i.Item.Name) || i.Item is Chest || i.Item.category == Object.furnitureCategory))
            {
                return true;
            }
            return false;
        }
    }
}
