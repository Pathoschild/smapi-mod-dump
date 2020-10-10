/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

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