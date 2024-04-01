/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicMaster;

internal static partial class ConsoleSupport {
	internal static void InvokeHelp(Dictionary<string, Command> commandMap, string? unknownCommand = null) {
		var output = new StringBuilder();
		output.AppendLine();
		output.AppendLine(Versioning.StringHeader);
		if (unknownCommand is not null) {
			output.AppendLine($"Unknown Command: '{unknownCommand}'");
		}
		output.AppendLine("Help Command Guide");
		output.AppendLine();

		int maxKeyLength = commandMap.Keys.Max(k => k.Length);

		foreach (var kv in commandMap) {
			output.AppendLine($"{kv.Key.PadRight(maxKeyLength)} : {kv.Value.Description}");
		}

		Debug.Message(output.ToString());
	}
}
