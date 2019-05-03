using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;

namespace BeyondTheValleyExpansion.Framework
{
    public class RefFarm
    {
        /// <summary> accepted map layer values to use for custom tile actions </summary>
        public static string[] layerValues = { "Back", "Buildings", "Front", "AlwaysFront" };

        /// <summary> Farm/standard farm id </summary>
        public static int standard_Farm = 0;
        /// <summary> Farm_Fishing/riverlands farm id </summary>
        public static int fishing_Farm = 1;
        /// <summary> Farm_Foraging/forest farm id </summary>
        public static int foraging_Farm = 2;
        /// <summary> Farm_Mining/hilltop farm id </summary>
        public static int mining_Farm = 3;
        /// <summary> Farm_Combat/wilderness farm id </summary>
        public static int combat_Farm = 4;
    }
}
