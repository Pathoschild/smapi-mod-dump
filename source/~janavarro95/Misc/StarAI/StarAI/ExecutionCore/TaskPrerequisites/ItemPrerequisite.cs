using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ExecutionCore.TaskPrerequisites
{
    public class ItemPrerequisite:GenericPrerequisite
    {
        public Item item;
        public int amount;

        public ItemPrerequisite(Item DesiredItem, int Amount)
        {
            this.item = DesiredItem;
            this.amount = Amount;
        }

        public override bool checkAllPrerequisites()
        {
            if (this.item == null || this.amount == 0) return true;
            if (doesPlayerInventoryContainMe() == false) return false;
            if (doesPlayerHaveEnoughOfMe() == false) return false;
            return true;
        }

        public bool doesPlayerInventoryContainMe()
        {
            foreach(var item in Game1.player.items)
            {
                if (item == null) continue;
                if (isItemSameTypeAsMe(item)) return true;
            }
            return false;
        }

        public bool isItemSameTypeAsMe(Item I)
        {
            if (I.GetType() == this.item.GetType()) return true;
            else return false;
        }

        public bool doesPlayerHaveEnoughOfMe()
        {
            foreach (var item in Game1.player.items)
            {
                if (item == null) continue;
                if (isItemSameTypeAsMe(item)&&item.Stack>=amount) return true;
            }
            return false;
        }

    }
}
