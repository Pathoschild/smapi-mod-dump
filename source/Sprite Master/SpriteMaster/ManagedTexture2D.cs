/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using SpriteMaster.Types;
using SpriteMaster.Extensions;
using TeximpNet.Compression;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace SpriteMaster {
	internal sealed class ManagedTexture2D : Texture2D {
		private static long TotalAllocatedSize = 0L;
		private static int TotalManagedTextures = 0;
		private const bool UseMips = false;

		public readonly WeakReference<Texture2D> Reference;
		public readonly ScaledTexture Texture;
		public readonly Vector2I Dimensions;

		internal static void DumpStats(List<string> output) {
			output.Add("\tManagedTexture2D:");
			output.Add($"\t\tTotal Managed Textures : {TotalManagedTextures}");
			output.Add($"\t\tTotal Texture Size     : {TotalAllocatedSize.AsDataSize()}");
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public ManagedTexture2D (
			ScaledTexture texture,
			Texture2D reference,
			Vector2I dimensions,
			SurfaceFormat format,
			string name = null
		) : base(reference.GraphicsDevice.IsDisposed ? DrawState.Device : reference.GraphicsDevice, dimensions.Width, dimensions.Height, UseMips, format) {
			this.Name = name ?? $"{reference.SafeName()} [RESAMPLED {(CompressionFormat)format}]";

			Reference = reference.MakeWeak();
			Texture = texture;
			Dimensions = dimensions - texture.BlockPadding;

			reference.Disposing += (_, _1) => OnParentDispose();

			TotalAllocatedSize += this.SizeBytes();
			++TotalManagedTextures;

			Garbage.MarkOwned(format, dimensions.Area);
			Disposing += (_, _1) => {
				Garbage.UnmarkOwned(format, dimensions.Area);
				TotalAllocatedSize -= this.SizeBytes();
				--TotalManagedTextures;
			};
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		~ManagedTexture2D() {
			if (!IsDisposed) {
				Dispose(false);
			}
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		private void OnParentDispose() {
			if (!IsDisposed) {
				Debug.TraceLn($"Disposing ManagedTexture2D '{Name}'");
				Dispose();
			}
		}
	}
}
