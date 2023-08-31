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
    internal class MineElevatorMenuPatch
    {
        internal static void DrawPatch(List<ClickableComponent> ___elevators)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                for (int i = 0; i < ___elevators.Count; i++)
                {
                    if (___elevators[i].containsPoint(x, y))
                    {
                        MainClass.ScreenReader.SayWithMenuChecker($"{___elevators[i].name} level", true);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
