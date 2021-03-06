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
using StardewValley.TerrainFeatures;

namespace CropTransplantMod
{
    public static class TreeExtension
    {
        public static int GetSeedSaplingIndex(this TerrainFeature value)
        {
            switch (value)
            {
                case Tree tree:
                    return GetSeedIndex(tree);
                case FruitTree fruitTree:
                    return GetSaplingIndex(fruitTree);
                case Bush bush:
                    return GetSaplingIndex(bush);
                default:
                    return -1;
            }
        }

        public static int GetSeedIndex(this Tree value)
        {
            int seedIndex = -1;
            switch (value.treeType.Value)
            {
                case 1:
                    seedIndex = 309;
                    break;
                case 2:
                    seedIndex = 310;
                    break;
                case 3:
                    seedIndex = 311;
                    break;
                case 8:
                    seedIndex = 292;
                    break;
                case 7:
                    seedIndex = 891;
                    break;
                case 6:
                case 9:
                    seedIndex = 88;
                    break;
            }
            return seedIndex;
        }

        public static int GetSaplingIndex(this FruitTree value)
        {
            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\fruitTrees");
            int seedIndex = data.Where(t => Convert.ToInt32(t.Value.Split('/')[0]) == value.treeType.Value).Select(t=>t.Key).FirstOrDefault();
            return seedIndex;
        }

        public static int GetSaplingIndex(this Bush value)
        {
            return 251;
        }
    }
}
