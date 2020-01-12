using SpriteMaster.Types;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace SpriteMaster {
	public static class Runtime {
		[Pure]
		private static string ArgVToString(string[] args) {
			if (args.Length == 0) {
				return string.Empty;
			}

			var result = string.Empty;
			// https://stackoverflow.com/questions/5510343/escape-command-line-arguments-in-c-sharp/6040946#6040946
			foreach (var arg in args) {
				var s = Regex.Replace(arg, @"(\\*)" + "\"", @"$1$1\" + "\"");
				s = "\"" + Regex.Replace(s, @"(\\+)$", @"$1$1") + "\"";
				result += s + " ";
			}
			return result.Remove(result.Length - 1);
		}

		public static Process Open2(string command, string arg) {
			return Open2(command, Arrays.Singleton(arg));
		}

		public static Process Open2(string command, string[] args = null) {
			if (command == null || command == "")
				throw new ArgumentOutOfRangeException(nameof(command));
			args ??= Arrays<string>.Empty;

			var process = new Process {
				StartInfo = {
					UseShellExecute = false,
					RedirectStandardOutput = true,
					FileName = command,
					Arguments = ArgVToString(args)
				}
			};
			process.Start();
			return process;
		}

		public static string Capture1 (string command, string arg) {
			return Capture1(command, Arrays.Singleton(arg));
		}

		public static string Capture1(string command, string[] args = null) {
			using (var process = Open2(command, args)) {
				return process.StandardOutput.ReadToEnd();
			}
		}

		[ImmutableObject(true)]
		public readonly ref struct Result2 {
			[ImmutableObject(true)]
			public readonly string StandardOutput;
			[ImmutableObject(true)]
			public readonly string StandardError;
			internal Result2(string StdOut, string StdErr) {
				StandardOutput = StdOut ?? string.Empty;
				StandardError = StdErr ?? string.Empty;
			}
		}

		public static Result2 Capture1E (string command, string arg) {
			return Capture1E(command, Arrays.Singleton(arg));
		}

		public static Result2 Capture1E(string command, string[] args = null) {
			using (var process = Open2(command, args)) {
				return new Result2(
					process.StandardOutput.ReadToEnd(),
					process.StandardError.ReadToEnd()
				);
			}
		}

		[Pure]
		private static PlatformType GetUnixType() {
			var system = Capture1("uname", "-s").Trim().ToLowerInvariant();

			return system switch
			{
				var _ when system.Contains("darwin") => PlatformType.Macintosh,
				var _ when system.Contains("linux") => PlatformType.Linux,
				var _ when system.Contains("msys") || system.Contains("mingw") => PlatformType.Windows,// This is actually Windows.
				_ => PlatformType.BSD,// We assume that if it isn't darwin or linux, it's probably BSD.
			};
		}

		static Runtime() {
			// Figure out the executing platform
			Platform = Environment.OSVersion.Platform switch
			{
				PlatformID.Win32NT => PlatformType.Windows,
				PlatformID.Unix => GetUnixType(),
				_ => throw new ApplicationException($"Unknown Platform: {Environment.OSVersion.Platform}"),
			};
			Framework = Platform switch
			{
				PlatformType.Windows => FrameworkType.DotNET,
				_ => FrameworkType.Mono,
			};
		}

		public enum PlatformType {
			// Windows NT
			Windows,
			// Any Linux Distro. Should probably determine if it's Debian because Debian is dumb
			Linux,
			// The BSDs, probably FreeBSD
			BSD,
			// Mac OS X, Darwin Kernel
			Macintosh
		}

		public enum FrameworkType {
			// Windows uses .NET
			DotNET,
			// Everything else uses Mono
			Mono
		}

		[ImmutableObject(true)]
		public static readonly string FullSystem = Capture1("uname").Trim();

		public static readonly FrameworkType Framework;
		public static readonly PlatformType Platform;
		public static readonly int Bits = IntPtr.Size * 8;

		public static bool IsWindows => Platform == PlatformType.Windows;
		public static bool IsUnix => Platform != PlatformType.Windows;
		public static bool IsLinux => Platform == PlatformType.Linux;
		public static bool IsBSD => Platform == PlatformType.BSD;
		public static bool IsMacintosh => Platform == PlatformType.Macintosh;
	}
}
