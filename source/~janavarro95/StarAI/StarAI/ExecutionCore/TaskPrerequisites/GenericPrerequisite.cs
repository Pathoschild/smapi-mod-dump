using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ExecutionCore.TaskPrerequisites
{
    /// <summary>
    /// Template for shared dunctionality across prerequisites. Also allows for generalization.
    /// </summary>
    public class GenericPrerequisite
    {


        public virtual bool checkAllPrerequisites()
        {
            return true;
            //return false;
        }
    }
}
