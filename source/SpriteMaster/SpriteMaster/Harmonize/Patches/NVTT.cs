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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using TeximpNet;
using TeximpNet.Unmanaged;

namespace SpriteMaster.Harmonize.Patches;

[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
[SuppressUnmanagedCodeSecurity]
internal static class NVTT {

	//[DllImport("__Internal")]
	//private static extern IntPtr dlerror (String fileName, int flags);

	static NVTT() {
		if (Runtime.IsLinux) {
			// This needs to be done because Debian-based systems don't always have a libdl.so, and instead have libdl.so.2.
			// We need to determine which libdl we actually need to talk to.
			var dlTypes = Arrays.Of(
				typeof(LibDl),
				typeof(LibDl2),
				typeof(LibDl3),
				typeof(LibDl1)
			);

			foreach (var dlType in dlTypes) {
				var newDl = (LibDl?)Activator.CreateInstance(dlType);
				try {
					if (newDl is null) {
						throw new NullReferenceException(nameof(newDl));
					}
					newDl.error();
				}
				catch {
					Debug.Trace($"Failed DL: {dlType}");
					continue;
				}
				Dl = newDl;
				Debug.Trace($"New DL: {dlType}");
				break;
			}

			if (Dl is null) {
				Debug.Error("A valid libdl could not be found.");
				throw new NotSupportedException("A valid libdl could not be found.");
			}
		}
	}

	/*
[DllImport("__Internal", CharSet = CharSet.Ansi)]
private static extern void mono_dllmap_insert(IntPtr assembly, string dll, string func, string tdll, string tfunc);

// and then somewhere:
mono_dllmap_insert(IntPtr.Zero, "somelib", null, "/path/to/libsomelib.so", null);
	*/

	private static readonly LibDl? Dl = null;

	// NVTT's CUDA compressor for block compression is _not_ threadsafe. I have a version locally from a while back that I made threadsafe,
	// but I never validated it and am not comfortable jamming it in here.
	[Harmonize(
		typeof(NvTextureToolsLibrary),
		"TeximpNet.Unmanaged.NvTextureToolsLibrary",
		"EnableCudaAcceleration",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last
	)]
	public static void EnableCudaAcceleration(IntPtr compressor, ref bool value) {
		value = true;
	}

	[Harmonize(
		typeof(NvTextureToolsLibrary),
		"TeximpNet.Unmanaged.NvTextureToolsLibrary",
		"IsCudaAccelerationEnabled",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.Last
	)]
	public static void IsCudaAccelerationEnabled(IntPtr compressor, ref bool __result) {
		__result = false;
	}

	[Harmonize(
		typeof(NvTextureToolsLibrary),
		"TeximpNet.Unmanaged.PlatformHelper",
		"GetAppBaseDirectory",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.First,
		instance: false
	)]
	public static bool GetAppBaseDirectory(ref string? __result) {
		__result = SpriteMaster.AssemblyPath;
		Debug.Trace($"GetAppBaseDirectory: {__result}");
		return false;
	}

	private const int RTLD_NOW = 2;

	[Harmonize(
		typeof(NvTextureToolsLibrary),
		new[] { "TeximpNet.Unmanaged.UnmanagedLibrary", "UnmanagedLinuxLibraryImplementation" },
		"NativeLoadLibrary",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.First,
		platform: Harmonize.Platform.Linux
	)]
	public static bool NativeLoadLibrary(UnmanagedLibrary __instance, ref IntPtr __result, String path) {
		if (Dl is null) {
			throw new NullReferenceException(nameof(Dl));
		}

		var libraryHandle = Dl.open(path, RTLD_NOW);

		if (libraryHandle == IntPtr.Zero && __instance.ThrowOnLoadFailure) {
			var errPtr = Dl.error();
			var msg = Marshal.PtrToStringAnsi(errPtr);
			if (!String.IsNullOrEmpty(msg))
				throw new TeximpException($"Error loading unmanaged library from path: {path}\n\n{msg}");
			else
				throw new TeximpException($"Error loading unmanaged library from path: {path}");
		}

		__result = libraryHandle;

		return false;
	}

	[Harmonize(
		typeof(NvTextureToolsLibrary),
		new[] { "TeximpNet.Unmanaged.UnmanagedLibrary", "UnmanagedLinuxLibraryImplementation" },
		"NativeGetProcAddress",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.First,
		platform: Harmonize.Platform.Linux
	)]
	public static bool NativeGetProcAddress(ref IntPtr __result, IntPtr handle, String functionName) {
		if (Dl is null) {
			throw new NullReferenceException(nameof(Dl));
		}

		__result = Dl.sym(handle, functionName);

		return false;
	}

	[Harmonize(
		typeof(NvTextureToolsLibrary),
		new[] { "TeximpNet.Unmanaged.UnmanagedLibrary", "UnmanagedLinuxLibraryImplementation" },
		"NativeFreeLibrary",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.First,
		platform: Harmonize.Platform.Linux
	)]
	public static bool NativeFreeLibrary(IntPtr handle) {
		Dl?.close(handle);
		return false;
	}

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Native Code")]
	private class LibDl {
		private const string Lib = "libdl.so";

		internal virtual IntPtr open(string fileName, int flags) {
			return dlopen(fileName, flags);
		}

		internal virtual IntPtr sym(IntPtr handle, string functionName) {
			return dlsym(handle, functionName);
		}

		internal virtual int close(IntPtr handle) {
			return dlclose(handle);
		}

		internal virtual IntPtr error() {
			return dlerror();
		}

		[DllImport(Lib)]
		private static extern IntPtr dlopen(String fileName, int flags);

		[DllImport(Lib)]
		private static extern IntPtr dlsym(IntPtr handle, String functionName);

		[DllImport(Lib)]
		private static extern int dlclose(IntPtr handle);

		[DllImport(Lib)]
		private static extern IntPtr dlerror();
	}

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Native Code")]
	private sealed class LibDl2 : LibDl {
		private const string Lib = "libdl.so.2";

		internal override IntPtr open(string fileName, int flags) {
			return dlopen(fileName, flags);
		}

		internal override IntPtr sym(IntPtr handle, string functionName) {
			return dlsym(handle, functionName);
		}

		internal override int close(IntPtr handle) {
			return dlclose(handle);
		}

		internal override IntPtr error() {
			return dlerror();
		}

		[DllImport(Lib)]
		private static extern IntPtr dlopen(String fileName, int flags);

		[DllImport(Lib)]
		private static extern IntPtr dlsym(IntPtr handle, String functionName);

		[DllImport(Lib)]
		private static extern int dlclose(IntPtr handle);

		[DllImport(Lib)]
		private static extern IntPtr dlerror();
	}

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Native Code")]
	private sealed class LibDl3 : LibDl {
		private const string Lib = "libdl.so.3";

		internal override IntPtr open(string fileName, int flags) {
			return dlopen(fileName, flags);
		}

		internal override IntPtr sym(IntPtr handle, string functionName) {
			return dlsym(handle, functionName);
		}

		internal override int close(IntPtr handle) {
			return dlclose(handle);
		}

		internal override IntPtr error() {
			return dlerror();
		}

		[DllImport(Lib)]
		private static extern IntPtr dlopen(String fileName, int flags);

		[DllImport(Lib)]
		private static extern IntPtr dlsym(IntPtr handle, String functionName);

		[DllImport(Lib)]
		private static extern int dlclose(IntPtr handle);

		[DllImport(Lib)]
		private static extern IntPtr dlerror();
	}

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Native Code")]
	private sealed class LibDl1 : LibDl {
		private const string Lib = "libdl.so.1";

		internal override IntPtr open(string fileName, int flags) {
			return dlopen(fileName, flags);
		}

		internal override IntPtr sym(IntPtr handle, string functionName) {
			return dlsym(handle, functionName);
		}

		internal override int close(IntPtr handle) {
			return dlclose(handle);
		}

		internal override IntPtr error() {
			return dlerror();
		}

		[DllImport(Lib)]
		private static extern IntPtr dlopen(String fileName, int flags);

		[DllImport(Lib)]
		private static extern IntPtr dlsym(IntPtr handle, String functionName);

		[DllImport(Lib)]
		private static extern int dlclose(IntPtr handle);

		[DllImport(Lib)]
		private static extern IntPtr dlerror();
	}
}
