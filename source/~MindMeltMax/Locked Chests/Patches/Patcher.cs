/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockChests.Patches
{
    internal static class Patcher
    {
        public static void Patch()
        {
            Harmony harmony = new Harmony(ModEntry.IHelper.ModRegistry.ModID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.ShowMenu)),
                prefix: new HarmonyMethod(typeof(ChestPatches), nameof(ChestPatches.ShowMenu_prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.receiveLeftClick)),
                prefix: new HarmonyMethod(typeof(ItemGrabMenuPatches), nameof(ItemGrabMenuPatches.receiveLeftClick_prefix))
            );
        }
    }
}
