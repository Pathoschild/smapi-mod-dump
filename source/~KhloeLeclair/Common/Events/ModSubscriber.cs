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

using StardewModdingAPI;


namespace Leclair.Stardew.Common.Events {
	public abstract class ModSubscriber : Mod {

		private Dictionary<MethodInfo, RegisteredEvent> Events;

		public override void Entry(IModHelper helper) {
			RegisterEvents();
		}

		public virtual void Log(string message, LogLevel level = LogLevel.Debug, Exception ex = null, LogLevel? exLevel = null) {
			Monitor.Log(message, level: level);
			if (ex != null)
				Monitor.Log($"Details:\n{ex}", level: exLevel ?? level);
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			UnregisterEvents();
		}

		public void RegisterEvents(Action<string, LogLevel> logger = null) {
			Events = EventHelper.RegisterEvents(this, Helper.Events, Events, logger);
		}

		public void UnregisterEvents() {
			if (Events == null)
				return;

			EventHelper.UnregisterEvents(Events);
			Events = null;
		}

	}
}
