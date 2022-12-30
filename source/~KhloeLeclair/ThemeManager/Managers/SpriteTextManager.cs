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

using BmFont;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley.BellsAndWhistles;

using Leclair.Stardew.Common.Events;

using Leclair.Stardew.ThemeManager.Models;
using StardewModdingAPI.Events;

namespace Leclair.Stardew.ThemeManager.Managers;

public class SpriteTextManager : BaseManager {

	public const string SPRITE_TEXTURE_ASSET = "Mods/leclair.thememanager/Game/DefaultSpriteFont/SpriteTexture";
	public const string COLORED_TEXTURE_ASSET = "Mods/leclair.thememanager/Game/DefaultSpriteFont/ColoredTexture";
	public const string FONT_DATA_ASSET = "Mods/leclair.thememanager/Game/DefaultSpriteFont/FontData";

	internal static SpriteTextManager? Instance;

	#region Static Default Font Storage

	internal static IBmFontData? DefaultFont;

	internal static Texture2D DefaultSpriteTexture = SpriteText.spriteTexture;
	internal static Texture2D DefaultColoredTexture = SpriteText.coloredTexture;

	private static bool EverUpdatedFont;
	private static bool EverUpdatedTextures;

	#endregion

	#region Normal Font Storage

	/// <summary>
	/// The normal font is the font that has been applied via Theme Manager, or
	/// the default font if no font has been applied.
	/// </summary>
	internal IBmFontData? Font;
	internal Texture2D? SpriteTexture;
	internal Texture2D? ColoredTexture;

	internal IManagedAsset<IBmFontData>? ManagedFont;
	internal IManagedAsset<Texture2D>? ManagedSpriteTexture;
	internal IManagedAsset<Texture2D>? ManagedColoredTexture;

	#endregion

	#region Fields - SpriteText Private Access

	private bool Fields_Loaded;

	private IReflectedField<FontFile?>? Field_FontFile;
	private IReflectedField<Dictionary<char, FontChar>?>? Field_CharacterMap;
	private IReflectedField<List<Texture2D>?>? Field_FontPages;
	private IReflectedMethod? Method_setUpCharacterMap;

	#endregion

	public SpriteTextManager(ModEntry mod) : base(mod) {
		Instance = this;
		InitializeFields();
	}

	private void InitializeFields() {
		if (Fields_Loaded)
			return;

		Fields_Loaded = true;

		try {
			Field_FontFile = Mod.Helper.Reflection.GetField<FontFile?>(typeof(SpriteText), "FontFile");
			Field_CharacterMap = Mod.Helper.Reflection.GetField<Dictionary<char, FontChar>?>(typeof(SpriteText), "_characterMap");
			Field_FontPages = Mod.Helper.Reflection.GetField<List<Texture2D>?>(typeof(SpriteText), "fontPages");
			Method_setUpCharacterMap = Mod.Helper.Reflection.GetMethod(typeof(SpriteText), "setUpCharacterMap");

		} catch (Exception ex) {
			Log($"Unable to get SpriteText fields. Custom SpriteText fonts will not work correctly: {ex}", LogLevel.Warn);
			Field_FontFile = null;
			Field_CharacterMap = null;
			Field_FontPages = null;
			Method_setUpCharacterMap = null;
		}
	}

	#region Default Font Updates

	public void MaybeUpdateDefaultTextures() {
		if (!EverUpdatedTextures)
			UpdateDefaultTextures();
	}

	public void UpdateDefaultTextures() {
		EverUpdatedTextures = true;

		if (SpriteText.spriteTexture != DefaultSpriteTexture) {
			DefaultSpriteTexture = SpriteText.spriteTexture;
			Mod.Helper.GameContent.InvalidateCache(SPRITE_TEXTURE_ASSET);
		}

		if (SpriteText.coloredTexture != DefaultColoredTexture) {
			DefaultColoredTexture = SpriteText.coloredTexture;
			Mod.Helper.GameContent.InvalidateCache(COLORED_TEXTURE_ASSET);
		}
	}

	public void MaybeUpdateDefaultFont() {
		if (!EverUpdatedFont)
			UpdateDefaultFont();
	}

