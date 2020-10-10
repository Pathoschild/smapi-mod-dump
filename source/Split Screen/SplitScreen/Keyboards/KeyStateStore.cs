/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace SplitScreen.Keyboards
{
	class KeyStateStore
	{
		//Optimization in case GetKeyboardState() called more than once in tick
		private bool cacheNeedsUpdating = true;
		private KeyboardState cachedState;

		// Button(aka Keys)
		// 1=Down,0=Up
		private Dictionary<int, int> keyStates = new Dictionary<int, int>();

		public void SetKeyState (int key, int state)
		{
			keyStates.Remove(key);
			keyStates.Add(key, state);
			cacheNeedsUpdating = true;
		}

		public void Clear()
		{
			keyStates.Clear();
			cacheNeedsUpdating = true;
		}

		/// <summary>
		/// state=true: pressed
		/// </summary>
		public void SetKeyState(int key, bool state) => SetKeyState(key, state ? 1 : 0);

		public IEnumerable<Keys> GetPressedKeys()
		{
			return (keyStates.Where(x => x.Value == 1)).Select(x => (Keys)x.Key);
		}

		public KeyboardState GetKeyboardState()
		{
			if (cacheNeedsUpdating)
				cachedState = new KeyboardState(GetPressedKeys().ToArray());

			cacheNeedsUpdating = false;
			return cachedState;
		}
	}
}
