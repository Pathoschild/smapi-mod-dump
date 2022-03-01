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
using System.Reflection;


/* Unmerged change from project 'MoveToConnected'
Before:
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Leclair.Stardew.Common.Events;
After:
using Leclair.Stardew.Common.Events;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley.Menus;
*/
using StardewModdingAPI;

using StardewValley.Menus;


namespace Leclair.Stardew.Common.Events {
	public abstract class MenuSubscriber<T> : IClickableMenu, IDisposable where T : Mod {

		public readonly T Mod;

		private Dictionary<MethodInfo, RegisteredEvent> Events;

		public MenuSubscriber(T mod, int x, int y, int width, int height, bool registerImmediate = true) : base(x, y, width, height) {
			Mod = mod;

			if (registerImmediate)
				RegisterEvents();
		}
		public virtual void Dispose() {
			UnregisterEvents();
		}

		protected virtual void Log(string message, LogLevel level = LogLevel.Trace, Exception ex = null, string name = null) {
			if (string.IsNullOrEmpty(name))
				name = GetType().Name;

			Mod.Monitor.Log($"[{name}] {message}", level: level);
			if (ex != null)
				Mod.Monitor.Log($"[{name}] Details:\n{ex}", level: level);
		}

		public void RegisterEvents() {
			Events = EventHelper.RegisterEvents(this, Mod.Helper.Events, Events, (msg, level) => Log(msg, level));
		}

		public void UnregisterEvents() {
			if (Events == null)
				return;

			EventHelper.UnregisterEvents(Events);
			Events = null;
		}
	}
}
