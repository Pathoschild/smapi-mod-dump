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
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ExitPagePatch
    {
        internal static void DrawPatch(ExitPage __instance)
        {
            try
            {
                if (__instance.exitToTitle.visible &&
                        __instance.exitToTitle.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    string toSpeak = "Exit to Title Button";
                    MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
                    return;
                }
                if (__instance.exitToDesktop.visible &&
                    __instance.exitToDesktop.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    string toSpeak = "Exit to Desktop Button";
                    MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
                    return;
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
