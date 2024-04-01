/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCompanions.Framework.Models
{
    public class CompanionData
    {
        public int NumberToSummon { get; set; } = 1;
    }

    public class RingModel
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public Dictionary<string, CompanionData> Companions { get; set; }
    }
}
