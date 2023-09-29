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
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class LanguageSelectionMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(LanguageSelectionMenu), nameof(LanguageSelectionMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(LanguageSelectionMenuPatch), nameof(LanguageSelectionMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(LanguageSelectionMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string translationKey = "";

                if (__instance.nextPageButton != null && __instance.nextPageButton.containsPoint(x, y))
                {
                    translationKey = "common-ui-next_page_button";
                }
                else if (__instance.previousPageButton != null && __instance.previousPageButton.containsPoint(x, y))
                {
                    translationKey = "common-ui-previous_page_button";
                }
                else
                {
                    foreach (ClickableComponent languageButton in __instance.languages)
                    {
                        if (languageButton == null || !languageButton.visible || !languageButton.containsPoint(x, y))
                            continue;

                        MainClass.ScreenReader.SayWithMenuChecker(languageButton.name, true);
                        return;
                    }
                }

                MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true);
            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
