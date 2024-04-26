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
using System.Reflection;

using StardewModdingAPI;

using Leclair.Stardew.Common.Types;
using StardewModdingAPI.Events;
using System.Text;
using System.Linq;

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
			for(int i = 0; i < entry.Length; i++)
				longest[i] = Math.Max(longest[i], entry[i].Length);
		}

		// Build a format string.
		StringBuilder sb = new();

		for(int i = 0; i < longest.Length; i++) {
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
		Events = EventHelper.RegisterEvents(this, Helper.Events, Events, logger ?? ((msg, level) => Log(msg, level)));
	}

	public void UnregisterEvents() {
		if (Events == null)
			return;

		EventHelper.UnregisterEvents(Events);
		Events = null;
	}

	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		EventHelper.RegisterConsoleCommands(this, Helper.ConsoleCommands, (msg, level) => Log(msg, level));
	}

	public void CheckRecommendedIntegrations() {
		// Missing Integrations?
		RecommendedIntegration[]? integrations;

		try {
			integrations = Helper.Data.ReadJsonFile<RecommendedIntegration[]>("assets/recommended_integrations.json");
			if (integrations == null) {
				Log("No recommendations found. Our data file seems to be missing.");
				return;
			}
		} catch (Exception ex) {
			Log($"Unable to load recommended integrations data file.", LogLevel.Warn, ex);
			return;
		}

		LoadingHelper.CheckIntegrations(this, integrations);
	}

}
