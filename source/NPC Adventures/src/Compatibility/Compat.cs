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
    internal class Compat
    {
        private static readonly SetOnce<Compat> instance = new SetOnce<Compat>();
        public static Compat Instance { get => instance.Value; private set => instance.Value = value; }

        public ICustomKissingModApi CustomKissing { get; }

        private readonly IModRegistry registry;

        private Compat(IModRegistry registry, IMonitor monitor)
        {
            this.CustomKissing = new CustomKissingModProxy(registry, monitor);
            this.registry = registry;
        }

        public static void Setup(IModRegistry registry, IMonitor monitor)
        {
            Instance = new Compat(registry, monitor);
        }

        public static bool IsModLoaded(string modUid)
        {
            if (Instance == null)
                return false;

            return Instance.registry.IsLoaded(modUid);
        }
    }

    internal class ModUids
    {
        public const string PACIFISTMOD_UID = "Aedenthorn.PacifistValley";
        public const string KISSINGMOD_UID = "Digus.CustomKissingMod";
    }
}
