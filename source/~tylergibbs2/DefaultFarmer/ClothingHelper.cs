/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.GameData.Pants;
using StardewValley.GameData.Shirts;
using StardewValley;

namespace DefaultFarmer
{
    internal static class ClothingHelper
    {
        private static List<string> GetValidClothingIds<TData>(string equippedId, IDictionary<string, TData> data, Func<TData, bool> canChooseDuringCharacterCustomization)
        {
            List<string> validIds = new();
            foreach (KeyValuePair<string, TData> pair in data)
            {
                if (pair.Key == equippedId || canChooseDuringCharacterCustomization(pair.Value))
                    validIds.Add(pair.Key);
            }
            return validIds;
        }

        private static List<string> GetValidPantsIds()
        {
            return GetValidClothingIds("0", Game1.pantsData, (PantsData data) => data.CanChooseDuringCharacterCustomization);
        }

        private static List<string> GetValidShirtIds()
        {
            return GetValidClothingIds("0", Game1.shirtData, (ShirtData data) => data.CanChooseDuringCharacterCustomization);
        }

        public static string GetNewPantsId(int originalId)
        {
            string itemId = "0";
            List<string> validIds = new();
            foreach (KeyValuePair<string, PantsData> pantsDatum in Game1.pantsData)
                validIds.Add(pantsDatum.Key);

            int index = validIds.IndexOf(itemId);
            if (index == -1)
            {
                itemId = validIds.FirstOrDefault()!;
                if (itemId != null)
                    return itemId;
            }
            else
            {
                index = Utility.WrapIndex(index + originalId, validIds.Count);
                itemId = validIds[index];
                return itemId;
            }

            return itemId is null ? "0" : itemId;
        }

        public static string GetNewShirtId(int originalId)
        {
            string itemId = "1000";
            List<string> validIds = new();
            foreach (KeyValuePair<string, ShirtData> shirtDatum in Game1.shirtData)
                validIds.Add(shirtDatum.Key);

            int index = validIds.IndexOf(itemId);
            if (index == -1)
            {
                itemId = validIds.FirstOrDefault()!;
                if (itemId != null)
                    return itemId;
            }
            else
            {
                index = Utility.WrapIndex(index + originalId, validIds.Count);
                itemId = validIds[index];
                return itemId;
            }

            return itemId is null ? "0" : itemId;
        }
    }
}
