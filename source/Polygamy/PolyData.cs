/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bwdymods/SDV-Polygamy
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygamy
{
    public class PolyData
    {
        public Dictionary<long, HashSet<string>> PolySpouses;
        public Dictionary<long, HashSet<string>> PolyDates;
        public Dictionary<long, HashSet<string>> PolyDivorces;
        public Dictionary<long, Dictionary<string, bwdyworks.GameDate>> PolyEngagements;
        public string PrimarySpouse;
        public string TomorrowSpouse;
        public bool EnableSpouseRoom;
    }
}
