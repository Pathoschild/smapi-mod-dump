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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Inventories;
using StardewValley.Objects;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Junimatic
{
    internal class CrabPotMachine
        : ObjectMachine
    {
        internal CrabPotMachine(CrabPot machine, Point accessPoint)
            : base(machine, accessPoint)
        {
        }

        public new CrabPot Machine => (CrabPot)base.Machine;

        public override bool IsIdle => this.Machine.heldObject.Value is null && this.Machine.bait.Value is null;

        public override List<Item>? GetRecipeFromChest(GameStorage storage)
        {
            // TODO? Crab pots have the notion of an owner and the owner may or may not have the profession that
            //  makes it so that the traps don't need bait.  This ignores that.  Perhaps it's actually by-design
            //  since we're not, granting farming XP and so forth when a Junimo does the trapping.  Perhaps the
            //  Junimo can be said to not have the ability, and so the owner doesn't matter at all.

            foreach (var item in storage.RawInventory)
            {
                if (item.TypeDefinitionId == I(ItemRegistry.type_object) && item.Category == StardewValley.Object.baitCategory && item.Stack > 0)
                {
                    return new List<Item> { ItemRegistry.Create(item.QualifiedItemId) };
                }
            }

            return null;
        }

        protected override StardewValley.Object TakeItemFromMachine()
        {
            var oldHeldObject = this.Machine.heldObject.Value;
            this.Machine.heldObject.Value = null;
            this.Machine.readyForHarvest.Value = false;
            this.Machine.tileIndexToShow = 710;
            this.Machine.bait.Value = null;
            this.Machine.lidFlapping = true;
            this.Machine.lidFlapTimer = 60f;
            this.Machine.shake = Vector2.Zero;
            this.Machine.shakeTimer = 0f;
            return oldHeldObject;
        }

        public override bool FillMachineFromChest(GameStorage storage)
        {
            if (!base.FillMachineFromChest(storage))
            {
                return false;
            }

            // The game code seems to be busted.  It does properly fill the machine, but it doesn't
            // remove the items.
            var inventoryStack = storage.RawInventory.First(i => i.QualifiedItemId == this.Machine.bait.Value.QualifiedItemId);
            if (inventoryStack.Stack > 1)
            {
                --inventoryStack.Stack;
            }
            else
            {
                storage.RawInventory.Remove(inventoryStack);
            }
            return true;
        }

        public override bool FillMachineFromInventory(Inventory inventory)
        {
            if (!base.FillMachineFromInventory(inventory))
            {
                return false;
            }


            // The game code seems to be busted.  It does properly fill the machine, but it doesn't
            // remove the items.
            var inventoryStack = inventory.First(i => i.QualifiedItemId == this.Machine.bait.Value.QualifiedItemId);
            if (inventoryStack.Stack > 1)
            {
                --inventoryStack.Stack;
            }
            else
            {
                inventory.Remove(inventoryStack);
            }
            return true;
        }
    }
}
