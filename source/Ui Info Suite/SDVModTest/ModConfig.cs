/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cdaragorn/Ui-Info-Suite
**
*************************************************/

using StardewModdingAPI;

namespace UIInfoSuite
{
    class ModConfig
    {

        public int[][] Sprinkler { get; set; } = new int[][]
        {
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public int[][] QualitySprinkler { get; set; } = new int[][]
        {
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public int[][] IridiumSprinkler { get; set; } = new int[][]
        {
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 },
            new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 },
            new int[] { 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0 },
            new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 },
            new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };
		
		public int[][] PrismaticSprinkler { get; set; } = new int[][]
        {
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0 },
            new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0 },
            new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0 },
            new int[] { 0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0 },
            new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0 },
            new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0 },
            new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public int[][] Beehouse { get; set; } = new int[][]
        {
            new int[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 },
            new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0 },
            new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            new int[] { 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0 },
            new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0 },
            new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 }
        };
    }
}
