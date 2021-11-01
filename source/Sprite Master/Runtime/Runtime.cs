/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SpriteMaster {
	public static class Runtime {
		public static class MethodImpl {
			public const MethodImplOptions Optimize = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
		}

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
			using var process = Open2(command, args);
			return process.StandardOutput.ReadToEnd();
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
			using var process = Open2(command, args);
			return new Result2(
				process.StandardOutput.ReadToEnd(),
				process.StandardError.ReadToEnd()
			);
		}

		[Pure]
		private static PlatformType GetUnixType() {
			try {
				var system = Capture1("uname", "-s").Trim().ToLowerInvariant();

				return system switch {
					var _ when system.Contains("darwin") => PlatformType.Macintosh,
					var _ when system.Contains("linux") => PlatformType.Linux,
					var _ when system.Contains("msys") || system.Contains("mingw") => PlatformType.Windows,// This is actually Windows.
					_ => PlatformType.BSD,// We assume that if it isn't darwin or linux, it's probably BSD.
				};
			}
			catch {
				// if for some reason uname fails... we should probably just assume that it is linux, to be safe.
				return PlatformType.Linux;
			}
		}

		private static readonly (GameFrameworkType Framework, string Prefix)[] GameFrameworkPairs = {
			( GameFrameworkType.MonoGame, "MonoGame.Framework" ), // this is first as Mono aliases the XNA frameworks.
			( GameFrameworkType.XNA, "Microsoft.XNA.Framework" )
		};

		static Runtime() {
			// Figure out the executing platform
			Platform = Environment.OSVersion.Platform switch
			{
				PlatformID.Win32NT => PlatformType.Windows,
				PlatformID.Unix => GetUnixType(),
				PlatformID.MacOSX => GetUnixType(),
				_ => throw new ApplicationException($"Unknown Platform: {Environment.OSVersion.Platform}"),
			};

			// Check for Mono
			if (Type.GetType("Mono.Runtime") != null) {
				Framework = FrameworkType.Mono;
			}
			else {
				// Otherwise, determine which dotNET we're on.
				var runtimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
				switch (true) {
					case var _ when runtimeVersion.StartsWith(".NET Framework"):
						Framework = FrameworkType.DotNETFramework;
						break;
					default:
						Framework = FrameworkType.DotNET;
						break;
				}
			}

			// Determine the game framework.
			// Set a base default based on the platform and bits
			GameFramework = (!IsWindows || Bits == 64) ? GameFrameworkType.MonoGame : GameFrameworkType.XNA;
			foreach (var frameworkPair in GameFrameworkPairs) {
				var exists = AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.GetName().Name.StartsWith(frameworkPair.Prefix));
				if (exists) {
					GameFramework = frameworkPair.Framework;
					break;
				}
			}

			// Determine the renderer
			// https://community.monogame.net/t/solved-how-to-determine-if-the-app-is-using-desktop-gl-or-dx/10494/2
			if (GameFramework == GameFrameworkType.XNA) {
				// XNA is built upon D3D9
				Renderer = RendererType.D3D9;
			}
			else {
				try {
					var gameAssembly = AppDomain.CurrentDomain.GetAssemblies().Single(assembly => assembly.GetName().Name == "MonoGame.Framework");
					var shaderType = gameAssembly.GetType("Microsoft.Xna.Framework.Graphics.Shader");
					var profileProperty = shaderType.GetProperty("Profile");
					var profile = (int)profileProperty.GetValue(null);
					Renderer = profile switch {
						0 => RendererType.OpenGL,
						1 => RendererType.D3D11,
						_ => throw new ApplicationException($"Unknown Shader Profile: {profile}")
					};
				}
				catch {
					// Uh, I guess default to D3D9?
					// It will never be 'right', but it will probably be 'safe'
					Renderer = RendererType.D3D9;
				}
			}

			try {
				FullSystem = Platform switch
				{
					PlatformType.Windows => "Windows",
					_ => Capture1("uname").Trim()
				};
			}
			catch {
				FullSystem = "Unknown";
			}
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
			DotNETFramework,
			// Newer SDV uses .NET 5
			DotNET,
			// Everything else uses Mono
			Mono
		}

		public enum GameFrameworkType {
			XNA,
			MonoGame
		}

		public enum RendererType {
			OpenGL,
			D3D9,
			D3D11
		}

		[ImmutableObject(true)]
		public static readonly string FullSystem;

		public static readonly FrameworkType Framework;
		public static readonly GameFrameworkType GameFramework;
		public static readonly RendererType Renderer;
		public static readonly PlatformType Platform;
		public static readonly int Bits = IntPtr.Size * 8;

		public static bool IsWindows => Platform == PlatformType.Windows;
		public static bool IsUnix => Platform != PlatformType.Windows;
		public static bool IsLinux => Platform == PlatformType.Linux;
		public static bool IsBSD => Platform == PlatformType.BSD;
		public static bool IsMacintosh => Platform == PlatformType.Macintosh;

		public static bool IsMonoGame => GameFramework == GameFrameworkType.MonoGame;
		public static bool IsXNA => GameFramework == GameFrameworkType.XNA;

		public static class Capabilities {
			public static bool AsyncStores => Renderer != RendererType.OpenGL;
			public static bool AsynchronousRenderingAPI => Renderer == RendererType.D3D11;
		}
	}
}
