/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using CustomCrystalariumMod;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;
using DataLoader = CustomCrystalariumMod.DataLoader;
using StardewValley.GameData.Machines;

namespace CCRMAutomate.Automate
{
    internal class ClonerMachine : IMachine
    {
        public string MachineTypeID { get; }

        public readonly SObject Machine;
        public GameLocation Location { get; }
        public Rectangle TileArea { get; }

        public ClonerMachine(SObject machine, GameLocation location, Vector2 tile)
        {
            MachineTypeID = "CCRM.Cloner." + machine.Name;
            Machine = machine;
            Location = location;
            TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }

        public MachineState GetState()
        {
            return this.Machine.heldObject.Value == null
                ? (CCRMAutomateModEntry.ModConfig.EnableAutomateClonerInput ? MachineState.Empty : MachineState.Disabled)
                : this.Machine.readyForHarvest.Value
                    ? MachineState.Done
                    : MachineState.Processing;
        }

        public ITrackedStack GetOutput()
        {
            SObject machine = this.Machine;
            SObject heldObject = machine.heldObject.Value;
            return new TrackedItem(heldObject.getOne(), item =>
            {
                CustomCloner cloner = ClonerController.GetCloner(machine.QualifiedItemId);
                int? machineMinutesUntilReady = null;
                if (cloner.CloningDataId.TryGetValue(item.QualifiedItemId, out var value)
                    || cloner.CloningDataId.TryGetValue(item.Category.ToString(), out value))
                {
                    machineMinutesUntilReady = value;
                }
                if (machineMinutesUntilReady.HasValue)
                {
                    machine.heldObject.Value = heldObject;
                    machine.MinutesUntilReady = machineMinutesUntilReady.Value;
                    machine.initializeLightSource(machine.TileLocation, false);
                }
                else
                {
                    machine.heldObject.Value = (SObject)null;
                    machine.MinutesUntilReady = -1;
                }
                machine.readyForHarvest.Value = false;
            });
        }

        public bool SetInput(IStorage input)
        {
            if (!CCRMAutomateModEntry.ModConfig.EnableAutomateClonerInput) return false;
            SObject machine = this.Machine;
            SObject heldObject = machine.heldObject.Value;
            foreach (ITrackedStack trackedStack in input.GetItems())
            {
                if (trackedStack.Sample is not SObject objectInput) continue;
                CustomCloner cloner = ClonerController.GetCloner(machine.QualifiedItemId);
                int? machineMinutesUntilReady = null;
                if (cloner.CloningDataId.TryGetValue(objectInput.QualifiedItemId, out var value)
                    || cloner.CloningDataId.TryGetValue(objectInput.Category.ToString(), out value))
                {
                    machineMinutesUntilReady = value;
                }
                if (!machineMinutesUntilReady.HasValue) continue;
                machine.heldObject.Value = (SObject)objectInput.getOne();
                if ((ClonerController.GetCloner(machine.QualifiedItemId) != null && !ClonerController.GetCloner(machine.QualifiedItemId).KeepQuality))
                {
                    machine.heldObject.Value.Quality = 0;
                }
                machine.readyForHarvest.Value = false;
                if (DataLoader.ModConfig.OverrideContentPackGetObjectProperties ? DataLoader.ModConfig.GetObjectBackImmediately : cloner.GetObjectBackImmediately)
                {
                    machine.MinutesUntilReady = 0;
                    machine.minutesElapsed(0);
                }
                else
                {
                    machine.MinutesUntilReady = machineMinutesUntilReady.Value;
                }
                machine.lastInputItem.Value = machine.heldObject.Value.getOne();
                machine.lastInputItem.Value.Stack = 1;
                machine.initializeLightSource(machine.TileLocation, false);
                var machineData = machine.GetMachineData();
                if (machineData?.LoadEffects != null)
                {
                    foreach (MachineEffects effect in machineData.LoadEffects)
                    {
                        if (machine.PlayMachineEffect(effect, true))
                        {
                            break;
                        }
                    }
                }
                MachineDataUtility.UpdateStats(machineData?.StatsToIncrementWhenLoaded, objectInput, 1);
                trackedStack.Reduce(1);
                return true;
            }
            return false;
        }
    }
}
