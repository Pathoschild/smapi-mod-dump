using StarAI.ExecutionCore.TaskPrerequisites;
using StarAI.PathFindingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ExecutionCore
{
    public class TaskMetaData
    {
        public string name;
        public float priority;
        public float cost;
        public float utility;
        public float frequency;
        public StarAI.ExecutionCore.TaskPrerequisites.StaminaPrerequisite staminaPrerequisite;
        public StarAI.ExecutionCore.TaskPrerequisites.ToolPrerequisite toolPrerequisite;
        public TaskPrerequisites.InventoryFullPrerequisite inventoryPrerequisite;

        public TaskPrerequisites.BedTimePrerequisite bedTimePrerequisite;

        public TaskPrerequisites.ItemPrerequisite itemPrerequisite;
        public LocationPrerequisite locationPrerequisite;

        public List<TaskPrerequisites.GenericPrerequisite> prerequisitesList;


        public List<List<TileNode>> pathsToTake = new List<List<TileNode>>();



        public TaskMetaData(string Name, LocationPrerequisite LocationPrereque, StaminaPrerequisite StaminaPrerequisite = null, ToolPrerequisite ToolPrerequisite = null, InventoryFullPrerequisite InventoryFull=null,BedTimePrerequisite bedTimePrereq=null, ItemPrerequisite ItemPrereque = null)
        {
            this.name = Name;
            this.staminaPrerequisite = StaminaPrerequisite;
            this.toolPrerequisite = ToolPrerequisite;
            this.inventoryPrerequisite = InventoryFull;
            this.pathsToTake = new List<List<TileNode>>();
            this.bedTimePrerequisite = bedTimePrereq;

            this.itemPrerequisite = ItemPrereque;
            this.locationPrerequisite = LocationPrereque;
            //Make sure to set values correctly incase of null
            setUpLocationPrerequsiteIfNull();
            setUpStaminaPrerequisiteIfNull();
            setUpToolPrerequisiteIfNull();
            setUpInventoryPrerequisiteIfNull();
            setUpBedTimeIfNull();
            setUpItemPrerequisiteIfNull();
            this.prerequisitesList = new List<TaskPrerequisites.GenericPrerequisite>();
            this.prerequisitesList.Add(this.staminaPrerequisite);
            this.prerequisitesList.Add(this.toolPrerequisite);
            this.prerequisitesList.Add(this.inventoryPrerequisite);
            this.prerequisitesList.Add(this.bedTimePrerequisite);
            this.prerequisitesList.Add(this.itemPrerequisite);
        }

        public void calculateTaskCost(TileNode source,bool unknownPath)
        {
            var pair = TaskMetaDataHeuristics.calculateTaskCost(source, this.toolPrerequisite, unknownPath);
            this.cost=  pair.Key;
            this.pathsToTake = new List<List<TileNode>>();
            this.pathsToTake.Add(pair.Value); 
            //this.path = Utilities.calculatePath(source, false);
        }

        public void calculateTaskCost(List<TileNode> source, bool unknownPath)
        {
            var pair = TaskMetaDataHeuristics.calculateTaskCost(source, this.toolPrerequisite, unknownPath); 
            this.cost = pair.Key;
            this.pathsToTake = new List<List<TileNode>>();
            this.pathsToTake.Add(pair.Value);
            //this.path = Utilities.calculatePath(source, false);
        }

        public void calculateTaskCost(List<List<TileNode>> source, bool unknownPath)
        {
            var pair = TaskMetaDataHeuristics.calculateTaskCost(source, this.toolPrerequisite, unknownPath);
            this.cost = pair.Key;
            this.pathsToTake = pair.Value;
            //this.path = Utilities.calculatePath(source, false);
        }

        private void setUpLocationPrerequsiteIfNull()
        {
            if (this.locationPrerequisite == null) this.locationPrerequisite = new LocationPrerequisite(null);
        }

        private void setUpToolPrerequisiteIfNull()
        {
            if (this.toolPrerequisite == null)
            {
                this.toolPrerequisite = new TaskPrerequisites.ToolPrerequisite(false, null,0);
            }
        }

        private void setUpItemPrerequisiteIfNull()
        {
            if (this.itemPrerequisite == null)
            {
                this.itemPrerequisite = new TaskPrerequisites.ItemPrerequisite(null, 0);
            }
        }

        private void setUpStaminaPrerequisiteIfNull()
        {
            if (this.staminaPrerequisite == null)
            {
                this.staminaPrerequisite = new TaskPrerequisites.StaminaPrerequisite(false, 0);
            }
        }

        private void setUpInventoryPrerequisiteIfNull()
        {
            if (this.inventoryPrerequisite == null) this.inventoryPrerequisite = new TaskPrerequisites.InventoryFullPrerequisite(false);
        }

        private void setUpBedTimeIfNull()
        {
            if (this.bedTimePrerequisite == null) this.bedTimePrerequisite = new TaskPrerequisites.BedTimePrerequisite(true);
        }

        
        public bool verifyAllPrerequisitesHit()
        {
            foreach(var prerequisite in this.prerequisitesList)
            {
                if (prerequisite.checkAllPrerequisites() == false) return false;
            }
            return true;
        }

        public void printMetaData()
        {
            string s = "";
            s += "Queued Task:"+"\n";
            s += "  TaskName: " + this.name + "\n";
            s += "  TaskCost: " + this.cost + "\n";
            if (this.locationPrerequisite.location != null)
            {
                s += "  Task Location: " + this.locationPrerequisite.location.name + "\n";
            }
            TileNode t = pathsToTake.ElementAt(pathsToTake.Count - 1).ElementAt(pathsToTake.ElementAt(pathsToTake.Count - 1).Count-1);
            s += "  Goal Location: " + t.thisLocation.name + " " + t.tileLocation;

            s += "  Task Requires Stamina: " + this.staminaPrerequisite.requiresStamina + "\n";
            if(this.staminaPrerequisite.requiresStamina==true) s += "  Requires : " + this.staminaPrerequisite.staminaCost + "Stamina.\n";
            s += "  Task Requires Tool: " + this.toolPrerequisite.requiresTool + "\n";
            if (this.toolPrerequisite.requiresTool == true) s += "  Requires a : " + this.toolPrerequisite.requiredTool + "\n";
            s += "  Task Requires Tool: " + this.toolPrerequisite.requiresTool + "\n";
            s += "  Checks if inventory full: "+this.inventoryPrerequisite.doesTaskRequireInventorySpace.ToString() + "\n";
            ModCore.CoreMonitor.Log(s);
        }

    }
}
