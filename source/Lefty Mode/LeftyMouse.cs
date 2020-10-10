/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/LeftyMode
**
*************************************************/

using Microsoft.Xna.Framework.Input;

namespace LeftyMode
{
    public class LeftyMouse
    {
        public static MouseState GetState(MouseState ms)
        {
            return new MouseState(
                ms.X,
                ms.Y,
                ms.ScrollWheelValue,
                ms.RightButton,
                ms.MiddleButton,
                ms.LeftButton,
                ms.XButton1,
                ms.XButton2
                );
        }
    }
}
