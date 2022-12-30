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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Leclair.Stardew.Common.Events;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;

using StardewValley;

namespace Leclair.Stardew.ThemeManager.Managers;

public class SpriteFontManager : BaseManager {

	public const string SMALL_FONT_ASSET = "Mods/leclair.thememanager/Game/DefaultFont/Small";
	public const string DIALOGUE_FONT_ASSET = "Mods/leclair.thememanager/Game/DefaultFont/Dialogue";
	public const string TINY_FONT_ASSET = "Mods/leclair.thememanager/Game/DefaultFont/Tiny";
	public const string TINY_FONT_BORDER_ASSET = "Mods/leclair.thememanager/Game/DefaultFont/TinyBorder";

	#region Static Default Font Storage

	internal static SpriteFont DefaultSmallFont = Game1.smallFont;
	internal static SpriteFont DefaultDialogueFont = Game1.dialogueFont;
	internal static SpriteFont DefaultTinyFont = Game1.tinyFont;
	internal static SpriteFont DefaultTinyFontBorder = Game1.tinyFontBorder;

	private static bool EverUpdated = false;

	#endregion

	#region Managed Fonts

	private IManagedAsset<SpriteFont>? ManagedSmall;
	private IManagedAsset<SpriteFont>? ManagedDialogue;
	private IManagedAsset<SpriteFont>? ManagedTiny;
	private IManagedAsset<SpriteFont>? ManagedTinyFontBorder;

	#endregion

	public SpriteFontManager(ModEntry mod) : base(mod) {



	}

	#region Default Font Updates

	public void MaybeUpdateDefaultFonts() {
		if (!EverUpdated)
			UpdateDefaultFonts();
	}

	public void UpdateDefaultFonts() {
		EverUpdated = true;

		if (DefaultSmallFont != Game1.smallFont) {
			DefaultSmallFont = Game1.smallFont;
			Mod.Helper.GameContent.InvalidateCache(SMALL_FONT_ASSET);
		}

		if (DefaultDialogueFont != Game1.dialogueFont) {
			DefaultDialogueFont = Game1.dialogueFont;
			Mod.Helper.GameContent.InvalidateCache(DIALOGUE_FONT_ASSET);
		}

		if (DefaultTinyFont != Game1.tinyFont) {
			DefaultTinyFont = Game1.tinyFont;
			Mod.Helper.GameContent.InvalidateCache(TINY_FONT_ASSET);
		}

		if (DefaultTinyFontBorder != Game1.tinyFontBorder) {
			DefaultTinyFontBorder = Game1.tinyFontBorder;
			Mod.Helper.GameContent.InvalidateCache(TINY_FONT_BORDER_ASSET);
		}
	}

	#endregion

	#region Override Default Fonts

	private bool HandleManagedFonts(ref IManagedAsset<SpriteFont>? existing, IManagedAsset<SpriteFont>? updated) {
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
		MaybeUpdateDefaultFonts();

		bool changed = HandleManagedFonts(ref ManagedSmall, theme?.GetManagedFontVariable("Small"));
		changed = HandleManagedFonts(ref ManagedDialogue, theme?.GetManagedFontVariable("Dialogue")) || changed;
		changed = HandleManagedFonts(ref ManagedTiny, theme?.GetManagedFontVariable("Tiny")) || changed;
		changed = HandleManagedFonts(ref ManagedTinyFontBorder, theme?.GetManagedFontVariable("TinyBorder")) || changed;

		if (changed)
			OnManagedMarkedStale(this, EventArgs.Empty);
	}

	#endregion

	#region Events

	private void OnManagedMarkedStale(object? sender, EventArgs e) {
		Game1.smallFont = ManagedSmall?.Value ?? DefaultSmallFont;
		Game1.dialogueFont = ManagedDialogue?.Value ?? DefaultDialogueFont;
		Game1.tinyFont = ManagedTiny?.Value ?? DefaultTinyFont;
		Game1.tinyFontBorder = ManagedTinyFontBorder?.Value ?? DefaultTinyFontBorder;
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		if (e.Name.IsEquivalentTo(SMALL_FONT_ASSET))
			e.LoadFrom(() => DefaultSmallFont, priority: AssetLoadPriority.Low);
		if (e.Name.IsEquivalentTo(DIALOGUE_FONT_ASSET))
			e.LoadFrom(() => DefaultDialogueFont, priority: AssetLoadPriority.Low);
		if (e.Name.IsEquivalentTo(TINY_FONT_ASSET))
			e.LoadFrom(() => DefaultTinyFont, priority: AssetLoadPriority.Low);
		if (e.Name.IsEquivalentTo(TINY_FONT_BORDER_ASSET))
			e.LoadFrom(() => DefaultTinyFontBorder, priority: AssetLoadPriority.Low);
	}

	#endregion

}
