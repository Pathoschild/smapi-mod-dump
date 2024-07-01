/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace RadialMenu.Gmcm;

internal record ClickRepeat(double DelayMs, double IntervalMs);

internal class ClickDetector(ClickRepeat? repeat = null)
{
    // GMCM checks oldMouseState and oldPadState instead of tracking directly, but it doesn't seem
    // to work here; the old state is always the same as the current state and there is never a
    // frame when the button is pressed but "old" state was released.
    private bool wasLeftMouseButtonPressed = false;
    private bool wasPadAButtonPressed = false;
    private bool isRepeating = false;
    private double lastClickTime;

    public bool HasLeftClick()
    {
        var isLeftMouseButtonPressed =
            Game1.input.GetMouseState().LeftButton == ButtonState.Pressed;
        var isPadAButtonPressed = Game1.input.GetGamePadState().IsButtonDown(Buttons.A);

        var hasNewClick =
            isLeftMouseButtonPressed && !wasLeftMouseButtonPressed
            || isPadAButtonPressed && !wasPadAButtonPressed;

        var result = hasNewClick;
        if (repeat is not null)
        {
            var currentTime = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
            if (hasNewClick)
            {
                isRepeating = false;
                lastClickTime = currentTime;
            }
            else
            {
                var hasHeldClick =
                    isLeftMouseButtonPressed && wasLeftMouseButtonPressed
                    || isPadAButtonPressed && wasPadAButtonPressed;
                if (hasHeldClick)
                {
                    var elapsedSinceLastClick = currentTime - lastClickTime;
                    if ((!isRepeating && elapsedSinceLastClick > repeat.DelayMs)
                        || (isRepeating && elapsedSinceLastClick > repeat.IntervalMs))
                    {
                        isRepeating = true;
                        lastClickTime = currentTime;
                        result = true;
                    }
                }
            }
        }

        wasLeftMouseButtonPressed = isLeftMouseButtonPressed;
        wasPadAButtonPressed = isPadAButtonPressed;

        return result;
    }
}
