using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ExecutionCore.TaskPrerequisites
{
    public class StaminaPrerequisite:GenericPrerequisite
    {
       public bool requiresStamina;
       public int staminaCost;

        public StaminaPrerequisite(bool RequiresStamina,int howMuchStamina)
        {
            requiresStamina = RequiresStamina;
            staminaCost = howMuchStamina;
            verifyStaminaSetUp();
        }

        public bool doIHaveEnoughStaminaToPerformTask()
        {
            if (Game1.player.stamina >= staminaCost) return true;
            else return false;
        }

        private void verifyStaminaSetUp()
        {
            if (this.requiresStamina == false) this.staminaCost = 0;
        }

        public override bool checkAllPrerequisites()
        {
            if (doIHaveEnoughStaminaToPerformTask()) return true;
            else
            {
                ModCore.CoreMonitor.Log("A task failed due to not having enough stamina");
                return false;
            }
        }

        //include method to eat food here????
    }
}
