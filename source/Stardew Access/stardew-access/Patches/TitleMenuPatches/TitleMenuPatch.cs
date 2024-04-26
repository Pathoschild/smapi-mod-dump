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
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class TitleMenuPatch : IPatch
{
    private static bool HasSpokenSkipMessage = false;
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
                original: AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(TitleMenuPatch), nameof(TitleMenuPatch.DrawPatch))
        );
    }

    private static void DrawPatch(TitleMenu __instance, bool ___isTransitioningButtons)
    {
        try
        {
            if (___isTransitioningButtons)
                return;

            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
            string translationKey = "";
            object? translationTokens = null;

            if (TitleMenu.subMenu == null)
            {
                if (__instance.muteMusicButton.containsPoint(x, y))
                {
                    translationKey = "menu-title-mute_music_button";
                }
                else if (__instance.aboutButton.containsPoint(x, y))
                {
                    translationKey = "menu-title-about_button";
                }
                else if (__instance.languageButton.containsPoint(x, y))
                {
                    translationKey = "menu-title-language_button";
                }
                else if (__instance.windowedButton.containsPoint(x, y))
                {
                    translationKey = "menu-title-fullscreen_button";
                    translationTokens = new
                    {
                        is_enabled = Game1.isFullscreen ? 1 : 0
                    };
                }
                else if (__instance.fadeFromWhiteTimer <= 0)
                {
                    if (!HasSpokenSkipMessage)
                    {
                        MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-title-click_to_skip", true);
                        HasSpokenSkipMessage = true;
                    }
                    if (__instance.titleInPosition && __instance.showButtonsTimer <= 0)
                    {
                        foreach (var button in __instance.buttons)
                        {
                            if (!button.containsPoint(x, y))
                                continue;

                            translationKey = GetTranslationKeyForButton(button.name);
                            break;
                        }
                    }
                }
            }
            else if (TitleMenu.subMenu != null)
            {
                if (__instance.backButton.containsPoint(x, y) && TitleMenu.subMenu is not CharacterCustomization)
                {
                    translationKey = "common-ui-back_button";
                    MouseUtils.SimulateMouseClicks(
                        (_, _) => __instance.backButtonPressed(),
                        null
                    );
                }
            }

            if (!string.IsNullOrEmpty(translationKey))
                MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in title menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }

    private static string GetTranslationKeyForButton(string buttonName) => buttonName.ToLower() switch
    {
        "new" => "menu-title-new_game_button",
        "co-op" => "menu-title-co_op_button",
        "load" => "menu-title-load_button",
        "exit" => "menu-title-exit_button",
        "invite" => "menu-title-invite_button",
        _ => ""
    };
}
