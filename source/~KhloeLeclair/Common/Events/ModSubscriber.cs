/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Leclair.Stardew.Common.Events;

public abstract class ModSubscriber : Mod {

	private Dictionary<MethodInfo, RegisteredEvent>? Events;

	public override void Entry(IModHelper helper) {
		RegisterEvents();

		Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
	}

	public virtual void Log(string message, LogLevel level = LogLevel.Debug, Exception? ex = null, LogLevel? exLevel = null, bool once = false) {
		if (once)
			Monitor.LogOnce(message, level: level);
		else
			Monitor.Log(message, level: level);

		if (ex != null) {
			string errMessage = $"Details:\n{ex}";
			if (once)
				Monitor.LogOnce(errMessage, level: exLevel ?? level);
			else
				Monitor.Log(errMessage, level: exLevel ?? level);
		}
	}

	public virtual void LogTable(StringBuilder sb, string[]? headers, IEnumerable<string[]> entries, LogLevel level = LogLevel.Debug, string separator = "  ") {
		// First, determine the maximum column count.
		int columns = 0;

		if (headers is not null)
			columns = headers.Length;
		else {
			foreach (string[] entry in entries)
				columns = Math.Max(columns, entry.Length);
		}

		// Now determine the length of each column.
		int[] longest = new int[columns];

		if (headers is not null) {
			for (int i = 0; i < headers.Length; i++)
				longest[i] = headers[i].Length;
		}

		foreach (string[] entry in entries) {
			for (int i = 0; i < entry.Length; i++)
				longest[i] = Math.Max(longest[i], entry[i].Length);
		}

		// Build a format string.
		StringBuilder sb2 = new();

		for (int i = 0; i < longest.Length; i++) {
			if (i > 0)
				sb2.Append(separator);
			sb2.Append($"{{{i},-{longest[i]}}}");
		}

		string fmt = sb2.ToString();

		if (headers is not null) {
			sb.AppendLine(string.Format(fmt, args: headers));

			int sum = longest.Sum() + (columns - 1) * separator.Length;
			sb.AppendLine(new string('=', sum));
		}

		foreach (string[] entry in entries) {
			string[] args;
			if (entry.Length < columns) {
				args = new string[columns];
				Array.Copy(entry, args, entry.Length);
			} else
				args = entry;

			sb.AppendLine(string.Format(fmt, args: args));
		}
	}

	public virtual void LogTable(string[]? headers, IEnumerable<string[]> entries, LogLevel level = LogLevel.Debug, string separator = "  ") {
		// First, determine the maximum column count.
		int columns = 0;

		if (headers is not null)
			columns = headers.Length;
		else {
			foreach (string[] entry in entries)
				columns = Math.Max(columns, entry.Length);
		}

		// Now determine the length of each column.
		int[] longest = new int[columns];

		if (headers is not null) {
			for (int i = 0; i < headers.Length; i++)
				longest[i] = headers[i].Length;
		}

		foreach (string[] entry in entries) {
			for (int i = 0; i < entry.Length; i++)
				longest[i] = Math.Max(longest[i], entry[i].Length);
		}

		// Build a format string.
		StringBuilder sb = new();

		for (int i = 0; i < longest.Length; i++) {
			if (i > 0)
				sb.Append(separator);
			sb.Append($"{{{i},-{longest[i]}}}");
		}

		string fmt = sb.ToString();

		if (headers is not null) {
			Monitor.Log(string.Format(fmt, args: headers), level);

			int sum = longest.Sum() + (columns - 1) * separator.Length;
			Monitor.Log(
				new string('=', sum),
				level
			);
		}

		foreach (string[] entry in entries) {
			string[] args;
			if (entry.Length < columns) {
				args = new string[columns];
				Array.Copy(entry, args, entry.Length);
			} else
				args = entry;

			Monitor.Log(string.Format(fmt, args: args), level);
		}
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		UnregisterEvents();
	}

	public void RegisterEvents(Action<string, LogLevel>? logger = null) {
		Events = EventHelper.RegisterEvents(this, Helper.Events, Events, logger ?? Monitor.Log);
	}

	public void UnregisterEvents() {
		if (Events == null)
			return;

		EventHelper.UnregisterEvents(Events);
		Events = null;
	}

	protected virtual void RegisterTriggerActions() {
		List<string> registered = EventHelper.RegisterTriggerActions(this, $"{ModManifest.UniqueID}_", Monitor.Log);
		registered.AddRange(EventHelper.RegisterTriggerActions(GetType(), $"{ModManifest.UniqueID}_", Monitor.Log));

		if (registered.Count > 0)
			Log($"Registered trigger actions: {string.Join(", ", registered)}", LogLevel.Trace);
	}

	protected virtual void RegisterGameStateQueries() {
		List<string> registered = EventHelper.RegisterGameStateQueries(this, [$"{ModManifest.UniqueID}_"], Monitor.Log);
		registered.AddRange(EventHelper.RegisterGameStateQueries(GetType(), [$"{ModManifest.UniqueID}_"], Monitor.Log));

		if (registered.Count > 0)
			Log($"Registered Game State Query conditions: {string.Join(", ", registered)}", LogLevel.Trace);
	}

	protected virtual void RegisterConsoleCommands() {
		List<string> registered = EventHelper.RegisterConsoleCommands(this, Helper.ConsoleCommands, Monitor.Log);
		registered.AddRange(EventHelper.RegisterConsoleCommands(GetType(), Helper.ConsoleCommands, Monitor.Log));

		if (registered.Count > 0)
			Log($"Registered console commands: {string.Join(", ", registered)}", LogLevel.Trace);
	}

	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		RegisterTriggerActions();
		RegisterGameStateQueries();
		RegisterConsoleCommands();
	}

}
