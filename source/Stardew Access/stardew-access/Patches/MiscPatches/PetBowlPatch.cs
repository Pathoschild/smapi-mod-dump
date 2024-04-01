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
using StardewValley.Buildings;

namespace stardew_access.Patches;

internal class PetBowlPatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(PetBowl), "draw"),
            postfix: new HarmonyMethod(typeof(PetBowlPatch), nameof(PetBowlPatch.DrawPatch))
        );
    }

    private static void DrawPatch(int ___nameTimer, string ___nameTimerMessage)
    {
        try
        {
            if (___nameTimer >= 3400)
            {
                MainClass.ScreenReader.Say(___nameTimerMessage, true);
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occured PetBowlPatch::DrawPatch():\n{e.Message}\n{e.StackTrace}");
        }
    }
}
