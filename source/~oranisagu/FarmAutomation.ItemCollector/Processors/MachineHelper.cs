using System.Linq;
using FarmAutomation.Common;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace FarmAutomation.ItemCollector.Processors
{
    public static class MachineHelper
    {
        public const int ChestMaxItems = 36;
        private static GhostFarmer _who;
        public static GhostFarmer Who => _who ?? (_who = GhostFarmer.CreateFarmer());

        public static void DailyReset()
        {
            _who = null;
        }

        public static void ProcessMachine(Object machine, Chest connectedChest, MaterialHelper materialHelper)
        {
            if (connectedChest.items.Any(i => i == null))
            {
                connectedChest.items.RemoveAll(i => i == null);
            }
            if (MachineIsReadyForHarvest(machine))
            {
                if (connectedChest.items.Count >= ChestMaxItems)
                {
                    Log.Error($"Your Chest in is already full, can't process the {machine.Name} as the item would get lost.");
                    return;
                }
                HandleFinishedObjectInMachine(machine, connectedChest);
            }
            if (MachineIsReadyForProcessing(machine))
            {
                var refillable = materialHelper.FindMaterialForMachine(machine.Name, connectedChest);
                Object coal = null;
                if (machine.Name == "Furnace")
                {
                    coal = materialHelper.FindMaterialForMachine("Coal", connectedChest);
                    if (coal == null)
                    {
                        //no coal to power the furnace
                        return;
                    }
                }
                if (refillable != null)
                {
                    // furnace needs an additional coal
                    if (machine.Name == "Furnace")
                    {
                        var coalAmount = materialHelper.GetMaterialAmountForMachine(machine.Name, coal);
                        MoveItemToFarmer(coal, connectedChest, Who, coalAmount);
                    }

                    var materialAmount = materialHelper.GetMaterialAmountForMachine(machine.Name, refillable);
                    if (materialAmount > refillable.Stack)
                    {
                        return;
                    }
                    var tempRefillable = MoveItemToFarmer(refillable, connectedChest, Who, materialAmount);

                    if (!PutItemInMachine(machine, tempRefillable, Who))
                    {
                        // item was not accepted by the machine, transfer it back to the chest
                        Who.items.ForEach(i => connectedChest.addItem(i));
                    }
                    else
                    {
                        Log.Info($"Refilled your {machine.Name} with a {refillable.Name} of {(ItemQuality)refillable.quality} quality. The machine now takes {machine.minutesUntilReady} minutes to process. You have {refillable.Stack} {refillable.Name} left");
                    }
                    Who.ClearInventory();
                }
            }
        }

        private static Object MoveItemToFarmer(Object itemToMove, Chest sourceChest, Farmer target, int amount)
        {
            var temporaryItem = (Object)itemToMove.getOne();
            temporaryItem.Stack = amount;
            var freeIndex = target.items.IndexOf(null);
            target.items[freeIndex] = temporaryItem;
            ItemHelper.RemoveItemFromChest(itemToMove, sourceChest, amount);
            return temporaryItem;
        }

        public static void HandleFinishedObjectInMachine(Object machine, Chest connectedChest)
        {
            var logMessage = $"Collecting a {machine.heldObject?.Name} from your {machine.Name}.";
            if (connectedChest.items.Count > ChestMaxItems)
            {
                Log.Error($"Your chest is already full. Cannot place item from {machine.Name} into it.");
                return;
            }
            machine.checkForAction(Who);
            Who.items.ForEach(i =>
            {
                if (i != null)
                {
                    var result = connectedChest.addItem(i);
                    if (result != null)
                    {
                        Game1.player.addItemToInventory(result);
                    }
                }
            });

            if (machine.heldObject != null && machine.minutesUntilReady > 0)
            {
                logMessage += $" The next {machine.heldObject.Name} will be ready in {machine.minutesUntilReady}";
            }
            Who.ClearInventory();
            Log.Info(logMessage);
        }

        public static bool MachineIsReadyForHarvest(Object machine)
        {
            return machine.readyForHarvest;
        }


        public static bool MachineIsReadyForProcessing(Object machine)
        {
            return !(machine is Chest) && machine.minutesUntilReady == 0 && machine.heldObject == null;
        }


        public static bool PutItemInMachine(Object machine, Object refillable, Farmer who)
        {
            return machine.performObjectDropInAction(refillable, false, who);
        }
    }
}