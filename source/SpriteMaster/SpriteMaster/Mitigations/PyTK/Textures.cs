/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using FastExpressionCompiler.LightExpression;
using LinqFasterer;
using SpriteMaster.Extensions;
using SpriteMaster.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Mitigations.PyTK;

internal static class Textures {
	private static readonly Type? MappedTexture2DType = ReflectionExt.GetTypeExt("PyTK.Types.MappedTexture2D");
	private static readonly Type? AnimatedTexture2DType = ReflectionExt.GetTypeExt("PyTK.Types.AnimatedTexture2D");
	private static readonly Type? ScaledTexture2DType = ReflectionExt.GetTypeExt("PyTK.Types.ScaledTexture2D");

	private static readonly Type? PortraitureTextureLoaderType = ReflectionExt.GetTypeExt("Portraiture.TextureLoader");

	private static readonly Func<Dictionary<string, XTexture2D>>? PortraitureTextureList =
		PortraitureTextureLoaderType?.GetFieldGetter<Dictionary<string, XTexture2D>>("pTextures");

	private static readonly bool IsAnimatedTextureDerivedFromScaledTexture =
		AnimatedTexture2DType?.IsAssignableTo(ScaledTexture2DType) ?? false;

#pragma warning disable CS8714
	private static readonly Func<XTexture2D, Dictionary<XRectangle?, XTexture2D?>?> GetTextureMap =
		MappedTexture2DType?.GetMemberGetter<XTexture2D, Dictionary<XRectangle?, XTexture2D?>?>("Map") ?? (_ => null);
	private static readonly Func<XTexture2D, List<XTexture2D?>?> GetTextureFrames =
		AnimatedTexture2DType?.GetMemberGetter<XTexture2D, List<XTexture2D?>?>("Frames") ?? (_ => null);
#pragma warning restore CS8714
	private static readonly Func<XTexture2D, XTexture2D?> GetSTexture =
		ScaledTexture2DType?.GetMemberGetter<XTexture2D, XTexture2D?>("STexture") ?? (_ => null);

	private static Predicate<XTexture2D> GenerateTextureTypePredicate(Type? type) {
		if (type is null) {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool NullGenerator(XTexture2D _) => false;
			return NullGenerator;
		}

		var textureExpression = Expression.Parameter(typeof(XTexture2D), "texture");
		var resultExpression = Expression.TypeIs(textureExpression, type);

		return Expression.Lambda<Predicate<XTexture2D>>(resultExpression, textureExpression).CompileFast();
	}

	private static Predicate<XTexture2D> GenerateCombinedTextureTypePredicate(params Type?[] types) {
		var nonNullTypes = types.WhereF(type => type is not null).DistinctF();

		// For each type, check if it is actually a derived type of any other types
		for (int i = 0; i < nonNullTypes.Count; ++i) {
			var refType = nonNullTypes[i];
			if (refType is null) {
				continue;
			}
			for (int j = 0; j < nonNullTypes.Count; ++j) {
				if (i == j) {
					continue;
				}

				var cmpType = nonNullTypes[j];

				if (cmpType is null) {
					continue;
				}
				
				if (refType.IsSubclassOf(cmpType)) {
					nonNullTypes[i] = null;
					break;
				}
			}
		}

		nonNullTypes = nonNullTypes.WhereF(type => type is not null);

		if (nonNullTypes.Count == 0) {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool NullGenerator(XTexture2D _) => false;
			return NullGenerator;
		}

		var textureExpression = Expression.Parameter(typeof(XTexture2D), "texture");
		var isExpressions = nonNullTypes.SelectF(type => Expression.TypeIs(textureExpression, type));

		Expression resultExpression = isExpressions[0];
		// Chain subsequent expressions in ORs
		for (int i = 1; i < nonNullTypes.Count; ++i) {
			resultExpression = Expression.Or(resultExpression, isExpressions[i]);
		}

		return Expression.Lambda<Predicate<XTexture2D>>(resultExpression, textureExpression).CompileFast();
	}

