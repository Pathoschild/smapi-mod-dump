/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace Custom_Farm_Loader.Lib
{
    public class Main
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            Filter.Initialize(mod);
            DailyUpdate.Initialize(mod);
            ItemObject.Initialize(mod);
            Furniture.Initialize(mod);
            FishingRule.Initialize(mod);
            Fish.Initialize(mod);
            FarmProperties.Initialize(mod);
            FarmTypeCache.Initialize(mod);

            CustomFarm.Initialize(mod);
        }
    }
}
