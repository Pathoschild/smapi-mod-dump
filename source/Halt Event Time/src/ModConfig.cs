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
    public class ModConfig
    {

        public static class Default
        {
            public static readonly TaskType Limit = TaskType.ActionControl;
            public static readonly int AsyncThreshold = 233;
#if DEBUG
            public static readonly bool Debug = true;
#else
            public static readonly bool Debug = false;
#endif
        }

        internal static ModConfig Instance;
        public bool Debug { get; set; }
        public TaskType Limit { get; set; }
        public int AsyncThreshold { get; set; }

        public ModConfig()
        {
            //原版第二年进去是166
            //SVE第一年是666
            Debug = Default.Debug;
            Limit = Default.Limit;
            AsyncThreshold = Default.AsyncThreshold;
        }

    }
}
