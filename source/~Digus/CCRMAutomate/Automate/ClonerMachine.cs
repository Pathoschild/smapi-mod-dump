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
                CustomCloner cloner = ClonerController.GetCloner(machine.Name);
                int? machineMinutesUntilReady = null;
                if (cloner.CloningDataId.ContainsKey(item.ParentSheetIndex))
                {
                    machineMinutesUntilReady = cloner.CloningDataId[item.ParentSheetIndex];
                }
                else if (cloner.CloningDataId.ContainsKey(item.Category))
                {
                    machineMinutesUntilReady = cloner.CloningDataId[item.Category];
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
            if (CCRMAutomateModEntry.ModConfig.EnableAutomateClonerInput)
            {
                SObject machine = this.Machine;
                SObject heldObject = machine.heldObject.Value;
                foreach (ITrackedStack trackedStack in input.GetItems())
                {
                    if (trackedStack.Sample is SObject objectInput
                        && !objectInput.bigCraftable.Value)
                    {
                        CustomCloner cloner = ClonerController.GetCloner(machine.Name);
                        int? machineMinutesUntilReady = null;
                        if (cloner.CloningDataId.ContainsKey(objectInput.ParentSheetIndex))
                        {
                            machineMinutesUntilReady = cloner.CloningDataId[objectInput.ParentSheetIndex];
                        }
                        else if (cloner.CloningDataId.ContainsKey(objectInput.Category))
                        {
                            machineMinutesUntilReady = cloner.CloningDataId[objectInput.Category];
                        }
                        if (machineMinutesUntilReady.HasValue)
                        {
                            machine.heldObject.Value = (SObject)objectInput.getOne();
                            machine.readyForHarvest.Value = false;
                            if (DataLoader.ModConfig.OverrideContentPackGetObjectProperties ? DataLoader.ModConfig.GetObjectBackImmediately : cloner.GetObjectBackImmediately)
                            {
                                machine.MinutesUntilReady = 0;
                                machine.minutesElapsed(0, Location);
                            }
                            else
                            {
                                machine.MinutesUntilReady = machineMinutesUntilReady.Value;
                            }
                            machine.initializeLightSource(machine.TileLocation, false);
                            trackedStack.Reduce(1);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
