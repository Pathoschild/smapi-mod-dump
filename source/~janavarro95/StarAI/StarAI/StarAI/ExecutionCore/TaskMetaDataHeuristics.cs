using StarAI.ExecutionCore.TaskPrerequisites;
using StarAI.PathFindingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ExecutionCore
{
    /// <summary>
    /// This will be used to determine how much any given action, pathing distance, etc will have on the overall cost of a given task.
    /// </summary>
   public class TaskMetaDataHeuristics
    {

        /// <summary>
        /// Multiplier to be used to multiply out pathCost on any given action. Higher numbers will mean more discrimination against actions with long manhattan distances.
        /// </summary>
        public static int pathCostMultiplier=1;
        /// <summary>
        /// This is a dictionary that holds the action cost for every tool when it is used.
        /// </summary>
        public static Dictionary<Type, int> toolCostDictionary = new Dictionary<Type, int>();

        /// <summary>
        /// Used to set the values at the beginning.
        /// </summary>
        public static void initializeToolCostDictionary()
        {
            toolCostDictionary.Add(typeof(StardewValley.Tools.WateringCan), 2);
            toolCostDictionary.Add(typeof(StardewValley.Tools.Axe), 4);
            toolCostDictionary.Add(typeof(StardewValley.Tools.Pickaxe), 3);
            toolCostDictionary.Add(typeof(StardewValley.Tools.FishingRod), 5);
            toolCostDictionary.Add(typeof(StardewValley.Tools.Hoe), 2);
            toolCostDictionary.Add(typeof(StardewValley.Tools.MeleeWeapon), 1);
            toolCostDictionary.Add(typeof(StardewValley.Tools.Sword), 1);
        }

        /// <summary>
        /// Used to assign a weight to using a tool a single time.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int parseToolCostMultiplier(TaskPrerequisites.ToolPrerequisite t)
        {
            Type tool = t.requiredTool;
            int value=2;
            bool f = toolCostDictionary.TryGetValue(tool,out value);
            if (f == true) return value;
            else return 2;
        } 

        /// <summary>
        /// Used to calculate the weight of using a tool to add to the overall cost of a TaskMetaData cost.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="numberOfTimesToolIsUsed"></param>
        /// <returns></returns>
        public static int calculateToolCostMultiplier(TaskPrerequisites.ToolPrerequisite t)
        {
            if (t.requiresTool == false || t.requiredTool==null) return 0; //Default tool not used.
            return (parseToolCostMultiplier(t) * t.estimatedNumberOfUses);
        }

        public static float calculateTaskCost(TileNode v,ToolPrerequisite t)
        {
          float pathCost= StarAI.PathFindingCore.Utilities.calculatePathCost(v) * pathCostMultiplier;
            float toolCost = calculateToolCostMultiplier(t);
            return (pathCost + toolCost);  
        }

    }
}
