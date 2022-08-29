/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;

namespace Shockah.DontStopMeNow
{
	internal static class InputHelper
	{
		public enum ButtonType
		{
			Mouse,
			Keyboard,
			Gamepad
		}

		public static ButtonType? GetButtonType(this SButton button)
		{
			switch (button)
			{
				case SButton.MouseLeft:
				case SButton.MouseRight:
				case SButton.MouseMiddle:
				case SButton.MouseX1:
				case SButton.MouseX2:
					return ButtonType.Mouse;
				default:
					if (button.TryGetKeyboard(out _))
						return ButtonType.Keyboard;
					if (button.TryGetController(out _))
						return ButtonType.Gamepad;
					return null;
			}
		}

		public static bool IsPressed(this SButton button)
		{
			switch (button)
			{
				case SButton.MouseLeft:
					return Game1.oldMouseState.LeftButton == ButtonState.Pressed;
				case SButton.MouseRight:
					return Game1.oldMouseState.RightButton == ButtonState.Pressed;
				case SButton.MouseMiddle:
					return Game1.oldMouseState.MiddleButton == ButtonState.Pressed;
				case SButton.MouseX1:
					return Game1.oldMouseState.XButton1 == ButtonState.Pressed;
				case SButton.MouseX2:
					return Game1.oldMouseState.XButton2 == ButtonState.Pressed;
				default:
					if (button.TryGetKeyboard(out var key))
						return Game1.oldKBState.IsKeyDown(key);
					if (button.TryGetController(out var controllerButton))
						return Game1.oldPadState.IsButtonDown(controllerButton);
					return false;
			}
		}
	}
}
