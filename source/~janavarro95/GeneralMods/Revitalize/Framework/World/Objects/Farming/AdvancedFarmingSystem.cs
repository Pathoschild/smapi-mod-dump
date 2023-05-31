/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Resources;
using Omegasis.Revitalize.Framework.World.Objects.SupportClasses;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace Omegasis.Revitalize.Framework.World.Objects.Farming
{
    //TODO: CREATE OBJECT GRAPHIC OBJECT IN OBJECTMANAGER, AND FINISH PLANTING LOGIC.
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Farming.AdvancedFarmingSystem")]
    /// <summary>
    /// Plants seeds, fertilizes, and harvests crops from irrigated watering pots that have the proper attachments!
    /// 
    /// </summary>
    public class AdvancedFarmingSystem : CustomObject
    {

        //public readonly NetRef<ChestFunctionality> inventory = new NetRef<ChestFunctionality>();

        public AdvancedFarmingSystem()
        {

        }

        public AdvancedFarmingSystem(BasicItemInformation Info) : base(Info)
        {
        }

        public AdvancedFarmingSystem(BasicItemInformation Info, Vector2 TilePosition) : base(Info, TilePosition)
        {
        }

        /// <summary>
        /// When the chair is right clicked ensure that all pieces associated with it are also rotated.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            this.doWorkOnIrrigatedWaterPots(who);
            SoundUtilities.PlaySound(Enums.StardewSound.Ship);
            return true;
        }

        public override void doActualDayUpdateLogic(GameLocation location)
        {
            this.doWorkOnIrrigatedWaterPots(null);
        }

        public override void resetOnPlayerEntry(GameLocation environment, bool dropDown = false)
        {
            base.resetOnPlayerEntry(environment, dropDown);
            this.doWorkOnIrrigatedWaterPots(Game1.player);
        }

        public virtual void doWorkOnIrrigatedWaterPots(Farmer farmerActuallyDoingWork)
        {
            Dictionary<Vector2, StardewValley.Object> connectedObjects = WorldUtilities.ObjectUtilities.GetAllConnectedObjectsStartingAtTilePosition(this.getCurrentLocation(), this.TileLocation);


            List<IrrigatedGardenPot> gardenPots = new List<IrrigatedGardenPot>();
            List<ResourceBush> resourceBushes = new List<ResourceBush>();
            List<Chest> chests = new List<Chest>();

            foreach (KeyValuePair<Vector2, StardewValley.Object> tileToObject in connectedObjects)
            {

                //Filter out unnecessary items.
                if (tileToObject.Value is Chest)
                {
                    chests.Add((Chest)tileToObject.Value);
                }

                if (tileToObject.Value is IrrigatedGardenPot)
                {
                    gardenPots.Add((IrrigatedGardenPot)tileToObject.Value);
                }

                if(tileToObject.Value is ResourceBush)
                {
                    resourceBushes.Add((ResourceBush)tileToObject.Value);
                }
            }

            Queue<Item> itemsToPutIntoChestsOrDropToGround = new Queue<Item>();

            //This will only output to chests, it is up to the player to decide what to do from there, or if Automate is installed, then Automate will take over with it's processing system.
            foreach (IrrigatedGardenPot gardenPot in gardenPots)
            {

                if (gardenPot.hasAutoHarvestAttachment && gardenPot.Crop != null && gardenPot.isCropReadyForHarvest()==true)
                {
                    //find the first chest that has an inventory that can accept this crop and then break.
                    //Will probably need to adjust my code that adds items to the player's inventory to do the same with other inventories.

                    //Need to have a harvest crop method on the irrigated watering pot.

                    //List of harvested items, but need to make sure chests can actually accept them.

                    List<Item> harvestedItems = gardenPot.harvest();

                    foreach (Item item2 in harvestedItems)
                    {
                        itemsToPutIntoChestsOrDropToGround.Enqueue(item2);
                    }
                }

                if (gardenPot.Crop != null)
                {
                    //if pot has a crop that is not a seed and not ready to be harvested, it can be skipped!
                    if (gardenPot.Crop.currentPhase.Value != 0 && gardenPot.isCropReadyForHarvest() == false)
                    {
                        continue;
                    }
                    //if pot has a crop that is a seed and is fertilized, it can be skipped!
                    if (gardenPot.Crop.currentPhase.Value == 0 && gardenPot.hoeDirt.Value.fertilizer.Value != 0)
                    {
                        continue;
                    }
                }


                foreach (Chest chest in chests)
                {
                    //Only grab items from regular chests.
                    if (chest.SpecialChestType != Chest.SpecialChestTypes.None)
                    {
                        continue;
                    }

                    if (gardenPot.Crop != null)
                    {
                        //if pot has a crop that is not a seed and not ready to be harvested, it can be skipped!
                        if (gardenPot.Crop.currentPhase.Value != 0 && gardenPot.isCropReadyForHarvest() == false)
                        {
                            continue;
                        }
                        //if pot has a crop that is a seed and is fertilized, it can be skipped!
                        if (gardenPot.Crop.currentPhase.Value == 0 && gardenPot.hoeDirt.Value.fertilizer.Value != 0)
                        {
                            continue;
                        }
                    }

                    for (int i = 0; i < chest.GetActualCapacity(); i++)
                    {

                        if (i >= chest.items.Count)
                        {
                            break;
                        }

                        Item item = chest.items[i];


                        if (item == null) continue;

                        if (gardenPot.hasEnricherAttachment && gardenPot.Fertilizer == 0 && item.Category == StardewValley.Object.fertilizerCategory)
                        {
                            //find the first chest with fertilizer and use here only if crop is null, or seeds are in the first stage.
                            gardenPot.plant(item.parentSheetIndex, (int)gardenPot.tileLocation.X, (int)gardenPot.tileLocation.Y, this.getOwner(), farmerActuallyDoingWork, true, this.getCurrentLocation());
                            ObjectUtilities.ReduceChestItemStackByAmountAndRemoveIfNecessary(chest, i, 1);
                        }
                        if (gardenPot.hasPlanterAttachment && gardenPot.Crop == null && item.Category == StardewValley.Object.SeedsCategory)
                        {
                            gardenPot.plant(item.parentSheetIndex, (int)gardenPot.tileLocation.X, (int)gardenPot.tileLocation.Y, this.getOwner(), farmerActuallyDoingWork, false, this.getCurrentLocation());
                            ObjectUtilities.ReduceChestItemStackByAmountAndRemoveIfNecessary(chest, i,1);
                        }
                    }

                }
            }

            foreach(ResourceBush resourceBush in resourceBushes)
            {
                if (this.heldObject.Value != null)
                {
                    itemsToPutIntoChestsOrDropToGround.Enqueue(resourceBush.heldObject.Value);
                    resourceBush.clearHeldObject();
                }
            }

            //Try to put all harvest items into chests or drop them to the ground.
            while (itemsToPutIntoChestsOrDropToGround.Count > 0)
            {

                Item item = itemsToPutIntoChestsOrDropToGround.Dequeue();
                bool added = false;
                foreach (Chest chest in chests)
                {
                    added = ObjectUtilities.addItemToChest(chest, item);
                    if (added) break;
                }
                if (added == true)
                {
                    continue;
                }
                else
                {
                    WorldUtility.CreateItemDebrisAtTileLocation(this.getCurrentLocation(), item, this.TileLocation);
                }

            }



        }


        public override Item getOne()
        {
            AdvancedFarmingSystem component = new AdvancedFarmingSystem(this.getItemInformation().Copy());
            return component;
        }
    }
}

