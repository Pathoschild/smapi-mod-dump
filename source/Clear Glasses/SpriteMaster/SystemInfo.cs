/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using LinqFasterer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.GL;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace SpriteMaster;

internal static partial class SystemInfo {
	internal static class Graphics {
		internal enum Vendors {
			Unknown = 0,
			Intel,
			Nvidia,
			AMD,
			ATI,
			VMware
		}

		[StructLayout(LayoutKind.Auto)]
		private readonly struct Pattern {
			internal readonly Vendors Vendor;
			internal readonly string[] Tokens;
			internal readonly bool Integrated;

			internal Pattern(Vendors vendor, bool integrated, params string[] tokens) {
				Vendor = vendor;
				Integrated = integrated;
				Tokens = tokens;
			}
		}

		private static class Patterns {
			internal static readonly Pattern Intel = new(Vendors.Intel, integrated: true, "Intel", "Parallels using Intel");
			internal static readonly Pattern Nvidia = new(Vendors.Nvidia, integrated: false, "Nvidia", "nouveau");
			internal static readonly Pattern AMD = new(Vendors.AMD, integrated: false, "AMD", "ATI", "Parallels and ATI");
			internal static readonly Pattern VMware = new(Vendors.VMware, integrated: true, "VMware, Inc.");
		}

		internal static Vendors Vendor { get; private set; } = Vendors.Unknown;
		internal static string VendorName { get; private set; } = "Unknown";
		internal static string Description { get; private set; } = "Unknown";
		internal static ulong? DedicatedMemory { get; private set; } = null;
		internal static ulong? TotalMemory { get; private set; } = null;
		internal static bool IsIntegrated { get; private set; } = true;
		internal static bool IsDedicated => !IsIntegrated;

		internal static void Update(GraphicsDeviceManager gdm, GraphicsDevice device) {
			try {
				if (device.IsDisposed) {
					return;
				}

				if (device.Adapter is not {} adapter) {
					return;
				}

				var description = adapter.Description;
				Description = adapter.Description;

				string vendor;
				try {
					vendor = MonoGame.OpenGL.GL.GetString(MonoGame.OpenGL.StringName.Vendor);
				}
				catch {
					vendor = description;
				}

				bool ParsePattern(in Pattern pattern) {
					if (!pattern.Tokens.AnyF(token => vendor.StartsWith(token, StringComparison.InvariantCultureIgnoreCase))) {
						return false;
					}

					Vendor = pattern.Vendor;
					VendorName = Vendor.ToString();
					IsIntegrated = pattern.Integrated;
					return true;
				}

				DedicatedMemory = null;
				TotalMemory = null;

				bool hasNvidiaExtension = GLExt.Extensions.Contains("GL_NVX_gpu_memory_info");
				bool hasAtiExtension = GLExt.Extensions.Contains("GL_ATI_meminfo");

				static unsafe long? GetInteger(int code) {
					try {
						GLExt.FlushErrors();

						long result;

						if (GLExt.GetInteger64v.Enabled) {
							Span<long> localResult = stackalloc long[4];

							fixed (long* resultPtr = localResult) {
								GLExt.GetInteger64v.Function(code, resultPtr);
							}

							result = localResult[0];
						}
						else {
							Span<int> localResult = stackalloc int[4];

							fixed (int* resultPtr = localResult) {
								MonoGame.OpenGL.GL.GetIntegerv(code, resultPtr);
							}

							result = localResult[0];
						}

						return MonoGame.OpenGL.GL.GetError() == MonoGame.OpenGL.ErrorCode.NoError ? result : null;
					}
					catch {
						// Swallow exceptions.
						return null;
					}
				}

				if ((!DedicatedMemory.HasValue || !TotalMemory.HasValue) && hasNvidiaExtension) {
					const int DedicatedVidMemNvx = 0x9047;
					const int TotalAvailableMemoryNvx = 0x9048;

					if (GetInteger(DedicatedVidMemNvx) is {} dedicatedMemory) {
						DedicatedMemory = (ulong)(dedicatedMemory * 1024L);
					}
					if (GetInteger(TotalAvailableMemoryNvx) is {} totalMemory) {
						TotalMemory = (ulong)(totalMemory * 1024L);
					}
				}
				if ((!DedicatedMemory.HasValue || !TotalMemory.HasValue) && hasAtiExtension) {
					// https://www.khronos.org/registry/OpenGL/extensions/ATI/ATI_meminfo.txt
					// ReSharper disable InconsistentNaming
					//const int VboFreeMemoryAti = 0x87FB;
					const int TextureFreeMemoryAti = 0x87FC;
					//const int RenderBufferFreeMemoryAti = 0x87FD;
					// ReSharper restore InconsistentNaming

					if (GetInteger(TextureFreeMemoryAti) is { } totalMemory) {
						TotalMemory = DedicatedMemory = (ulong)(totalMemory * 1024L);
					}
				}

				if (
					!(
						ParsePattern(Patterns.Intel) ||
						ParsePattern(Patterns.Nvidia) ||
						ParsePattern(Patterns.AMD) ||
						ParsePattern(Patterns.VMware)
					)
				) {
					// I have no idea
					Vendor = Vendors.Unknown;
					VendorName = vendor.Split(null, 2).FirstF();
					IsIntegrated = true;
				}

				IsIntegrated = Vendor switch {
					Vendors.AMD when description.Contains("Vega") => true,
					Vendors.Intel when description.Contains("Arc") => false,
					_ => IsIntegrated
				};
			}
			catch {
				// ignored
			}
		}
	}

