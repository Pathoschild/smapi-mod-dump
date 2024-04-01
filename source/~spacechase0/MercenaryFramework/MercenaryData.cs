/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercenaryFramework
{
    public class MercenaryData
    {
        public string ID { get; set; }

        public string CanRecruit { get; set; } = "TRUE";

        public int RecruitCost { get; set; } = 0;
    }
}
