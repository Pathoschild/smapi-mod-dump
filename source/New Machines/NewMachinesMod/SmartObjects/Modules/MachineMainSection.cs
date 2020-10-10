/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;
using Microsoft.Xna.Framework;
using StardewValley;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Modules
{
    public abstract class MachineMainSection : CustomMachineBase
    {
        protected MachineMainSection(int id) : base(id) { }

        protected IReadOnlyList<DynamicID<CraftableID>> AllowedModules => MachineInformation.AllowedModules;

        protected MachineModule GetModule(Farmer farmer)
        {
            Object o;
            return farmer.currentLocation.Objects.TryGetValue(TileLocation + new Vector2(1, 0), out o) 
                && o.bigCraftable 
                && AllowedModules.Any(m => m == o.ParentSheetIndex)? o as MachineModule : null;
        }

        protected override bool CanPerformDropIn(Object item, Farmer farmer)
        {
            var module = GetModule(farmer);
            return module?.CanModuleProcessItem(item, farmer) ?? true;
        }

        protected override bool PerformDropIn(Object dropInItem, Farmer farmer)
        {
            var module = GetModule(farmer);
            if (module == null)
            {
                ShowRedMessage(farmer, "Requires additional module before usage.");
                return false;
            }
            if (Output.Animation != null) PlayAnimation(farmer, Output.Animation.Value);
            return module.ModuleProcessItem(dropInItem, farmer);
        }
    }
}