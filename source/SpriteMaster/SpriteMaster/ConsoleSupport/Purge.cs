/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using HarmonyLib;
using SpriteMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpriteMaster;

internal static partial class ConsoleSupport {
	internal static void InvokePurgeHelp(string command, Queue<string> arguments) {

	}

	internal static void InvokePurge(string command, Queue<string> arguments) {
		if (arguments.Count == 0) {
			arguments.AddItem("all");
		}

		bool hard = false;
		bool soft = false;
		bool resident = false;
		bool file = false;
		bool texture = false;
		bool suspended = false;
		bool collect = true;

		HashSet<string>? unknownArguments = null;

		foreach (var argument in arguments) {
			var invariantArgument = argument.ToLowerInvariant();

			bool enable = true;

			if (invariantArgument.StartsWith('-')) {
				enable = false;
				invariantArgument = invariantArgument[1..];
			}
			else if (invariantArgument.StartsWith("no-")) {
				enable = false;
				invariantArgument = invariantArgument[3..];
			}

			switch (invariantArgument) {
				case "all":
					hard = enable;
					soft = enable;
					resident = enable;
					file = enable;
					texture = enable;
					suspended = enable;
					break;
				case "hard":
					hard = enable;
					break;
				case "soft":
					soft = enable;
					break;
				case "resident":
				case "residentcache":
					resident = enable;
					break;
				case "file":
				case "filecache":
					file = enable;
					break;
				case "texture":
				case "texturecache":
					texture = enable;
					break;
				case "suspended":
				case "suspendedcache":
					suspended = enable;
					break;
				case "collect":
				case "gc":
					collect = enable;
					break;
				default:
					unknownArguments ??= new(StringComparer.InvariantCultureIgnoreCase);
					unknownArguments.Add(argument);
					break;
			}
		}

		if (unknownArguments is not null) {
			Debug.Error($"Unknown purge modes: {string.Join(", ", unknownArguments.Select(arg => $"'{arg}'"))}");
			InvokePurgeHelp(command, arguments);
			return;
		}

		if (collect) {
			Garbage.Collect(compact: false, blocking: false, background: true);
		}

		if (file) {
			Caching.FileCache.Purge();
		}

		if (texture) {
			Caching.TextureFileCache.Purge();
		}

		if (suspended) {
			Caching.SuspendedSpriteCache.Purge();
		}

		if (resident) {
			Caching.ResidentCache.Purge();
		}

		if (hard || soft) {
			SpriteMaster.Self.MemoryMonitor.TriggerPurge(hard, soft, collect);
		}

		if (collect) {
			Garbage.Collect(compact: true, blocking: true, background: false);
		}
	}
}