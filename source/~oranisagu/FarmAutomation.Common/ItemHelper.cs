/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/oranisagu/SDV-FarmAutomation
**
*************************************************/

using StardewValley.Objects;
using Object = StardewValley.Object;

namespace FarmAutomation.Common
{
    public class ItemHelper
    {
        public static void RemoveItemFromChest(Object refillable, Chest chest, int amount = 1)
        {
            refillable.Stack -= amount;
            if (refillable.Stack <= 0)
            {
                //used last item of stack - delete from chest
                chest.items[chest.items.IndexOf(refillable)] = null;
                chest.items.RemoveAll(i => i == null);
            }
        }

        /// <summary>
        /// Copy of the private method Object.getMinutesForCrystalarium
        /// </summary>
        /// <param name="whichGem"></param>
        /// <returns></returns>
        public static int GetMinutesForCrystalarium(int whichGem)
        {
            switch (whichGem)
            {
                case 60:
                    return 3000;
                case 61:
                case 63:
                case 65:
                case 67:
                case 69:
                case 71:
                    break;
                case 62:
                    return 2240;
                case 64:
                    return 3000;
                case 66:
                    return 1360;
                case 68:
                    return 1120;
                case 70:
                    return 2400;
                case 72:
                    return 7200;
                default:
                    switch (whichGem)
                    {
                        case 80:
                            return 420;
                        case 82:
                            return 1300;
                        case 84:
                            return 1120;
                        case 86:
                            return 800;
                    }
                    break;
            }
            return 5000;
        }
    }
}
