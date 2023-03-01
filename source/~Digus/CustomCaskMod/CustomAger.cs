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

namespace CustomCaskMod
{
    public class CustomAger
    {
        public string ModUniqueID;
        public string Name;
        public bool EnableAgingAnywhere;
        public bool EnableMoreThanOneQualityIncrementPerDay;
        public bool EnableAgeEveryObject;
        public float DefaultAgingRate = 1.0f;
        public Dictionary<object, float> AgingData;
        public Dictionary<int, float> AgingDataId = new Dictionary<int, float>();
        public List<string> OverrideMod = new List<string>();
        public List<string> MergeIntoMod = new List<string>();
    }
}
