/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using MusicMaster.Extensions.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MusicMaster;

internal static partial class ConsoleSupport {
	internal delegate void CallbackDelegate(string command, Queue<string> arguments);
	internal readonly record struct Command(CallbackDelegate Action, string Description);

	private static readonly Dictionary<string, Command> CommandMap = new() {
		{ "help", new((_, _) => InvokeHelp(CommandMap!), "Prints this command guide") }
	};

	static ConsoleSupport() {
		var commandMap = new ConcurrentDictionary<string, Command>();

		MusicMaster.Assembly.GetTypes().AsParallel()
			.SelectMany(type => type.GetStaticMethods()).ForAll(
				method => {
					if (method.GetCustomAttribute<CommandAttribute>() is not { } command) {
						return;
					}

					var parameters = method.GetParameters();
					if (parameters.Length != 2) {
						Debug.Error(
							$"Console command '{command.Name}' for method '{method.GetFullName()}' does not have the expected number of parameters"
						);
						return;
					}

					if (parameters[0].ParameterType != typeof(string)) {
						Debug.Error(
							$"Console command '{command.Name}' for method '{method.GetFullName()}' : parameter 0 type {parameters[0].ParameterType} is not {typeof(string)}"
						);
						return;
					}

					if (parameters[1].ParameterType != typeof(Queue<string>)) {
						Debug.Error(
							$"Console command '{command.Name}' for method '{method.GetFullName()}' : parameter 1 type {parameters[1].ParameterType} is not {typeof(Queue<string>)}"
						);
						return;
					}

					if (!commandMap.TryAdd(command.Name, new(method.CreateDelegate<CallbackDelegate>(), command.Description))) {
						Debug.Error($"Console command is already registered: '{command.Name}'");
					}
				}
		);

		CommandMap.EnsureCapacity(CommandMap.Count + commandMap.Count);
		foreach (var pair in commandMap) {
			if (!CommandMap.TryAdd(pair.Key, pair.Value)) {
				Debug.Error($"Console command is already registered: '{pair.Key}'");
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
