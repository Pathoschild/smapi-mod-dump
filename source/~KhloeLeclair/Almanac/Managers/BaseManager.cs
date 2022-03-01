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

using Leclair.Stardew.Common.Events;

using StardewModdingAPI;

namespace Leclair.Stardew.Almanac.Managers {
	public class BaseManager : EventSubscriber<ModEntry> {

		public readonly string Name;

		public BaseManager(ModEntry mod, string name = null) : base(mod) {
			Name = name ?? GetType().Name;
		}

		protected void Log(string message, LogLevel level = LogLevel.Debug, Exception ex = null) {
			Mod.Monitor.Log($"[{Name}] {message}", level: level);
			if (ex != null)
				Mod.Monitor.Log($"[{Name}] Details:\n{ex}", level: level);
		}

	}
}
