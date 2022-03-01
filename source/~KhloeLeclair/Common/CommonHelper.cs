/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using StardewValley.Menus;


namespace Leclair.Stardew.Common {
	public static class CommonHelper {
		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T> {
			if (value.CompareTo(min) < 0) return min;
			if (value.CompareTo(max) > 0) return max;
			return value;
		}

		public static IEnumerable<T> GetValues<T>() {
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		public static void YeetMenu(IClickableMenu menu) {
			if (menu == null) return;

			MethodInfo CleanupMethod = menu.GetType().GetMethod("cleanupBeforeExit", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			menu.behaviorBeforeCleanup?.Invoke(menu);

			if (CleanupMethod != null && CleanupMethod.GetParameters().Length == 0)
				CleanupMethod.Invoke(menu, null);

			if (menu.exitFunction != null) {
				IClickableMenu.onExit exitFunction = menu.exitFunction;
				menu.exitFunction = null;
				exitFunction();
			}
		}

	}
}
