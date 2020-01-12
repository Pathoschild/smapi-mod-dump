using StardewModdingAPI;

namespace NpcAdventure.Compatibility
{
    /// <summary>
    /// Third party mod compatibility gateway
    /// </summary>
    internal class TPMC
    {
        public static TPMC Instance { get; private set; }

        public ICustomKissingModApi CustomKissing { get; }

        private TPMC(IModRegistry registry)
        {
            this.CustomKissing = new CustomKissingModProxy(registry);
        }

        public static void Setup(IModRegistry registry)
        {
            if (Instance == null)
            {
                Instance = new TPMC(registry);
            }
        }
    }
}
