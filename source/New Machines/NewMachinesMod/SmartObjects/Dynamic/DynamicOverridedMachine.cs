using System.Linq;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.NewMachinesMod.Data;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Dynamic
{
    public sealed class DynamicOverridedMachine : OverridedMachineBase, IDynamic
    {
        public int DynamicClassID { get; }

        public DynamicOverridedMachine(int dynamicClassID) : base(dynamicClassID)
        {
            DynamicClassID = dynamicClassID;
        }

        protected override OverridedMachineInformation MachineInformation => NewMachinesMod.Config.MachineOverrides.First(m => m.ID == DynamicClassID);
    }
}
