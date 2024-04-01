/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Framework;
using HappyHomeDesigner.Menus;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using System.Reflection;

namespace HappyHomeDesigner.Patches
{
	internal class SearchFocusFix
	{
		public static void Apply(Harmony harmony)
		{
			harmony.TryPatch(
				typeof(Game1).GetProperty(nameof(Game1.IsChatting)).GetMethod,
				postfix: new(typeof(SearchFocusFix), nameof(IsActive))
			);

			harmony.TryPatch(
				typeof(Game1).GetMethod("UpdateChatBox", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic),
				prefix: new(typeof(SearchFocusFix), nameof(SkipChat))
			);
		}

		public static bool IsActive(bool chatActive)
			=> chatActive || Game1.keyboardDispatcher.Subscriber is SearchBox;

		public static bool SkipChat()
		{
			bool run = Game1.keyboardDispatcher.Subscriber is not SearchBox;

			if (run)
				return true;

			KeyboardState keyState = Game1.input.GetKeyboardState();
			GamePadState padState = Game1.input.GetGamePadState();

			if (Game1.input.GetMouseState().RightButton is ButtonState.Pressed)
				(Game1.keyboardDispatcher.Subscriber as SearchBox).Reset();

			if (keyState.IsKeyDown(Keys.Escape) || padState.IsButtonDown(Buttons.B) || padState.IsButtonDown(Buttons.Back))
			{
				Game1.oldKBState = keyState;
				Game1.oldPadState = padState;
				Game1.keyboardDispatcher.Subscriber = null;
			}

			return false;
		}
	}
}
