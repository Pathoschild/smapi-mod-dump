using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ExecutionCore.TaskPrerequisites
{
    public class BedTimePrerequisite : GenericPrerequisite
    {

        public bool checkIfEnoughTimeRemaining;
        public BedTimePrerequisite(bool CheckForBedtime)
        {
            this.checkIfEnoughTimeRemaining = CheckForBedtime;
        }

        public int timeRemainingInDay()
        {
            int passOutTime = 2600;
            return passOutTime - Game1.timeOfDay;
        }

        /// <summary>
        /// The default here will give you 2 hrs which should be enough for bedTime.
        /// </summary>
        /// <returns></returns>
        public bool enoughTimeToDoTask()
        {
            int timeRemaining = timeRemainingInDay();
            if (timeRemaining > 200) return true;
            else return false;
        }

        public bool enoughTimeToDoTask(int timeToDoTask)
        {
            int timeRemaining = timeRemainingInDay();
            if (timeRemaining > timeToDoTask) return true;
            else return false;
        }

        public override bool checkAllPrerequisites()
        {
            if (this.checkIfEnoughTimeRemaining == false) return true;
            if (enoughTimeToDoTask()) return true;
            else
            {
                ModCore.CoreMonitor.Log("Not enough time remaining in the day. You should go home.");
                //Add functionality here to return home.
                return false;
            }
        }

    }
}
