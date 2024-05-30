/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using BZP_Allergies.HarmonyPatches.UI;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace BZP_Allergies.HarmonyPatches
{
    internal class UI_Patches
    {
        public static void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Constructor(typeof(GameMenu), new Type[] { typeof(bool) }),
                postfix: new HarmonyMethod(typeof(UI_Patches), nameof(GameMenuConstructor_Postfix))
            );
        }

        // we do this instead of an OnMenuChanged event because otherwise it may conflict with spacecore skills
        public static void GameMenuConstructor_Postfix(GameMenu __instance)
        {
            try
            {
                if (ModEntry.Instance.Config.EnableTab)
                    __instance.pages[1] = new PatchedSkillsPage(__instance.xPositionOnScreen, __instance.yPositionOnScreen, __instance.width + ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it) ? 64 : 0), __instance.height);
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(GameMenuConstructor_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

    }
}
