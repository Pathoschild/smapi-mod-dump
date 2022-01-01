/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using SullySDVcore;
using StardewObject = StardewValley.Object;

namespace BetterTappers
{
    internal class CoreLogic
    {
        private static readonly Log log = BetterTappers.Instance.log;
        public const int LvlCap = 100;
        public const int formula = 0;

        public static int GetTreeAgeMonths(Tree tree)
        {
            return (int)Math.Floor(GetTreeAge(tree)/28f);
        }

        public static int GetTreeAge(Tree tree)
        {
            tree.modData.TryGetValue($"{BetterTappers.UID}/treeAge", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                return int.Parse(data);
            }
            log.D("Could not get tree age.", true);
            return 0;
        }

        internal static void IncreaseTreeAges()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            log.T("Increasing the age of trees.");

            foreach (var location in Game1.locations)
            {
                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    if (terrainfeature.Value is Tree tree)
                    {
                        IncreaseTreeAge(tree);
                    }
                }
            }
        }

        internal static void IncreaseTreeAge(Tree tree)
        {
            tree.modData.TryGetValue($"{BetterTappers.UID}/treeAge", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                tree.modData[$"{BetterTappers.UID}/treeAge"] = (int.Parse(data) + 1).ToString();
            }
            else
            {
                tree.modData[$"{BetterTappers.UID}/treeAge"] = "1";
            }
        }

        public static bool IsAnyTapper(StardewObject o)
        {
            return o is not null && o.bigCraftable.Value && (o.ParentSheetIndex == 105 || o.ParentSheetIndex == 264);
        }
        public static bool IsTapper(StardewObject o)
        {
            return o is not null && o.bigCraftable.Value && o.ParentSheetIndex == 105;
        }
        public static bool IsHeavyTapper(StardewObject o)
        {
            return o is not null && o.bigCraftable.Value && o.ParentSheetIndex == 264;
        }
    }
}