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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;

using Leclair.Stardew.ThemeManager;
using System.Collections.Generic;

namespace ThemeManagerExample;

public class ThemeData {

	public float TextScale { get; set; } = 1;

	public Color? TextColor { get; set; }

}

public class ModEntry : Mod {

	internal IThemeManager<ThemeData>? ThemeManager;
	internal IManagedAsset<Texture2D>? Background;
	internal ThemeData Theme = new();

	internal IGameTheme? GameTheme;

	public override void Entry(IModHelper helper) {
		Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
		Helper.Events.Display.RenderedHud += Display_RenderedHud;
	}

	private void Display_RenderedHud(object? sender, StardewModdingAPI.Events.RenderedHudEventArgs e) {
		// Read values from our theme!
		float scale = Theme.TextScale;
		Color color = Theme.TextColor ?? GameTheme?.GetColorVariable("Text") ?? Game1.textColor;

		// Set up the text!
		string text = $"Hello!\n\nSelected Theme: {ThemeManager?.SelectedThemeId}\nActive Theme: {ThemeManager?.ActiveThemeId}";
		var size = Game1.smallFont.MeasureString(text) * scale;

		// Draw a box! Not just any box, but a box using our
		// Background texture.
		if (Background?.Value != null)
			IClickableMenu.drawTextureBox(
				e.SpriteBatch,
				texture: Background.Value,
				sourceRect: new Rectangle(0, 0, 15, 15),
				x: 16, y: 16,
				width: 48 + (int) size.X,
				height: 48 + (int) size.Y,
				color: Color.White,
				scale: 4f
			);

		// Now draw our text in the box, using the color
		// and scale from our theme.
		e.SpriteBatch.DrawString(
			Game1.smallFont,
			text,
			new Vector2(40, 40),
			color,
			0f,
			Vector2.Zero,
			scale,
			SpriteEffects.None,
			1f
		);
	}

	private void GameLoop_GameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
		SetupTheme();

		Background = LoadManaged<Texture2D>("Background.png");
	}

	private IManagedAsset<T> LoadManaged<T>(string path) where T : notnull {
		if (ThemeManager is not null)
			return ThemeManager.GetManagedAsset<T>(path);
		return new FallbackManagedAsset<T>($"assets/{path}", Helper, Monitor);
	}

	private void SetupTheme() {
		if (!Helper.ModRegistry.IsLoaded("leclair.thememanager"))
			return;

		IThemeManagerApi? api;
		try {
			api = Helper.ModRegistry.GetApi<IThemeManagerApi>("leclair.thememanager");
		} catch (Exception ex) {
			Monitor.Log($"Unable to get Theme Manager's API: {ex}", LogLevel.Error);
			return;
		}

		if (api is null)
			return;

		ThemeManager = api.GetOrCreateManager<ThemeData>();

		GameTheme = api.GameTheme;
		api.GameThemeChanged += OnBaseThemeChanged;

		Theme = ThemeManager.Theme;
		ThemeManager.ThemeChanged += OnThemeChanged;
	}

	private void OnBaseThemeChanged(IThemeChangedEvent<IGameTheme> e) {
		GameTheme =  e.NewData;
	}

	private void OnThemeChanged(IThemeChangedEvent<ThemeData> e) {
		Theme = e.NewData;
		Background = LoadManaged<Texture2D>("Background.png");
	}
}
