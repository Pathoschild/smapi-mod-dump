using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;

namespace BeyondTheValleyExpansion
{
    public class RefFarm
    {
        /// <summary> accepted map layer values to use for custom tile actions </summary>
        public static string[] LayerValues = { "Back", "Buildings", "Front", "AlwaysFront" };

        /// <summary> Farm/standard farm id </summary>
        public static int Standard_Farm = 0;
        /// <summary> Farm_Fishing/riverlands farm id </summary>
        public static int Fishing_Farm = 1;
        /// <summary> Farm_Foraging/forest farm id </summary>
        public static int Foraging_Farm = 2;
        /// <summary> Farm_Mining/hilltop farm id </summary>
        public static int Mining_Farm = 3;
        /// <summary> Farm_Combat/wilderness farm id </summary>
        public static int Combat_Farm = 4;
    }
}
