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
    internal class CollectionsPagePatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(CollectionsPage), nameof(CollectionsPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(CollectionsPagePatch), nameof(CollectionsPagePatch.DrawPatch))
            );
        }

        internal static void DrawPatch(CollectionsPage __instance)
        {
            try
            {
                int x = StardewValley.Game1.getMousePosition().X, y = StardewValley.Game1.getMousePosition().Y;
                if (__instance.letterviewerSubMenu != null)
                {
                    LetterViewerMenuPatch.NarrateMenu(__instance.letterviewerSubMenu);
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"An error occurred in collections page patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
