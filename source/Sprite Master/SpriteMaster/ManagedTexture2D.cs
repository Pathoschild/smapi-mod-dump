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
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TeximpNet.Compression;

namespace SpriteMaster;
sealed class ManagedTexture2D : Texture2D {
	private static long TotalAllocatedSize = 0L;
	private static int TotalManagedTextures = 0;
	private const bool UseMips = false;

	internal readonly WeakReference<Texture2D> Reference;
	internal readonly ScaledTexture Texture;
	internal readonly Vector2I Dimensions;

	internal static void DumpStats(List<string> output) {
		output.AddRange(new[]{
			"\tManagedTexture2D:",
			$"\t\tTotal Managed Textures : {TotalManagedTextures}",
			$"\t\tTotal Texture Size     : {TotalAllocatedSize.AsDataSize()}"
			});
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal ManagedTexture2D(
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

		reference.Disposing += (_, _) => OnParentDispose();

		TotalAllocatedSize += this.SizeBytes();
		++TotalManagedTextures;

		Garbage.MarkOwned(format, dimensions.Area);
		Disposing += (_, _) => {
			Garbage.UnmarkOwned(format, dimensions.Area);
			TotalAllocatedSize -= this.SizeBytes();
			--TotalManagedTextures;
		};
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	~ManagedTexture2D() {
		if (!IsDisposed) {
			Dispose(false);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private void OnParentDispose() {
		if (!IsDisposed) {
			Debug.TraceLn($"Disposing ManagedTexture2D '{Name}'");
			Dispose();
		}
	}
}
