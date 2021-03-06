/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace StackToMapChests
{
    public class ModEntry : Mod
    {
        private Dictionary<GameLocation, string> Zones;

        public override void Entry(IModHelper helper)
        {
            Helper.Events.Input.ButtonPressed += OnButtonPress;
            Helper.Events.GameLoop.DayStarted += OnDayStart;
        }

        private void LogDebug(string message)
        {
            Monitor.Log(message, LogLevel.Trace);
        }

        private void OnDayStart(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            var zoneAreas = Helper.Data.ReadJsonFile<ModData>("assets/ModData.json").ZoneAreas;
            Zones = new Dictionary<GameLocation, string>();

            foreach (var loc in Game1.locations)
            {
                var zone = zoneAreas.Where(area => area.Value.Contains(loc.Name)).Select(area => area.Key).FirstOrDefault() ?? loc.Name;
                Zones.Add(loc, zone);
                LogDebug($"Zoned {loc.Name} as {zone}");
                if (loc is BuildableGameLocation buildableLoc)
                {
                    foreach (var building in buildableLoc.buildings.Where(building => building.indoors.Value != null))
                    {
                        Zones.Add(building.indoors.Value, zone);
                        LogDebug($"Zoned interior {building.indoors.Value.Name} as {zone}");
                    }
                }
            }
        }

        private void OnButtonPress(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!(Game1.activeClickableMenu != null && Game1.activeClickableMenu is GameMenu)) return;

            // get all chests in location, enumerate through Objects in inventory (exclude tools and equipment), get priority system for chests
            // priority: if chest is in current location
            if (e.Button == SButton.Q)
            {
                var currentLocation = Game1.currentLocation;
                var currentZone = Zones[currentLocation];
                LogDebug($"Current location: {currentLocation.Name}, current zone: {currentZone}");
                var accessibleLocations = Zones.Where(zone => zone.Value == currentZone).Select(zone => zone.Key).ToList();

                List<ManagedChest> chests = new List<ManagedChest>();
                foreach (var loc in accessibleLocations)
                {
                    // chests
                    foreach (var pair in loc.Objects.Pairs)
                    {
                        var obj = pair.Value;
                        if (obj is Chest chest && chest.playerChest.Value)
                        {
                            chests.Add(
                                new ManagedChest()
                                {
                                    Chest = chest,
                                    Tile = pair.Key,
                                    Location = loc
                                });
                        }
                    }

                    // fridges
                    if (loc is FarmHouse farmhouse && farmhouse.fridgePosition != Point.Zero) 
                    {
                        var fridge = farmhouse.fridge.Value;
                        if (fridge != null)
                        {
                            chests.Add(
                                new ManagedChest()
                                {
                                    Chest = fridge,
                                    Tile = new Vector2(farmhouse.fridgePosition.X, farmhouse.fridgePosition.Y),
                                    Location = farmhouse
                                });
                        }
                    } 
                    else if (loc is IslandFarmHouse islandFarmhouse && islandFarmhouse.visited.Value && islandFarmhouse.fridgePosition != Point.Zero)
                    {
                        var fridge = islandFarmhouse.fridge.Value;
                        if (fridge != null)
                        {
                            chests.Add(
                                new ManagedChest()
                                {
                                    Chest = fridge,
                                    Tile = new Vector2(islandFarmhouse.fridgePosition.X, islandFarmhouse.fridgePosition.Y),
                                    Location = islandFarmhouse
                                });
                        }
                    }
                }

                foreach (var managedChest in chests)
                {
                    LogDebug($"Found {managedChest}");
                }

                var stashablePlayerItems = Game1.player.Items.Where(item => item != null && item is Object).Select(item => item as Object);
                foreach (var obj in stashablePlayerItems)
                {
                    List<SortableChest> sortedChests = new List<SortableChest>();

                    foreach (var managedChest in chests)
                    {
                        var chest = managedChest.Chest;
                        var chestItems = chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID).Select(i => i as Object).Where(i => i != null);
                        var capacity = chest.GetActualCapacity();

                        var sortableChest = new SortableChest()
                        {
                            ManagedChest = managedChest,
                            StackableWithScore = 0,
                            SameIdScore = 0,
                            ExistingAmountScore = 0,
                            SameCategoryScore = 0
                        };
                        foreach (var item in chestItems)
                        {
                            if (item.Category == obj.Category) sortableChest.SameCategoryScore++;
                            if (item.canStackWith(obj)) sortableChest.StackableWithScore++;
                            if (item.ParentSheetIndex == obj.ParentSheetIndex && item.bigCraftable.Value == obj.bigCraftable.Value)
                            {
                                sortableChest.SameIdScore++;
                                sortableChest.ExistingAmountScore += item.Stack;
                            }
                        }
                        sortedChests.Add(sortableChest);
                    }

                    sortedChests.Sort();
                    
                    foreach (var sortedChest in sortedChests)
                    {
                        var managedChest = sortedChest.ManagedChest;
                        var chest = managedChest.Chest;
                        LogDebug($"Stashing {obj.Name} ({obj.Quality}) to {managedChest}");
                        LogDebug($"Score: {sortedChest.Score}");
                        var outcome = chest.addItem(obj);
                        if (outcome == null)
                        {
                            Game1.player.removeItemFromInventory(obj);
                            break;
                        }
                    }
                }

                Game1.addHUDMessage(new HUDMessage("Stashed away inventory items to chests.", null));
            }
            else if (e.Button == SButton.X)
            {
                // take items from nearby auto grabbers and put in inventory
                // see Chest::grabItemFromChest()
                // save current menu, and then set it back to active menu
            }
        }
    }
}
