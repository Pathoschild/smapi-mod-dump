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
using AnimalHusbandryMod.common;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Network;

namespace AnimalHusbandryMod.recipes
{
    internal class TrashBearOverrides
    {
        internal static bool updateItemWanted(TrashBear __instance)
        {
            int num1 = 0;
            if (NetWorldState.checkAnywhereForWorldStateID("trashBear1"))
                num1 = 1;
            if (NetWorldState.checkAnywhereForWorldStateID("trashBear2"))
                num1 = 2;
            if (NetWorldState.checkAnywhereForWorldStateID("trashBear3"))
                num1 = 3;
            int randomSeedAddition = 777111 + num1;
            var itemWantedIndex = DataLoader.Helper.Reflection.GetField<int>(__instance,"itemWantedIndex");
            itemWantedIndex.SetValue(Utility.getRandomPureSeasonalItem(Game1.currentSeason, randomSeedAddition));
            if (num1 <= 1)
                return false;
            int num2 = new Random((int)Game1.uniqueIDForThisGame + randomSeedAddition).Next(CraftingRecipe.cookingRecipes.Count);
            int num3 = 0;
            foreach (string str in CraftingRecipe.cookingRecipes.Values)
            {
                if (num3 == num2)
                {
                    itemWantedIndex.SetValue(Convert.ToInt32(str.Split('/')[2].Split(' ')[0]));
                    break;
                }
                ++num3;
            }

            return false;
        }
    }
}
