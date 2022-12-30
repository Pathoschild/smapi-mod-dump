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
using System.Runtime.CompilerServices;
using System.Text;

using StardewModdingAPI;

namespace Leclair.Stardew.Common.Extensions;

internal static class EventExtensions {

	internal static void SafeInvoke(this EventHandler evt, object? sender, IMonitor? monitor = null, [CallerArgumentExpression("evt")] string name = "") {
		Delegate[]? handlers = evt?.GetInvocationList();
		if (handlers?.Length is null or 0)
			return;

		foreach (Delegate handler in handlers) {
			if (handler is EventHandler thandler)
				try {
					thandler.Invoke(sender, EventArgs.Empty);

				} catch (Exception ex) {
					monitor?.Log($"Exception while handling event {name}: {ex}", LogLevel.Error);
				}
		}
	}

	internal static void SafeInvoke<T>(this EventHandler<T> evt, object? sender, T args, IMonitor? monitor = null, [CallerArgumentExpression("evt")] string name = "") {
		Delegate[]? handlers = evt?.GetInvocationList();
		if (handlers?.Length is null or 0)
			return;

		foreach(Delegate handler in handlers) {
			if (handler is EventHandler<T> thandler)
				try {
					thandler.Invoke(sender, args);

				} catch (Exception ex) {
					monitor?.Log($"Exception while handling event {name}: {ex}", LogLevel.Error);
				}
		}
	}

}
