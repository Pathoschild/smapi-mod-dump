/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/ServerBookmarker
**
*************************************************/

using StardewValley.Menus;
using System;

namespace ServerBookmarker
{
    class StartListPopulationListener : Patch
    {
        public static event Action<CoopMenu> StartPopulation;

        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(LoadGameMenu), "startListPopulation");

        public static bool Prefix(LoadGameMenu __instance)
        {
            if(__instance is CoopMenu menu)
            {
                StartPopulation?.Invoke(menu);
            }
            return true;
        }
    }
}
