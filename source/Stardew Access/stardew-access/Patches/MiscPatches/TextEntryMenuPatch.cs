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
    internal class TextEntryMenuPatch
    {
        internal static void DrawPatch(StardewValley.Menus.TextEntryMenu __instance, StardewValley.Menus.TextBox ____target)
        {
            try
            {
                TextBoxPatch.DrawPatch(____target);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"An error occured in DrawPatch() in TextEntryPatch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void ClosePatch()
        {
            TextBoxPatch.activeTextBoxes = "";
        }
    }
}
