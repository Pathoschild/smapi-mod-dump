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
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class ChooseFromIconsMenuPatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(ChooseFromIconsMenu), "draw"),
            postfix: new HarmonyMethod(typeof(ChooseFromIconsMenuPatch), nameof(ChooseFromIconsMenuPatch.DrawPatch))
        );
    }

    private static void DrawPatch(ChooseFromIconsMenu __instance, string ___which, int ___selected)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            // Bobber Machine
            if (___which == "bobbers")
            {
                foreach (var icon in __instance.icons)
                {
                    if (icon is not { visible: true } || !icon.containsPoint(x, y)) continue;

                    int index = int.Parse(icon.name);

                    if (index == -2)
                    {
                        // The random button
                        MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-naming-random_button", true);
                        return;
                    }

                    string styleName = Translator.Instance.Translate("menu-choose_from_icons-bobber_styles", tokens: new
                    {
                        bobber_id = index > (int)(Game1.player.fishCaught.Count() / 2) ? "locked" : $"id_{index + 1}",
                        selected = ___selected == index ? 1 : 0
                    }, translationCategory: TranslationCategory.Menu);

                    MainClass.ScreenReader.SayWithMenuChecker(styleName, true, customQuery: $"{styleName} {icon.myID}");
                    return;
                }
                return;
            }

            // Dwarf Statue
            foreach (var icon in __instance.icons)
            {
                if (icon is not { visible: true } || !icon.containsPoint(x, y)) continue;

                MainClass.ScreenReader.SayWithMenuChecker(icon.hoverText, true);
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in choose from icons menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
