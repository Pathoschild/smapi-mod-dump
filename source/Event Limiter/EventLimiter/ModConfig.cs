/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/EventLimiter
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLimiter
{
    class ModConfig
    {
        public int EventsPerDay { get; set; } = 4;
        public int EventsInARow { get; set; } = 2;
        public bool ExemptEventsCountTowardsLimit { get; set; } = true;
        public int[] Exceptions { get; set; } = { };
    }
}
