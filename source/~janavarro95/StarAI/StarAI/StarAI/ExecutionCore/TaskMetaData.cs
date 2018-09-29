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

        public List<TaskPrerequisites.GenericPrerequisite> prerequisitesList;

        public TaskMetaData(string Name, float Priority, float Cost, float Utility, float Frequency, TaskPrerequisites.StaminaPrerequisite StaminaPrerequisite=null, TaskPrerequisites.ToolPrerequisite ToolPrerequisite=null, TaskPrerequisites.InventoryFullPrerequisite InventoryFull = null, TaskPrerequisites.BedTimePrerequisite BedTimePrereq=null)
        {
            this.name = Name;
            this.priority = Priority;
            this.cost = Cost;
            this.utility = Utility;
            this.frequency = Frequency;
            this.staminaPrerequisite = StaminaPrerequisite;
            this.toolPrerequisite = ToolPrerequisite;
            this.inventoryPrerequisite = InventoryFull;
            this.bedTimePrerequisite = BedTimePrereq;
            //Make sure to set values correctly incase of null
            setUpStaminaPrerequisiteIfNull();
            setUpToolPrerequisiteIfNull();
            setUpInventoryPrerequisiteIfNull();
            setUpBedTimeIfNull();
            this.prerequisitesList = new List<TaskPrerequisites.GenericPrerequisite>();
            this.prerequisitesList.Add(this.staminaPrerequisite);
            this.prerequisitesList.Add(this.toolPrerequisite);
            this.prerequisitesList.Add(this.inventoryPrerequisite);

            this.prerequisitesList.Add(this.bedTimePrerequisite);
        }

        public TaskMetaData(string Name,float Cost,TaskPrerequisites.StaminaPrerequisite StaminaPrerequisite = null, TaskPrerequisites.ToolPrerequisite ToolPrerequisite = null, TaskPrerequisites.InventoryFullPrerequisite InventoryFull = null,TaskPrerequisites.BedTimePrerequisite BedTimePrereq=null)
        {
            this.name = Name;
            this.cost = Cost;
            this.staminaPrerequisite = StaminaPrerequisite;
            this.toolPrerequisite = ToolPrerequisite;
            this.inventoryPrerequisite = InventoryFull;

            this.bedTimePrerequisite = BedTimePrereq;
            //Make sure to set values correctly incase of null
            setUpStaminaPrerequisiteIfNull();
            setUpToolPrerequisiteIfNull();
            setUpInventoryPrerequisiteIfNull();
            setUpBedTimeIfNull();
            this.prerequisitesList = new List<TaskPrerequisites.GenericPrerequisite>();
            this.prerequisitesList.Add(this.staminaPrerequisite);
            this.prerequisitesList.Add(this.toolPrerequisite);
            this.prerequisitesList.Add(this.inventoryPrerequisite);
            this.prerequisitesList.Add(this.bedTimePrerequisite);
        }

        public TaskMetaData(string Name, TaskPrerequisites.StaminaPrerequisite StaminaPrerequisite = null, TaskPrerequisites.ToolPrerequisite ToolPrerequisite = null, TaskPrerequisites.InventoryFullPrerequisite InventoryFull=null,TaskPrerequisites.BedTimePrerequisite bedTimePrereq=null)
        {
            this.name = Name;
            this.staminaPrerequisite = StaminaPrerequisite;
            this.toolPrerequisite = ToolPrerequisite;
            this.inventoryPrerequisite = InventoryFull;

            this.bedTimePrerequisite = bedTimePrereq;
            //Make sure to set values correctly incase of null
            setUpStaminaPrerequisiteIfNull();
            setUpToolPrerequisiteIfNull();
            setUpInventoryPrerequisiteIfNull();
            setUpBedTimeIfNull();
            this.prerequisitesList = new List<TaskPrerequisites.GenericPrerequisite>();
            this.prerequisitesList.Add(this.staminaPrerequisite);
            this.prerequisitesList.Add(this.toolPrerequisite);
            this.prerequisitesList.Add(this.inventoryPrerequisite);
            this.prerequisitesList.Add(this.bedTimePrerequisite);
        }

        public void calculateTaskCost(TileNode source)
        {
           this.cost=TaskMetaDataHeuristics.calculateTaskCost(source, this.toolPrerequisite);
        }

        private void setUpToolPrerequisiteIfNull()
        {
            if (this.toolPrerequisite == null)
            {
                this.toolPrerequisite = new TaskPrerequisites.ToolPrerequisite(false, null,0);
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
            
            s += "  Task Requires Stamina: " + this.staminaPrerequisite.requiresStamina + "\n";
            if(this.staminaPrerequisite.requiresStamina==true) s += "  Requires : " + this.staminaPrerequisite.staminaCost + "Stamina.\n";
            s += "  Task Requires Tool: " + this.toolPrerequisite.requiresTool + "\n";
            if (this.toolPrerequisite.requiresTool == true) s += "   Requires a : " + this.toolPrerequisite.requiredTool + "\n";
            s += "  Task Requires Tool: " + this.toolPrerequisite.requiresTool + "\n";
            s += "    Checks if inventory full: "+this.inventoryPrerequisite.doesTaskRequireInventorySpace.ToString() + "\n";
            ModCore.CoreMonitor.Log(s);
        }

    }
}
