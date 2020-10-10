/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bitwisejon/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using BitwiseJonMods.Common;


namespace BitwiseJonMods
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

            //Harvest items into inventory
            foreach (var container in _buildingInfo.ReadyToHarvestContainers)
            {
                //Get the item stored in the container
                StardewValley.Item item = null;
                if (container.name.Equals("Crystalarium"))
                {
                    item = (StardewValley.Item)container.heldObject.Value.getOne();
                }
                else
                { 
                    item = (StardewValley.Item)container.heldObject.Value;
                }

                //Make sure player can collect item and inventory is not already full
                if (player.couldInventoryAcceptThisItem(item))
                {
                    //this.Monitor.Log($"  Harvesting item {item.Name} from container {container.Name} and placing in {player.Name}'s inventory.");
                    if (!player.addItemToInventoryBool(item, false))
                    {
                        //Inventory was full - throw exception so we can show a message
                        Utility.Log($"  {player.Name} has run out of inventory space. Stopping harvest.");
                        throw new InventoryFullException();
                    }
                    numItemsHarvested++;

                    //Remove this item permanently from the container (except Crystalarium).
                    if (container.name.Equals("Crystalarium"))
                    {
                        container.MinutesUntilReady = this.getMinutesForCrystalarium(item.ParentSheetIndex);
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
                    Utility.Log($"  {player.Name} has run out of inventory space. Stopping harvest.");
                    throw new InventoryFullException();
                }
            }

            return numItemsHarvested;
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
                        if (container.performObjectDropInAction(player.ActiveObject, false, player))
                        {
                            player.reduceActiveItemByOne();
                            numItemsLoaded++;
                        }
                        else
                        {
                            Utility.Log($"  Unable to load item. Container {container.Name} does not accept items of type {player.ActiveObject.Name}.");
                        }
                    }
                    else
                    {
                        Utility.Log($"  {player.Name} has run out of items to load. Stopping load.");
                        break;
                    }
                }
            }
            else
            {
                Utility.Log($"  {player.Name} is not holding an item, so not loading containers.");
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
