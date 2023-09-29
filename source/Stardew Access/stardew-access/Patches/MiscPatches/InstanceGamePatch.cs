/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using HarmonyLib;
using StardewValley;

namespace stardew_access.Patches
{
    internal class InstanceGamePatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                    original: AccessTools.Method(typeof(InstanceGame), nameof(InstanceGame.Exit)),
                    prefix: new HarmonyMethod(typeof(InstanceGamePatch), nameof(InstanceGamePatch.ExitPatch))
            );
        }

        private static void ExitPatch()
        {
            MainClass.ScreenReader?.CloseScreenReader();
        }
    }
}
