/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

namespace StopRugRemoval.HarmonyPatches.Niceties;

/// <summary>
/// Patches the jumino menu so paging can be done with arrow keys.
/// </summary>
[HarmonyPatch(typeof(JunimoNoteMenu))]
internal static class JunimoNotePatcher
{
    [HarmonyPatch(nameof(JunimoNoteMenu.receiveKeyPress))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void Postfix(JunimoNoteMenu __instance, Keys key, bool ___specificBundlePage)
    {
        if (__instance.fromGameMenu && !___specificBundlePage)
        {
            switch (key)
            {
                case Keys.Left:
                    __instance.SwapPage(-1);
                    break;
                case Keys.Right:
                    __instance.SwapPage(1);
                    break;
            }
        }
    }
}