	private static readonly Predicate<XTexture2D> IsManagedTexture = GenerateCombinedTextureTypePredicate(
		MappedTexture2DType,
		AnimatedTexture2DType,
		ScaledTexture2DType
	);
	private static readonly Predicate<XTexture2D> IsMappedTexture = GenerateTextureTypePredicate(MappedTexture2DType);
	private static readonly Predicate<XTexture2D> IsAnimatedTexture = GenerateTextureTypePredicate(AnimatedTexture2DType);
	private static readonly Predicate<XTexture2D> IsScaledTexture = GenerateTextureTypePredicate(ScaledTexture2DType);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XTexture2D? GetTextureFromCollection<TCollection>(TCollection collection)
		where TCollection : ICollection<XTexture2D?> {
		foreach (var mappedTexture in collection) {
			if (!string.IsNullOrEmpty(mappedTexture?.Name)) {
				return mappedTexture;
			}
		}

		return collection is IList<XTexture2D?> list ?
			list.FirstOrDefaultF() :
			collection.FirstOrDefault();
	}

	private readonly record struct ParsedTextureResult(XTexture2D? InnerTexture, bool IsManaged);

	private delegate ParsedTextureResult TextureParserDelegate(XTexture2D texture);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TextureParserDelegate GetTextureParser(this XTexture2D texture) {
		try {
			return true switch {
				_ when IsMappedTexture(texture) => ParseMappedTexture,
				_ when IsAnimatedTexture(texture) => ParseAnimatedTexture,
				_ when IsScaledTexture(texture) => ParseScaledTexture,
				_ => ParseNoneTexture
			};
		}
		catch {
			return ParseNoneTexture;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ParsedTextureResult ParseNoneTexture(XTexture2D texture) {
		return new(null, false);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ParsedTextureResult ParseMappedTexture(XTexture2D texture) {
		if (GetTextureMap(texture) is { } map) {
			return new(GetTextureFromCollection(map.Values), true);
		}

		return new(null, true);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ParsedTextureResult ParseAnimatedTexture(XTexture2D texture) {
		if (GetTextureFrames(texture) is { } list) {
			if (GetTextureFromCollection(list) is { } innerTexture) {
				return new(innerTexture, true);
			}
		}

		if (IsAnimatedTextureDerivedFromScaledTexture) {
			try {
				return ParseScaledTexture(texture);
			}
			catch {
				// Swallow Exceptions
			}
		}

		return new(null, true);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ParsedTextureResult ParseScaledTexture(XTexture2D texture) {
		if (GetSTexture(texture) is { } underlyingTexture) {
			return new(underlyingTexture, true);
		}

		return new(null, true);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XTexture2D? ParseTexture(this XTexture2D texture, out bool isManaged) {
		try {
			var textureParser = texture.GetTextureParser();
			(var innerTexture, isManaged) = textureParser(texture);
			return innerTexture;
		}
		catch {
			// swallow exceptions
			isManaged = true;
			return null;
		}
	}

	internal static XTexture2D GetUnderlyingTexture(this XTexture2D texture, out bool isManaged) {
		bool wasManaged = false;

		using var seenSet = ObjectPoolExt.TakeLazy<HashSet<XTexture2D>>();

		bool lastIsManaged;

		var startTexture = texture;

		{
			if (string.IsNullOrEmpty(texture.Name) && PortraitureTextureList?.Invoke() is { } textureList) {
				if (textureList.FirstOrDefault(pair => pair.Value == texture) is { } texturePair) {
					texture.Name = texturePair.Key;
				}
			}
		}

		while (texture.ParseTexture(out lastIsManaged) is {} parsedTexture) {
			if (!seenSet.Value.Add(parsedTexture)) {
				break;
			}
			texture = parsedTexture;
			wasManaged = true;
		}

		isManaged = wasManaged || lastIsManaged;

		{
			if (startTexture != texture && string.IsNullOrEmpty(texture.Name) &&
					PortraitureTextureList?.Invoke() is { } textureList) {
				if (textureList.FirstOrDefault(pair => pair.Value == texture) is { } texturePair) {
					texture.Name = texturePair.Key;
				}
			}
		}

		return texture;
	}

	// ReSharper disable once InconsistentNaming
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsPyTKTexture(this XTexture2D texture) {
		return IsManagedTexture(texture);
	}
}
