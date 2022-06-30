/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Environments;
using JetBrains.Annotations;
using SpriteMaster.Extensions;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Benchmarks.BenchmarkBase;

[PublicAPI]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class OptionAttribute : Attribute {
	internal readonly string Description;
	internal readonly string LongOpt;
	internal readonly char? ShortOpt = null;

	public OptionAttribute(string longOpt, string description) {
		Description = description;
		LongOpt = longOpt;
	}

	public OptionAttribute(string longOpt, char shortOpt, string description) {
		Description = description;
		LongOpt = longOpt;
		ShortOpt = shortOpt;
	}
}

[PublicAPI]
public enum GCType {
	Workstation = 0,
	Server
}

[PublicAPI]
public abstract class Options {
	[Option("min", "Minimum Range Value")]
	public long Min { get; set; } = 0;

	[Option("max", "Maximum Range Value")]
	public long Max { get; set; } = 0x8000000;

	public HashSet<string> Set { get; } = new(StringComparer.InvariantCultureIgnoreCase);

	public HashSet<string> Runners { get; } = new(StringComparer.InvariantCultureIgnoreCase);

	[Option("clear", "Clear Screen")]
	public bool Clear { get; set; } = false;

	[Option("in-process", "Run In-Process")]
	public bool InProcess { get; set; } = false;

	[Option("diag-cpu", "CPU Usage Diagnostics")]
	public bool DiagnoseCpu { get; set; } = false;

	[Option("diag-memory", "Memory/Allocation Diagnostics")]
	public bool DiagnoseMemory { get; set; } = false;

	[Option("diag-inlining", "Inlining Diagnostics")]
	public bool DiagnoseInlining { get; init; } = false;

	[Option("diag-tailcall", "Tail Call Diagnostics")]
	public bool DiagnoseTailCall { get; init; } = false;

	[Option("diag-etw", "ETW Diagnostics")]
	public bool DiagnoseEtw { get; init; } = false;

	[Option("cold", "Test Cold Start")]
	public bool Cold { get; init; } = false;

	[Option("validate", "Perform Validation")]
	public bool DoValidate { get; init; } = false;

	[Option("reverse", "Reverse Runner Order")]
	public bool Reverse { get; init; } = false;

	public HashSet<GCType> GCTypes { get; } = new();

	public HashSet<Runtime> Runtimes { get; } = new();

	public virtual void Validate(HashSet<string> validSets) {
		if (Set.Count == 0) {
			foreach (var item in validSets) {
				Set.Add(item);
			}
		}
	}

