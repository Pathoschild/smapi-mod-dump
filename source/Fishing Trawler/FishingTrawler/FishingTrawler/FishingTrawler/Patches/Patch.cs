/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;

namespace FishingTrawler.Patches
{
    public abstract class Patch
    {
        internal static IMonitor Monitor;

        internal Patch(IMonitor monitor)
        {
            Monitor = monitor;
        }

        internal abstract void Apply(Harmony harmony);
    }
}
