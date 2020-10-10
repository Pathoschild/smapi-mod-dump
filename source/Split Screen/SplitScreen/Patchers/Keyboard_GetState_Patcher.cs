/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework.Input;
using SplitScreen.Keyboards;
using System;

namespace SplitScreen.Patchers
{
	//Return attached keyboard if there is one
	//If not, ignore any keyboard input when window is inactive (Don't use Main.IsActive, that is always set to true)

	[HarmonyPatch(typeof(Microsoft.Xna.Framework.Input.Keyboard))]
	[HarmonyPatch("GetState")]
	[HarmonyPatch(new Type[] { typeof(Microsoft.Xna.Framework.PlayerIndex) })]
	public class Keyboard_GetState_Patcher
	{
		public static KeyboardState Postfix(KeyboardState m, KeyboardState __result)
		{
			if (MultipleKeyboardManager.HasAttachedKeyboard())
				return MultipleKeyboardManager.GetAttachedKeyboardState() ?? default(KeyboardState);
			else if (!Utils.TrueIsWindowActive())
				return default(KeyboardState);
			else
				return __result;
		}
	}
}
