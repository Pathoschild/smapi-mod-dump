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

using StardewModdingAPI;

using Leclair.Stardew.BreakPintail;

namespace Leclair.Stardew.BreakPintailClient;

public class ModEntry : Mod {

	public override void Entry(IModHelper helper) {
		helper.Events.GameLoop.GameLaunched += OnGameLaunched;
	}

	private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {

		var api = Helper.ModRegistry.GetApi<IBreakPintailApi>("leclair.stardew.breakpintail");
		if (api is null) {
			Monitor.Log($"Could not get BreakPintail API.", LogLevel.Error);
			return;
		}

		var thing = api.GetThingGetter();

		Monitor.Log($"Starting", LogLevel.Warn);

		// This fails
		Monitor.Log($"Enumerate directly:", LogLevel.Info);
		try {
			foreach (var entry in thing)
				Monitor.Log($"Thing: {entry.Key}: {entry.Value.Value}", LogLevel.Info);
		} catch (Exception ex) {
			Monitor.Log($"Error: {ex}", LogLevel.Error);
		}

		// This fails
		Monitor.Log($"Enumerate sub-readonlydict:", LogLevel.Info);
		try {
			foreach (var entry in thing.CalculatedValues)
				Monitor.Log($"Thing: {entry.Key}: {entry.Value.Value}", LogLevel.Info);
		} catch (Exception ex) {
			Monitor.Log($"Error: {ex}", LogLevel.Error);
		}

		// This fails
		Monitor.Log($"Manual Enumerator:", LogLevel.Info);
		try {
			var enumerator = thing.GetEnumerator();
			while (enumerator.MoveNext()) {
				var current = enumerator.Current;
				Monitor.Log($"Thing: {current.Key}: {current.Value.Value}", LogLevel.Info);
			}
		} catch(Exception ex) {
			Monitor.Log($"Error: {ex}", LogLevel.Error);
		}

		// This is ok
		Monitor.Log($"Enumerate Keys, access values directly:", LogLevel.Info);
		try {
			foreach (string entry in thing.Keys)
				Monitor.Log($"Thing: {entry}: {thing[entry].Value}", LogLevel.Info);
		} catch(Exception ex) {
			Monitor.Log($"Error: {ex}", LogLevel.Error);
		}

		Monitor.Log($"Done", LogLevel.Info);

	}
}
