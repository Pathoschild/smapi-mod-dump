/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

// #define SUPPORT_DIFFERENT_FRAMEWORKS

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SpriteMaster;

internal static class Runtime {
	internal static class MethodImpl {
		internal const MethodImplOptions Inline = MethodImplOptions.AggressiveInlining;
		internal const MethodImplOptions Cold = MethodImplOptions.NoInlining;
		internal const MethodImplOptions ErrorPath = MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization;
		internal const MethodImplOptions RunOnce = MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization;
		internal const MethodImplOptions IgnoreOptimization = MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization;
	}

#if SUPPORT_DIFFERENT_FRAMEWORKS
	private static readonly (GameFrameworkType Framework, string Prefix)[] GameFrameworkPairs = {
		( GameFrameworkType.MonoGame, "MonoGame.Framework" ), // this is first as Mono aliases the XNA frameworks.
		( GameFrameworkType.XNA, "Microsoft.XNA.Framework" )
	};
#endif

	[MethodImpl(MethodImpl.RunOnce)]
	static Runtime() {
		// Figure out the executing platform
		if (OperatingSystem.IsWindows()) {
			Platform = PlatformType.Windows;
		}
		else if (OperatingSystem.IsLinux()) {
			Platform = PlatformType.Linux;
		}
		else if (OperatingSystem.IsFreeBSD()) {
			Platform = PlatformType.BSD;
		}
		else if (OperatingSystem.IsMacOS()) {
			Platform = PlatformType.Macintosh;
		}
		else {
			throw new ApplicationException($"Unknown Platform: {Environment.OSVersion.Platform}");
		}

#if SUPPORT_DIFFERENT_FRAMEWORKS
		// Check for Mono
		if (ReflectionExt.GetTypeExt("Mono.Runtime") is not null) {
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
			var exists = AppDomain.CurrentDomain.GetAssemblies().AnyF(assembly => assembly.GetName().Name?.StartsWith(frameworkPair.Prefix) ?? false);
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
				var gameAssembly = AssemblyExt.GetAssembly("MonoGame.Framework");
				var shaderType = gameAssembly.GetType("Microsoft.Xna.Framework.Graphics.Shader");
				if (shaderType is null) {
					throw new NullReferenceException(nameof(shaderType));
				}
				var profileProperty = shaderType.GetProperty("Profile");
				if (profileProperty is null) {
					throw new NullReferenceException(nameof(profileProperty));
				}
				var profile = (int?)profileProperty.GetValue(null);
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
#endif
	}

	internal enum PlatformType {
		// Windows NT
		Windows,
		// Any Linux Distro. Should probably determine if it's Debian because Debian is dumb
		Linux,
		// The BSDs, probably FreeBSD
		// ReSharper disable once InconsistentNaming
		BSD,
		// Mac OS X, Darwin Kernel
		Macintosh
	}

	internal enum FrameworkType {
		// Windows uses .NET
		// ReSharper disable once InconsistentNaming
		DotNETFramework,
		// Newer SDV uses .NET 5
		// ReSharper disable once InconsistentNaming
		DotNET,
		// Everything else uses Mono
		Mono
	}

	internal enum GameFrameworkType {
		// ReSharper disable once InconsistentNaming
		XNA,
		MonoGame
	}

	internal enum RendererType {
		// ReSharper disable once InconsistentNaming
		OpenGL,
		D3D9,
		D3D11
	}

#if SUPPORT_DIFFERENT_FRAMEWORKS
	internal static readonly FrameworkType Framework;
	internal static readonly GameFrameworkType GameFramework;
	internal static readonly RendererType Renderer;
#else
	internal const FrameworkType Framework = FrameworkType.DotNET;
	internal const GameFrameworkType GameFramework = GameFrameworkType.MonoGame;
	internal const RendererType Renderer = RendererType.OpenGL;
#endif
	internal static readonly PlatformType Platform;
#if SUPPORT_DIFFERENT_FRAMEWORKS
	internal static readonly int Bits = IntPtr.Size * 8;
#else
	internal const int Bits = 64;
#endif

	internal static bool IsWindows => Platform == PlatformType.Windows;
	internal static bool IsUnix => Platform != PlatformType.Windows;
	internal static bool IsLinux => Platform == PlatformType.Linux;
	internal static bool IsBSD => Platform == PlatformType.BSD;
	internal static bool IsMacintosh => Platform == PlatformType.Macintosh;

#if SUPPORT_DIFFERENT_FRAMEWORKS
	internal static bool IsMonoGame => GameFramework == GameFrameworkType.MonoGame;
	internal static bool IsXNA => GameFramework == GameFrameworkType.XNA;
#else
	internal const bool IsMonoGame = true;
	internal const bool IsXNA = false;
#endif

	internal static class Capabilities {
#if SUPPORT_DIFFERENT_FRAMEWORKS
		internal static bool AsyncStores => Renderer != RendererType.OpenGL;
		internal static bool AsynchronousRenderingAPI => Renderer == RendererType.D3D11;
#else
		internal const bool AsyncStores = false;
		internal const bool AsynchronousRenderingAPI = false;
#endif
	}

	internal static bool IsHDR => GetIsHDR();

	private static bool GetIsHDR() {
		return false;
	}

	private static readonly bool EnableProcessAffinity = new Func<bool>(
		() => {
			try {
				var currentProcess = Process.GetCurrentProcess();
				var affinity = currentProcess.ProcessorAffinity;
				currentProcess.ProcessorAffinity = affinity;
				return true;
			}
			catch (PlatformNotSupportedException) {
				return false;
			}
			catch (Exception) {
				return false;
			}
		})();

	private static readonly int ProcessorCount = Environment.ProcessorCount;
	private static readonly nint ExpectedMask = ProcessorCount == 64 ? nint.MaxValue : ((nint)1 << ProcessorCount) - 1;

	private static readonly Process CurrentProcess = Process.GetCurrentProcess();

	internal static void CorrectProcessorAffinity() {
		if (!EnableProcessAffinity) {
			return;
		}
		
		if (ProcessorCount > 64) {
			return;
		}

		try {
			nint expectedMask = ExpectedMask;

			var currentAffinity = (nint)CurrentProcess.ProcessorAffinity;
			if ((currentAffinity & expectedMask) != expectedMask) {
				CurrentProcess.ProcessorAffinity = expectedMask;
			}
		}
		catch (Exception) {
			// swallow exception
		}
	}
}
