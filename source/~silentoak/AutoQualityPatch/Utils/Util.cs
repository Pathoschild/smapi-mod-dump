using System;
using StardewModdingAPI;

namespace SilentOak.AutoQualityPatch.Utils
{
    public static class Util
    {
        public static IModHelper Helper { get; private set; }
        public static IMonitor Monitor { get; private set; }

        public static void Init(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;
        }
    }
}
