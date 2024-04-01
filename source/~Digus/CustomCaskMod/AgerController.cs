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
using System.Collections.Immutable;
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

        public static CustomAger GetAger(string qualifiedItemId)
        {
            AgerData.TryGetValue(qualifiedItemId, out CustomAger result);
            return result;
        }

        public static bool HasAger(string qualifiedItemIdName)
        {
            return AgerData.ContainsKey(qualifiedItemIdName);
        }

        public static void SetAger(CustomAger ager)
        {
            AgerData[ager.QualifiedItemId] = ager;
        }

        public static float? GetAgingMultiplierForItem(CustomAger customAger, Item ageable)
        {
            if (customAger.AgingDataId.ContainsKey(ageable.QualifiedItemId))
            {
                return customAger.AgingDataId[ageable.QualifiedItemId];
            }
            else if (customAger.AgingDataId.ContainsKey(ageable.Category.ToString()))
            {
                return customAger.AgingDataId[ageable.Category.ToString()];
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

        public static ImmutableDictionary<string, CustomAger> GetAgers()
        {
            return AgerData.ToImmutableDictionary();
        }
    }
}
