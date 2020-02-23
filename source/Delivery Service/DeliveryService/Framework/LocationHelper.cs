using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Common;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace DeliveryService.Framework
{
    static public class LocationHelper
    {
        static public bool FindAtLocation(GameLocation location, Chest chest)
        {
            if (location is FarmHouse house && Game1.player.HouseUpgradeLevel > 0)
            {
                Chest fridge = house.fridge.Value;
                if (fridge == chest)
                    return true;
            }
            Item item = location.getObjectAtTile((int)chest.TileLocation.X, (int)chest.TileLocation.Y);
            if (item != null && item is Chest tmpchest && tmpchest == chest)
                return true;
            return false;
        }
        static public ObjectLocation FindLocation(SObject obj)
        {
            foreach (GameLocation location in GetAccessibleLocations())
            {
                if (location is FarmHouse house && Game1.player.HouseUpgradeLevel > 0)
                {
                    SObject fridge = house.fridge.Value;
                    if (fridge == obj)
                        return new ObjectLocation(location, obj.TileLocation);
                }
                if (obj.TileLocation.X == 0 && obj.TileLocation.Y == 0)
                {
                    //Item has no tile, do it the hard way
                    foreach (KeyValuePair <Vector2, SObject > pair in location.Objects.Pairs)
                    {
                        if (pair.Value == obj)
                        {
                            return new ObjectLocation(location, pair.Key);
                        }
                    }
                }
                Item item = location.getObjectAtTile((int)obj.TileLocation.X, (int)obj.TileLocation.Y);
                if (item != null && item is SObject tmpobj && tmpobj == obj)
                    return new ObjectLocation(location, obj.TileLocation);
            }
            return null;
        }

        // From Pathoschild.ChestsAnywhere
        /// <summary>Get the locations which are accessible to the current player (regardless of settings).</summary>
        static public IEnumerable<GameLocation> GetAccessibleLocations()
        {
            // main player can access chests in any location
            if (Context.IsMainPlayer)
                return CommonHelper.GetLocations();

            // secondary player can only safely access chests in their current location
            // (changes to other locations aren't synced to the other players)
            return new[] { Game1.player.currentLocation };
        }
    }
}
