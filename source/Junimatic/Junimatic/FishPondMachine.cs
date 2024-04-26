/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;

namespace NermNermNerm.Junimatic
{
    public class FishPondMachine
        : BuildingMachine
    {
        public FishPondMachine(FishPond fishPond, Point accessPoint)
            : base(fishPond, accessPoint)
        { }

        public new FishPond Building => (FishPond)base.Building;

        public override bool IsIdle => false;

        public override bool FillMachineFromChest(GameStorage storage)
        {
            // IsIdle being hard-coded to false should prevent this from being called.
            throw new NotImplementedException();
        }

        public override bool FillMachineFromInventory(Inventory inventory)
        {
            // IsIdle being hard-coded to false should prevent this from being called.
            throw new NotImplementedException();
        }

        public override List<Item>? GetRecipeFromChest(GameStorage storage)
        {
            // IsIdle being hard-coded to false should prevent this from being called.
            throw new NotImplementedException();
        }

        public override bool IsCompatibleWithJunimo(JunimoType projectType)
        {
            return projectType == JunimoType.Fishing;
        }
        public override StardewValley.Object? HeldObject => this.Building.output.Value as StardewValley.Object;

        protected override StardewValley.Object TakeItemFromMachine()
        {
            var oldValue = this.Building.output.Value;
            this.Building.output.Value = null;
            return (StardewValley.Object)oldValue;
        }
    }
}
