using Pelican.Config;
using StardewModdingAPI;

namespace Pelican
{
    public class Meta
    {
        public static ModConfig Config { get; internal set; }
        public static IMonitor Console { get; internal set; }
        public static ITranslationHelper Lang { get; internal set; }
    }
}
