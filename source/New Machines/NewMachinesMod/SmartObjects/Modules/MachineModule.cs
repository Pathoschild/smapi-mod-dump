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
    public abstract class MachineModule : CustomMachineBase
    {
        protected MachineModule(int id) : base(id) { }

        protected override bool CanPerformDropIn(Object item, Farmer farmer) => false;

        public bool CanModuleProcessItem(Object item, Farmer farmer) => base.CanPerformDropIn(item, farmer);

        public bool ModuleProcessItem(Object item, Farmer farmer) => PerformDropIn(item, farmer);

        protected MachineMainSection GetSection(GameLocation location, int x, int y)
        {
            Object section;
            return location.Objects.TryGetValue(new Vector2(x - 1, y), out section) 
                && section.bigCraftable
                && AllowedSections.Any(s => s == section.ParentSheetIndex)? section as MachineMainSection : null;
        }

        public override bool canBePlacedHere(GameLocation location, Vector2 tile)
        {
            return (GetSection(location, (int)tile.X, (int)tile.Y) != null) && base.canBePlacedHere(location, tile);
        }

        protected IReadOnlyList<DynamicID<CraftableID>> AllowedSections => MachineInformation.AllowedSections;
    }
}