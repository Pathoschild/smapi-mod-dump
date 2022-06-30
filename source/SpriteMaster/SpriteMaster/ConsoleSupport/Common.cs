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
using System.Collections.Generic;
using System.Reflection;

namespace SpriteMaster;

internal static partial class ConsoleSupport {
	internal delegate void CallbackDelegate(string command, Queue<string> arguments);
	internal readonly record struct Command(CallbackDelegate Action, string Description);

	private static readonly Dictionary<string, Command> CommandMap = new() {
		{ "help", new((_, _) => InvokeHelp(CommandMap!), "Prints this command guide") },
		{ "all-stats", new((_, _) => Debug.DumpAllStats(), "Dump Statistics") },
		{ "memory", new((_, _) => Debug.DumpMemory(), "Dump Memory") },
		{ "purge", new((_, _) => SpriteMaster.Self.MemoryMonitor.TriggerPurge(), "Trigger Purge") }
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

					CommandMap.Add(command.Name, new(method.CreateDelegate<CallbackDelegate>(), command.Description));
				}
			}
		}
	}

	internal static void Invoke(string command, string[] arguments) =>
		Invoke(CommandMap, command, arguments);

	internal static void Invoke(Dictionary<string, Command> commandMap, string command, string[] arguments) =>
		Invoke(commandMap, command, new Queue<string>(arguments));

	internal static void Invoke(Dictionary<string, Command> commandMap, string command, Queue<string> arguments) {
		if (arguments.Count == 0) {
			InvokeHelp(commandMap);
			return;
		}

		var subCommand = arguments.Dequeue().ToLowerInvariant();
		if (commandMap.TryGetValue(subCommand, out var commandPair)) {
			commandPair.Action(subCommand, arguments);
		}
		else {
			InvokeHelp(commandMap, subCommand);
		}
	}
}
