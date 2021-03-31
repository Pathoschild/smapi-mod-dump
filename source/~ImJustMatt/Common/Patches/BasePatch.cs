/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using Harmony;
using StardewModdingAPI;

namespace ImJustMatt.Common.Patches
{
    internal abstract class BasePatch<T> where T : IMod
    {
        private protected static T Mod;

        internal BasePatch(IMod mod, HarmonyInstance harmony)
        {
            Mod = (T) mod;
        }

        private protected static IMonitor Monitor => Mod.Monitor;
    }
}