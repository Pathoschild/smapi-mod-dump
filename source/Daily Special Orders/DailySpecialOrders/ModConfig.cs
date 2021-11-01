/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/shekurika/DailySpecialOrders
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailySpecialOrders
{
    public class ModConfig
    {
        public bool RefreshAfterPicking { get; set; } = true;
        public bool RefreshTuesday { get; set; } = true;
        public bool RefreshWednesday { get; set; } = true;
        public bool RefreshThursday { get; set; } = true;
        public bool RefreshFriday { get; set; } = true;
        public bool RefreshSaturday { get; set; } = true;
        public bool RefreshSunday { get; set; } = true;
    }
}
