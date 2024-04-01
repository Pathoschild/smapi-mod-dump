/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bitwisejon/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Linq;

namespace BitwiseJonMods.OneClickShedReloader
{
    class BuildingContentsHandler
    {
        private BuildingContentsInfo _buildingInfo;

        public BuildingContentsHandler(BuildingContentsInfo buildingInfo)
        {
            _buildingInfo = buildingInfo;
        }

        public int HarvestContents(StardewValley.Farmer player)
        {
            int numItemsHarvested = 0;

            try
            {
                //Harvest items into inventory
                foreach (var container in _buildingInfo.ReadyToHarvestContainers)
                {
                    //Get the item stored in the container
                    StardewValley.Item item = null;
                    if (container.name.Equals("Crystalarium"))
                    {
                        item = (StardewValley.Item)container.heldObject.Value.getOne();
                        if (TryAddItemToPlayerInventory(player, item, container)) numItemsHarvested++;
                    }
                    else if (container.name.Equals("Auto-Grabber"))
                    {
                        var chest = container.heldObject.Value as Chest;
                        //Netcode.NetObjectList<Item> chestItems = new Netcode.NetObjectList<Item>();
                        //chestItems.AddRange(chest.items.ToList());

                        foreach (var chestItem in chest.Items.ToList())
                        {
                            if (chestItem != null)
                            {
                                //Get stack size and add that to number of items harvested.
                                if (TryAddItemToPlayerInventory(player, chestItem, container)) numItemsHarvested += chestItem.Stack;
                            }
                        }
                    }
                    else
                    {
                        item = (StardewValley.Item)container.heldObject.Value;
                        if (TryAddItemToPlayerInventory(player, item, container)) numItemsHarvested++;
                    }
                }
            }
            catch (InventoryFullException ex)
            {
                ex.NumItemsHarvestedBeforeFull = numItemsHarvested;
                throw ex;
            }

            return numItemsHarvested;
        }

        private bool TryAddItemToPlayerInventory(Farmer player, Item item, Object container)
        {
            //Make sure player can collect item and inventory is not already full
            if (player.couldInventoryAcceptThisItem(item))
            {
                //this.Monitor.Log($"  Harvesting item {item.Name} from container {container.Name} and placing in {player.Name}'s inventory.");
                if (!player.addItemToInventoryBool(item, false))
                {
                    //Inventory was full - throw exception so we can show a message
                    Common.Utility.Log($"  {player.Name} has run out of inventory space. Stopping harvest.");
                    throw new InventoryFullException();
                }

                //Remove this item permanently from the container (except Crystalarium).
                if (container.name.Equals("Crystalarium"))
                {
                    container.MinutesUntilReady = this.getMinutesForCrystalarium(item.ParentSheetIndex);
                }
                else if (container.name.Equals("Auto-Grabber"))
                {
                    var chest = container.heldObject.Value as Chest;
                    chest.Items.Remove(item);
                    chest.clearNulls();
                }
                else
                {
                    container.heldObject.Value = (StardewValley.Object)null;
                }

                container.readyForHarvest.Value = false;
                container.showNextIndex.Value = false;
            }
            else
            {
                //Inventory was full - throw exception so we can show a message
                Common.Utility.Log($"  {player.Name} has run out of inventory space. Stopping harvest.");
                throw new InventoryFullException();
            }

            return true;
        }

        public int LoadContents(StardewValley.Farmer player)
        {
            int numItemsLoaded = 0;

            if (player.ActiveObject != null)
            {
                foreach (var container in _buildingInfo.ReadyToLoadContainers)
                {
                    if (player.ActiveObject != null)
                    {
                        //this.Monitor.Log($"  {player.Name} is holding {player.ActiveObject.Name} so placing it in container {container.Name}.");

                        //jon, 11/14/20: If the container is a cask, we need to make sure it is in a cellar. Also, the game automatically detects the player
                        //  is standing outside and rejects putting items in casks by default. So we need to fake the current location as being in that cellar
                        //  for the length of time it takes to put the objects in.
                        //Save the actual current location so we can reset it later
                        var currentLocation = player.currentLocation;

                        try
                        {
                            if (container is Cask)
                            {
                                //Make sure the container is in the cellar and move the player there temporarily (does not show on screen)
                                var homeOfFarmer = StardewValley.Utility.getHomeOfFarmer(player);
                                var cellar = Game1.getLocationFromName(homeOfFarmer.GetCellarName()) as Cellar;
                                if (cellar != null && _buildingInfo.IsCellar)
                                {
                                    player.currentLocation = cellar;
                                }
                            }

                            //Remember the held stack size so we can compare it afterwards since furnaces/charcoal kilns are weird.
                            var originalStackSize = player.ActiveObject.Stack;

                            if (container.performObjectDropInAction(player.ActiveObject, false, player))
                            {
                                player.reduceActiveItemByOne();
                                numItemsLoaded++;
                            }
                            else
                            {
                                //Furnace/charcoal kiln drop in action always returns false but since we first probed, the action should have been successful. Compare stacks to be sure.
                                if ((container.Name == "Furnace" || container.Name == "Charcoal Kiln") && player.ActiveObject.Stack < originalStackSize)
                                {
                                    numItemsLoaded++;
                                }
                                else
                                {
                                    Common.Utility.Log($"  Unable to load item. Container {container.Name} does not accept items of type {player.ActiveObject.Name}.");
                                }
                            }
                        }
                        finally
                        {
                            //Reset player's location in case it was temporarily moved for inserting to a cask
                            if (player.currentLocation != currentLocation) player.currentLocation = currentLocation;
                        }
                    }
                    else
                    {
                        Common.Utility.Log($"  {player.Name} has run out of items to load. Stopping load.");
                        break;
                    }
                }
            }
            else
            {
                Common.Utility.Log($"  {player.Name} is not holding an item, so not loading containers.");
            }

            return numItemsLoaded;
        }

        private int getMinutesForCrystalarium(int whichGem)
        {
            switch (whichGem)
            {
                case 60:
                    return 3000;
                case 62:
                    return 2240;
                case 64:
                    return 3000;
                case 66:
                    return 1360;
                case 68:
                    return 1120;
                case 70:
                    return 2400;
                case 72:
                    return 7200;
                case 80:
                    return 420;
                case 82:
                    return 1300;
                case 84:
                    return 1120;
                case 86:
                    return 800;
                default:
                    return 5000;
            }
        }
    }
}
