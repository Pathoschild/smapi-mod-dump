using Harmony;
using Microsoft.Xna.Framework.Input;
using System;

namespace SplitScreen.Keyboards
{
	[HarmonyPatch(typeof(Microsoft.Xna.Framework.Input.Keyboard))]
	[HarmonyPatch("GetState")]
	[HarmonyPatch(new Type[] { })]
	public static class KeyboardPatcher_GetState
	{
		//Seems unecessary?
		static bool Prefix()
		{
			return !MultipleKeyboardManager.HasAttachedKeyboard();
		}

		static KeyboardState Postfix(KeyboardState k, KeyboardState __result)
		{
			if (MultipleKeyboardManager.HasAttachedKeyboard())
				return MultipleKeyboardManager.GetAttachedKeyboardState() ?? new KeyboardState();

			return __result;
		}
	}
}
