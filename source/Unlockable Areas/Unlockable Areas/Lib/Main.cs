/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace Unlockable_Areas.Lib
{
    public class Main
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            AssetRequested.Initialize();
            SaveDataEvents.Initialize();
            ShopObject.Initialize();
            ShopPlacement.Initialize();
            UpdateHandler.Initialize();
            UA_NoGrass.Initialize();
        }
    }
}
