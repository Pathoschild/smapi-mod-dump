/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Tools.Preview.Converter;
using SpriteMaster.Tools.Preview.Preview;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SpriteMaster.Tools.Preview;

public static class Program {
	[STAThread]
	public static int Main(string[] args) {
		var arguments = LexArgs(args);
		arguments = ParseArgs(arguments, out var options);

		AbstractProgram program = options.Preview ? new PreviewProgram() : new ConverterProgram();
		return program.SubMain(options, arguments);
	}

	private static readonly Regex ArgumentPattern = new(@"^(.+)[=:](.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

	private static List<Argument> LexArgs(string[] args) {
		int i = 0;

		string? PopArg() {
			if (i < args.Length) {
				return args[i++];
			}
			return null;
		}

		bool TryPopArg([NotNullWhen(true)] out string? arg) {
			arg = PopArg();
			return arg is not null;
		}

		var result = new List<Argument>(args.Length);

		while (i < args.Length) {
			if (!TryPopArg(out var arg)) {
				break;
			}

			string key;
			string? value = null;
			if (arg[0] is '-' or '/' && (arg.Contains('=') || arg.Contains(':'))) {
				var match = ArgumentPattern.Match(arg);
				key = match.Groups[1].Value;
				value = match.Groups[2].Value;
			}
			else {
				key = arg;
			}

			result.Add(new(key, value));
		}

		return result;
	}

	private static List<Argument> ParseArgs(List<Argument> args, out Options options) {
		bool preview = false;
		List<string> paths = new(args.Count);
		List<Argument> result = new(args.Count);

		foreach (var arg in args) {
			switch (arg.Key) {
				case var key when arg.IsCommand:
					var rawKey = arg.Command;

					if (rawKey is null) {
						throw new ArgumentException(nameof(arg.Command));
					}

					switch (rawKey.ToLowerInvariant()) {
						case "ui":
						case "preview":
							preview = true;
							break;
						case "convert":
							preview = false;
							break;
						default:
							result.Add(arg);
							break;
					}
					break;
				default:
					paths.Add(arg.Key);
					break;
			}
		}

		options = new Options {
			Preview = preview,
			Paths = paths
		};

		return result;
	}
}
