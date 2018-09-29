using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.Data;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Modules;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class Separator : MachineMainSection
    {
        public Separator() : base(ClassMapperService.Instance.GetCraftableID<Separator>()) { }
        protected override MachineInformation MachineInformation => NewMachinesMod.Config.Separator;
    }
}