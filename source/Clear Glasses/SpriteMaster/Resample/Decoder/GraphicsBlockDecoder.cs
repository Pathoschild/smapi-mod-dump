/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.IO;

namespace SpriteMaster.Resample.Decoder;

internal static class GraphicsBlockDecoder {
	internal static Span<byte> Decode(ReadOnlySpan<byte> data, Vector2I size, SurfaceFormat format) {
		using var tempTexture = new DecodingTexture2D(DrawState.Device, size.Width, size.Height, false, format) { Name = "Decode Texture" };
		tempTexture.SetData(data.ToArray());
		using var pngStream = new MemoryStream();
		tempTexture.SaveAsPng(pngStream, tempTexture.Width, tempTexture.Height);
		pngStream.Flush();
		using var pngTexture = XTexture2D.FromStream(DrawState.Device, pngStream);
		var resultData = GC.AllocateUninitializedArray<byte>(pngTexture.Area() * sizeof(uint), pinned: true);
		pngTexture.GetData(resultData);

		return resultData.AsSpan();
	}
}
