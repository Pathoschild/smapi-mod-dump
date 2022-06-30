/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Caching;

internal static partial class TextureFileCache {
	private static class Stb {
		private const string StbNamespace = "StbImageSharp";

		internal static class ColorComponentsReflect {
			internal static readonly Type? ColorComponentsType =
				typeof(XTexture2D).Assembly.GetType($"{StbNamespace}.ColorComponents");

			internal static readonly int? RedGreenBlueAlpha =
				EnumExt.Parse<int>(ColorComponentsType, "RedGreenBlueAlpha");

			[MemberNotNullWhen(
				true,
				nameof(ColorComponentsType),
				nameof(RedGreenBlueAlpha)
			)]
			internal static bool Enabled { get; } =
				ColorComponentsType is not null &&
				RedGreenBlueAlpha is not null;
		}

		internal static class ImageResultReflect {
			internal static readonly Type? ImageResultType =
				typeof(XTexture2D).Assembly.GetType($"{StbNamespace}.ImageResult");

			internal static readonly Func<byte[], int, object>? FromMemory = ImageResultType
				?.GetStaticMethod("FromMemory")?.CreateDelegate<Func<byte[], int, object>>();

			internal static readonly Func<object, int>? GetWidth =
				ImageResultType?.GetPropertyGetter<object, int>("Width");

			internal static readonly Func<object, int>? GetHeight =
				ImageResultType?.GetPropertyGetter<object, int>("Height");

			internal static readonly Func<object, byte[]>? GetData =
				ImageResultType?.GetPropertyGetter<object, byte[]>("Data");

			[MemberNotNullWhen(
				true,
				nameof(ImageResultType),
				nameof(FromMemory),
				nameof(GetWidth),
				nameof(GetHeight),
				nameof(GetData)
			)]
			internal static bool Enabled { get; } =
				ImageResultType is not null &&
				FromMemory is not null &&
				GetWidth is not null &&
				GetHeight is not null &&
				GetData is not null;
		}

		internal static readonly bool Enabled = ColorComponentsReflect.Enabled && ImageResultReflect.Enabled;

		internal readonly ref struct ImageResult {
			private readonly object Handle;
			internal readonly Vector2I Size;
			internal readonly byte[] Data;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal ImageResult(byte[] data) {
				Handle = ImageResultReflect.FromMemory!(data, ColorComponentsReflect.RedGreenBlueAlpha!.Value);
				Size = (
					ImageResultReflect.GetWidth!(Handle),
					ImageResultReflect.GetHeight!(Handle)
				);
				Data = ImageResultReflect.GetData!(Handle);
			}
		}
	}
}
