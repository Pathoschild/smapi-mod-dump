/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/arruda/BalancedCombineManyRings
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
namespace BalancedCombineManyRings
{
    public class DataLoader
    {
        public static IModHelper Helper;
        public static ModConfig ModConfig;
        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            ModConfig = helper.ReadConfig<ModConfig>();
        }
    }
}
