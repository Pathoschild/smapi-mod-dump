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
