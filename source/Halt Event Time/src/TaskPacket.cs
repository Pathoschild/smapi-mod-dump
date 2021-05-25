/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chawolbaka/HaltEventTime
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaltEventTime
{
    public enum TaskType
    {
        None,
        ActionControl,
        SpeedControl,
        TimeControl,
        BuffControl,
        WalkOnly
    }
    public enum TaskOperation
    {
        Run,
        Stop,
        Remove
    }
    public class TaskPacket
    {
        public TaskType Type;
        public TaskOperation Operate;

        public TaskPacket(TaskType type, TaskOperation operate)
        {
            Type = type;
            Operate = operate;
        }

        public override string ToString()
        {
            return $"{nameof(TaskPacket)}: Type={Type}, Operate={Operate}";
        }
    }
}
