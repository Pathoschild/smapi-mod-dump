/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.Lib
{
    internal class _GameLocation
    {
        public static void Initialize()
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(GameLocation), nameof(GameLocation.reloadMap)),
                postfix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.reloadMap_Postfix))
            );
        }

        public static void reloadMap_Postfix(GameLocation __instance)
        {
            MapPatches.checkIfMapPatchesNeedToBeReapplied(__instance);
        }
    }
}
