using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.Data;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Modules;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class Fermenter : MachineModule
    {
        public Fermenter() : base(ClassMapperService.Instance.GetCraftableID<Fermenter>()) { }
        protected override MachineInformation MachineInformation => NewMachinesMod.Config.Fermenter;
    }
}