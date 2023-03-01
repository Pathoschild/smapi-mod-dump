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
using Object = StardewValley.Object;

namespace CustomCrystalariumMod
{
    public class ClonerController
    {
        private static Dictionary<string, CustomCloner> ClonerData =  new Dictionary<string, CustomCloner>();

        public static CustomCloner GetCloner(string name)
        {
            ClonerData.TryGetValue(name, out CustomCloner result);
            return result;
        }

        public static bool HasCloner(string name)
        {
            return ClonerData.ContainsKey(name);
        }

        public static void SetCloner(CustomCloner cloner)
        {
            ClonerData[cloner.Name] = cloner;
        }

        public static int? GetMinutesUntilReady(CustomCloner customCloner, Object clonable)
        {
            if (customCloner.CloningDataId.ContainsKey(clonable.ParentSheetIndex))
            {
                return customCloner.CloningDataId[clonable.ParentSheetIndex];
            }
            else if (customCloner.CloningDataId.ContainsKey(clonable.Category))
            {
                return customCloner.CloningDataId[clonable.Category];
            }
            else if (customCloner.EnableCloneEveryObject)
            {
                return customCloner.DefaultCloningTime;
            }
            return null;
        }
    }
}
