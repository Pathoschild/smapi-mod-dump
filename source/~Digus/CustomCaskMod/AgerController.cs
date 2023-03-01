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
using StardewValley;
using SObject = StardewValley.Object;

namespace CustomCaskMod
{
    public class AgerController
    {
        private static readonly Dictionary<string, CustomAger> AgerData = new Dictionary<string, CustomAger>();

        public static CustomAger GetAger(string name)
        {
            AgerData.TryGetValue(name, out CustomAger result);
            return result;
        }

        public static bool HasAger(string name)
        {
            return AgerData.ContainsKey(name);
        }

        public static void SetAger(CustomAger ager)
        {
            AgerData[ager.Name] = ager;
        }

        public static float? GetAgingMultiplierForItem(CustomAger customAger, Item ageable)
        {
            if (customAger.AgingDataId.ContainsKey(ageable.ParentSheetIndex))
            {
                return customAger.AgingDataId[ageable.ParentSheetIndex];
            }
            else if (customAger.AgingDataId.ContainsKey(ageable.Category))
            {
                return customAger.AgingDataId[ageable.Category];
            }
            else if (customAger.EnableAgeEveryObject)
            {
                return customAger.DefaultAgingRate;
            }
            return null;
        }

        public static void ClearAgers()
        {
            AgerData.Clear();
        }
    }
}
