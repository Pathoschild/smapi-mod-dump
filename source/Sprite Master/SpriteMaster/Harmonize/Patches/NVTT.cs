using SpriteMaster.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using TeximpNet;
using TeximpNet.Unmanaged;

namespace SpriteMaster.Harmonize.Patches {
	[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
	internal static class NVTT {

		//[DllImport("__Internal")]
		//private static extern IntPtr dlerror (String fileName, int flags);

		static NVTT () {
			if (Runtime.IsLinux) {
				// This needs to be done because Debian-based systems don't always have a libdl.so, and instead have libdl.so.2.
				// We need to determine which libdl we actually need to talk to.
				var dlTypes = Arrays.Of(
					typeof(LibDL),
					typeof(LibDL2),
					typeof(LibDL227),
					typeof(LibDL1)
				);

				foreach (var dlType in dlTypes) {
					var newDL = (LibDL)Activator.CreateInstance(dlType);
					try {
						newDL.error();
					}
					catch {
						Debug.TraceLn($"Failed DL: {dlType}");
						continue;
					}
					dl = newDL;
					Debug.TraceLn($"New DL: {dlType}");
					break;
				}

				if (dl == null) {
					Debug.ErrorLn("A valid libdl could not be found.");
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

		private static readonly LibDL dl = null;

		// NVTT's CUDA compressor for block compression is _not_ threadsafe. I have a version locally from a while back that I made threadsafe,
		// but I never validated it and am not comfortable jamming it in here.
		[Harmonize(
			typeof(NvTextureToolsLibrary),
			"TeximpNet.Unmanaged.NvTextureToolsLibrary",
			"EnableCudaAcceleration",
			HarmonizeAttribute.Fixation.Prefix,
			Harmonize.PriorityLevel.Last
		)]
		internal static bool EnableCudaAcceleration (IntPtr compressor, ref bool value) {
			value = false;
			return true;
		}

		[Harmonize(
			typeof(NvTextureToolsLibrary),
			"TeximpNet.Unmanaged.NvTextureToolsLibrary",
			"IsCudaAccelerationEnabled",
			HarmonizeAttribute.Fixation.Postfix,
			Harmonize.PriorityLevel.Last
		)]
		internal static void IsCudaAccelerationEnabled (IntPtr compressor, ref bool __result) {
			__result = false;
		}

		[Harmonize(
			typeof(NvTextureToolsLibrary),
			"TeximpNet.Unmanaged.PlatformHelper",
			"GetAppBaseDirectory",
			HarmonizeAttribute.Fixation.Prefix,
			Harmonize.PriorityLevel.First
		)]
		internal static bool GetAppBaseDirectory (ref string __result) {
			__result = SpriteMaster.AssemblyPath;
			Debug.TraceLn($"GetAppBaseDirectory: {__result}");
			return false;
		}

		private const int RTLD_NOW = 2;

		[Harmonize(
			typeof(NvTextureToolsLibrary),
			new [] { "TeximpNet.Unmanaged.UnmanagedLibrary", "UnmanagedLinuxLibraryImplementation" },
			"NativeLoadLibrary",
			HarmonizeAttribute.Fixation.Prefix,
			Harmonize.PriorityLevel.First,
			platform: HarmonizeAttribute.Platform.Linux
		)]
		internal static bool NativeLoadLibrary (UnmanagedLibrary __instance, ref IntPtr __result, String path) {
			var libraryHandle = dl.open(path, RTLD_NOW);

			if (libraryHandle == IntPtr.Zero && __instance.ThrowOnLoadFailure) {
				var errPtr = dl.error();
				var msg = Marshal.PtrToStringAnsi(errPtr);
				if (!String.IsNullOrEmpty(msg))
					throw new TeximpException(String.Format("Error loading unmanaged library from path: {0}\n\n{1}", path, msg));
				else
					throw new TeximpException(String.Format("Error loading unmanaged library from path: {0}", path));
			}

			__result = libraryHandle;

			return false;
		}

		[Harmonize(
			typeof(NvTextureToolsLibrary),
			new [] { "TeximpNet.Unmanaged.UnmanagedLibrary", "UnmanagedLinuxLibraryImplementation" },
			"NativeGetProcAddress",
			HarmonizeAttribute.Fixation.Prefix,
			Harmonize.PriorityLevel.First,
			platform: HarmonizeAttribute.Platform.Linux
		)]
		internal static bool NativeGetProcAddress (ref IntPtr __result, IntPtr handle, String functionName) {
			__result = dl.sym(handle, functionName);

			return false;
		}

		[Harmonize(
			typeof(NvTextureToolsLibrary),
			new [] { "TeximpNet.Unmanaged.UnmanagedLibrary", "UnmanagedLinuxLibraryImplementation" },
			"NativeFreeLibrary",
			HarmonizeAttribute.Fixation.Prefix,
			Harmonize.PriorityLevel.First,
			platform: HarmonizeAttribute.Platform.Linux
		)]
		internal static bool NativeFreeLibrary (IntPtr handle) {
			dl.close(handle);
			return false;
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Native Code")]
		private class LibDL {
			private const string lib = "libdl.so";

			internal virtual IntPtr open (string fileName, int flags) {
				return dlopen(fileName, flags);
			}

			internal virtual IntPtr sym (IntPtr handle, string functionName) {
				return dlsym(handle, functionName);
			}

			internal virtual int close (IntPtr handle) {
				return dlclose(handle);
			}

			internal virtual IntPtr error () {
				return dlerror();
			}

			[DllImport(lib)]
			private static extern IntPtr dlopen (String fileName, int flags);

			[DllImport(lib)]
			private static extern IntPtr dlsym (IntPtr handle, String functionName);

			[DllImport(lib)]
			private static extern int dlclose (IntPtr handle);

			[DllImport(lib)]
			private static extern IntPtr dlerror ();
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Native Code")]
		private sealed class LibDL2 : LibDL {
			private const string lib = "libdl.so.2";

			internal override IntPtr open (string fileName, int flags) {
				return dlopen(fileName, flags);
			}

			internal override IntPtr sym (IntPtr handle, string functionName) {
				return dlsym(handle, functionName);
			}

			internal override int close (IntPtr handle) {
				return dlclose(handle);
			}

			internal override IntPtr error () {
				return dlerror();
			}

			[DllImport(lib)]
			private static extern IntPtr dlopen (String fileName, int flags);

			[DllImport(lib)]
			private static extern IntPtr dlsym (IntPtr handle, String functionName);

			[DllImport(lib)]
			private static extern int dlclose (IntPtr handle);

			[DllImport(lib)]
			private static extern IntPtr dlerror ();
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Native Code")]
		private sealed class LibDL227 : LibDL {
			private const string lib = "libdl-2.27.so";

			internal override IntPtr open (string fileName, int flags) {
				return dlopen(fileName, flags);
			}

			internal override IntPtr sym (IntPtr handle, string functionName) {
				return dlsym(handle, functionName);
			}

			internal override int close (IntPtr handle) {
				return dlclose(handle);
			}

			internal override IntPtr error () {
				return dlerror();
			}

			[DllImport(lib)]
			private static extern IntPtr dlopen (String fileName, int flags);

			[DllImport(lib)]
			private static extern IntPtr dlsym (IntPtr handle, String functionName);

			[DllImport(lib)]
			private static extern int dlclose (IntPtr handle);

			[DllImport(lib)]
			private static extern IntPtr dlerror ();
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Native Code")]
		private sealed class LibDL1 : LibDL {
			private const string lib = "libdl.so.1";

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

			[DllImport(lib)]
			private static extern IntPtr dlopen (String fileName, int flags);

			[DllImport(lib)]
			private static extern IntPtr dlsym (IntPtr handle, String functionName);

			[DllImport(lib)]
			private static extern int dlclose (IntPtr handle);

			[DllImport(lib)]
			private static extern IntPtr dlerror ();
		}
	}
}
