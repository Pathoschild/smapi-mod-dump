/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using System.Collections.Generic;

namespace Survivalistic.Framework.Common
{
    public class Data
    {
        public float actual_hunger { get; set; } = 100;
        public float actual_thirst { get; set; } = 100;

        public float max_hunger { get; set; } = 100;
        public float max_thirst { get; set; } = 100;

        public float initial_hunger { get; set; } = 100;
        public float initial_thirst { get; set; } = 100;

        public int actual_day { get; set; } = 0;
        public int actual_season { get; set; } = 0;
        public int actual_year { get; set; } = 0;
        public int actual_tick { get; set; } = 0;

        public static Dictionary<string, string> foodDatabase { get; set; }
    }
}
