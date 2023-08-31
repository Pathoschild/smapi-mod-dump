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
    internal class TitleTextInputMenuPatch
    {
        internal static void DrawPatch(TitleTextInputMenu __instance)
        {
            try
            {
                string toSpeak = "";
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (__instance.pasteButton != null && __instance.pasteButton.containsPoint(x, y))
                    toSpeak = $"Paste button";

                if (toSpeak != "")
                    MainClass.ScreenReader.SayWithChecker(toSpeak, true);
            }
            catch (System.Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