	public void UpdateDefaultFont() {
		EverUpdatedFont = true;

		// If we don't have our fields, we can't do anything.
		if (Field_FontFile is null)
			return;

		// Read the values.
		Dictionary<char, FontChar>? charMap = Field_CharacterMap?.GetValue();

		// If character map is null, call setUpCharacterMap.
		if (charMap is null) {
			Method_setUpCharacterMap?.Invoke();
			charMap = Field_CharacterMap?.GetValue();
		}

		FontFile? file = Field_FontFile?.GetValue();
		List<Texture2D>? fontPages = Field_FontPages?.GetValue();

		// Save the default font, which may be but should not be null.
		if (charMap is null || file is null || fontPages is null)
			DefaultFont = null;
		else
			DefaultFont = new BmFontData(
				file: file,
				fontPages: fontPages,
				pixelZoom: SpriteText.fontPixelZoom,
				characterMap: charMap
			);

		Mod.Helper.GameContent.InvalidateCache(FONT_DATA_ASSET);
	}

	#endregion

	#region Override Default Fonts

	private bool HandleManaged<T>(ref IManagedAsset<T>? existing, IManagedAsset<T>? updated) where T : notnull {
		if (existing == updated)
			return false;

		if (existing is not null)
			existing.MarkedStale -= OnManagedMarkedStale;

		if (updated is not null)
			updated.MarkedStale += OnManagedMarkedStale;

		existing = updated;
		return true;
	}

	public void AssignFonts(IGameTheme? theme) {
		MaybeUpdateDefaultFont();
		MaybeUpdateDefaultTextures();

		bool changed = HandleManaged(ref ManagedFont, theme?.GetManagedBmFontVariable("ST:Font"));
		changed = HandleManaged(ref ManagedSpriteTexture, theme?.GetManagedTextureVariable("ST:Normal")) || changed;
		changed = HandleManaged(ref ManagedColoredTexture, theme?.GetManagedTextureVariable("ST:Colored")) || changed;

		if (changed)
			OnManagedMarkedStale(this, EventArgs.Empty);
	}

	#endregion

	#region Applying Fonts

	public void ApplyNormalFont() {
		if (EverUpdatedTextures) {
			SpriteText.spriteTexture = SpriteTexture ?? DefaultSpriteTexture;
			SpriteText.coloredTexture = ColoredTexture ?? DefaultColoredTexture;
		}

		if (!EverUpdatedFont || Field_FontFile is null)
			return;

		var font = Font ?? DefaultFont;

		Field_FontFile.SetValue(font?.File);
		Field_CharacterMap?.SetValue(font?.CharacterMap);
		Field_FontPages?.SetValue(font?.FontPages);
		SpriteText.fontPixelZoom = font?.PixelZoom ?? 3f;
	}

	public void ApplyFont(Texture2D? spriteTexture, Texture2D? coloredTexture, IBmFontData? font) {
		if (!EverUpdatedFont)
			UpdateDefaultFont();
		if (!EverUpdatedTextures)
			UpdateDefaultTextures();

		if (spriteTexture is not null)
			SpriteText.spriteTexture = spriteTexture;

		if (coloredTexture is not null)
			SpriteText.coloredTexture = coloredTexture;

		if (font is not null && Field_FontFile is not null) {
			if (Field_FontFile is not null) {
				Field_FontFile.SetValue(font.File);
				Field_CharacterMap!.SetValue(font.CharacterMap);
				Field_FontPages!.SetValue(font.FontPages);
				SpriteText.fontPixelZoom = font.PixelZoom;
			}
		}
	}

	#endregion

	#region Events

	private void OnManagedMarkedStale(object? sender, EventArgs e) {
		SpriteTexture = ManagedSpriteTexture?.Value;
		SpriteText.spriteTexture = SpriteTexture ?? DefaultSpriteTexture;

		ColoredTexture = ManagedColoredTexture?.Value;
		SpriteText.coloredTexture = ColoredTexture ?? DefaultColoredTexture;

		Font = ManagedFont?.Value;
		if (Field_FontFile is not null) { 
			Font ??= DefaultFont;
			Field_FontFile.SetValue(Font?.File);
			Field_CharacterMap!.SetValue(Font?.CharacterMap);
			Field_FontPages!.SetValue(Font?.FontPages);
			SpriteText.fontPixelZoom = Font?.PixelZoom ?? 3f;
		}
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		if (e.Name.IsEquivalentTo(SPRITE_TEXTURE_ASSET))
			e.LoadFrom(() => DefaultSpriteTexture, priority: AssetLoadPriority.Low);
		if (e.Name.IsEquivalentTo(COLORED_TEXTURE_ASSET))
			e.LoadFrom(() => DefaultColoredTexture, priority: AssetLoadPriority.Low);

		// We load this as exclusive because we need the type to match.
		if (e.Name.IsEquivalentTo(FONT_DATA_ASSET))
			e.LoadFrom(() => DefaultFont!, priority: AssetLoadPriority.Exclusive);
	}

	#endregion

}
