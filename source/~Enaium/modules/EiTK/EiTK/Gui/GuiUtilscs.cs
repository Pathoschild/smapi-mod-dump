/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

namespace EiTK.Utils
{
    public class GuiUtils
    {
        public static bool isHovered(int mouseX, int mouseY, int x, int y, int width, int height)
        {
            return mouseX >= x && mouseX - width <= x && mouseY >= y && mouseY - height <= y;
        }
    }
}