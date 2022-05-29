/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpriteMaster;

internal static partial class ConsoleSupport {
	private static readonly Dictionary<string, (Action<string, Queue<string>> Action, string Description)> CommandMap = new() {
		{ "help", ((_, _) => InvokeHelp(null), "Prints this command guide") },
		{ "all-stats", ((_, _) => Debug.DumpAllStats(), "Dump Statistics") },
		{ "memory", ((_, _) => Debug.DumpMemory(), "Dump Memory") },
		{ "gc", ((_, _) => SpriteMaster.Self.MemoryMonitor.TriggerGarbageCollection(), "Trigger full GC") },
		{ "purge", ((_, _) => SpriteMaster.Self.MemoryMonitor.TriggerPurge(), "Trigger Purge") }
	};

	static ConsoleSupport() {
		foreach (var type in SpriteMaster.Assembly.GetTypes()) {
			foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public)) {
				var command = method.GetCustomAttribute<CommandAttribute>();
				if (command is not null) {
					var parameters = method.GetParameters();
					if (parameters.Length != 2) {
						Debug.Error($"Console command '{command.Name}' for method '{method.GetFullName()}' does not have the expected number of parameters");
						continue;
					}
					if (parameters[0].ParameterType != typeof(string)) {
						Debug.Error($"Console command '{command.Name}' for method '{method.GetFullName()}' : parameter 0 type {parameters[0].ParameterType} is not {typeof(string)}");
						continue;
					}
					if (parameters[1].ParameterType != typeof(Queue<string>)) {
						Debug.Error($"Console command '{command.Name}' for method '{method.GetFullName()}' : parameter 1 type {parameters[1].ParameterType} is not {typeof(Queue<string>)}");
						continue;
					}

					if (CommandMap.ContainsKey(command.Name)) {
						Debug.Error($"Console command is already registered: '{command.Name}'");
						continue;
					}

					CommandMap.Add(command.Name, (method.CreateDelegate<Action<string, Queue<string>>>(), command.Description));
				}
			}
		}
	}

	internal static void Invoke(string command, string[] arguments) {
		var argumentQueue = new Queue<string>(arguments);

		if (argumentQueue.Count == 0) {
			InvokeHelp();
			return;
		}

		var subCommand = argumentQueue.Dequeue().ToLowerInvariant();
		if (CommandMap.TryGetValue(subCommand, out var commandPair)) {
			commandPair.Action(subCommand, argumentQueue);
		}
		else {
			InvokeHelp(subCommand);
		}
	}
}
