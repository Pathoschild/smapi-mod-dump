/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewRoguelike.UI;
using StardewValley;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(SObject), "checkForAction")]
    internal class ObjectCheckForActionPatch
    {
        public static bool Prefix(SObject __instance, ref bool __result, Farmer who, bool justCheckingForActivity = false)
        {
            if (__instance.bigCraftable.Value)
            {
                if (justCheckingForActivity)
                    return true;

                if (__instance.ParentSheetIndex != 239)
                    return true;

                if (!Context.IsMainPlayer)
                {
                    Game1.drawObjectDialogue("Only the host can use this.");
                    __result = true;
                    return false;
                }

                __instance.shakeTimer = 500;
                who.currentLocation.localSound("DwarvishSentry");
                who.freezePause = 500;
                DelayedAction.functionAfterDelay(delegate
                {
                    Game1.activeClickableMenu = new SeedMenu();
                }, 500);

                __result = true;
                return false;
            }

            return true;
        }
    }
}