	public static Regex CreatePattern(string pattern) {
		if (pattern.StartsWith('^') || pattern.EndsWith('$')) {
			return new(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}
		else {
			pattern = Regex.Escape(pattern);
			pattern = pattern.Replace("\\*", ".*");
			return new(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}
	}

	protected Options() { }

	protected virtual void Process(string[] args) {

	}

	public static TOptions From<TOptions>(string[] args) where TOptions : Options, new() {
		Type optionsType = typeof(TOptions);

		HashSet<string> ValidSets = new(
			optionsType.Assembly.GetTypes()
				.Where(type => !type.IsAbstract && type.IsAssignableTo(typeof(Benchmarks.BenchmarkBase)))
				.Select(type => type.Name),
			StringComparer.InvariantCultureIgnoreCase
		);

		HashSet<string> ValidRunners = new(StringComparer.InvariantCultureIgnoreCase);
		foreach (var type in optionsType.Assembly.GetTypes().Where(type => type.IsAssignableTo(typeof(Benchmarks.BenchmarkBase)))) {
			foreach (var method in type.GetMethods()) {
				if (!method.HasAttribute<BenchmarkAttribute>()) {
					continue;
				}

				ValidRunners.Add(method.Name);
			}
		}

		MemberInfo[] OptionFields = optionsType.
			GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).
			Where(member => member.HasAttribute<OptionAttribute>()).ToArray();

		Dictionary<string, MemberInfo> OptionFieldsLongMap = new(
			OptionFields.Select(
				field => {
					var attr = field.GetCustomAttribute<OptionAttribute>()!;
					return new KeyValuePair<string, MemberInfo>(attr.LongOpt, field);
				}
			),
			StringComparer.InvariantCultureIgnoreCase
		);

		Dictionary<char, MemberInfo> OptionFieldsShortMap = new(
			OptionFields
				.Where(
					field => {
						var attr = field.GetCustomAttribute<OptionAttribute>()!;
						return attr.ShortOpt.HasValue;
					}
				)
				.Select(
					field => {
						var attr = field.GetCustomAttribute<OptionAttribute>()!;
						return new KeyValuePair<char, MemberInfo>(attr.ShortOpt.Value, field);
					}
				)
		);

		Runtime CoreRuntime7 = CoreRuntime.CreateForNewVersion("net7.0", ".NET 7.0");

		var result = new TOptions();

		List<string> fatal = new();

		void FatalError(string message) {
			Console.Error.WriteLine(message);
			fatal.Add(message);
		}

		bool onlyList = false;

		static bool RemoveFromStart(ref string str, string comparand) {
			if (str.StartsWith(comparand)) {
				str = str[comparand.Length..];
				return true;
			}

			return false;
		}

		static bool RemoveFromEnd(ref string str, string comparand) {
			if (str.EndsWith(comparand)) {
				str = str[..^comparand.Length];
				return true;
			}

			return false;
		}

		void ParseFlag(string flag, bool isShortFlag) {
			bool? state = null;

			if (RemoveFromStart(ref flag, "no-")) {
				state = false;
			}
			if (RemoveFromEnd(ref flag, "-")) {
				state = false;
			}

			string? arg = null;

			uint equals = (uint)flag.IndexOf('=');
			uint colon = (uint)flag.IndexOf(':');
			if ((int)equals != -1 || (int)colon != -1) {
				uint offset = Math.Min(equals, colon);

				arg = flag[((int)offset + 1)..];
				flag = flag[..(int)offset];
			}

			switch (flag) {
				case "list-runners": {
					Console.WriteLine("Valid Runners:");
					foreach (var runner in ValidRunners) {
						Console.WriteLine($"  {runner}");
					}

					onlyList = true;
					break;
				}
				case "list-sets": {
					Console.WriteLine("Valid Sets:");
					foreach (var set in ValidSets) {
						Console.WriteLine($"  {set}");
					}

					onlyList = true;
					break;
				}
				case "run":
				case "r" when isShortFlag: {
					if (arg is null) {
						FatalError($"'run' requires an argument");
						break;
					}

					var localRunners = arg.Split(',', StringSplitOptions.RemoveEmptyEntries);
					result.Runners.AddRange(localRunners);

					foreach (var runner in localRunners) {
						var runnerPattern = CreatePattern(runner);
						if (!ValidRunners.Any(runnerPattern.IsMatch)) {
							FatalError($"Unknown runner: '{runner}'");
							break;
						}
					}

					break;
				}
				default:
					MemberInfo? member;

					if (isShortFlag) {
						if (OptionFieldsShortMap.TryGetValue(flag[0], out member)) {

						}
					}
					else {
						if (OptionFieldsLongMap.TryGetValue(flag, out member)) {

						}
					}

					switch (flag.ToLowerInvariant()) {
						case "gc":
							if (arg is null) {
								FatalError("No GC specified");
								break;
							}
							switch (arg.ToLowerInvariant()) {
								case "default":
								case "workstation":
									result.GCTypes.Add(GCType.Workstation);
									break;
								case "server":
									result.GCTypes.Add(GCType.Server);
									break;
								default:
									FatalError($"Unknown GC: {arg}");
									break;
							}
							return;
						case "runtime":
							if (arg is null) {
								FatalError("No runtime specified");
								break;
							}

							var args = arg.Split(',', StringSplitOptions.RemoveEmptyEntries);
							foreach (var rt in args) {
								switch (rt.ToLowerInvariant()) {
									case "3.1":
										result.Runtimes.Add(CoreRuntime.Core31);
										break;
									case "5":
									case "5.1":
										result.Runtimes.Add(CoreRuntime.Core50);
										break;
									case "6":
									case "6.0":
										result.Runtimes.Add(CoreRuntime.Core60);
										break;
									case "7":
									case "7.0":
										result.Runtimes.Add(CoreRuntime7);
										break;
									default:
										FatalError($"Unknown Runtime: {rt}");
										break;
								}
							}

							return;
					}

					static long ParseLong(string arg) {
						int radix = 10;
						
						if (
							arg.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase) ||
							arg.StartsWith("0h", StringComparison.InvariantCultureIgnoreCase)
						) {
							radix = 16;
							arg = arg.Substring(2);
						}
						else if (
							arg.StartsWith("x", StringComparison.InvariantCultureIgnoreCase) ||
							arg.StartsWith("h", StringComparison.InvariantCultureIgnoreCase)
						) {
							radix = 16;
							arg = arg.Substring(1);
						}
						else if (
							arg.EndsWith("h", StringComparison.InvariantCultureIgnoreCase)
						) {
							radix = 16;
							arg = arg.Substring(0, arg.Length - 1);
						}

						arg = arg.Trim();

						switch (radix) {
							case 10:
								return long.Parse(arg);
							case 16:
								return long.Parse(arg, NumberStyles.HexNumber);
							default:
								return long.Parse(arg);
						}
					}

					if (member is not null) {
						switch (member) {
							case FieldInfo field: {
								object? par = null;
								var type = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
								if (type == typeof(bool)) {
									par = arg is null ? state : bool.Parse(arg);
									par ??= true;
								}
								else if (type == typeof(long)) {
									par = arg is null ? null : ParseLong(arg);
								}
								if (par is not null) {
									field.SetValue(result, par);
								}

								break;
							}
							case PropertyInfo property: {
								object? par = null;
								var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
								if (type == typeof(bool)) {
									par = arg is null ? state : bool.Parse(arg);
									par ??= true;
									}
								else if (type == typeof(long)) { 
									par = arg is null ? null : ParseLong(arg);
								}
								if (par is not null) {
									property.SetValue(result, par);
								}

								break;
							}
							case MethodInfo method:
								method.Invoke(result, new object?[] { (object?)state ?? arg });
								break;
							default:
								throw new NotImplementedException();
						} ;
					}
					else {
						FatalError($"Unknown flag: '{flag}'");
						break;
					}

					break;
			}
		}

		bool isInArgs = false;
		foreach (var arg in args) {
			if (arg.Length == 0) {
				continue;
			}

			switch (arg) {
				case "--" when !isInArgs: {
					isInArgs = true;
					break;
				}
				case var flag when !isInArgs && (arg[0] == '/' || arg[0] == '-'): {
					bool shortFlag = false;

					if (arg.StartsWith("--")) {
						flag = flag[2..];
					}
					else {
						shortFlag = arg[0] == '-';
						flag = flag[1..];
					}

					ParseFlag(flag, isShortFlag: shortFlag);
					break;
				}
				default: {
					if (!ValidSets.Contains(arg)) {
						//FatalError($"Unknown set specified: '{arg}'");
						//break;
					}

					if (!result.Set.Add(arg)) {
						Console.Error.WriteLine($"Duplicate test set specified: '{arg}'");
					}
					break;
				}
			}
		}

		if (fatal.Count != 0) {
			Console.Error.WriteLine($"There were {fatal.Count} fatal errors in configuration.");
			Environment.Exit(-fatal.Count);
		}

		if (onlyList) {
			Environment.Exit(0);
		}

		result.Validate(ValidSets);

		result.Process(args);

		return result;
	}
}
