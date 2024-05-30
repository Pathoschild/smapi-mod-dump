/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jingshenSN2/ReplaceFertilizer
**
*************************************************/

using StardewModdingAPI;

namespace JingshenSN2.ReplaceFertilizer.Patches
{
    internal class Patch
    {
        internal static IMonitor? Monitor;

        public Patch(IMonitor monitor)
        {
            Monitor = monitor;
        }
    }
}