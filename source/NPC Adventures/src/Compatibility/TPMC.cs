/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using PurrplingCore;
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
