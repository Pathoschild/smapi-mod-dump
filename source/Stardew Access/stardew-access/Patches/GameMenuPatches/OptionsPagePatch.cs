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
using stardew_access.Utils;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class OptionsPagePatch :IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(OptionsPage), "draw"),
                postfix: new HarmonyMethod(typeof(OptionsPagePatch), nameof(OptionsPagePatch.DrawPatch))
        );
    }

    private static void DrawPatch(OptionsPage __instance)
    {
        try
        {
            int currentItemIndex = Math.Max(0, Math.Min(__instance.options.Count - 7, __instance.currentItemIndex));
            OptionsElementUtils.NarrateOptionsElementSlots(__instance.optionSlots, __instance.options, currentItemIndex);
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in options page patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
