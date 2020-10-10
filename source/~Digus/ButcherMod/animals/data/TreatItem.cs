/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AnimalHusbandryMod.animals.data
{
    public interface TreatItem
    {
        int MinimumDaysBetweenTreats { get; set; }
        object[] LikedTreats { get; set; }
        [JsonIgnore]
        ISet<int> LikedTreatsId { get; set; }
    }
}
