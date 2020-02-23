using NpcAdventure.Internal;
using StardewModdingAPI;

namespace NpcAdventure.Compatibility
{
    /// <summary>
    /// Third party mod compatibility gateway
    /// </summary>
    internal class TPMC
    {
        private static readonly SetOnce<TPMC> instance = new SetOnce<TPMC>();
        public static TPMC Instance { get => instance.Value; private set => instance.Value = value; }

        public ICustomKissingModApi CustomKissing { get; }

        private TPMC(IModRegistry registry, IMonitor monitor)
        {
            this.CustomKissing = new CustomKissingModProxy(registry, monitor);
        }

        public static void Setup(IModRegistry registry, IMonitor monitor)
        {
            Instance = new TPMC(registry, monitor);
        }
    }
}