	internal static void Update(GraphicsDeviceManager gdm, GraphicsDevice device) {
		Graphics.Update(gdm, device);
	}

	private readonly record struct InstructionSetRecord(string Name, bool IsSupported, bool IsEnabled);

	internal static void Dump(GraphicsDeviceManager gdm, GraphicsDevice device) {
		Update(gdm, device);

		var dumpBuilder = new StringBuilder();

		void AppendTabbedLine(int tabs, string str) {
			dumpBuilder.AppendLine($"{new string('\t', tabs)}{str}");
		}

		AppendTabbedLine(0, "System Information:");

		try {
			AppendTabbedLine(1, $"Architecture: {(Environment.Is64BitProcess ? "x64" : "x86")}");
			AppendTabbedLine(1, $"Number of Cores: {Environment.ProcessorCount}");
			var osVersion = Environment.OSVersion.ToString();
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
				osVersion = osVersion.Replace("Unix", "Linux");
			}
			AppendTabbedLine(1, $"OS Version: {osVersion}");
			AppendTabbedLine(1, "CPU Information");
			{
				AppendTabbedLine(2, $"Brand             : {CpuIdentifier.Brand}");
				AppendTabbedLine(2, $"Microarchitecture : {Microarchitecture}");
				AppendTabbedLine(2, $"Family            : {CpuIdentifier.Family}");
				AppendTabbedLine(2, $"Model             : {CpuIdentifier.Model}");
				AppendTabbedLine(2, $"Stepping          : {CpuIdentifier.Stepping}");
				AppendTabbedLine(2, $"Type              : {CpuIdentifier.Type}");
				AppendTabbedLine(2, "Instruction Sets:");
				{
					var sets = new InstructionSetRecord[] {
						new("SSSE3", Ssse3.IsSupported, Extensions.Simd.Support.Ssse3),
						new("SSE4.1", Sse41.IsSupported, Extensions.Simd.Support.Sse41),
						new("BMI2", Bmi2.IsSupported, Extensions.Simd.Support.Bmi2),
						new("AVX2", Avx2.IsSupported, Extensions.Simd.Support.Avx2),
					};

					var maxNameLen = sets.MaxF(set => set.Name.Length);

					void PrintSet(int tabs, string name, bool isSupported, bool isEnabled) {
						AppendTabbedLine(
							tabs,
							!isSupported
								? $"{name.PadRight(maxNameLen)} : false"
								: $"{name.PadRight(maxNameLen)} : true ({(isEnabled ? "enabled" : "disabled")})"
						);
					}

					foreach (var set in sets) {
						PrintSet(3, set.Name, set.IsSupported, set.IsEnabled);
					}
				}
			}
		}
		catch {
			// ignored
		}

		try {
			GC.Collect(int.MaxValue, GCCollectionMode.Forced, blocking: true, compacting: true);
			var memoryInfo = GC.GetGCMemoryInfo();
			AppendTabbedLine(1, $"Total Committed Memory: {memoryInfo.TotalCommittedBytes.AsDataSize()}");
			AppendTabbedLine(1, $"Total Available Memory: {memoryInfo.TotalAvailableMemoryBytes.AsDataSize()}");
		}
		catch {
			// ignored
		}

		try {
			AppendTabbedLine(1, "Graphics Adapter");
			AppendTabbedLine(2, $"Description    : {Graphics.Description}");
			AppendTabbedLine(2, $"Vendor         : {Graphics.VendorName}");
			AppendTabbedLine(2, $"Dedicated      : {Graphics.IsDedicated}");
			if (Graphics.DedicatedMemory is {} dedicatedMemory) {
				AppendTabbedLine(2, $"Dedicated VRAM : {dedicatedMemory.AsDataSize()}");
			}
			if (Graphics.TotalMemory is {} totalMemory) {
				AppendTabbedLine(2, $"Total VRAM     : {totalMemory.AsDataSize()}");
			}
		}
		catch {
			// ignored
		}

		Debug.Message(dumpBuilder.ToString());
	}
}
