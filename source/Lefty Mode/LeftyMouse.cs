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
