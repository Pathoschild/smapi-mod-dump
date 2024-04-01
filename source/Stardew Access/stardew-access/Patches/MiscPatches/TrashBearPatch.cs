/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewValley;
using StardewValley.Characters;
using stardew_access.Translation;
using HarmonyLib;
using StardewValley.TokenizableStrings;

namespace stardew_access.Patches;

internal class TrashBearPatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(TrashBear), nameof(TrashBear.checkAction)),
            postfix: new HarmonyMethod(typeof(TrashBearPatch), nameof(TrashBearPatch.CheckActionPatch))
        );
    }

    private static void CheckActionPatch(TrashBear __instance, bool __result, string ___itemWantedIndex,
        int ___showWantBubbleTimer)
    {
        try
        {
            if (__result)
                return; // The true `true` value of __result indicates the bear is interactable i.e. when giving the bear the wanted item
            if (__instance.Sprite.CurrentAnimation != null) return;

            string itemName = TokenParser.ParseText(Game1.objectData[___itemWantedIndex].DisplayName);
            MainClass.ScreenReader.Say(
                Translator.Instance.Translate("patch-trash_bear-wanted_item",
                    new { trash_bear_name = __instance.displayName, item_name = itemName }), true);
        }
        catch (Exception e)
        {
            Log.Error($"An error occured TrashBearPatch::CheckActionPatch():\n{e.Message}\n{e.StackTrace}");
        }
    }
}
