using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ExecutionCore.TaskPrerequisites
{
    public class ToolPrerequisite:GenericPrerequisite
    {
        public bool requiresTool;
        public Type requiredTool;
        public int estimatedNumberOfUses;

        public ToolPrerequisite(bool TaskNeedsTool, Type RequiredTool, int EstimatedNumberOfUses)
        {
            requiresTool = TaskNeedsTool;
            requiredTool = RequiredTool;
            this.estimatedNumberOfUses = EstimatedNumberOfUses;
            verifyToolSetUp();
        }

        public void verifyToolSetUp()
        {
            if (requiresTool == false)
            {
                requiredTool = null;
                estimatedNumberOfUses = 0;
            }
        }

        public bool isToolInInventory()
        {
            if (requiresTool == false) return true;
            foreach (var item in Game1.player.items) {
                Type t = requiredTool.GetType();
                if ( item.GetType()==requiredTool) return true;
            }
            return false;
        }

        public override bool checkAllPrerequisites()
        {
            if (isToolInInventory()) return true;
            else
            {
                ModCore.CoreMonitor.Log("A task failed due to not having the required tool: "+this.requiredTool);
                return false;
            }
        }
        //istoolinanychests???? Needs function to be able to request an item from any given chest and go fetch it which can require moving across maps.
        
    }
}
