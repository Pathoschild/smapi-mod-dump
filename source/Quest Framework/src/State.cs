using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework
{
    /// <summary>
    /// Quest framework lifecycle status
    /// </summary>
    public enum State
    {
        DISABLED,
        STANDBY,
        CLEANING,
        AWAITING,
        LAUNCHING,
        LAUNCHED,
    }
}
