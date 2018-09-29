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

        public static KeyValuePair<int,List<TileNode>> calculateTaskCost(TileNode v,ToolPrerequisite t,bool unknownPath)
        {
           //if(unknownPath) PathFindingLogic.delay = 18;
           // else PathFindingLogic.delay = 0;
            List<TileNode> idealPath = StarAI.PathFindingCore.Utilities.getIdealPath(v);
            int costCalculation;

            if (idealPath.Count == 0) costCalculation = Int32.MaxValue;
            else costCalculation = idealPath.Count;

            if (costCalculation == Int32.MaxValue) return  new KeyValuePair<int, List<TileNode>>(Int32.MaxValue,new List<TileNode>());
            
            float pathCost=  costCalculation* pathCostMultiplier;
            float toolCost = calculateToolCostMultiplier(t);
            return new KeyValuePair<int,List<TileNode>>(((int)pathCost + (int)toolCost),idealPath);  
        }

        public static KeyValuePair<int, List<TileNode>> calculateTaskCost(List<TileNode> v, ToolPrerequisite t, bool unknownPath)
        {
            //if (unknownPath) PathFindingLogic.delay = 18;
            //else PathFindingLogic.delay = 0;
            List<TileNode> idealPath = StarAI.PathFindingCore.Utilities.getIdealPath(v);
            int costCalculation;

            if (idealPath.Count == 0) costCalculation = Int32.MaxValue;
            else costCalculation = idealPath.Count;

            if (costCalculation == Int32.MaxValue) return new KeyValuePair<int, List<TileNode>>(Int32.MaxValue, new List<TileNode>());

            float pathCost = costCalculation * pathCostMultiplier;
            float toolCost = calculateToolCostMultiplier(t);
            return new KeyValuePair<int, List<TileNode>>(((int)pathCost + (int)toolCost), idealPath);
        }

        public static KeyValuePair<int, List<List<TileNode>>> calculateTaskCost(List<List<TileNode>> v, ToolPrerequisite t, bool unknownPath)
        {
            //if (unknownPath) PathFindingLogic.delay = 18;
            //else PathFindingLogic.delay = 0;
            float totalCalculation = 0;
            List < List < TileNode >> idealPaths=new List<List<TileNode>>();
            foreach (var path in v)
            {
                ModCore.CoreMonitor.Log("HMMM" + path.ElementAt(0).thisLocation.name.ToString());
                ModCore.CoreMonitor.Log("HMMM" + path.ElementAt(0).tileLocation.ToString());
                List<TileNode> idealPath = StarAI.PathFindingCore.Utilities.getIdealPath(path);
                int costCalculation=0;

                if (idealPath.Count == 0) costCalculation = Int32.MaxValue;
                else costCalculation = idealPath.Count;

                //There was an error somewhere and now this won't work!!!!
                if (costCalculation == Int32.MaxValue) return new KeyValuePair<int, List<List<TileNode>>>(Int32.MaxValue, new List<List<TileNode>>());

                float pathCost = costCalculation * pathCostMultiplier;

                ModCore.CoreMonitor.Log("I THINK THIS IS MY PATH COST: " + costCalculation);
                float toolCost = calculateToolCostMultiplier(t);
                totalCalculation += toolCost + pathCost;
                idealPaths.Add(idealPath);
            }
            return new KeyValuePair<int, List<List<TileNode>>>((int)totalCalculation, idealPaths);
        }

    }
}
