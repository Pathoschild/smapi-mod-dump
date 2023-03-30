/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

namespace stardew_access.Patches
{
    internal class CollectionsPagePatch
    {
        internal static void DrawPatch(StardewValley.Menus.CollectionsPage __instance)
        {
            try
            {
                int x = StardewValley.Game1.getMousePosition().X, y = StardewValley.Game1.getMousePosition().Y;
                if (__instance.letterviewerSubMenu != null)
                {
                    LetterViwerMenuPatch.narrateLetterContent(__instance.letterviewerSubMenu);
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
