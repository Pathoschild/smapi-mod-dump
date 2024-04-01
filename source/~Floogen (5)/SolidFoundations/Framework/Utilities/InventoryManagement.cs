/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using StardewValley;
using System.Linq;

namespace SolidFoundations.Framework.Utilities
{
    internal static class InventoryManagement
    {
        public static void ConsumeItemBasedOnQuantityAndQuality(Farmer who, Item targetItem, int quantity, int quality = -1)
        {
            int requiredCount = quantity;
            foreach (var item in who.Items.Where(i => i is not null && i.Name.Equals(targetItem.Name)).ToList())
            {
                if (requiredCount <= 0)
                {
                    break;
                }

                if (quality == -1 || (item is Object itemObject && itemObject is not null && itemObject.Quality >= (int)quality))
                {
                    if (item.Stack <= requiredCount)
                    {
                        requiredCount -= item.Stack;
                        who.Items.Remove(item);
                    }
                    else
                    {
                        item.Stack -= requiredCount;
                        break;
                    }
                }
            }
        }
    }
}
