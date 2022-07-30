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
using SpriteMaster.Configuration;
using SpriteMaster.Types;
using StardewValley;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class ShadowedText {
	private static bool LongWords => Game1.content.GetCurrentLanguage() switch {
		LocalizedContentManager.LanguageCode.ru => true,
		LocalizedContentManager.LanguageCode.de => true,
		_ => false
	};

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowNullArgumentException(string name) =>
		throw new ArgumentNullException(name);

	[Harmonize(
		typeof(StardewValley.Utility),
		"drawTextWithShadow",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool DrawTextWithShadow(
		XSpriteBatch b,
		StringBuilder text,
		SpriteFont font,
		XVector2 position,
		XColor color,
		float scale = 1f,
		float layerDepth = -1f,
		int horizontalShadowOffset = -1,
		int verticalShadowOffset = -1,
		float shadowIntensity = 1f,
		int numShadows = 3
	) {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.StrokeShadowedText) {
			return true;
		}

		if (layerDepth == -1f) {
			layerDepth = position.Y / 10000f;
		}

		/*
		if (horizontalShadowOffset == -1) {
			horizontalShadowOffset = ((font.Equals(Game1.smallFont) || LongWords) ? (-2) : (-3));
		}
		if (verticalShadowOffset == -1) {
			verticalShadowOffset = ((font.Equals(Game1.smallFont) || LongWords) ? 2 : 3);
		}
		*/

		if (text is null) {
			ThrowNullArgumentException(nameof(text));
		}

		DrawStrokedText(
			b,
			text.ToString(),
			font,
			position,
			color,
			new XColor(221, 148, 84) * shadowIntensity,
			scale,
			layerDepth,
			(horizontalShadowOffset, verticalShadowOffset),
			numShadows
		);

		return false;
	}

	[Harmonize(
		typeof(StardewValley.Utility),
		"drawTextWithShadow",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool DrawTextWithShadow(
		XSpriteBatch b,
		string? text,
		SpriteFont? font,
		XVector2 position,
		XColor color,
		float scale = 1f,
		float layerDepth = -1f,
		int horizontalShadowOffset = -1,
		int verticalShadowOffset = -1,
		float shadowIntensity = 1f,
		int numShadows = 3
	) {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.StrokeShadowedText) {
			return true;
		}

		if (font is null) {
			ThrowNullArgumentException(nameof(font));
		}

		if (layerDepth == -1f) {
			layerDepth = position.Y / 10000f;
		}

		if (horizontalShadowOffset == -1) {
			horizontalShadowOffset = ((font.Equals(Game1.smallFont) || LongWords) ? (-2) : (-3));
		}
		if (verticalShadowOffset == -1) {
			verticalShadowOffset = ((font.Equals(Game1.smallFont) || LongWords) ? 2 : 3);
		}

		text ??= "";

		// true;

		DrawStrokedText(
			b,
			text,
			font,
			position,
			color,
			new XColor(221, 148, 84) * shadowIntensity,
			scale,
			layerDepth,
			(horizontalShadowOffset, verticalShadowOffset),
			numShadows
		);

		return false;
	}

	[Harmonize(
		typeof(StardewValley.Utility),
		"drawTextWithColoredShadow",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool DrawTextWithColoredShadow(
		XSpriteBatch b,
		string? text,
		SpriteFont? font,
		XVector2 position,
		XColor color,
		XColor shadowColor,
		float scale = 1f,
		float layerDepth = -1f,
		int horizontalShadowOffset = -1,
		int verticalShadowOffset = -1,
		int numShadows = 3
	) {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.StrokeShadowedText) {
			return true;
		}

		if (font is null) {
			ThrowNullArgumentException(nameof(font));
		}

		if (layerDepth == -1f) {
			layerDepth = position.Y / 10000f;
		}

		if (horizontalShadowOffset == -1) {
			horizontalShadowOffset = ((font.Equals(Game1.smallFont) || LongWords) ? (-2) : (-3));
		}
		if (verticalShadowOffset == -1) {
			verticalShadowOffset = ((font.Equals(Game1.smallFont) || LongWords) ? 2 : 3);
		}

		text ??= "";

		DrawStrokedText(
			b,
			text,
			font,
			position,
			color,
			shadowColor,
			scale,
			layerDepth,
			(horizontalShadowOffset, verticalShadowOffset),
			numShadows
		);

		return false;
	}

	private static void DrawStrokedText(
		XSpriteBatch b,
		string? text,
		SpriteFont? font,
		Vector2F position,
		XColor color,
		XColor shadowColor,
		float scale,
		float layerDepth,
		Vector2I shadowOffset,
		int numShadows
	) {
		if (font is null) {
			ThrowNullArgumentException(nameof(font));
		}

		for (int y = -1; y <= 1; ++y) {
			for (int x = -1; x <= 1; ++x) {
				b.DrawString(
					font,
					text,
					position + (x, y),
					shadowColor,
					0f,
					XVector2.Zero,
					scale,
					SpriteEffects.None,
					layerDepth
				);
			}
		}

		b.DrawString(
			font,
			text,
			position,
			color,
			0f,
			XVector2.Zero,
			scale,
			SpriteEffects.None,
			layerDepth
		);
	}
}
