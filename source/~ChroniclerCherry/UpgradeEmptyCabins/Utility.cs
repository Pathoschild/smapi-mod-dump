/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewValley;
using StardewValley.Buildings;
using System.Collections.Generic;

namespace UpgradeEmptyCabins
{
    internal static class ModUtility
    {
        public static Building GetCabin(string name)
        {
            foreach (var cabin in GetCabins())
                if (cabin.nameOfIndoors == name)
                    return cabin;
            return null;
        }

        public static IEnumerable<Building> GetCabins()
        {
            foreach (var building in Game1.getFarm().buildings)
            {
                if (building.isCabin)
                    yield return building;
            }
        }
    }
}
