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
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class TextEntryMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                    original: AccessTools.Method(typeof(TextEntryMenu), nameof(TextEntryMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    prefix: new HarmonyMethod(typeof(TextEntryMenuPatch), nameof(TextEntryMenuPatch.DrawPatch))
            );

            harmony.Patch(
                    original: AccessTools.Method(typeof(TextEntryMenu), nameof(TextEntryMenu.Close)),
                    prefix: new HarmonyMethod(typeof(TextEntryMenuPatch), nameof(TextEntryMenuPatch.ClosePatch))
            );
        }

        internal static void DrawPatch(StardewValley.Menus.TextEntryMenu __instance, StardewValley.Menus.TextBox ____target)
        {
            try
            {
                TextBoxPatch.DrawPatch(____target);
            }
            catch (Exception e)
            {
                Log.Error($"An error occured in DrawPatch() in TextEntryPatch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void ClosePatch()
        {
            TextBoxPatch.activeTextBoxes = "";
        }
    }
}
