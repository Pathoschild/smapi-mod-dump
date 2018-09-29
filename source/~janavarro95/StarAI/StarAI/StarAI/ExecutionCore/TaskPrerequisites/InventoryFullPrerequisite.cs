using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ExecutionCore.TaskPrerequisites
{
    /// <summary>
    /// Weirdly enough this will be empty because it's just a placeholder prerequisite. Doesn't need to hold any more data since the player will always have an updated inventory.
    /// </summary>
   public class InventoryFullPrerequisite:GenericPrerequisite
    {
        public bool doesTaskRequireInventorySpace;
        public InventoryFullPrerequisite(bool RequiresInventorySpace)
        {
            this.doesTaskRequireInventorySpace = RequiresInventorySpace;
        }

        public bool isPlayerInventoryFull()
        {

            return Game1.player.isInventoryFull();
        }

        public override bool checkAllPrerequisites()
        {
            if (this.doesTaskRequireInventorySpace == false) return true;
            if (isPlayerInventoryFull() == false) return true;
            else//player inventory is full
            {
                ModCore.CoreMonitor.Log("Player inventory is full failed the task prerequisite");
                return false;
            }
        }

    }
}
