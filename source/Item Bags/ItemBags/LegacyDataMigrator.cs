/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using ItemBags.Persistence;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace ItemBags
{
    /// <summary>Attempts to migrate old saved data (created by serializing items with PyTK) to newer data that can be saved by SpaceCore</summary>
    public class LegacyDataMigrator
    {
        internal static void OnModEntry(IModHelper Helper)
        {
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        /// <summary>Get all game locations.</summary>
        /// <param name="includeTempLevels">Whether to include temporary mine/dungeon locations.</param>
        public static IEnumerable<GameLocation> GetLocations(bool includeTempLevels = false)
        {
            //
            // Copied from CommonHelper in Pathoschild's repo: https://github.com/Pathoschild/StardewMods
            //

            var locations = Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );

            if (includeTempLevels)
                locations = locations.Concat(MineShaft.activeMines).Concat(VolcanoDungeon.activeLevels);

            return locations;
        }

        private static void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                foreach (var player in Game1.getAllFarmers())
                {
                    for (int i = 0; i < player.Items.Count; i++)
                        TryMigrateData(player.Items[i], replacement => player.Items[i] = replacement);

                    foreach (GameLocation location in GetLocations())
                    {
                        foreach (Vector2 key in location.Objects.Keys)
                            TryMigrateData(location.Objects[key], _ => { /* can't be placed directly on a map */ });

                        foreach (StorageFurniture furniture in location.furniture.OfType<StorageFurniture>())
                        {
                            for (int i = 0; i < furniture.heldItems.Count; i++)
                                TryMigrateData(furniture.heldItems[i], replacement => furniture.heldItems[i] = replacement);
                        }
                    }
                }
            }
        }

        private static void TryMigrateData(Item item, Action<Item> insertReplacement)
        {
            try
            {
                if (item is Chest chest)
                {
                    for (int i = 0; i < chest.items.Count; i++)
                        TryMigrateData(chest.items[i], replacement => chest.items[i] = replacement);
                }
                else if (item is SObject obj)
                {
                    if (BagInstance.TryDeserializePyTKData(item, out BagInstance bagInstance) && bagInstance.TryDecode(out ItemBag bag))
                    {
                        insertReplacement(bag);
                    }
                }
            }
            catch (Exception ex) 
            {
                string errorMsg = $"Error while attempting to migrate a PyTK saved item. Your bag saved in an older version of ItemBags 1.X.X could not be converted to newer data format, and will remain as a junk item. Error message: {ex}";
                ItemBagsMod.ModInstance.Monitor.Log(errorMsg, LogLevel.Warn); 
            }
        }
    }
}
