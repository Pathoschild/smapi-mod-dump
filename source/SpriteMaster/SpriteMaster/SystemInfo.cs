/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
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

				var adapter = device.Adapter;
				if (adapter is null) {
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

				bool hasNvidiaExtension = false;
				bool hasAtiExtension = false;
				foreach (var extension in MonoGame.OpenGL.GL.Extensions) {
					switch (extension) {
						case "GL_NVX_gpu_memory_info":
							hasNvidiaExtension = true;
							break;
						case "GL_ATI_meminfo":
							hasAtiExtension = true;
							break;
					}

					if (hasNvidiaExtension && hasAtiExtension) {
						break;
					}
				}

				if ((!DedicatedMemory.HasValue || !TotalMemory.HasValue) && hasNvidiaExtension) {
					const int DedicatedVidMemNvx = 0x9047;
					const int TotalAvailableMemoryNvx = 0x9048;

					unsafe {
						try {
							while (MonoGame.OpenGL.GL.GetError() != MonoGame.OpenGL.ErrorCode.NoError) {
								// Flush the error buffer
							}

							if (GLExt.GetInteger64v is not null) {
								long result = 0;
								GLExt.GetInteger64v(DedicatedVidMemNvx, &result);
								if (MonoGame.OpenGL.GL.GetError() == MonoGame.OpenGL.ErrorCode.NoError) {
									DedicatedMemory = (ulong)(result * 1024);
								}
							}
							else {
								int result = 0;
								MonoGame.OpenGL.GL.GetIntegerv(DedicatedVidMemNvx, &result);
								if (MonoGame.OpenGL.GL.GetError() == MonoGame.OpenGL.ErrorCode.NoError) {
									DedicatedMemory = (ulong)result * 1024;
								}
							}

							if (GLExt.GetInteger64v is not null) {
								long result = 0;
								GLExt.GetInteger64v(TotalAvailableMemoryNvx, &result);
								if (MonoGame.OpenGL.GL.GetError() == MonoGame.OpenGL.ErrorCode.NoError) {
									TotalMemory = (ulong)(result * 1024);
								}
							}
							else {
								int result = 0;
								MonoGame.OpenGL.GL.GetIntegerv(TotalAvailableMemoryNvx, &result);
								if (MonoGame.OpenGL.GL.GetError() == MonoGame.OpenGL.ErrorCode.NoError) {
									TotalMemory = (ulong)result * 1024;
								}
							}
						}
						catch {
							// ignored
						}
					}
				}
				if ((!DedicatedMemory.HasValue || !TotalMemory.HasValue) && hasAtiExtension) {
					// https://www.khronos.org/registry/OpenGL/extensions/ATI/ATI_meminfo.txt
					const int VboFreeMemoryAti = 0x87FB;
					const int TextureFreeMemoryAti = 0x87FC;
					const int RenderBufferFreeMemoryAti = 0x87FD;

					unsafe {
						try {
							if (GLExt.GetInteger64v is not null) {
								Span<long> result = stackalloc long[4];
								GLExt.GetInteger64v(TextureFreeMemoryAti, (long*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(result)));
								if (MonoGame.OpenGL.GL.GetError() == MonoGame.OpenGL.ErrorCode.NoError) {
									TotalMemory = DedicatedMemory = (ulong)(result[0] * 1024);
								}
							}
							else {
								Span<int> result = stackalloc int[4];
								MonoGame.OpenGL.GL.GetIntegerv(TextureFreeMemoryAti, (int*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(result)));
								if (MonoGame.OpenGL.GL.GetError() == MonoGame.OpenGL.ErrorCode.NoError) {
									TotalMemory = DedicatedMemory = (ulong)result[0] * 1024;
								}
							}
						}
						catch {
							// ignored
						}
					}
				}

				if (
					!(
						ParsePattern(Patterns.Intel) ||
						ParsePattern(Patterns.Nvidia) ||
						ParsePattern(Patterns.AMD)
					)
				) {
					// I have no idea
					Vendor = Vendors.Unknown;
					VendorName = vendor.Split(null, 2).FirstF();
					IsIntegrated = true;
				}
			}
			catch {
				// ignored
			}
		}
	}

	internal static void Update(GraphicsDeviceManager gdm, GraphicsDevice device) {
		Graphics.Update(gdm, device);
	}

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
			AppendTabbedLine(1, $"OS Version: {Environment.OSVersion}");
			AppendTabbedLine(1, "CPU Information");
			{
				AppendTabbedLine(2, $"Brand             : {CpuIdentifier.Brand}");
				AppendTabbedLine(2, $"Microarchitecture : {Microarchitecture}");
				AppendTabbedLine(2, $"Family            : {CpuIdentifier.Family}");
				AppendTabbedLine(2, $"Model             : {CpuIdentifier.Model}");
				AppendTabbedLine(2, $"Stepping          : {CpuIdentifier.Stepping}");
				AppendTabbedLine(2, $"Type              : {CpuIdentifier.Type}");
				AppendTabbedLine(2, $"AVX2              : {Avx2.IsSupported} ({(Instructions.Avx2 ? "enabled" : "disabled")})");
			}
		}
		catch {
			// ignored
		}

		try {
			var memoryInfo = GC.GetGCMemoryInfo();
			AppendTabbedLine(1, $"Total Committed Memory: {memoryInfo.TotalCommittedBytes.AsDataSize()}");
			AppendTabbedLine(1, $"Total Available Memory: {memoryInfo.TotalAvailableMemoryBytes.AsDataSize()}");
		}
		catch {
			// ignored
		}

		try {
			AppendTabbedLine(1, "\tGraphics Adapter");
			AppendTabbedLine(2, $"Description    : {Graphics.Description}");
			AppendTabbedLine(2, $"Vendor         : {Graphics.VendorName}");
			AppendTabbedLine(2, $"Dedicated      : {Graphics.IsDedicated}");
			if (Graphics.DedicatedMemory.HasValue) {
				AppendTabbedLine(2, $"Dedicated VRAM : {Graphics.DedicatedMemory.Value.AsDataSize()}");
			}
			if (Graphics.TotalMemory.HasValue) {
				AppendTabbedLine(2, $"Total VRAM     : {Graphics.TotalMemory.Value.AsDataSize()}");
			}
		}
		catch {
			// ignored
		}

		Debug.Message(dumpBuilder.ToString());
	}
}
