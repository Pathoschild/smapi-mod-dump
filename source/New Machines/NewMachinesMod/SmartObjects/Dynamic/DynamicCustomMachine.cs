using System.Linq;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.NewMachinesMod.Data;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Dynamic
{
    public sealed class DynamicCustomMachine : CustomMachineBase, IDynamic
    {
        public int DynamicClassID { get; }

        public DynamicCustomMachine(int dynamicClassID) : base(dynamicClassID)
        {
            DynamicClassID = dynamicClassID;
        }

        protected override MachineInformation MachineInformation => NewMachinesMod.Config.SimpleMachines.First(m => m.ID == DynamicClassID);
    }
}
