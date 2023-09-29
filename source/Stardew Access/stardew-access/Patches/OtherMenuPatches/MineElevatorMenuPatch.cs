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
    internal class MineElevatorMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(MineElevatorMenu), nameof(MineElevatorMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MineElevatorMenuPatch), nameof(MineElevatorMenuPatch.DrawPatch))
            );
        }

        internal static void DrawPatch(List<ClickableComponent> ___elevators)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                foreach (var el in ___elevators)
                {
                    if (!el.containsPoint(x, y))
                        continue;
                    
                    MainClass.ScreenReader.SayWithMenuChecker(el.name, true);
                    break;
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in mine elevator patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
