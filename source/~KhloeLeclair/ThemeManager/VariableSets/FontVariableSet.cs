/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;

using StardewValley;

using Leclair.Stardew.Common.Extensions;
using Leclair.Stardew.ThemeManager.Serialization;
using Leclair.Stardew.ThemeManager.Managers;
using Leclair.Stardew.ThemeManager.Models;

namespace Leclair.Stardew.ThemeManager.VariableSets;

public enum Side {
	Top = 0,
	Left = 1,
	Right = 2,
	Bottom = 3
}


[JsonConverter(typeof(RealVariableSetConverter))]
public class FontVariableSet : BaseVariableSet<IManagedAsset<SpriteFont>> {

	#region Font Modification

	public static void AdjustCropping(SpriteFont font, Side side, int amount) {
		for (int i = 0; i < font.Glyphs.Length; i++) {
			var glyph = font.Glyphs[i];
			glyph.Cropping = new(
				glyph.Cropping.X + (side == Side.Left ? amount : 0),
				glyph.Cropping.Y + (side == Side.Top ? amount : 0),
				glyph.Cropping.Width + (side == Side.Right ? amount : 0),
				glyph.Cropping.Height + (side == Side.Bottom ? amount : 0)
			);

			font.Glyphs[i] = glyph;
		}
	}

	public static SpriteFont CloneFont(SpriteFont font) {
		List<Rectangle> bounds = new(font.Glyphs.Length);
		List<Rectangle> cropping = new(font.Glyphs.Length);
		List<char> chars = new(font.Glyphs.Length);
		List<Vector3> kerning = new(font.Glyphs.Length);

		for (int i = 0; i < font.Glyphs.Length; i++) {
			var glyph = font.Glyphs[i];
			bounds.Add(glyph.BoundsInTexture);
			cropping.Add(glyph.Cropping);
			chars.Add(glyph.Character);
			kerning.Add(new Vector3(
				x: glyph.LeftSideBearing,
				y: glyph.Width,
				z: glyph.RightSideBearing
			));
		}

		return new SpriteFont(
			texture: font.Texture,
			glyphBounds: bounds,
			cropping: cropping,
			characters: chars,
			lineSpacing: font.LineSpacing,
			spacing: font.Spacing,
			kerning: kerning,
			defaultCharacter: font.DefaultCharacter
		);
	}

	#endregion

	public override bool TryParseValue(string input, [NotNullWhen(true)] out IManagedAsset<SpriteFont>? result) {
		if (Manifest is null || Manager is null) {
			result = null;
			return false;
		}

		return TryParseValue(input, Manager, Manifest.UniqueID, out result);
	}

	public static bool TryParseValue(string input, IThemeManager? manager, string? themeId, [NotNullWhen(true)] out IManagedAsset<SpriteFont>? result) {
		try {
			Uri url = new(new Uri("theme:"), input);
			string path = HttpUtility.UrlDecode(url.AbsolutePath);

			IManagedAsset<SpriteFont>? main;

			if (url.Scheme.Equals("game", StringComparison.OrdinalIgnoreCase)) {
				if (!ModEntry.Instance.TryGetManagedAsset(path, out main))
					throw new Exception($"Could not load managed asset for: {path}");

			} else if (url.Scheme.Equals("theme", StringComparison.OrdinalIgnoreCase)) {
				if (manager is null)
					throw new Exception($"Could not load managed asset: theme is not available in current context");
				main = manager.GetManagedAsset<SpriteFont>(path, themeId: themeId ?? manager.ActiveThemeId);

			} else if (url.Scheme.Equals("default", StringComparison.OrdinalIgnoreCase)) {
				if (path.Equals("smallFont", StringComparison.OrdinalIgnoreCase)) {
					if (!ModEntry.Instance.TryGetManagedAsset(SpriteFontManager.SMALL_FONT_ASSET, out main))
						throw new Exception($"Could not load managed asset for: {path}");

				} else if (path.Equals("dialogueFont", StringComparison.OrdinalIgnoreCase)) {
					if (!ModEntry.Instance.TryGetManagedAsset(SpriteFontManager.DIALOGUE_FONT_ASSET, out main))
						throw new Exception($"Could not load managed asset for: {path}");

				} else if (path.Equals("tinyFont", StringComparison.OrdinalIgnoreCase)) {
					if (!ModEntry.Instance.TryGetManagedAsset(SpriteFontManager.TINY_FONT_ASSET, out main))
						throw new Exception($"Could not load managed asset for: {path}");

				} else
					throw new Exception($"invalid default font: {path}");
			} else
				throw new Exception($"invalid scheme for font: {url.Scheme}");

			if (main is not null) {
				var values = string.IsNullOrWhiteSpace(url.Query) ? null : HttpUtility.ParseQueryString(url.Query);

				int? LineSpacing = values?.ReadInt("LineSpacing");
				int? Spacing = values?.ReadInt("Spacing");
				int PaddingTop = values?.ReadInt("PaddingTop") ?? 0;
				int PaddingBottom = values?.ReadInt("PaddingBottom") ?? 0;
				int PaddingLeft = values?.ReadInt("PaddingLeft") ?? 0;
				int PaddingRight = values?.ReadInt("PaddingRight") ?? 0;

				SpriteFont Process(Action<IManagedAsset> useAsset, Action markStale) {
					useAsset(main);

					bool cloned = false;
					SpriteFont result = main.Value!;

					result.DefaultCharacter ??= '?';

					if (LineSpacing.HasValue) {
						if (!cloned) { result = CloneFont(result); cloned = true; }
						result.LineSpacing = LineSpacing.Value;
					}

					if (Spacing.HasValue) {
						if (!cloned) { result = CloneFont(result); cloned = true; }
						result.Spacing = Spacing.Value;
					}

					if (PaddingTop != 0) {
						if (!cloned) { result = CloneFont(result); cloned = true; }
						AdjustCropping(result, Side.Top, PaddingTop);
					}

					if (PaddingBottom != 0) {
						if (!cloned) { result = CloneFont(result); cloned = true; }
						AdjustCropping(result, Side.Bottom, PaddingBottom);
					}

					if (PaddingLeft != 0) {
						if (!cloned) { result = CloneFont(result); cloned = true; }
						AdjustCropping(result, Side.Left, PaddingLeft);
					}

					if (PaddingRight != 0) {
						if (!cloned) { result = CloneFont(result); cloned = true; }
						AdjustCropping(result, Side.Right, PaddingRight);
					}

					return result;
				}

				result = new ProcessedManagedAsset<SpriteFont>(main.AssetName, Process);

			} else
				result = null;

		} catch (Exception ex) {
			ModEntry.Instance.Log($"Failed to load SpriteFont asset from \"{input}\" for {themeId}: {ex}", StardewModdingAPI.LogLevel.Error);
			result = null;
			return false;
		}

		return result is not null;
	}

}
