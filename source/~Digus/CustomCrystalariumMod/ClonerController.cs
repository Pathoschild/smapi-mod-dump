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
using StardewValley.Extensions;
using Object = StardewValley.Object;

namespace CustomCrystalariumMod
{
    public class ClonerController
    {
        private static Dictionary<string, CustomCloner> ClonerData =  new Dictionary<string, CustomCloner>();

        public static CustomCloner GetCloner(string qualifiedItemId)
        {
            ClonerData.TryGetValue(qualifiedItemId, out CustomCloner result);
            return result;
        }

        public static bool HasCloner(string qualifiedItemId)
        {
            return ClonerData.ContainsKey(qualifiedItemId);
        }

        public static void SetCloner(CustomCloner cloner)
        {
            ClonerData[cloner.QualifiedItemId] = cloner;
        }

        public static int? GetMinutesUntilReady(CustomCloner customCloner, Object clonable)
        {
            int? minutesUntilReady = null;
            if (customCloner.CloningDataId.TryGetValue(clonable.QualifiedItemId, out var value))
            {
                minutesUntilReady = value;
            } 
            else if (customCloner.CloningDataId.TryGetValue(clonable.Category.ToString(), out value))
            {
                minutesUntilReady = value;
            } 
            else if (customCloner.EnableCloneEveryObject && !clonable.HasTypeBigCraftable())
            {
                minutesUntilReady = customCloner.DefaultCloningTime;
            }
            return minutesUntilReady;
        }
    }
}
