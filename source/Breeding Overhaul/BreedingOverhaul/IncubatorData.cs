/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StarAmy/BreedingOverhaul
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreedingOverhaul
{
    public class IncubatorData
    {
        public Dictionary<string, List<string>> IncubatorItems;

        public IncubatorData()
        {
            IncubatorItems = new Dictionary<string, List<string>>();
        }
    }
}
