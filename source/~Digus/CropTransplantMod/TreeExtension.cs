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
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.FruitTrees;
using StardewValley.TerrainFeatures;

namespace CropTransplantMod
{
    public static class TreeExtension
    {
        public static string GetSeedSaplingIndex(this TerrainFeature value)
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
                    return "-1";
            }
        }

        public static string GetSeedIndex(this Tree value)
        {
            string seedIndex = "-1";
            switch (value.treeType.Value)
            {
                case "1":
                    seedIndex = "309";
                    break;
                case "2":
                    seedIndex = "310";
                    break;
                case "3":
                    seedIndex = "311";
                    break;
                case "8":
                    seedIndex = "292";
                    break;
                case "7":
                    seedIndex = "891";
                    break;
                case "6":
                case "9":
                    seedIndex = "88";
                    break;
            }
            return seedIndex;
        }

        public static string GetSaplingIndex(this FruitTree value)
        {

            return value.treeId.Value;
        }

        public static string GetSaplingIndex(this Bush value)
        {
            return "251";
        }
    }
}
