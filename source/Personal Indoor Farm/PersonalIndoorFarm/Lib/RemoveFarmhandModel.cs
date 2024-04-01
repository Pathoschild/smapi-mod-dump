/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalIndoorFarm.Lib
{
    internal class RemoveFarmhandModel
    {
        public List<string> PifLocations = new();
        public long Who;
        public string CabinLocation;
        public RemoveFarmhandModel(long who, string cabinLocation)
        {
            Who = who;
            CabinLocation = cabinLocation;
        }
    }
}
