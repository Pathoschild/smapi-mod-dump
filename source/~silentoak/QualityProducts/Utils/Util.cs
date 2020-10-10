/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace SilentOak.QualityProducts.Utils
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
