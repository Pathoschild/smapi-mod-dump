using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;
using StardewModdingAPI;

namespace RejuvenatingForest
{
    internal class Globals
    {
        public static IManifest Manifest { get; set; }
        public static IModHelper Helper { get; set; }
        public static IMonitor Monitor { get; set; }
    }
}